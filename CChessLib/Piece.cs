namespace CChess;

public enum PieceColor { Red, Black, NoColor = -1 }

public enum PieceKind { King, Advisor, Bishop, Knight, Rook, Cannon, Pawn, NoKind = -1 }

public abstract class Piece
{
    public static readonly Piece Null = new NullPiece();

    public const char NullCh = '_';
    public const char NullName = '空';

    public static readonly PieceColor[] PieceColors = { PieceColor.Red, PieceColor.Black };
    public static readonly PieceKind[] PieceKinds = { PieceKind.King, PieceKind.Advisor,
        PieceKind.Bishop, PieceKind.Knight, PieceKind.Rook, PieceKind.Cannon, PieceKind.Pawn };

    public static readonly string[] NameChars = { "帅仕相马车炮兵", "将士象马车炮卒" };
    private static readonly string[] ChChars = { "KABNRCP", "kabnrcp" };
    private static readonly string ColorChars = "无红黑";

    protected Piece(PieceColor color, PieceKind kind) { Color = color; Kind = kind; }

    public PieceColor Color { get; }
    public PieceKind Kind { get; }

    virtual public char Char { get => ChChars[(int)Color][(int)Kind]; }
    virtual public char Name { get => GetName(Color, Kind); }
    virtual public char PrintName { get => GetPrintName(Color, Kind); }

    public static char GetName(PieceColor color, PieceKind kind) => NameChars[(int)color][(int)kind];

    public static char GetPrintName(PieceColor color, PieceKind kind)
    {
        const string nrcChars = "馬車砲";
        List<PieceKind> nrcKinds = new List<PieceKind> { PieceKind.Knight, PieceKind.Rook, PieceKind.Cannon };
        return (color == PieceColor.Black && nrcKinds.Contains(kind) ? nrcChars[nrcKinds.IndexOf(kind)] : GetName(color, kind));
    }

    virtual public List<Coord> PutCoord(bool isBottom) => Coord.Coords;
    public List<Coord> MoveCoord(Board board)
        => RuleMoveCoord(board).Where(coord => board[coord].Color != Color).ToList();
    abstract protected List<Coord> RuleMoveCoord(Board board);

    public List<Coord> CanMoveCoord(Board board)
    {
        Coord fromCoord = board.GetCoord(this);
        return MoveCoord(board).Where(toCoord => board.CanMove(fromCoord, toCoord)).ToList();
    }

    public static PieceColor GetOtherColor(PieceColor color) => color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;
    public static PieceColor GetColor(char ch) => char.IsUpper(ch) ? PieceColor.Red : PieceColor.Black;

    public static PieceKind GetKind(char ch) => (PieceKind)ChChars[(int)GetColor(ch)].IndexOf(ch);

    public static PieceKind GetKind_Name(char name)
        => (PieceKind)NameChars[NameChars[0].Contains(name) ? 0 : 1].IndexOf(name);

    public static bool IsLinePiece(PieceKind kind)
        => (kind == PieceKind.King || kind == PieceKind.Rook || kind == PieceKind.Cannon || kind == PieceKind.Pawn);

    override public string ToString() => $"{ColorChars[(int)Color + 1]}{PrintName}{Char}";
}

public class King : Piece
{
    public King(PieceColor color) : base(color, PieceKind.King) { }

    override public List<Coord> PutCoord(bool isBottom)
        => Enumerable.Range(isBottom ? 7 : 0, 3)
             .Select(row => Enumerable.Range(3, 3)
                                            .Select(col => Coord.Get(row, col)))
             .SelectMany(coords => coords)
             .ToList();

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        if (fCol > 3)
            coords.Add(Coord.Get(fRow, fCol - 1));
        if (fCol < 5)
            coords.Add(Coord.Get(fRow, fCol + 1));
        if (fRow > (isBottom ? 7 : 0))
            coords.Add(Coord.Get(fRow - 1, fCol));
        if (fRow < (isBottom ? 9 : 2))
            coords.Add(Coord.Get(fRow + 1, fCol));

        return coords;
    }
}

public class Advisor : Piece
{
    public Advisor(PieceColor color) : base(color, PieceKind.Advisor) { }

    override public List<Coord> PutCoord(bool isBottom)
    {
        List<Coord> coords = new();
        int minRow = isBottom ? 7 : 0,
            maxRow = isBottom ? 9 : 2;

        for (int row = minRow; row <= maxRow; row += 2)
            for (int col = 3; col <= 5; col += 2)
                coords.Add(Coord.Get(row, col));

        coords.Add(Coord.Get(minRow + 1, 4));
        return coords;
    }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        if (fCol != 4)
            coords.Add(Coord.Get(isBottom ? 8 : 1, 4));
        else
        {
            coords.Add(Coord.Get(fRow + 1, fCol - 1));
            coords.Add(Coord.Get(fRow + 1, fCol + 1));
            coords.Add(Coord.Get(fRow - 1, fCol - 1));
            coords.Add(Coord.Get(fRow - 1, fCol + 1));
        }

        return coords;
    }
}

public class Bishop : Piece
{
    public Bishop(PieceColor color) : base(color, PieceKind.Bishop) { }

    override public List<Coord> PutCoord(bool isBottom)
    {
        List<Coord> coords = new();
        int minRow = isBottom ? 5 : 0,
            midRow = isBottom ? 7 : 2,
            maxRow = isBottom ? 9 : 4;
        for (int row = minRow; row <= maxRow; row += 4)
            for (int col = 2; col < Coord.ColCount; col += 4)
                coords.Add(Coord.Get(row, col));

        for (int col = 0; col < Coord.ColCount; col += 4)
            coords.Add(Coord.Get(midRow, col));

        return coords;
    }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        int maxRow = isBottom ? Coord.RowCount - 1 : (Coord.RowCount - 1) / 2;
        void AddRowCol(int row, int col)
        {
            if (board.IsNull((row + fRow) / 2, (col + fCol) / 2))
                coords.Add(Coord.Get(row, col));
        }

        if (fRow > 0)
        {
            if (fCol > 0)
                AddRowCol(fRow - 2, fCol - 2);
            if (fCol < Coord.ColCount - 1)
                AddRowCol(fRow - 2, fCol + 2);
        }
        if (fRow < maxRow)
        {
            if (fCol > 0)
                AddRowCol(fRow + 2, fCol - 2);
            if (fCol < Coord.ColCount - 1)
                AddRowCol(fRow + 2, fCol + 2);
        }

        return coords;
    }
}

public class Knight : Piece
{
    public Knight(PieceColor color) : base(color, PieceKind.Knight) { }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        ((int row, int col) to, (int row, int col) leg)[] allToLegRowCols =
            {
                ((fRow + 2, fCol - 1), (fRow + 1, fCol)),
                ((fRow + 2, fCol + 1), (fRow + 1, fCol)),
                ((fRow + 1, fCol - 2), (fRow, fCol - 1)),
                ((fRow + 1, fCol + 2), (fRow, fCol + 1)),
                ((fRow - 1, fCol - 2), (fRow, fCol - 1)),
                ((fRow - 1, fCol + 2), (fRow, fCol + 1)),
                ((fRow - 2, fCol - 1), (fRow - 1, fCol)),
                ((fRow - 2, fCol + 1), (fRow - 1, fCol))
            };

        return allToLegRowCols.Where(
                toLeg => Coord.IsValid(toLeg.to.row, toLeg.to.col)
                    && (board.IsNull(toLeg.leg.row, toLeg.leg.col)))
                .Select(toLeg => Coord.Get(toLeg.to.row, toLeg.to.col))
                .ToList();
    }
}

public class Rook : Piece
{
    public Rook(PieceColor color) : base(color, PieceKind.Rook) { }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        int fRow = coord.Row, fCol = coord.Col;
        bool AddRowCol(int row, int col)
        {
            coords.Add(Coord.Get(row, col));
            return board.IsNull(row, col);
        }

        for (int r = fRow + 1; r < Coord.RowCount; ++r)
            if (!AddRowCol(r, fCol))
                break;

        for (int r = fRow - 1; r >= 0; --r)
            if (!AddRowCol(r, fCol))
                break;

        for (int c = fCol - 1; c >= 0; --c)
            if (!AddRowCol(fRow, c))
                break;

        for (int c = fCol + 1; c < Coord.ColCount; ++c)
            if (!AddRowCol(fRow, c))
                break;

        return coords;
    }
}

public class Cannon : Piece
{
    public Cannon(PieceColor color) : base(color, PieceKind.Cannon) { }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        int fRow = coord.Row, fCol = coord.Col;
        bool skiped = false;
        bool AddCoordToBreak(int row, int col)
        {
            bool isNull = board.IsNull(row, col);
            if (!skiped)
            {
                if (isNull)
                    coords.Add(Coord.Get(row, col));
                else
                    skiped = true;
            }
            else if (!isNull)
            {
                coords.Add(Coord.Get(row, col));
                return true;
            }

            return false;
        }

        for (int r = fRow + 1; r < Coord.RowCount; ++r)
            if (AddCoordToBreak(r, fCol))
                break;

        skiped = false;
        for (int r = fRow - 1; r >= 0; --r)
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

        return coords;
    }
}

public class Pawn : Piece
{
    public Pawn(PieceColor color) : base(color, PieceKind.Pawn) { }

    override public List<Coord> PutCoord(bool isBottom)
    {
        List<Coord> coords = new();
        int minRow = isBottom ? 5 : 3,
            maxRow = isBottom ? 6 : 4;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 0; col < Coord.ColCount; col += 2)
                coords.Add(Coord.Get(row, col));

        minRow = isBottom ? 0 : 5;
        maxRow = isBottom ? 4 : 9;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 0; col < Coord.ColCount; ++col)
                coords.Add(Coord.Get(row, col));

        return coords;
    }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom,
            isBottomColor = board.IsBottom(Color);
        int fRow = coord.Row, fCol = coord.Col;

        // 已过河
        if (isBottomColor != isBottom)
        {
            if (fCol > 0)
                coords.Add(Coord.Get(fRow, fCol - 1));
            if (fCol < Coord.ColCount - 1)
                coords.Add(Coord.Get(fRow, fCol + 1));
        }

        if (!isBottomColor && fRow < Coord.RowCount - 1)
            coords.Add(Coord.Get(fRow + 1, fCol));
        else if (isBottomColor && fRow > 0)
            coords.Add(Coord.Get(fRow - 1, fCol));

        return coords;
    }
}

public class NullPiece : Piece
{
    public NullPiece() : base(PieceColor.NoColor, PieceKind.NoKind) { }

    override public char Char { get => Piece.NullCh; }
    override public char Name { get => Piece.NullName; }
    override public char PrintName { get => Piece.NullName; }


    override public List<Coord> PutCoord(bool isBottom) => new();
    override protected List<Coord> RuleMoveCoord(Board board) => new();
}