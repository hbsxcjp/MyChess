namespace CChess;

// public enum VisibleType { All, True, False }

public class Move
{
    public static Move RootMove() => new(CoordPair.Null);

    private Move(CoordPair coordPair, Move? before = null, string? remark = null, bool visible = true)
    {
        Before = before;

        CoordPair = coordPair;
        Remark = remark;
        Visible = visible;
    }

    public Move? Before { get; }
    public CoordPair CoordPair { get; }

    // 对象属性
    public string? Remark { get; set; }
    public bool Visible { get; set; }

    public int Id { get; private set; }
    public List<Move>? AfterMoves { get; private set; }

    public List<Move>? UntilThis
    {
        get
        {
            List<Move>? untilThis = null;
            Move move = this;
            while (move.Before != null)
            {
                (untilThis ??= new()).Insert(0, move);
                move = move.Before;
            }

            return untilThis;
        }
    }

    // 代替ManualMove枚举器
    public List<Move> AllAfterMoves
    {
        get
        {
            List<Move> allAfterMoves = new(100);
            Queue<Move> moveQueue = new();
            void AppendAfter(Move move) => move.AfterMoves?.ForEach(aMove => moveQueue.Enqueue(aMove));

            AppendAfter(this);
            while (moveQueue.Count > 0)
            {
                Move move = moveQueue.Dequeue();
                allAfterMoves.Add(move);
                AppendAfter(move);
            }

            int id = 0;
            allAfterMoves.ForEach(move => move.Id = ++id);
            return allAfterMoves;
        }
    }

    public List<Move>? OtherMoves => Before?.AfterMoves;

    public int AfterNum => AfterMoves?.Count ?? 0;
    public bool HasAfter => AfterMoves != null;

    public Move AddAfter(CoordPair coordPair, string? remark = null, bool visible = true)
    {
        Move move = new(coordPair, this, remark, visible);
        (AfterMoves ??= new()).Add(move);

        return move;
    }

    public override string ToString()
       => $"{new string('\t', UntilThis?.Count ?? 0)}{Before?.Id}-{CoordPair}.{Id} {Remark}\n";

    public string SymmetryVToString()
       => $"{new string('\t', UntilThis?.Count ?? 0)}{Before?.Id}-{CoordPair.SymmetryVToString()}.{Id} {Remark}\n";
}
