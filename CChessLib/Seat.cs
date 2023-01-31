using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CChess;

public class Seat
{
    private Piece _piece;

    internal Seat(Coord coord)
    {
        Coord = coord;
        _piece = Piece.Null;
    }

    public Coord Coord { get; }
    public Piece Piece
    {
        get { return _piece; }
        set
        {
            _piece.Coord = Coord.Null;
            if (value != Piece.Null)
                value.Coord = Coord;

            _piece = value;
        }
    }

    public void MoveTo(Seat toSeat, Piece fillPiece)
    {
        Piece fromPiece = Piece;
        Piece = fillPiece;

        toSeat.Piece = fromPiece;
    }

    public override string ToString() => $"{Coord}:{Piece}";
}

public class Seats
{
    public readonly List<Seat> AllSeats;
    public Seats()
    {
        AllSeats = Coord.AllCoords.Select(coord => new Seat(coord)).ToList();
    }

    public Seat this[int row, int col] { get { return AllSeats[Coord.GetIndex(row, col)]; } }
    public Seat this[(int row, int col) rowCol] { get { return AllSeats[Coord.GetIndex(rowCol.row, rowCol.col)]; } }
    public Seat this[Coord coord] { get { return AllSeats[coord.Index]; } }

    public Coord GetCoord(Piece piece) => AllSeats.First(seat => seat.Piece == piece).Coord;

    /// <summary>
    /// 初始棋盘布局面时，棋子可放置的位置
    /// </summary>
    /// <returns>棋盘位置坐标的列表</returns>
    public List<Coord> PutCoord(Piece piece, bool isBottomColor)
        => (piece.Kind == PieceKind.Rook || piece.Kind == PieceKind.Cannon || piece.Kind == PieceKind.Knight)
                ? Coord.AllCoords
                : piece.PutRowCols(isBottomColor).Select(rowCol => this[rowCol].Coord).ToList();

    /// <summary>
    /// 当前棋盘局面下，棋子可移动位置, 已排除规则不允许行走的位置、同色棋子的位置
    /// </summary>
    /// <returns>棋盘位置坐标的列表</returns>
    public List<Coord> MoveCoord(Piece piece, bool isBottomColor)
        => piece.MoveRowCols(this, isBottomColor).Where(
                rowCol => this[rowCol].Piece.Color != piece.Color).
                Select(rowCol => this[rowCol].Coord).ToList();

    public Piece Done(CoordPair coordPair)
    {
        Seat toSeat = this[coordPair.ToCoord];
        Piece toPiece = toSeat.Piece;

        this[coordPair.FromCoord].MoveTo(toSeat, Piece.Null);
        return toPiece;
    }

    public void Undo(CoordPair coordPair, Piece toPiece)
        => this[coordPair.ToCoord].MoveTo(this[coordPair.FromCoord], toPiece);

    public void Reset() => AllSeats.ForEach(seat => seat.Piece = Piece.Null);

    public string GetFEN() => GetFEN(string.Concat(AllSeats.Select(seat => seat.Piece.Char)));

    public static string GetFEN(string pieceChars)
    {
        StringBuilder fen = new();
        for (int row = Coord.RowCount - 1; row > 0; --row)
        {
            fen.Append(pieceChars.Substring(row * Coord.ColCount, Coord.ColCount)).Append(Piece.FENSplitChar);
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

        string[] fenArray = fen.Split(Piece.FENSplitChar);
        if (fenArray.Length != Coord.RowCount)
            return fen;

        IEnumerable<string> ReverseRow(IEnumerable<string> fenArray) => fenArray.Reverse();
        IEnumerable<string> ReverseCol(IEnumerable<string> fenArray)
            => fenArray.Select(line => string.Concat(line.Reverse()));

        return string.Join(Piece.FENSplitChar, ct switch
        {
            ChangeType.Symmetry_H => ReverseCol(fenArray),
            ChangeType.Symmetry_V => ReverseRow(fenArray),
            _ => ReverseCol(ReverseRow(fenArray)) //ChangeType.Rotate
        });
    }

    public bool SetFEN(string fen, Pieces pieces)
    {
        var fenArray = fen.Split(Piece.FENSplitChar);
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
                    // this[row, col++].Piece = pieces.GetPiece_SeatNull(ch);
                    this[row, col++].Piece = pieces.GetPieces(ch)
                        .First(piece => AllSeats.FindIndex(seat => seat.Piece == piece) == -1);

            row++;
        }

        return true;
    }

    public string GetZhStr(CoordPair coordPair, Pieces pieces)
    {
        string zhStr;
        Seat fromSeat = this[coordPair.FromCoord],
            toSeat = this[coordPair.ToCoord];
        Debug.Assert(!fromSeat.Piece.IsNull);

        Piece fromPiece = fromSeat.Piece;
        PieceColor color = fromPiece.Color;
        PieceKind kind = fromPiece.Kind;
        char name = fromPiece.Name;
        int fromRow = fromSeat.Coord.Row, fromCol = fromSeat.Coord.Col,
            toRow = toSeat.Coord.Row, toCol = toSeat.Coord.Col;
        bool isSameRow = fromRow == toRow, isBottomColor = pieces.IsBottom(color);

        var livePieces = pieces.GetLivePieces(color, kind, fromCol);
        if (livePieces.Count > 1 && kind > PieceKind.Bishop)
        {
            // 该列有多兵时，需检查多列多兵的情况
            if (kind == PieceKind.Pawn)
                livePieces = pieces.GetLivePieces_MultiPawns(color);

            SortPieces(livePieces, isBottomColor);
            zhStr = $"{Piece.PreChars(livePieces.Count)[livePieces.IndexOf(fromPiece)]}{name}";
        }
        else
        {  //将帅, 仕(士),相(象): 不用“前”和“后”区别，因为能退的一定在前，能进的一定在后
            zhStr = $"{name}{Piece.GetColChar(color, Coord.GetCol(fromCol, isBottomColor))}";
        }

        int numOrCol = !isSameRow && Piece.IsLinePiece(kind) ? Math.Abs(fromRow - toRow) - 1 : Coord.GetCol(toCol, isBottomColor);
        zhStr += $"{Piece.MoveChar(isSameRow, isBottomColor == toRow > fromRow)}{Piece.GetColChar(color, numOrCol)}";

        Debug.Assert(GetCoordPair(zhStr, pieces).Equals(coordPair));
        return zhStr;
    }

    public CoordPair GetCoordPair(string zhStr, Pieces pieces)
    {
        Debug.Assert(zhStr.Length == 4);
        PieceColor color = Piece.GetColor_Num(zhStr[3]);
        bool isBottomColor = pieces.IsBottom(color);
        int index = 0, movDir = Piece.MoveDir(zhStr[2]),
            absMovDir = movDir * (isBottomColor ? 1 : -1);

        List<Piece> livePieces;
        PieceKind kind = Piece.GetKind(zhStr[0]);
        if (kind != PieceKind.NoKind)
        {   // 首字符为棋子名
            int col = Coord.GetCol(Piece.GetCol(color, zhStr[1]), isBottomColor);
            livePieces = pieces.GetLivePieces(color, kind, col);
            Debug.Assert(livePieces.Count > 0);

            // 士、象同列时不分前后，以进、退区分棋子。移动方向为退时，修正index
            if (livePieces.Count == 2)
                index = (movDir == 1) ? 1 : 0;
        }
        else
        {
            kind = Piece.GetKind(zhStr[1]);
            livePieces = kind == PieceKind.Pawn
                ? pieces.GetLivePieces_MultiPawns(color) : pieces.GetLivePieces(color, kind);
            Debug.Assert(livePieces.Count > 1);

            if (livePieces.Count < 2)
                return CoordPair.Null;

            index = Piece.PreChars(livePieces.Count).IndexOf(zhStr[0]);
        }

        if (livePieces.Count <= index)
            return CoordPair.Null;

        SortPieces(livePieces, isBottomColor);
        Coord fromCoord = livePieces[index].Coord;

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

    private static void SortPieces(List<Piece> pieces, bool isBottomColor)
    {
        pieces.Sort();
        if (isBottomColor)
            pieces.Reverse();
    }

    public bool IsNull(int row, int col) => this[row, col].Piece.IsNull;
    public bool IsKilled(PieceColor color, Pieces pieces)
    {
        Coord kingCoord = pieces.GetKing(color).Coord;
        var otherColor = color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;

        // 将帅是否对面
        int col = kingCoord.Col;
        Coord otherKingCoord = pieces.GetKing(otherColor).Coord;
        if (col == otherKingCoord.Col)
        {
            int thisRow = kingCoord.Row, otherRow = otherKingCoord.Row,
                lowRow = Math.Min(thisRow, otherRow) + 1, count = Math.Abs(thisRow - otherRow) - 1;
            if (Enumerable.Range(lowRow, count).All(row => IsNull(row, col)))
                return true;
        }

        // 是否正被将军
        return pieces.GetLivePieces(otherColor).Any(
            piece => MoveCoord(piece, pieces.IsBottom(piece.Color)).Contains(kingCoord));
    }

    public bool IsFailed(PieceColor color, Pieces pieces)
        => pieces.GetLivePieces(color).All(piece => CanMoveCoord(piece.Coord, pieces).Count == 0);

    /// <summary>
    /// 可移动位置, 排除将帅对面、被将军的位置
    /// </summary>
    /// <param name="fromCoord"></param>
    /// <returns></returns>
    public List<Coord> CanMoveCoord(Coord fromCoord, Pieces pieces)
    {
        Piece piece = this[fromCoord].Piece;
        List<Coord> coords = MoveCoord(piece, pieces.IsBottom(piece.Color));
        coords.RemoveAll(toCoord => !CanMove(fromCoord, toCoord, pieces));
        return coords;
    }

    /// <summary>
    /// 检测是否可移动, 包括直接杀将、移动后将帅未对面、未被将军
    /// </summary>
    /// <param name="fromCoord"></param>
    /// <param name="toCoord"></param>
    /// <returns></returns>
    public bool CanMove(Coord fromCoord, Coord toCoord, Pieces pieces)
    {
        Seat fromSeat = this[fromCoord];
        Debug.Assert(!fromSeat.Piece.IsNull);

        var color = fromSeat.Piece.Color;
        Seat toSeat = this[toCoord];
        Piece toPiece = toSeat.Piece;

        // 如是对方将帅的位置则直接可走，不用判断是否被将军（如加以判断，则会直接走棋吃将帅）
        if (toPiece.Kind == PieceKind.King)
            return true;

        fromSeat.MoveTo(toSeat, Piece.Null);
        bool killed = IsKilled(color, pieces);
        toSeat.MoveTo(fromSeat, toPiece);
        return !killed;
    }
}