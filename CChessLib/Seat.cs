namespace CChess;

internal class Seat
{
    public static readonly Seat Null = new(Coord.Null);
    private Piece _piece;

    public Seat(Coord coord)
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

            value.Seat = this;
            _piece = value;
        }
    }
    public bool IsNull { get { return this == Null; } }
    public bool HasNullPiece { get { return Piece == Piece.Null; } }

    public void MoveTo(Seat toSeat, Piece fromPiece)
    {
        Piece piece = Piece;
        Piece = fromPiece;
        toSeat.Piece = piece;
    }

    public static Seat[,] CreatSeats()
    {
        var seats = new Seat[Coord.RowCount, Coord.ColCount];
        foreach(Coord coord in Coord.CreatCoords())
            seats[coord.row, coord.col] = new(coord);

        return seats;
    }

    public override string ToString() => $"{Coord}:{_piece}";
}


