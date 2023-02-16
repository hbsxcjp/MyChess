namespace CChess;

public enum VisibleType { All, True, False }

public class Move
{
    private List<Move>? _afterMoves;

    private Move(CoordPair coordPair, Move? before = null, string? remark = null, bool visible = true)
    {
        Before = before;

        CoordPair = coordPair;
        Remark = remark;
        Visible = visible;
        EatPiece = Piece.Null;

        _afterMoves = null;
    }

    // 对象属性
    public Move? Before { get; }
    public CoordPair CoordPair { get; }

    public int Id { get; set; }
    public string? Remark { get; set; }
    public bool Visible { get; set; }
    public Piece EatPiece { get; set; }

    public int BeforeNum
    {
        get
        {
            int count = 0;
            Move? move = this;
            while ((move = move.Before) != null)
                count++;

            return count;
        }
    }
    public int AfterNum { get { return _afterMoves?.Count ?? 0; } }
    public bool HasAfter { get { return _afterMoves != null; } }

    public static Move RootMove() => new(CoordPair.Null);
    public Move AddMove(CoordPair coordPair, string? remark = null, bool visible = true)
    {
        Move move = new(coordPair, this, remark, visible);
        (_afterMoves ??= new()).Add(move);

        return move;
    }

    // 后置着法列表
    public List<Move>? AfterMoves() { return _afterMoves; }

    // 同步变着列表
    public List<Move>? OtherMoves() => Before?._afterMoves ?? null;

    public override string ToString()
       => $"{new string('\t', BeforeNum)}{Before?.Id}-{CoordPair}.{Id} {Remark}\n";
}
