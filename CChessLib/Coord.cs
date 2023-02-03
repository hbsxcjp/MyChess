namespace CChess;

public enum ChangeType
{
    Exchange,
    Rotate,
    Symmetry_H,
    Symmetry_V,
    NoChange = -1,
}

public class Coord
{
    public static readonly Coord Null = new(-10); // (-1, -1)
    public static readonly List<Coord> AllCoords =
        Enumerable.Range(0, RowCount * ColCount).Select(index => new Coord(index)).ToList();

    public const string ColChars = "ABCDEFGHI";
    public const int RowCount = 10;
    public const int ColCount = 9;

    private Coord(int index) { Index = index; }

    public int Index { get; }
    public int Row { get { return Index / ColCount; } }
    public int Col { get { return Index % ColCount; } }
    public string RowCol { get { return $"{Row}{Col}"; } }
    public string Iccs { get { return $"{ColChars[Col]}{Row}"; } }
    public bool IsBottom { get { return (Row << 1) < RowCount; } }

    public static int GetIndex(int row, int col) => row * ColCount + col;

    public static string GetRowCol(string rowCol, ChangeType ct)
    {
        if (ct == ChangeType.NoChange || ct == ChangeType.Exchange)
            return rowCol;

        int frow = int.Parse(rowCol[0].ToString()),
            fcol = int.Parse(rowCol[1].ToString()),
            trow = int.Parse(rowCol[2].ToString()),
            tcol = int.Parse(rowCol[3].ToString());

        return ct switch
        {
            ChangeType.Symmetry_H => $"{frow}{SymmetryCol(fcol)}{trow}{SymmetryCol(tcol)}",
            ChangeType.Symmetry_V => $"{SymmetryRow(frow)}{fcol}{SymmetryRow(trow)}{tcol}",
            ChangeType.Rotate => $"{SymmetryRow(frow)}{SymmetryCol(fcol)}{SymmetryRow(trow)}{SymmetryCol(tcol)}",
            _ => rowCol
        };
    }

    public static string RowCols(string iccses)
    {
        // System.Text.StringBuilder builder = new();
        // for (int i = 0; i < iccses.Length - 1; i += 2)
        //     builder.Append($"{iccses[i + 1]}{ColChars.IndexOf(iccses[i])}");

        // return builder.ToString();
        return Enumerable.Range(0, iccses.Length / 2)
                .Select(i => $"{iccses[i * 2 + 1]}{ColChars.IndexOf(iccses[i * 2])}")
                .ToString() ?? String.Empty;
    }

    public static int GetCol(int col, bool isBottomColor) => isBottomColor ? SymmetryCol(col) : col;

    public static int GetDoubleIndex(Coord coord) => SymmetryRow(coord.Row) * 2 * (ColCount * 2) + coord.Col * 2;

    public static bool IsValid(int row, int col) => row >= 0 && row < RowCount && col >= 0 && col < ColCount;

    private static int SymmetryRow(int row) => RowCount - 1 - row;
    private static int SymmetryCol(int col) => ColCount - 1 - col;

    public override string ToString() => $"({Row},{Col})";
}
