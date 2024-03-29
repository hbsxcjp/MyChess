namespace CChess;

public enum ChangeType { Exchange, Rotate, Symmetry_H, Symmetry_V, NoChange = -1, }

public class Coord
{
    public static readonly Coord Null = new(-10);  // (-1, -1)
    public static readonly List<Coord> Coords =
        Enumerable.Range(0, RowCount * ColCount).Select(index => new Coord(index)).ToList();

    public const string ColChars = "ABCDEFGHI";
    public const int RowCount = 10;
    public const int ColCount = 9;

    private Coord(int index) { Index = index; Row = index / ColCount; Col = index % ColCount; }

    public int Index { get; }
    public int Row { get; }
    public int Col { get; }
    public string RowCol { get => $"{Row}{Col}"; }
    public string Iccs { get => $"{ColChars[Col]}{Row}"; }
    public bool IsBottom { get => (Row << 1) > RowCount - 1; }

    public static Coord Get(int index) => Coords[index];
    public static Coord Get(int row, int col) => Coords[row * ColCount + col];

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

    public static string GetRowCols(string iccses)
        => string.Concat(Enumerable.Range(0, iccses.Length / 2)
                .Select(i => $"{iccses[i * 2 + 1]}{ColChars.IndexOf(iccses[i * 2])}"));

    public static int GetCol(int col, bool isBottomColor) => isBottomColor ? SymmetryCol(col) : col;

    public static int GetDoubleIndex(Coord coord) => coord.Row * 2 * (ColCount * 2) + coord.Col * 2;

    public static bool IsValid(int row, int col) => row >= 0 && row < RowCount && col >= 0 && col < ColCount;

    private static int SymmetryRow(int row) => RowCount - 1 - row;
    private static int SymmetryCol(int col) => ColCount - 1 - col;

    public override string ToString() => $"({Row},{Col})";

    public string SymmetryRowToString() => $"({SymmetryRow(Row)},{Col})";
}
