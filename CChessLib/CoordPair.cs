namespace CChess;

public struct CoordPair
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

    public string Iccs { get => $"{FromCoord.Iccs}{ToCoord.Iccs}"; }
    public string RowCol { get => $"{FromCoord.RowCol}{ToCoord.RowCol}"; }

    public bool Equals(CoordPair other) 
        => FromCoord.Index == other.FromCoord.Index && ToCoord.Index == other.ToCoord.Index;

    public override string ToString() => $"[{FromCoord},{ToCoord}]";
}