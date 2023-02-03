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

public abstract class Piece
{
    public static readonly Piece Null = new NullPiece();

    public const int KindNum = 7;
    private const string NameChars = "帅仕相马车炮兵将士象马车炮卒";
    private static readonly string[] NumChars = { "一二三四五六七八九", "１２３４５６７８９" };
    private const string PositionChars = "前中后";
    private const string MoveChars = "退平进";

    protected Piece(PieceColor color) { Color = color; }

    public PieceColor Color { get; }
    abstract public PieceKind Kind { get; }
    abstract public char Char { get; }
    abstract public char Name { get; }
    virtual public char PrintName { get { return Name; } }

    virtual public List<(int row, int col)> PutRowCols(bool isBottomColor) => new();
    abstract public List<(int row, int col)> MoveRowCols(Board board);

    public static PieceColor GetColor(char ch) => char.IsUpper(ch) ? PieceColor.Red : PieceColor.Black;
    public static PieceKind GetKind(char ch) => (PieceKind)(("KABNRCPkabnrcp".IndexOf(ch)) % KindNum);

    public static PieceKind GetKind_Name(char name) => (PieceKind)(NameChars.IndexOf(name) % KindNum);
    public static bool IsLinePiece(PieceKind kind)
        => (kind == PieceKind.King || kind == PieceKind.Rook || kind == PieceKind.Cannon || kind == PieceKind.Pawn);

    public static char GetColChar(PieceColor color, int col) => NumChars[(int)color][col];
    public static int GetCol(PieceColor color, char colChar) => NumChars[(int)color].IndexOf(colChar);

    public static PieceColor GetColor_Num(char numChar) => NumChars[0].Contains(numChar) ? PieceColor.Red : PieceColor.Black;
    public static string PreChars(int count) => (count == 2 ? "前后" : (count == 3 ? PositionChars : "一二三四五"));
    public static char MoveChar(bool isSameRow, bool isGo) => MoveChars[isSameRow ? 1 : (isGo ? 2 : 0)];
    public static int MoveDir(char movCh) => MoveChars.IndexOf(movCh) - 1;
    public static string PGNZHChars() => $"{NameChars}{NumChars[0]}{NumChars[1]}{PositionChars}{MoveChars}";

    override public string ToString()
        => $"{(Color == PieceColor.Red ? "红" : (Color == PieceColor.Black ? "黑" : "无"))}{PrintName}{Char}";
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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        List<(int row, int col)> rowCols = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        if (fCol > 3)
            rowCols.Add((fRow, fCol - 1));
        if (fCol < 5)
            rowCols.Add((fRow, fCol + 1));
        if (fRow < (isBottom ? 2 : 9))
            rowCols.Add((fRow + 1, fCol));
        if (fRow > (isBottom ? 0 : 7))
            rowCols.Add((fRow - 1, fCol));

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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        List<(int row, int col)> rowCols = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        if (fCol != 4)
            rowCols.Add((isBottom ? 1 : 8, 4));
        else
        {
            rowCols.Add((fRow - 1, fCol - 1));
            rowCols.Add((fRow - 1, fCol + 1));
            rowCols.Add((fRow + 1, fCol - 1));
            rowCols.Add((fRow + 1, fCol + 1));
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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        List<(int row, int col)> rowCols = new();
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        int maxRow = isBottom ? (Coord.RowCount - 1) / 2 : Coord.RowCount - 1;
        void AddRowCol(int row, int col)
        {
            if (board.IsNull((row + fRow) / 2, (col + fCol) / 2))
                rowCols.Add((row, col));
        }

        if (fRow < maxRow)
        {
            if (fCol > 0)
                AddRowCol(fRow + 2, fCol - 2);
            if (fCol < Coord.ColCount - 1)
                AddRowCol(fRow + 2, fCol + 2);
        }
        if (fRow > 0)
        {
            if (fCol > 0)
                AddRowCol(fRow - 2, fCol - 2);
            if (fCol < Coord.ColCount - 1)
                AddRowCol(fRow - 2, fCol + 2);
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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        Coord coord = board.GetCoord(this);
        int row = coord.Row, col = coord.Col;
        ((int row, int col) to, (int row, int col) leg)[] allToLegRowCols =
        {
                ((row - 2, col - 1), (row - 1, col))  ,
                ((row - 2, col + 1), (row - 1, col)),
                ((row - 1, col - 2), (row, col - 1)),
                ((row - 1, col + 2), (row, col + 1)),
                ((row + 1, col - 2), (row, col - 1)),
                ((row + 1, col + 2), (row, col + 1)),
                ((row + 2, col - 1), (row + 1, col)),
                ((row + 2, col + 1), (row + 1, col))
            };

        return allToLegRowCols.Where(
            toLeg => Coord.IsValid(toLeg.to.row, toLeg.to.col) && (board.IsNull(toLeg.leg.row, toLeg.leg.col)))
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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        List<(int row, int col)> rowCols = new();
        Coord coord = board.GetCoord(this);
        int fRow = coord.Row, fCol = coord.Col;
        bool AddRowCol(int row, int col)
        {
            rowCols.Add((row, col));
            return board.IsNull(row, col);
        }

        for (int r = fRow - 1; r >= 0; --r)
            if (!AddRowCol(r, fCol))
                break;

        for (int r = fRow + 1; r < Coord.RowCount; ++r)
            if (!AddRowCol(r, fCol))
                break;

        for (int c = fCol - 1; c >= 0; --c)
            if (!AddRowCol(fRow, c))
                break;

        for (int c = fCol + 1; c < Coord.ColCount; ++c)
            if (!AddRowCol(fRow, c))
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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        List<(int row, int col)> rowCols = new();
        Coord coord = board.GetCoord(this);
        int fRow = coord.Row, fCol = coord.Col;
        bool skiped = false;
        bool AddCoordToBreak(int row, int col)
        {
            bool isNull = board.IsNull(row, col);
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

        for (int r = fRow - 1; r >= 0; --r)
            if (AddCoordToBreak(r, fCol))
                break;

        skiped = false;
        for (int r = fRow + 1; r < Coord.RowCount; ++r)
            if (AddCoordToBreak(r, fCol))
                break;

        skiped = false;
        for (int c = fCol - 1; c >= 0; --c)
            if (AddCoordToBreak(fRow, c))
                break;

        skiped = false;
        for (int c = fCol + 1; c < Coord.ColCount; ++c)
            if (AddCoordToBreak(fRow, c))
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

    override public List<(int row, int col)> MoveRowCols(Board board)
    {
        List<(int row, int col)> rowCols = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom,
            isBottomColor = board.BottomColor == Color;
        int fRow = coord.Row, fCol = coord.Col;

        // 已过河
        if (isBottomColor != isBottom)
        {
            if (fCol > 0)
                rowCols.Add((fRow, fCol - 1));
            if (fCol < Coord.ColCount - 1)
                rowCols.Add((fRow, fCol + 1));
        }

        if (isBottomColor && fRow < Coord.RowCount - 1)
            rowCols.Add((fRow + 1, fCol));
        else if (!isBottomColor && fRow > 0)
            rowCols.Add((fRow - 1, fCol));

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
    override public List<(int row, int col)> MoveRowCols(Board board) => new();
}