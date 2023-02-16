using System.Collections;

namespace CChess;

public class ManualMoveEnum : IEnumerator
{
    private readonly Queue<Move> _moveQueue;
    private readonly ManualMove _manualMove;
    private readonly bool _enumMoveDone;
    private int _id; // 设置ID的临时变量

    public ManualMoveEnum(ManualMove manualMove)
    {
        _moveQueue = new();
        _manualMove = manualMove;
        _enumMoveDone = manualMove.EnumMoveDone;

        Current = manualMove.CurMove; // 消除未赋值警示
        Reset();
    }

    object IEnumerator.Current { get => Current; }

    public Move Current { get; set; }

    public void Reset()
    {
        _manualMove.BackStart();
        _moveQueue.Clear();
        _id = 0;
        SetCurrentEnqueueAfterMoves(_manualMove.CurMove);
    }

    // 迭代不含根节点。如执行着法，棋局执行至当前之前着，当前着法未执行
    public bool MoveNext()
    {
        if (_moveQueue.Count == 0)
        {
            if (_enumMoveDone)
                _manualMove.BackStart();

            return false;
        }

        SetCurrentEnqueueAfterMoves(_moveQueue.Dequeue());
        return true;
    }

    private void SetCurrentEnqueueAfterMoves(Move curMove)
    {
        Current = curMove;
        curMove.Id = _id++;
        // 根据枚举特性判断是否执行着法
        if (_enumMoveDone)
            _manualMove.GoTo(curMove.Before);

        curMove.AfterMoves()?.ForEach(move => _moveQueue.Enqueue(move));
    }
}