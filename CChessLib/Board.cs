#define DEBUG

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CChess;

internal class Board
{
    // [Color][Kind][Index]
    private readonly Piece[][][] _pieces;
    // [row, col]
    private readonly Seat[,] _seats;

    public Board()
    {
        _pieces = Piece.CreatPieces();
        _seats = Seat.CreatSeats();

        BottomColor = PieceColor.Red;
    }

    public PieceColor BottomColor { get; set; }

    public Seat this[int row, int col] { get { return _seats[row, col]; } }
    public Seat this[Coord coord] { get { return this[coord.row, coord.col]; } }

    public bool IsBottomColor(PieceColor color) => BottomColor == color;

    public List<Coord> GetAllCoords()
    {
        List<Coord> coords = new();
        foreach(var seat in _seats)
            coords.Add(seat.Coord);

        return coords;
    }

    private Seat GetKingSeat(PieceColor color) => _pieces[(int)color][(int)PieceKind.King][0].Seat;
    public bool IsKilled(PieceColor color)
    {
        Coord kingCoord = GetKingSeat(color).Coord;
        var otherColor = color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;

        // 将帅是否对面
        int col = kingCoord.col;
        Coord otherKingCoord = GetKingSeat(otherColor).Coord;
        if(col == otherKingCoord.col)
        {
            int thisRow = kingCoord.row, otherRow = otherKingCoord.row,
                lowRow = Math.Min(thisRow, otherRow) + 1, count = Math.Abs(thisRow - otherRow) - 1;
            if(Enumerable.Range(lowRow, count).Select(row => this[row, col]).All(seat => seat.HasNullPiece))
                return true;
        }

        // 是否正被将军
        return LivePieces(otherColor).Any(piece => piece.MoveCoord(this).Contains(kingCoord));
    }
    public bool IsFailed(PieceColor color)
        => LivePieces(color).All(piece => CanMoveCoord(piece.Coord).Count == 0);

    /// <summary>
    /// 可移动位置, 排除将帅对面、被将军的位置
    /// </summary>
    /// <param name="fromCoord"></param>
    /// <returns></returns>
    public List<Coord> CanMoveCoord(Coord fromCoord)
    {
        List<Coord> coords = this[fromCoord].Piece.MoveCoord(this);
        coords.RemoveAll(toCoord => !CanMove(fromCoord, toCoord));
        return coords;
    }

    /// <summary>
    /// 检测是否可移动, 包括直接杀将、移动后将帅未对面、未被将军
    /// </summary>
    /// <param name="fromCoord"></param>
    /// <param name="toCoord"></param>
    /// <returns></returns>
    public bool CanMove(Coord fromCoord, Coord toCoord)
    {
        Seat fromSeat = this[fromCoord];
        Debug.Assert(!fromSeat.HasNullPiece);

        var color = fromSeat.Piece.Color;
        Seat toSeat = this[toCoord];
        Piece toPiece = toSeat.Piece;

        // 如是对方将帅的位置则直接可走，不用判断是否被将军（如加以判断，则会直接走棋吃将帅）
        if(toPiece.Kind == PieceKind.King)
            return true;

        fromSeat.MoveTo(toSeat, Piece.Null);
        bool killed = IsKilled(color);
        toSeat.MoveTo(fromSeat, toPiece);
        return !killed;
    }

    public void Reset()
    {
        foreach(var seat in _seats)
            seat.Piece = Piece.Null;
    }

    public string GetFEN()
    {
        string pieceChars = "";
        foreach(var seat in _seats)
            pieceChars += seat.Piece.Char;

        return GetFEN(pieceChars);
    }
    public static string GetFEN(string pieceChars)
    {
        string fen = "";
        for(int row = Coord.RowCount - 1;row >= 0;--row)
            fen += pieceChars[(row * Coord.ColCount)..((row + 1) * Coord.ColCount)] + Piece.FENSplitChar;

        return Regex.Replace(fen.Remove(fen.Length - 1), $"{Piece.Null.Char}+",
            (Match match) => match.Value.Length.ToString());
    }
    public static string GetFEN(string fen, ChangeType ct)
    {
        if(ct == ChangeType.NoChange)
            return fen;

        if(ct == ChangeType.Exchange)
        {
            string resultFen = "";
            foreach(var ch in fen)
                resultFen += char.IsLetter(ch) ? (char.IsLower(ch) ? char.ToUpper(ch) : char.ToLower(ch)) : ch;
            return resultFen;
        }

        string[] fenArray = fen.Split(Piece.FENSplitChar);
        if(fenArray.Length != Coord.RowCount)
            return fen;

        IEnumerable<string> result;
        IEnumerable<string> ReverseRow(IEnumerable<string> fenArray) => fenArray.Reverse();
        IEnumerable<string> ReverseCol(IEnumerable<string> fenArray)
        {
            List<string> lines = new();
            foreach(var line in fenArray)
                lines.Add(string.Concat(line.Reverse()));
            return lines;
        }

        if(ct == ChangeType.Symmetry_H)
            result = ReverseCol(fenArray);
        else if(ct == ChangeType.Symmetry_V)
            result = ReverseRow(fenArray);
        else // if(ct == ChangeType.Rotate)
            result = ReverseCol(ReverseRow(fenArray));

        return string.Join(Piece.FENSplitChar, result);
    }
    public bool SetFEN(string fen)
    {
        Piece GetNotAtSeatPiece(char ch)
        {
            foreach(var piece in _pieces[Piece.GetColorIndex(ch)][Piece.GetKindIndex(ch)])
                if(piece.Seat.IsNull)
                    return piece;

            return Piece.Null;
        }

        var fenArray = fen.Split(Piece.FENSplitChar);
        if(fenArray.Length != Coord.RowCount)
            return false;

        Reset();
        int row = 0;
        foreach(var line in fenArray.Reverse())
        {
            int col = 0;
            foreach(char ch in line)
                if(char.IsDigit(ch))
                    col += Convert.ToInt32(ch.ToString());
                else
                    this[row, col++].Piece = GetNotAtSeatPiece(ch);

            row++;
        }

        Seat kingSeat = GetKingSeat(PieceColor.Red);
        Debug.Assert(!kingSeat.IsNull);

        BottomColor = kingSeat.Coord.IsBottom ? PieceColor.Red : PieceColor.Black;
        return true;
    }
    
    public bool ChangeLayout(ChangeType ct) => SetFEN(GetFEN(GetFEN(), ct));

    public string GetZhStrFromCoordPair(CoordPair coordPair)
    {
        string zhStr;
        Seat fromSeat = this[coordPair.FromCoord],
            toSeat = this[coordPair.ToCoord];
        Debug.Assert(!fromSeat.HasNullPiece);

        Piece fromPiece = fromSeat.Piece;
        PieceColor color = fromPiece.Color;
        PieceKind kind = fromPiece.Kind;
        char name = fromPiece.Name;
        int fromRow = fromSeat.Coord.row, fromCol = fromSeat.Coord.col,
            toRow = toSeat.Coord.row, toCol = toSeat.Coord.col;
        bool isSameRow = fromRow == toRow, isBottomColor = IsBottomColor(color);
        var pieces = LivePieces(color, kind, fromCol);
        if(pieces.Count > 1 && kind > PieceKind.Bishop)
        {
            // 有两条纵线，每条纵线上都有一个以上的兵
            if(kind == PieceKind.Pawn)
                pieces = LivePieces_MultiColPawns(color);

            pieces.Sort();
            if(isBottomColor)
                pieces.Reverse();
            int index = pieces.IndexOf(fromPiece);
            zhStr = $"{Piece.PreChars(pieces.Count)[index]}{name}";
        }
        else
        {  //将帅, 仕(士),相(象): 不用“前”和“后”区别，因为能退的一定在前，能进的一定在后
            char colChar = Piece.GetColChar(color, Coord.GetCol(fromCol, isBottomColor));
            zhStr = $"{name}{colChar}";
        }

        char movChar = Piece.MoveChar(isSameRow, isBottomColor == toRow > fromRow);
        int numOrCol = !isSameRow && Piece.IsLinePiece(kind)
            ? Math.Abs(fromRow - toRow) - 1
            : Coord.GetCol(toCol, isBottomColor);
        char toNumColChar = Piece.GetColChar(color, numOrCol);
        zhStr += $"{movChar}{toNumColChar}";

#if DEBUG
        CoordPair checkCoordPair = GetCoordPairFromZhstr(zhStr);
        Debug.Assert(checkCoordPair.FromCoord == coordPair.FromCoord
            && checkCoordPair.ToCoord == coordPair.ToCoord);
#endif
        return zhStr;
    }
    public CoordPair GetCoordPairFromZhstr(string zhStr)
    {
        Debug.Assert(zhStr.Length == 4);
        PieceColor color = Piece.GetColor(zhStr[3]);
        bool isBottomColor = IsBottomColor(color);
        int index = 0, movDir = Piece.MoveDir(zhStr[2]),
            absMovDir = movDir * (isBottomColor ? 1 : -1);

        List<Piece> pieces;
        PieceKind kind = Piece.GetKind(zhStr[0]);
        if(kind != PieceKind.NoKind)
        {   // 首字符为棋子名
            int col = Coord.GetCol(Piece.GetCol(color, zhStr[1]), isBottomColor);
            pieces = LivePieces(color, kind, col);
            //Debug.Assert(pieces.Count > 0);
            if(pieces.Count == 0)
                return CoordPair.Null;

            // 士、象同列时不分前后，以进、退区分棋子。移动方向为退时，修正index
            if(pieces.Count == 2)
                index = (movDir == 1) ? 1 : 0;
        }
        else
        {
            kind = Piece.GetKind(zhStr[1]);
            pieces = kind == PieceKind.Pawn ? LivePieces_MultiColPawns(color) : LivePieces(color, kind);
            //Debug.Assert(pieces.Count > 1);
            if(pieces.Count < 2)
                return CoordPair.Null;

            index = Piece.PreChars(pieces.Count).IndexOf(zhStr[0]);
        }

        if(pieces.Count <= index)
            return CoordPair.Null;

        pieces.Sort();
        if(isBottomColor)
            pieces.Reverse();
        Coord fromCoord = pieces[index].Coord;
        int toNum = Piece.GetCol(color, zhStr[3]) + 1,
            toRow = fromCoord.row,
            toCol = Coord.GetCol(toNum - 1, isBottomColor);
        if(Piece.IsLinePiece(kind))
        {
            if(absMovDir != 0)
            {
                toRow += absMovDir * toNum;
                toCol = fromCoord.col;
            }
        }
        else
        {   // 斜线走子：仕、相、马
            int colAway = Math.Abs(toCol - fromCoord.col);
            //  相距1或2列
            int rowInc = (kind == PieceKind.Advisor || kind == PieceKind.Bishop)
                ? colAway : (colAway == 1 ? 2 : 1);
            toRow += absMovDir * rowInc;
        }

        return new(fromCoord, this[toRow, toCol].Coord);
    }

    public CoordPair GetCoordPairFromRowCol(string rowCol)
    => GetCoordPair(int.Parse(rowCol[0].ToString()), int.Parse(rowCol[1].ToString()),
            int.Parse(rowCol[2].ToString()), int.Parse(rowCol[3].ToString()));
    public CoordPair GetCoordPairFromIccs(string iccs)
    => GetCoordPair(int.Parse(iccs[1].ToString()), Coord.ColChars.IndexOf(iccs[0]),
            int.Parse(iccs[3].ToString()), Coord.ColChars.IndexOf(iccs[2]));
    
    public List<Piece> Pieces(PieceColor color)
    {
        List<Piece> pieces = new();
        foreach(var kindPieces in _pieces[(int)color])
            pieces.AddRange(kindPieces);

        return pieces;
    }
    public List<Piece> LivePieces(PieceColor color) => LivePieces(Pieces(color));

    private List<Piece> LivePieces(PieceColor color, PieceKind kind)
        => LivePieces(_pieces[(int)color][(int)kind]);
    private List<Piece> LivePieces(PieceColor color, PieceKind kind, int col)
        => LivePieces(color, kind).Where(piece => piece.Coord.col == col).ToList();
    private List<Piece> LivePieces_MultiColPawns(PieceColor color)
    {
        List<Piece> pawnPieces = new();
        Dictionary<int, List<Piece>> colPieces = new();
        foreach(Piece piece in LivePieces(color, PieceKind.Pawn))
        {
            int col = piece.Coord.col;
            if(!colPieces.ContainsKey(col))
                colPieces[col] = new();

            colPieces[col].Add(piece);
        }

        foreach(var pieces in colPieces.Values)
            if(pieces.Count > 1)
                pawnPieces.AddRange(pieces);

        return pawnPieces;
    }

    private static List<Piece> LivePieces(IEnumerable<Piece> pieces)
        => pieces.Where(piece => !piece.Seat.IsNull).ToList();

    private CoordPair GetCoordPair(int frow, int fcol, int trow, int tcol)
        => new(this[frow, fcol].Coord, this[trow, tcol].Coord);
        
    override public string ToString()
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

        foreach(var seat in _seats)
            if(!seat.HasNullPiece)
                textBlankBoard[Coord.GetDoubleIndex(seat.Coord)] = seat.Piece.PrintName;

        int index = (int)BottomColor;
        return preStr[index] + textBlankBoard.ToString() + sufStr[index];
    }
}

