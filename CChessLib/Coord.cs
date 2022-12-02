namespace CChess;

enum ChangeType
{
    Exchange,
    Rotate,
    Symmetry_H,
    Symmetry_V,
    NoChange = -1,
}

internal class Coord: IComparable
{
    public const string ColChars = "ABCDEFGHI";
    public const int RowCount = 10;
    public const int ColCount = 9;
    public static readonly Coord Null = new(-1, -1);

    public readonly int row;
    public readonly int col;

    public static List<Coord> CreatCoords()
    {
        List<Coord> coords = new(RowCount * ColCount);
        for(int row = 0;row < RowCount;row++)
            for(int col = 0;col < ColCount;col++)
                coords.Add(new(row, col));

        return coords;
    }
    private Coord(int r, int c) { row = r; col = c; }

    public string RowCol { get { return $"{row}{col}"; } }
    public string Iccs { get { return $"{ColChars[col]}{row}"; } }
    public bool IsBottom { get { return (row << 1) < RowCount; } }

    public static string GetRowCol(string rowCol, ChangeType ct)
    {
        int frow = int.Parse(rowCol[0].ToString()),
            fcol = int.Parse(rowCol[1].ToString()),
            trow = int.Parse(rowCol[2].ToString()),
            tcol = int.Parse(rowCol[3].ToString());

        void symmetryCol() { fcol = SymmetryCol(fcol); tcol = SymmetryCol(tcol); }
        void symmetryRow() { frow = SymmetryRow(frow); trow = SymmetryRow(trow); }
        switch(ct)
        {
            case ChangeType.Symmetry_H:
                symmetryCol();
                break;
            case ChangeType.Symmetry_V:
                symmetryRow();
                break;
            case ChangeType.Rotate:
                symmetryCol();
                symmetryRow();
                break;
            default:
                break;
        };

        return $"{frow}{fcol}{trow}{tcol}";
    }

    public static string RowCols(string iccses)
    {
        System.Text.StringBuilder builder = new();
        for(int i = 0;i < iccses.Length - 1;i += 2)
            builder.Append($"{iccses[i + 1]}{ColChars.IndexOf(iccses[i])}");

        return builder.ToString();
    }
    
    int IComparable.CompareTo(object? obj)
    {
        if(obj is not Coord)
            return 0;

        return CompareTo((Coord)obj);
    }

    internal int CompareTo(Coord coord)
    {
        int colComp = col.CompareTo(coord.col);
        if(colComp != 0)
            return colComp;

        return row.CompareTo(coord.row);
    }

    public static int GetCol(int col, bool isBottomColor) => isBottomColor ? SymmetryCol(col) : col;
    public static int GetDoubleIndex(Coord coord) => SymmetryRow(coord.row) * 2 * (ColCount * 2) + coord.col * 2;
    public static bool IsValid(int row, int col) => row >= 0 && row < RowCount && col >= 0 && col < ColCount;

    public override string ToString() => $"({row},{col})";

    private static int SymmetryRow(int row) => RowCount - 1 - row;
    private static int SymmetryCol(int col) => ColCount - 1 - col;    
}

internal class CoordPair
{
    public static readonly CoordPair Null = new(Coord.Null, Coord.Null);
    public const int RowColICCSLength = 4;

    public CoordPair(Coord fromCoord, Coord toCoord)
    {
        FromCoord = fromCoord;
        ToCoord = toCoord;
    }

    public Coord FromCoord { get; }
    public Coord ToCoord { get; }

    public string RowCol { get { return $"{FromCoord.RowCol}{ToCoord.RowCol}"; } }
    public string Iccs { get { return $"{FromCoord.Iccs}{ToCoord.Iccs}"; } }

    public override string ToString() => $"[{FromCoord},{ToCoord}]";
}
