using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CChess;

public class Seat
{
    internal Seat(Coord coord)
    {
        Coord = coord;
        Piece = Piece.Null;
    }

    public Coord Coord { get; }
    public Piece Piece { get; set; }

    public void MoveTo(Seat toSeat, Piece fillPiece)
    {
        Piece fromPiece = Piece;
        Piece = fillPiece;

        toSeat.Piece = fromPiece;
    }

    public override string ToString() => $"{Coord}:{Piece}";
}