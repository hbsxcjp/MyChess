namespace CChess;

public enum PieceColor { Red, Black, NoColor = -1 }

public enum PieceKind { King, Advisor, Bishop, Knight, Rook, Cannon, Pawn, NoKind = -1 }

public abstract class Piece
{
    public static readonly Piece Null = new NullPiece();

    protected Piece(PieceColor color) { Color = color; }

    public PieceColor Color { get; }
    abstract public PieceKind Kind { get; }
    abstract public char Char { get; }
    abstract public char Name { get; }
    virtual public char PrintName { get { return Name; } }

    virtual public List<Coord> PutCoord(Board board) => Coord.Coords;
    public List<Coord> MoveCoord(Board board)
        => RuleMoveCoord(board).Where(coord => board[coord].Piece.Color != Color).ToList();
    abstract protected List<Coord> RuleMoveCoord(Board board);

    public List<Coord> CanMoveCoord(Board board)
    {
        Coord fromCoord = board.GetCoord(this);
        return MoveCoord(board).Where(toCoord => board.CanMove(fromCoord, toCoord)).ToList();
    }

    override public string ToString()
        => $"{(Color == PieceColor.Red ? "红" : (Color == PieceColor.Black ? "黑" : "无"))}{PrintName}{Char}";
}

public class King : Piece
{
    public King(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.King; } }
    override public char Char { get { return Color == PieceColor.Red ? 'K' : 'k'; } }
    override public char Name { get { return Color == PieceColor.Red ? '帅' : '将'; } }

    override public List<Coord> PutCoord(Board board)
        => Enumerable.Range(board.BottomColor == Color ? 0 : 7, 3)
             .Select(row => Enumerable.Range(3, 3)
                                            .Select(col => board[row, col].Coord))
             .SelectMany(coords => coords)
             .ToList();

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        if (fCol > 3)
            coords.Add(board[fRow, fCol - 1].Coord);
        if (fCol < 5)
            coords.Add(board[fRow, fCol + 1].Coord);
        if (fRow < (isBottom ? 2 : 9))
            coords.Add(board[fRow + 1, fCol].Coord);
        if (fRow > (isBottom ? 0 : 7))
            coords.Add(board[fRow - 1, fCol].Coord);

        return coords;
    }
}

public class Advisor : Piece
{
    public Advisor(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Advisor; } }
    override public char Char { get { return Color == PieceColor.Red ? 'A' : 'a'; } }
    override public char Name { get { return Color == PieceColor.Red ? '仕' : '士'; } }

    override public List<Coord> PutCoord(Board board)
    {
        List<Coord> coords = new();
        bool isBottomColor = board.BottomColor == Color;
        int minRow = isBottomColor ? 0 : 7,
            maxRow = isBottomColor ? 2 : 9;

        for (int row = minRow; row <= maxRow; row += 2)
            for (int col = 3; col <= 5; col += 2)
                coords.Add(board[row, col].Coord);

        coords.Add(board[minRow + 1, 4].Coord);
        return coords;
    }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        if (fCol != 4)
            coords.Add(board[isBottom ? 1 : 8, 4].Coord);
        else
        {
            coords.Add(board[fRow - 1, fCol - 1].Coord);
            coords.Add(board[fRow - 1, fCol + 1].Coord);
            coords.Add(board[fRow + 1, fCol - 1].Coord);
            coords.Add(board[fRow + 1, fCol + 1].Coord);
        }

        return coords;
    }
}

public class Bishop : Piece
{
    public Bishop(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Bishop; } }
    override public char Char { get { return Color == PieceColor.Red ? 'B' : 'b'; } }
    override public char Name { get { return Color == PieceColor.Red ? '相' : '象'; } }

    override public List<Coord> PutCoord(Board board)
    {
        List<Coord> coords = new();
        bool isBottomColor = board.BottomColor == Color;
        int minRow = isBottomColor ? 0 : 5,
            midRow = isBottomColor ? 2 : 7,
            maxRow = isBottomColor ? 4 : 9;
        for (int row = minRow; row <= maxRow; row += 4)
            for (int col = 2; col < Coord.ColCount; col += 4)
                coords.Add(board[row, col].Coord);

        for (int col = 0; col < Coord.ColCount; col += 4)
            coords.Add(board[midRow, col].Coord);

        return coords;
    }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        int maxRow = isBottom ? (Coord.RowCount - 1) / 2 : Coord.RowCount - 1;
        void AddRowCol(int row, int col)
        {
            if (board.IsNull((row + fRow) / 2, (col + fCol) / 2))
                coords.Add(board[row, col].Coord);
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

        return coords;
    }
}

public class Knight : Piece
{
    public Knight(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Knight; } }
    override public char Char { get { return Color == PieceColor.Red ? 'N' : 'n'; } }
    override public char Name { get { return '马'; } }
    override public char PrintName { get { return Color == PieceColor.Red ? Name : '馬'; } }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom;
        int fRow = coord.Row, fCol = coord.Col;
        ((int row, int col) to, (int row, int col) leg)[] allToLegRowCols =
        {
                ((fRow - 2, fCol - 1), (fRow - 1, fCol))  ,
                ((fRow - 2, fCol + 1), (fRow - 1, fCol)),
                ((fRow - 1, fCol - 2), (fRow, fCol - 1)),
                ((fRow - 1, fCol + 2), (fRow, fCol + 1)),
                ((fRow + 1, fCol - 2), (fRow, fCol - 1)),
                ((fRow + 1, fCol + 2), (fRow, fCol + 1)),
                ((fRow + 2, fCol - 1), (fRow + 1, fCol)),
                ((fRow + 2, fCol + 1), (fRow + 1, fCol))
            };

        return allToLegRowCols.Where(
                toLeg => Coord.IsValid(toLeg.to.row, toLeg.to.col)
                    && (board.IsNull(toLeg.leg.row, toLeg.leg.col)))
                .Select(toLeg => board[toLeg.to.row, toLeg.to.col].Coord)
                .ToList();
    }
}

public class Rook : Piece
{
    public Rook(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Rook; } }
    override public char Char { get { return Color == PieceColor.Red ? 'R' : 'r'; } }
    override public char Name { get { return '车'; } }
    override public char PrintName { get { return Color == PieceColor.Red ? Name : '車'; } }
    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        int fRow = coord.Row, fCol = coord.Col;
        bool AddRowCol(int row, int col)
        {
            coords.Add(board[row, col].Coord);
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

        return coords;
    }
}

public class Cannon : Piece
{
    public Cannon(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Cannon; } }
    override public char Char { get { return Color == PieceColor.Red ? 'C' : 'c'; } }
    override public char Name { get { return '炮'; } }
    override public char PrintName { get { return Color == PieceColor.Red ? Name : '砲'; } }
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
                    coords.Add(board[row, col].Coord);
                else
                    skiped = true;
            }
            else if (!isNull)
            {
                coords.Add(board[row, col].Coord);
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

        return coords;
    }
}

public class Pawn : Piece
{
    public Pawn(PieceColor color) : base(color) { }

    override public PieceKind Kind { get { return PieceKind.Pawn; } }
    override public char Char { get { return Color == PieceColor.Red ? 'P' : 'p'; } }
    override public char Name { get { return Color == PieceColor.Red ? '兵' : '卒'; } }

    override public List<Coord> PutCoord(Board board)
    {
        List<Coord> coords = new();
        bool isBottomColor = board.BottomColor == Color;
        int minRow = isBottomColor ? 3 : 5,
            maxRow = isBottomColor ? 4 : 6;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 0; col < Coord.ColCount; col += 2)
                coords.Add(board[row, col].Coord);

        minRow = isBottomColor ? 5 : 0;
        maxRow = isBottomColor ? 9 : 4;
        for (int row = minRow; row <= maxRow; ++row)
            for (int col = 0; col < Coord.ColCount; ++col)
                coords.Add(board[row, col].Coord);

        return coords;
    }

    override protected List<Coord> RuleMoveCoord(Board board)
    {
        List<Coord> coords = new();
        Coord coord = board.GetCoord(this);
        bool isBottom = coord.IsBottom,
            isBottomColor = board.BottomColor == Color;
        int fRow = coord.Row, fCol = coord.Col;

        // 已过河
        if (isBottomColor != isBottom)
        {
            if (fCol > 0)
                coords.Add(board[fRow, fCol - 1].Coord);
            if (fCol < Coord.ColCount - 1)
                coords.Add(board[fRow, fCol + 1].Coord);
        }

        if (isBottomColor && fRow < Coord.RowCount - 1)
            coords.Add(board[fRow + 1, fCol].Coord);
        else if (!isBottomColor && fRow > 0)
            coords.Add(board[fRow - 1, fCol].Coord);

        return coords;
    }
}

public class NullPiece : Piece
{
    public NullPiece() : base(PieceColor.NoColor) { }

    override public PieceKind Kind { get { return PieceKind.NoKind; } }
    override public char Char { get { return '_'; } }
    override public char Name { get { return '空'; } }
    override public List<Coord> PutCoord(Board board) => new();
    override protected List<Coord> RuleMoveCoord(Board board) => new();
}