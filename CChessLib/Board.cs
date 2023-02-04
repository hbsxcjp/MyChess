using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CChess;

public class Board
{
    private List<Seat> AllSeats { get; }

    private const char FENSplitChar = '/';

    public Board()
    {
        AllSeats = Coord.AllCoords.Select(coord => new Seat(coord)).ToList();
    }

    public Seat this[int row, int col]
    {
        get { return AllSeats[Coord.GetIndex(row, col)]; }
    }
    public Seat this[(int row, int col) rowCol]
    {
        get { return AllSeats[Coord.GetIndex(rowCol.row, rowCol.col)]; }
    }
    public Seat this[Coord coord]
    {
        get { return AllSeats[coord.Index]; }
    }

    public PieceColor BottomColor
    {
        get
        {
            return AllSeats.Find(seat => seat.Piece.Kind == PieceKind.King)?.Piece.Color ?? PieceColor.Red;
        }
    }

    public bool IsNull(int row, int col) => this[row, col].IsNull;

    /// <summary>
    /// 初始棋盘布局面时，棋子可放置的位置
    /// </summary>
    /// <returns>棋盘位置坐标的列表</returns>
    public List<Coord> PutCoord(Piece piece)
        => (piece.Kind == PieceKind.Rook || piece.Kind == PieceKind.Cannon || piece.Kind == PieceKind.Knight)
                ? Coord.AllCoords
                : piece.PutRowCols(BottomColor == piece.Color)
                    .Select(rowCol => this[rowCol].Coord).ToList();

    /// <summary>
    /// 当前棋盘局面下，棋子可移动位置, 已排除规则不允许行走的位置、同色棋子的位置
    /// </summary>
    /// <returns>棋盘位置坐标的列表</returns>
    public List<Coord> MoveCoord(Piece piece)
        => piece.MoveRowCols(this)
            .Where(rowCol => this[rowCol].Piece.Color != piece.Color)
            .Select(rowCol => this[rowCol].Coord)
            .ToList();

    /// <summary>
    /// 可移动位置, 排除将帅对面、被将军的位置
    /// </summary>
    public List<Coord> CanMoveCoord(Coord fromCoord)
    {
        Piece piece = this[fromCoord].Piece;
        List<Coord> coords = MoveCoord(piece);
        coords.RemoveAll(toCoord => !CanMove(fromCoord, toCoord));
        return coords;
    }

    /// <summary>
    /// 检测是否可移动, 包括直接杀将、移动后将帅未对面、未被将军
    /// </summary>
    public bool CanMove(Coord fromCoord, Coord toCoord)
    {
        Debug.Assert(!IsNull(fromCoord.Row, fromCoord.Col));

        Seat fromSeat = this[fromCoord];
        var color = fromSeat.Piece.Color;
        Seat toSeat = this[toCoord];
        Piece toPiece = toSeat.Piece;

        // 如是对方将帅的位置则直接可走，不用判断是否被将军（如加以判断，则会直接走棋吃将帅）
        if (toPiece.Kind == PieceKind.King)
            return true;

        fromSeat.MoveTo(toSeat, Piece.Null);
        bool killed = IsKilled(color);
        toSeat.MoveTo(fromSeat, toPiece);
        return !killed;
    }

    public void Done(Move move)
    {
        CoordPair coordPair = move.MoveInfo.CoordPair;
        Seat toSeat = this[coordPair.ToCoord];
        move.MoveInfo.EatPiece = toSeat.Piece;

        this[coordPair.FromCoord].MoveTo(toSeat, Piece.Null);
    }

    public void Undo(Move move)
    {
        CoordPair coordPair = move.MoveInfo.CoordPair;
        this[coordPair.ToCoord].MoveTo(this[coordPair.FromCoord],
            move.MoveInfo.EatPiece);
    }

    public void Reset() => AllSeats.ForEach(seat => seat.Piece = Piece.Null);

    public string GetFEN() => GetFEN(string.Concat(AllSeats.Select(seat => seat.Piece.Char)));

    public static string GetFEN(string pieceChars)
    {
        StringBuilder fen = new();
        for (int row = Coord.RowCount - 1; row > 0; --row)
        {
            fen.Append(pieceChars.Substring(row * Coord.ColCount, Coord.ColCount)).Append(FENSplitChar);
        }
        fen.Append(pieceChars[0..Coord.ColCount]);

        return Regex.Replace(fen.ToString(), $"{Piece.Null.Char}+", (Match match) => match.Value.Length.ToString());
    }
    public static string GetFEN(string fen, ChangeType ct)
    {
        if (ct == ChangeType.NoChange)
            return fen;

        if (ct == ChangeType.Exchange)
            return string.Concat(fen.Select(ch => char.IsLower(ch) ? char.ToUpper(ch) : char.ToLower(ch)));

        string[] fenArray = fen.Split(FENSplitChar);
        if (fenArray.Length != Coord.RowCount)
            return fen;

        IEnumerable<string> ReverseRow(IEnumerable<string> fenArray) => fenArray.Reverse();
        IEnumerable<string> ReverseCol(IEnumerable<string> fenArray)
            => fenArray.Select(line => string.Concat(line.Reverse()));

        return string.Join(FENSplitChar, ct switch
        {
            ChangeType.Symmetry_H => ReverseCol(fenArray),
            ChangeType.Symmetry_V => ReverseRow(fenArray),
            _ => ReverseCol(ReverseRow(fenArray)) //ChangeType.Rotate
        });
    }

    public bool SetFEN(string fen)
    {
        var fenArray = fen.Split(FENSplitChar);
        if (fenArray.Length != Coord.RowCount)
            return false;

        Reset();
        int row = 0;
        foreach (var line in fenArray.Reverse())
        {
            int col = 0;
            foreach (char ch in line)
                if (char.IsDigit(ch))
                    col += Convert.ToInt32(ch.ToString());
                else
                {
                    List<Piece> livePieces = GetLivePieces(Piece.GetColor(ch), Piece.GetKind(ch));
                    this[row, col++].Piece = Pieces.ThePieces.GetPieces(ch)
                        .Find(piece => !livePieces.Contains(piece)) ?? Piece.Null;
                }

            row++;
        }

        return true;
    }

    public bool ChangeLayout(ChangeType ct) => SetFEN(GetFEN(GetFEN(), ct));

    public Coord GetCoord(Piece piece)
        => AllSeats.Find(seat => seat.Piece == piece)?.Coord ?? Coord.Null;

    public Coord GetKingCoord(PieceColor color)
        => AllSeats.Find(seat => seat.Piece.Kind == PieceKind.King
            && seat.Piece.Color == color)?.Coord ?? Coord.Null;

    public List<Piece> GetLivePieces()
        => AllSeats.Where(seat => !seat.IsNull)
            .Select(seat => seat.Piece).ToList();

    public List<Piece> GetLivePieces(PieceColor color)
        => GetLivePieces().Where(piece => piece.Color == color).ToList();

    public List<Piece> GetLivePieces(PieceColor color, PieceKind kind)
        => GetLivePieces(color).Where(piece => piece.Kind == kind).ToList();

    public List<Piece> GetLivePieces(PieceColor color, PieceKind kind, int col)
        => GetLivePieces(color, kind).Where(piece => GetCoord(piece).Col == col).ToList();

    public bool IsKilled(PieceColor color)
    {
        Coord kingCoord = GetKingCoord(color);
        var otherColor = color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;

        // 将帅是否对面
        int col = kingCoord.Col;
        Coord otherKingCoord = GetKingCoord(otherColor);
        if (col == otherKingCoord.Col)
        {
            int thisRow = kingCoord.Row, otherRow = otherKingCoord.Row,
                lowRow = Math.Min(thisRow, otherRow) + 1, count = Math.Abs(thisRow - otherRow) - 1;
            if (Enumerable.Range(lowRow, count).All(row => IsNull(row, col)))
                return true;
        }

        // 是否正被将军
        return GetLivePieces(otherColor).Any(
            piece => MoveCoord(piece).Contains(kingCoord));
    }

    public bool IsFailed(PieceColor color)
        => GetLivePieces(color).All(
            piece => CanMoveCoord(GetCoord(piece)).Count == 0);

    public string GetZhStrFromCoordPair(CoordPair coordPair)
    {
        Debug.Assert(!IsNull(coordPair.FromCoord.Row, coordPair.FromCoord.Col));

        string zhStr;
        Seat fromSeat = this[coordPair.FromCoord],
            toSeat = this[coordPair.ToCoord];

        Piece fromPiece = fromSeat.Piece;
        PieceColor color = fromPiece.Color;
        PieceKind kind = fromPiece.Kind;
        char name = fromPiece.Name;
        int fromRow = fromSeat.Coord.Row, fromCol = fromSeat.Coord.Col,
            toRow = toSeat.Coord.Row, toCol = toSeat.Coord.Col;
        bool isSameRow = fromRow == toRow, isBottomColor = BottomColor == color;

        var livePieces = GetLivePieces(color, kind, fromCol);
        if (livePieces.Count > 1 && kind > PieceKind.Bishop)
        {
            // 该列有多兵时，需检查多列多兵的情况
            if (kind == PieceKind.Pawn)
                livePieces = GetLivePieces_MultiPawns(color);

            SortPieces(livePieces, isBottomColor);
            zhStr = $"{Piece.PreChars(livePieces.Count)[livePieces.IndexOf(fromPiece)]}{name}";
        }
        else
        {  //将帅, 仕(士),相(象): 不用“前”和“后”区别，因为能退的一定在前，能进的一定在后
            zhStr = $"{name}{Piece.GetColChar(color, Coord.GetCol(fromCol, isBottomColor))}";
        }

        int numOrCol = !isSameRow && Piece.IsLinePiece(kind) ? Math.Abs(fromRow - toRow) - 1 : Coord.GetCol(toCol, isBottomColor);
        zhStr += $"{Piece.MoveChar(isSameRow, isBottomColor == toRow > fromRow)}{Piece.GetColChar(color, numOrCol)}";

        Debug.Assert(GetCoordPairFromZhStr(zhStr).Equals(coordPair));
        return zhStr;
    }

    public CoordPair GetCoordPairFromZhStr(string zhStr)
    {
        Debug.Assert(zhStr.Length == 4);
        PieceColor color = Piece.GetColor_Num(zhStr[3]);
        bool isBottomColor = BottomColor == color;
        int index = 0, movDir = Piece.MoveDir(zhStr[2]),
            absMovDir = movDir * (isBottomColor ? 1 : -1);

        List<Piece> livePieces;
        PieceKind kind = Piece.GetKind_Name(zhStr[0]);
        if (kind != PieceKind.NoKind)
        {   // 首字符为棋子名
            int col = Coord.GetCol(Piece.GetCol(color, zhStr[1]), isBottomColor);
            livePieces = GetLivePieces(color, kind, col);
            Debug.Assert(livePieces.Count > 0);

            // 士、象同列时不分前后，以进、退区分棋子。移动方向为退时，修正index
            if (livePieces.Count == 2)
                index = (movDir == 1) ? 1 : 0;
        }
        else
        {
            kind = Piece.GetKind_Name(zhStr[1]);
            livePieces = kind == PieceKind.Pawn
                ? GetLivePieces_MultiPawns(color) : GetLivePieces(color, kind);
            Debug.Assert(livePieces.Count > 1);

            if (livePieces.Count < 2)
                return CoordPair.Null;

            index = Piece.PreChars(livePieces.Count).IndexOf(zhStr[0]);
        }

        if (livePieces.Count <= index)
            return CoordPair.Null;

        SortPieces(livePieces, isBottomColor);
        Coord fromCoord = GetCoord(livePieces[index]);

        int toNum = Piece.GetCol(color, zhStr[3]) + 1,
            toRow = fromCoord.Row, toCol = Coord.GetCol(toNum - 1, isBottomColor);
        if (Piece.IsLinePiece(kind))
        {
            if (absMovDir != 0)
            {
                toRow += absMovDir * toNum;
                toCol = fromCoord.Col;
            }
        }
        else
        {   // 斜线走子：仕、相、马
            int colAway = Math.Abs(toCol - fromCoord.Col);
            //  相距1或2列
            int rowInc = (kind == PieceKind.Advisor || kind == PieceKind.Bishop)
                ? colAway : (colAway == 1 ? 2 : 1);
            toRow += absMovDir * rowInc;
        }

        return new(fromCoord, this[toRow, toCol].Coord);
    }

    private void SortPieces(List<Piece> pieces, bool isBottomColor)
        => pieces.Sort(delegate (Piece piece1, Piece piece2)
        {
            Coord coord1 = GetCoord(piece1), coord2 = GetCoord(piece2);
            int comp = coord1.Col.CompareTo(coord2.Col);
            if (comp == 0)
                comp = coord1.Row.CompareTo(coord2.Row);

            return isBottomColor ? -comp : comp;
        });

    private List<Piece> GetLivePieces_MultiPawns(PieceColor color)
        => GetLivePieces(color, PieceKind.Pawn)
            .GroupBy(piece => GetCoord(piece).Col)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.AsEnumerable())
            .ToList();

    public CoordPair GetCoordPairFromRowCol(string rowCol)
        => GetCoordPair(int.Parse(rowCol[0].ToString()), int.Parse(rowCol[1].ToString()),
            int.Parse(rowCol[2].ToString()), int.Parse(rowCol[3].ToString()));
    public CoordPair GetCoordPairFromIccs(string iccs)
        => GetCoordPair(int.Parse(iccs[1].ToString()), Coord.ColChars.IndexOf(iccs[0]),
            int.Parse(iccs[3].ToString()), Coord.ColChars.IndexOf(iccs[2]));

    public CoordPair GetCoordPair(int frow, int fcol, int trow, int tcol)
        => new(this[frow, fcol].Coord, this[trow, tcol].Coord);

    public CoordPair GetCoordPair(string pgnText, FileExtType fileExtType)
        => fileExtType switch
        {
            FileExtType.PGNIccs => GetCoordPairFromIccs(pgnText),
            FileExtType.PGNRowCol => GetCoordPairFromRowCol(pgnText),
            _ => GetCoordPairFromZhStr(pgnText)
        };

    // 仅供测试使用
    public string AllSeatsString() => string.Join(" ",
            AllSeats.Select(seat => $"{seat}{Coord.Null}"));

    public override string ToString()
    {
        // 棋盘上边标识字符串
        string[] preStr = {
                "　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n",
                "　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n"
            };

        // 棋盘下边标识字符串
        string[] sufStr = {
                "九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n",
                "９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n"
            };

        // 文本空棋盘
        StringBuilder textBlankBoard =
            new(
                @"┏━┯━┯━┯━┯━┯━┯━┯━┓
┃　│　│　│╲│╱│　│　│　┃
┠─┼─┼─┼─╳─┼─┼─┼─┨
┃　│　│　│╱│╲│　│　│　┃
┠─╬─┼─┼─┼─┼─┼─╬─┨
┃　│　│　│　│　│　│　│　┃
┠─┼─╬─┼─╬─┼─╬─┼─┨
┃　│　│　│　│　│　│　│　┃
┠─┴─┴─┴─┴─┴─┴─┴─┨
┃　　　　　　　　　　　　　　　┃
┠─┬─┬─┬─┬─┬─┬─┬─┨
┃　│　│　│　│　│　│　│　┃
┠─┼─╬─┼─╬─┼─╬─┼─┨
┃　│　│　│　│　│　│　│　┃
┠─╬─┼─┼─┼─┼─┼─╬─┨
┃　│　│　│╲│╱│　│　│　┃
┠─┼─┼─┼─╳─┼─┼─┼─┨
┃　│　│　│╱│╲│　│　│　┃
┗━┷━┷━┷━┷━┷━┷━┷━┛
"
            );
        // 边框粗线

        foreach (var piece in GetLivePieces())
            textBlankBoard[Coord.GetDoubleIndex(GetCoord(piece))] = piece.PrintName;

        int index = BottomColor == PieceColor.Red ? 0 : 1;
        return preStr[index] + textBlankBoard.ToString() + sufStr[index];
    }
}

