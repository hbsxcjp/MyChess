using System.Diagnostics;

namespace CChess;

public enum PieceColor
{
    Red,
    Black,
    NoColor = -1
}

public enum PieceKind
{
    King,
    Advisor,
    Bishop,
    Knight,
    Rook,
    Cannon,
    Pawn,
    NoKind = -1
}

public abstract class Piece : IComparable
{
    public static readonly Piece Null = new NullPiece();
    public const int KindNum = 7;
    public const char FENSplitChar = '/';

    private const string NameChars = "帅仕相马车炮兵将士象马车炮卒";
    private static readonly string[] NumChars = { "一二三四五六七八九", "１２３４５６７８９" };
    private const string PositionChars = "前中后";
    private const string MoveChars = "退平进";

    protected Piece(PieceColor color)
    {
        Color = color;
        Coord = Coord.Null;
    }

    public PieceColor Color { get; }
    abstract public PieceKind Kind { get; }
    abstract public char Char { get; }
    abstract public char Name { get; }
    virtual public char PrintName { get { return Name; } }
    public bool IsNull { get { return this == Null; } }
    public Coord Coord { get; set; }

    virtual public List<(int row, int col)> PutRowCols(bool isBottomColor) => new();
    abstract public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor);

    override public string ToString()
        => $"{(Color == PieceColor.Red ? "红" : (Color == PieceColor.Black ? "黑" : "无"))}{PrintName}{Char}{Coord}";

    public static int GetColorIndex(char ch) => char.IsUpper(ch) ? 0 : 1;
    public static int GetKindIndex(char ch) => ("KABNRCPkabnrcp".IndexOf(ch)) % KindNum;

    public static PieceKind GetKind(char name) => (PieceKind)(NameChars.IndexOf(name) % KindNum);
    public static bool IsLinePiece(PieceKind kind)
        => (kind == PieceKind.King || kind == PieceKind.Rook || kind == PieceKind.Cannon || kind == PieceKind.Pawn);
    public static char GetColChar(PieceColor color, int col) => NumChars[(int)color][col];
    public static int GetCol(PieceColor color, char colChar) => NumChars[(int)color].IndexOf(colChar);
    public static PieceColor GetColor_Num(char numChar) => NumChars[0].Contains(numChar) ? PieceColor.Red : PieceColor.Black;
    public static string PreChars(int count) => (count == 2 ? "前后" : (count == 3 ? PositionChars : "一二三四五"));
    public static char MoveChar(bool isSameRow, bool isGo) => MoveChars[isSameRow ? 1 : (isGo ? 2 : 0)];
    public static int MoveDir(char movCh) => MoveChars.IndexOf(movCh) - 1;
    public static string PGNZHChars() => $"{NameChars}{NumChars[0]}{NumChars[1]}{PositionChars}{MoveChars}";

    int IComparable.CompareTo(object? obj)
    {
        if (obj is not Piece)
            return 0;

        return CompareTo((Piece)obj);
    }

    public int CompareTo(Piece piece) => Coord.CompareTo(piece.Coord);
}

public class King : Piece
{
    public King(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.King; } }
    override public char Char { get { return Color == PieceColor.Red ? 'K' : 'k'; } }
    override public char Name { get { return Color == PieceColor.Red ? '帅' : '将'; } }

    override public List<(int row, int col)> PutRowCols(bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        int minRow = isBottomColor ? 0 : 7,
            maxRow = isBottomColor ? 2 : 9;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 3; col <= 5; ++col)
                rowCols.Add((row, col));

        return rowCols;
    }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        bool isBottom = Coord.IsBottom;
        int Row = Coord.Row, Col = Coord.Col;
        if (Col > 3)
            rowCols.Add((Row, Col - 1));
        if (Col < 5)
            rowCols.Add((Row, Col + 1));
        if (Row < (isBottom ? 2 : 9))
            rowCols.Add((Row + 1, Col));
        if (Row > (isBottom ? 0 : 7))
            rowCols.Add((Row - 1, Col));

        return rowCols;
    }
}

public class Advisor : Piece
{
    public Advisor(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Advisor; } }
    override public char Char { get { return Color == PieceColor.Red ? 'A' : 'a'; } }
    override public char Name { get { return Color == PieceColor.Red ? '仕' : '士'; } }

    override public List<(int row, int col)> PutRowCols(bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        int minRow = isBottomColor ? 0 : 7,
            maxRow = isBottomColor ? 2 : 9;

        for (int row = minRow; row <= maxRow; row += 2)
            for (int col = 3; col <= 5; col += 2)
                rowCols.Add((row, col));

        rowCols.Add((minRow + 1, 4));
        return rowCols;
    }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        bool isBottom = Coord.IsBottom;
        int Row = Coord.Row, Col = Coord.Col;
        if (Col != 4)
            rowCols.Add((isBottom ? 1 : 8, 4));
        else
        {
            rowCols.Add((Row - 1, Col - 1));
            rowCols.Add((Row - 1, Col + 1));
            rowCols.Add((Row + 1, Col - 1));
            rowCols.Add((Row + 1, Col + 1));
        }

        return rowCols;
    }
}

public class Bishop : Piece
{
    public Bishop(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Bishop; } }
    override public char Char { get { return Color == PieceColor.Red ? 'B' : 'b'; } }
    override public char Name { get { return Color == PieceColor.Red ? '相' : '象'; } }

    override public List<(int row, int col)> PutRowCols(bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        int minRow = isBottomColor ? 0 : 5,
            midRow = isBottomColor ? 2 : 7,
            maxRow = isBottomColor ? 4 : 9;
        for (int row = minRow; row <= maxRow; row += 4)
            for (int col = 2; col < Coord.ColCount; col += 4)
                rowCols.Add((row, col));
        for (int col = 0; col < Coord.ColCount; col += 4)
            rowCols.Add((midRow, col));

        return rowCols;
    }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        List<Coord> coords = new();
        bool isBottom = Coord.IsBottom;
        int Row = Coord.Row, Col = Coord.Col;
        int maxRow = isBottom ? (Coord.RowCount - 1) / 2 : Coord.RowCount - 1;
        void AddRowCol(int row, int col)
        {
            if (seats.IsNull((row + Row) / 2, (col + Col) / 2))
                rowCols.Add((row, col));
        }

        if (Row < maxRow)
        {
            if (Col > 0)
                AddRowCol(Row + 2, Col - 2);
            if (Col < Coord.ColCount - 1)
                AddRowCol(Row + 2, Col + 2);
        }
        if (Row > 0)
        {
            if (Col > 0)
                AddRowCol(Row - 2, Col - 2);
            if (Col < Coord.ColCount - 1)
                AddRowCol(Row - 2, Col + 2);
        }

        return rowCols;
    }
}

public class Knight : Piece
{
    public Knight(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Knight; } }
    override public char Char { get { return Color == PieceColor.Red ? 'N' : 'n'; } }
    override public char Name { get { return '马'; } }
    override public char PrintName { get { return Color == PieceColor.Red ? Name : '馬'; } }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        // List<(int row, int col)> rowCols = new();
        int Row = Coord.Row, Col = Coord.Col;
        ((int row, int col) to, (int row, int col) leg)[] allToLegRowCols =
        {
                ((Row - 2, Col - 1), (Row - 1, Col))  ,
                ((Row - 2, Col + 1), (Row - 1, Col)),
                ((Row - 1, Col - 2), (Row, Col - 1)),
                ((Row - 1, Col + 2), (Row, Col + 1)),
                ((Row + 1, Col - 2), (Row, Col - 1)),
                ((Row + 1, Col + 2), (Row, Col + 1)),
                ((Row + 2, Col - 1), (Row + 1, Col)),
                ((Row + 2, Col + 1), (Row + 1, Col))
            };
        // foreach (var (to, leg) in allToLegRowCols)
        // {
        //     if (Coord.IsValid(to.row, to.col) && (board[leg.row, leg.col].Piece.IsNull))
        //         rowCols.Add((to.row, to.col));
        // }

        // return rowCols;
        return allToLegRowCols.Where(
            toLeg => Coord.IsValid(toLeg.to.row, toLeg.to.col) && (seats.IsNull(toLeg.leg.row, toLeg.leg.col)))
            .Select(toLeg => (toLeg.to.row, toLeg.to.col)).ToList();
    }
}

public class Rook : Piece
{
    public Rook(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Rook; } }
    override public char Char { get { return Color == PieceColor.Red ? 'R' : 'r'; } }
    override public char Name { get { return '车'; } }
    override public char PrintName { get { return Color == PieceColor.Red ? Name : '車'; } }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        int Row = Coord.Row, Col = Coord.Col;
        bool AddRowCol(int row, int col)
        {
            rowCols.Add((row, col));
            return seats.IsNull(row, col);
        }

        for (int r = Row - 1; r >= 0; --r)
            if (!AddRowCol(r, Col))
                break;

        for (int r = Row + 1; r < Coord.RowCount; ++r)
            if (!AddRowCol(r, Col))
                break;

        for (int c = Col - 1; c >= 0; --c)
            if (!AddRowCol(Row, c))
                break;

        for (int c = Col + 1; c < Coord.ColCount; ++c)
            if (!AddRowCol(Row, c))
                break;

        return rowCols;
    }
}

public class Cannon : Piece
{
    public Cannon(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Cannon; } }
    override public char Char { get { return Color == PieceColor.Red ? 'C' : 'c'; } }
    override public char Name { get { return '炮'; } }
    override public char PrintName { get { return Color == PieceColor.Red ? Name : '砲'; } }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        int Row = Coord.Row, Col = Coord.Col;
        bool skiped = false;
        bool AddCoordToBreak(int row, int col)
        {
            bool isNull = seats.IsNull(row, col);
            if (!skiped)
            {
                if (isNull)
                    rowCols.Add((row, col));
                else
                    skiped = true;
            }
            else if (!isNull)
            {
                rowCols.Add((row, col));
                return true;
            }

            return false;
        }

        for (int r = Row - 1; r >= 0; --r)
            if (AddCoordToBreak(r, Col))
                break;

        skiped = false;
        for (int r = Row + 1; r < Coord.RowCount; ++r)
            if (AddCoordToBreak(r, Col))
                break;

        skiped = false;
        for (int c = Col - 1; c >= 0; --c)
            if (AddCoordToBreak(Row, c))
                break;

        skiped = false;
        for (int c = Col + 1; c < Coord.ColCount; ++c)
            if (AddCoordToBreak(Row, c))
                break;

        return rowCols;
    }
}

public class Pawn : Piece
{
    public Pawn(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Pawn; } }
    override public char Char { get { return Color == PieceColor.Red ? 'P' : 'p'; } }
    override public char Name { get { return Color == PieceColor.Red ? '兵' : '卒'; } }

    override public List<(int row, int col)> PutRowCols(bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        int minRow = isBottomColor ? 3 : 5,
            maxRow = isBottomColor ? 4 : 6;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 0; col < Coord.ColCount; col += 2)
                rowCols.Add((row, col));

        minRow = isBottomColor ? 5 : 0;
        maxRow = isBottomColor ? 9 : 4;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 0; col < Coord.ColCount; ++col)
                rowCols.Add((row, col));

        return rowCols;
    }

    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor)
    {
        List<(int row, int col)> rowCols = new();
        bool isBottom = Coord.IsBottom;
        int Row = Coord.Row, Col = Coord.Col;
        // 已过河
        if (isBottomColor != isBottom)
        {
            if (Col > 0)
                rowCols.Add((Row, Col - 1));
            if (Col < Coord.ColCount - 1)
                rowCols.Add((Row, Col + 1));
        }

        if (isBottomColor && Row < Coord.RowCount - 1)
            rowCols.Add((Row + 1, Col));
        else if (!isBottomColor && Row > 0)
            rowCols.Add((Row - 1, Col));

        return rowCols;
    }
}

public class NullPiece : Piece
{
    public NullPiece() : base(PieceColor.NoColor) { }

    override public PieceKind Kind { get { return PieceKind.NoKind; } }
    override public char Char { get { return '_'; } }
    override public char Name { get { return '空'; } }
    override public List<(int row, int col)> PutRowCols(bool isBottomColor) => new();
    override public List<(int row, int col)> MoveRowCols(Seats seats, bool isBottomColor) => new();
}

public class Pieces
{
    private Piece[][][] _pieces;
    private const int ColorNum = 2;

    public Pieces()
    {
        static Piece[] getKindPieces(PieceColor color, Type type, int num)
        {
            var kindPieces = new Piece[num];
            var constructorInfo = type.GetConstructor(new Type[] { typeof(PieceColor) });
            if (constructorInfo != null)
                for (int i = 0; i < num; i++)
                    kindPieces[i] = (Piece)constructorInfo.Invoke(new object[] { color });

            return kindPieces;
        }

        static Piece[][] getColorPieces(PieceColor color)
        {
            Type[] pieceType = { typeof(King), typeof(Advisor), typeof(Bishop),
                    typeof(Knight), typeof(Rook), typeof(Cannon), typeof(Pawn) };
            int[] KindNums = { 1, 2, 2, 2, 2, 2, 5 };
            Piece[][] pieces = new Piece[Piece.KindNum][];
            for (int k = 0; k < Piece.KindNum; k++)
                pieces[k] = getKindPieces(color, pieceType[k], KindNums[k]);

            return pieces;
        }

        _pieces = new Piece[ColorNum][][];
        for (int c = 0; c < ColorNum; c++)
            _pieces[c] = getColorPieces((PieceColor)c);
    }

    // public Piece GetPiece_SeatNull(char ch)
    // {
    //     foreach (var piece in _pieces[Piece.GetColorIndex(ch)][Piece.GetKindIndex(ch)])
    //         if (piece.Coord.IsNull)
    //             return piece;

    //     return Piece.Null;
    // }

    public bool IsBottom(PieceColor color)
    {
        Coord kingCoord = GetKing(PieceColor.Red).Coord;
        return ((kingCoord.IsNull || kingCoord.IsBottom) ? PieceColor.Red : PieceColor.Black) == color;
    }

    public Piece GetKing(PieceColor color) => _pieces[(int)color][(int)PieceKind.King][0];

    public List<Piece> GetPieces()
    {
        List<Piece> pieces = GetPieces(PieceColor.Red);
        pieces.AddRange(GetPieces(PieceColor.Black));

        return pieces;
    }

    public List<Piece> GetPieces(PieceColor color)
    {
        List<Piece> pieces = new();
        foreach (var kindPieces in _pieces[(int)color])
            pieces.AddRange(kindPieces);

        return pieces;
    }

    public List<Piece> GetPieces(PieceColor color, PieceKind kind)
        => _pieces[(int)color][(int)kind].ToList();
    public List<Piece> GetPieces(char ch)
        => _pieces[Piece.GetColorIndex(ch)][Piece.GetKindIndex(ch)].ToList();

    public List<Piece> GetLivePieces(PieceColor color)
        => LivePieces(GetPieces(color));

    public List<Piece> GetLivePieces(PieceColor color, PieceKind kind)
         => LivePieces(GetPieces(color, kind));

    public List<Piece> GetLivePieces(PieceColor color, PieceKind kind, int col)
        => GetLivePieces(color, kind).Where(piece => piece.Coord.Col == col).ToList();

    private static List<Piece> LivePieces(IEnumerable<Piece> pieces)
        => pieces.Where(piece => !piece.Coord.IsNull).ToList();

    /// <summary>
    /// 获取多兵列
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Piece> GetLivePieces_MultiPawns(PieceColor color)
    {
        // List<Piece> pawnPieces = new();
        // Dictionary<int, List<Piece>> colPieces = new();
        // foreach (Piece piece in GetLivePieces(color, PieceKind.Pawn))
        // {
        //     int col = piece.Coord.Col;
        //     if (!colPieces.ContainsKey(col))
        //         colPieces[col] = new();

        //     colPieces[col].Add(piece);
        // }

        // foreach (var pieces in colPieces.Values)
        //     if (pieces.Count > 1)
        //         pawnPieces.AddRange(pieces);

        // return pawnPieces;

        List<Piece> pieces = GetLivePieces(color, PieceKind.Pawn), result = new();
        Debug.Assert(pieces.Count > 1);

        pieces.Sort();
        Piece prePiece = pieces[0];
        bool preAdded = false;
        for (int i = 1; i < pieces.Count; i++)
        {
            Piece piece = pieces[i];
            bool added = prePiece.Coord.Col == piece.Coord.Col;
            if (added)
            {
                if (!preAdded)
                    result.Add(prePiece);

                result.Add(piece);
            }

            prePiece = piece;
            preAdded = added;
        }

        return result;
    }

}