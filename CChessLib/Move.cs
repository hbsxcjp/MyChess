namespace CChess;

// public enum VisibleType { All, True, False }

public class Move
{
    private List<Move>? _afterMoves;

    public static Move RootMove() => new(CoordPair.Null);

    private Move(CoordPair coordPair, Move? before = null, string? remark = null, bool visible = true)
    {
        Before = before;

        CoordPair = coordPair;
        Remark = remark;
        Visible = visible;
    }

    // 对象属性
    public int Id { get; set; }
    public string? Remark { get; set; }
    public bool Visible { get; set; }

    public Move? Before { get; }
    public CoordPair CoordPair { get; }

    public List<Move> BeforeMoves
    {
        get
        {
            List<Move> beforeMoves = new();
            Move move = this;
            while (move.Before != null)
            {
                beforeMoves.Insert(0, move);
                move = move.Before;
            }

            return beforeMoves;
        }
    }

    public int AfterNum => _afterMoves?.Count ?? 0;
    public bool HasAfter => _afterMoves != null;

    public Move AddAfter(CoordPair coordPair, string? remark = null, bool visible = true)
    {
        Move move = new(coordPair, this, remark, visible);
        (_afterMoves ??= new()).Add(move);

        return move;
    }

    public List<Move>? AfterMoves() { return _afterMoves; }

    public List<Move>? OtherMoves() => Before?._afterMoves;

    // 代替ManualMove枚举器
    public List<Move> AllAfterMoves()
    {
        List<Move> allAfterMoves = new(100);
        Queue<Move> moveQueue = new();
        _afterMoves?.ForEach(aMove => moveQueue.Enqueue(aMove));
        while (moveQueue.Count > 0)
        {
            Move move = moveQueue.Dequeue();
            allAfterMoves.Add(move);
            move._afterMoves?.ForEach(aMove => moveQueue.Enqueue(aMove));
        }

        int id = 0;
        allAfterMoves.ForEach(move => move.Id = ++id);
        return allAfterMoves;
    }

    public override string ToString()
       => $"{new string('\t', BeforeMoves.Count)}{Before?.Id}-{CoordPair}.{Id} {Remark}\n";
}
