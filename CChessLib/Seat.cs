using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CChess;

public class Seat
{
    public static readonly Seat Null = new(Coord.Null);
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
            _piece.Seat = Null;
            if (value != Piece.Null)
                value.Seat = this;

            _piece = value;
        }
    }
    public bool IsNull { get { return this == Null; } }

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
    private Seat[,] _seats;

    public Seats()
    {
        _seats = new Seat[Coord.RowCount, Coord.ColCount];
        foreach (Coord coord in Coord.CreatCoords())
            _seats[coord.Row, coord.Col] = new(coord);
    }

    public Seat this[int row, int col] { get { return _seats[row, col]; } }
    public Seat this[Coord coord] { get { return _seats[coord.Row, coord.Col]; } }

    public List<Seat> GetAllSeats()
    {
        List<Seat> seats = new();
        foreach (var seat in _seats)
            seats.Add(seat);

        return seats;
    }
    public List<Coord> GetAllCoords() => GetAllSeats().Select(seat => seat.Coord).ToList();

    public Piece Done(CoordPair coordPair)
    {
        Seat toSeat = this[coordPair.ToCoord];
        Piece toPiece = toSeat.Piece;

        this[coordPair.FromCoord].MoveTo(toSeat, Piece.Null);
        return toPiece;
    }

    public void Undo(CoordPair coordPair, Piece toPiece)
        => this[coordPair.ToCoord].MoveTo(this[coordPair.FromCoord], toPiece);

    public void Reset()
    {
        foreach (var seat in _seats)
            seat.Piece = Piece.Null;
    }
    public string GetFEN()
    {
        StringBuilder pieceChars = new();
        foreach (var seat in _seats)
            pieceChars.Append(seat.Piece.Char);

        return GetFEN(pieceChars.ToString());
    }
    public static string GetFEN(string pieceChars)
    {
        StringBuilder fen = new();
        for (int row = Coord.RowCount - 1; row >= 0; --row)
        {
            fen.Append(pieceChars[(row * Coord.ColCount)..((row + 1) * Coord.ColCount)]);
            if (row > 0)
                fen.Append(Piece.FENSplitChar);
        }

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
        IEnumerable<string> ReverseCol(IEnumerable<string> fenArray) => fenArray.Select(line => string.Concat(line.Reverse()));

        IEnumerable<string> values;
        if (ct == ChangeType.Symmetry_H)
            values = ReverseCol(fenArray);
        else if (ct == ChangeType.Symmetry_V)
            values = ReverseRow(fenArray);
        else // if(ct == ChangeType.Rotate)
            values = ReverseCol(ReverseRow(fenArray));

        return string.Join(Piece.FENSplitChar, values);
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
                    _seats[row, col++].Piece = pieces.GetNotAtSeatPiece(ch);

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
        var livePieces = pieces.LivePieces(color, kind, fromCol);
        if (livePieces.Count > 1 && kind > PieceKind.Bishop)
        {
            // 有两条纵线，每条纵线上都有一个以上的兵
            if (kind == PieceKind.Pawn)
                livePieces = pieces.LivePieces_MultiColPawns(color);

            livePieces.Sort();
            if (isBottomColor)
                livePieces.Reverse();
            int index = livePieces.IndexOf(fromPiece);
            zhStr = $"{Piece.PreChars(livePieces.Count)[index]}{name}";
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

        Debug.Assert(GetCoordPair(zhStr, pieces).Equals(coordPair));
        return zhStr;
    }
    public CoordPair GetCoordPair(string zhStr, Pieces pieces)
    {
        Debug.Assert(zhStr.Length == 4);
        PieceColor color = Piece.GetColor(zhStr[3]);
        bool isBottomColor = pieces.IsBottom(color);
        int index = 0, movDir = Piece.MoveDir(zhStr[2]),
            absMovDir = movDir * (isBottomColor ? 1 : -1);

        List<Piece> livePieces;
        PieceKind kind = Piece.GetKind(zhStr[0]);
        if (kind != PieceKind.NoKind)
        {   // 首字符为棋子名
            int col = Coord.GetCol(Piece.GetCol(color, zhStr[1]), isBottomColor);
            livePieces = pieces.LivePieces(color, kind, col);

            Debug.Assert(livePieces.Count > 0);
            if (livePieces.Count == 0)
                return CoordPair.Null;

            // 士、象同列时不分前后，以进、退区分棋子。移动方向为退时，修正index
            if (livePieces.Count == 2)
                index = (movDir == 1) ? 1 : 0;
        }
        else
        {
            kind = Piece.GetKind(zhStr[1]);
            livePieces = kind == PieceKind.Pawn ? pieces.LivePieces_MultiColPawns(color) : pieces.LivePieces(color, kind);

            Debug.Assert(livePieces.Count > 1);
            if (livePieces.Count < 2)
                return CoordPair.Null;

            index = Piece.PreChars(livePieces.Count).IndexOf(zhStr[0]);
        }

        if (livePieces.Count <= index)
            return CoordPair.Null;

        livePieces.Sort();
        if (isBottomColor)
            livePieces.Reverse();
        Coord fromCoord = livePieces[index].Coord;
        int toNum = Piece.GetCol(color, zhStr[3]) + 1,
            toRow = fromCoord.Row,
            toCol = Coord.GetCol(toNum - 1, isBottomColor);
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
}