namespace CChess;

public class CoordPair
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