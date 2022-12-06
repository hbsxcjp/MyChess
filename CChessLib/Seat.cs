namespace CChess;

public class Seat
{
    public static readonly Seat Null = new(Coord.Null);
    private Piece _piece;

    private Seat(Coord coord)
    {
        Coord = coord;
        _piece = Piece.Null;
    }

    public Coord Coord { get; }
    public Piece Piece
    {
        get { return _piece; }
        set
        {
            _piece.Seat = Null;
            if (value != Piece.Null)
                value.Seat = this;

            _piece = value;
        }
    }
    public bool IsNull { get { return this == Null; } }

    public void MoveTo(Seat toSeat, Piece fillPiece)
    {
        Piece fromPiece = Piece;
        Piece = fillPiece;

        toSeat.Piece = fromPiece;
    }

    public static Seat[,] CreatSeats()
    {
        var seats = new Seat[Coord.RowCount, Coord.ColCount];
        foreach (Coord coord in Coord.CreatCoords())
            seats[coord.Row, coord.Col] = new(coord);

        return seats;
    }

    public override string ToString() => $"{Coord}:{Piece}";
}