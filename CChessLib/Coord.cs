namespace CChess;

public enum ChangeType
{
    Exchange,
    Rotate,
    Symmetry_H,
    Symmetry_V,
    NoChange = -1,
}

public class Coord : IComparable
{
    public static readonly Coord Null = new(-10); // (-1, -1)
    public static readonly List<Coord> AllCoords = Enumerable.Range(0, RowCount * ColCount).Select(
            index => new Coord(index)).ToList();
    public const string ColChars = "ABCDEFGHI";
    public const int RowCount = 10;
    public const int ColCount = 9;

    private Coord(int index) { Index = index; Row = index / ColCount; Col = index % ColCount; }

    public int Index { get; }
    public int Row { get; }
    public int Col { get; }
    public string RowCol { get { return $"{Row}{Col}"; } }
    public string Iccs { get { return $"{ColChars[Col]}{Row}"; } }
    public bool IsBottom { get { return (Row << 1) < RowCount; } }
    public bool IsNull { get { return this == Null; } }

    public static int GetIndex(int row, int col) => row * ColCount + col;
    public static string GetRowCol(string rowCol, ChangeType ct)
    {
        int frow = int.Parse(rowCol[0].ToString()),
            fcol = int.Parse(rowCol[1].ToString()),
            trow = int.Parse(rowCol[2].ToString()),
            tcol = int.Parse(rowCol[3].ToString());

        void symmetryCol() { fcol = SymmetryCol(fcol); tcol = SymmetryCol(tcol); }
        void symmetryRow() { frow = SymmetryRow(frow); trow = SymmetryRow(trow); }
        switch (ct)
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
        for (int i = 0; i < iccses.Length - 1; i += 2)
            builder.Append($"{iccses[i + 1]}{ColChars.IndexOf(iccses[i])}");

        return builder.ToString();
    }

    int IComparable.CompareTo(object? obj)
    {
        if (obj is not Coord)
            return 0;

        return CompareTo((Coord)obj);
    }

    internal int CompareTo(Coord coord)
    {
        int colComp = Col.CompareTo(coord.Col);
        if (colComp != 0)
            return colComp;

        return Row.CompareTo(coord.Row);
    }

    public static int GetCol(int col, bool isBottomColor) => isBottomColor ? SymmetryCol(col) : col;

    public static int GetDoubleIndex(Coord coord) => SymmetryRow(coord.Row) * 2 * (ColCount * 2) + coord.Col * 2;

    public static bool IsValid(int Row, int col) => Row >= 0 && Row < RowCount && col >= 0 && col < ColCount;

    public override string ToString() => $"({Row},{Col})";

    private static int SymmetryRow(int row) => RowCount - 1 - row;
    private static int SymmetryCol(int col) => ColCount - 1 - col;
}

public class CoordPair //: IEquatable<CoordPair>
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


    public bool Equals(CoordPair other) => FromCoord == other.FromCoord && ToCoord == other.ToCoord;

    public override string ToString() => $"[{FromCoord},{ToCoord}]";
}
