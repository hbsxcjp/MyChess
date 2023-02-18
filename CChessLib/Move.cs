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
    }

    // 对象属性
    public int Id { get; set; }
    public string? Remark { get; set; }
    public bool Visible { get; set; }

    public Move? Before { get; }
    public CoordPair CoordPair { get; }

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
    public List<CoordPair> BeforeCoordPairs
    {
        get
        {
            List<CoordPair> beforeCoordPairs = new();
            Move move = this;
            while (move.Before != null)
            {
                beforeCoordPairs.Insert(0, move.CoordPair);
                move = move.Before;
            }

            return beforeCoordPairs;
        }
    }

    public int AfterNum { get { return _afterMoves?.Count ?? 0; } }
    public bool HasAfter { get { return _afterMoves != null; } }

    public static Move RootMove() => new(CoordPair.Null);
    public Move AddAfter(CoordPair coordPair, string? remark = null, bool visible = true)
    {
        Move move = new(coordPair, this, remark, visible);
        (_afterMoves ??= new()).Add(move);

        return move;
    }

    // 后置着法列表
    public List<Move>? AfterMoves() { return _afterMoves; }

    // 同步变着列表
    public List<Move>? OtherMoves() => Before?._afterMoves;

    // 代替ManualMove枚举器
    public List<Move> AllAfterMoves()
    {
        List<Move> allAfterMoves = new(100);
        Queue<Move> moveQueue = new();
        _afterMoves?.ForEach(aMove => moveQueue.Enqueue(aMove));
        int id = 0;
        while (moveQueue.Count > 0)
        {
            Move move = moveQueue.Dequeue();
            move.Id = ++id;
            allAfterMoves.Add(move);
            move._afterMoves?.ForEach(aMove => moveQueue.Enqueue(aMove));
        }

        return allAfterMoves;
    }

    public override string ToString()
       => $"{new string('\t', BeforeNum)}{Before?.Id}-{CoordPair}.{Id} {Remark}\n";
}
