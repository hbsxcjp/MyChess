namespace CChess;

public class MoveInfo
{
    public MoveInfo(CoordPair coordPair, string? remark = null, bool visible = true)
    {
        CoordPair = coordPair;

        Remark = remark;
        Visible = visible;

        EatPiece = Piece.Null;
    }

    public CoordPair CoordPair { get; }

    public string? Remark { get; set; }
    public bool Visible { get; set; }
    public Piece EatPiece { get; set; }

}