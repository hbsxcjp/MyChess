using System.Collections;

namespace CChess;

public class ManualMoveEnum : IEnumerator
{
    private readonly Queue<Move> _moveQueue;
    private readonly ManualMove _manualMove;
    private Move _curMove;
    private int _id;

    public ManualMoveEnum(ManualMove manualMove)
    {
        _manualMove = manualMove;
        _moveQueue = new();
        _curMove = manualMove.CurMove; // 消除未赋值警示

        Reset();
    }

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
            if (_manualMove.EnumMoveDone)
                _manualMove.BackStart();
            return false;
        }

        SetCurrentEnqueueAfterMoves(_moveQueue.Dequeue());
        return true;
    }

    object IEnumerator.Current { get { return Current; } }

    public Move Current { get { return _curMove; } }

    private void SetCurrentEnqueueAfterMoves(Move curMove)
    {
        _curMove = curMove;
        _curMove.Id = _id++;
        // 根据枚举特性判断是否执行着法
        if (_manualMove.EnumMoveDone)
            _manualMove.GoTo(_curMove.Before);

        var afterMoves = _curMove.AfterMoves();
        if (afterMoves != null)
        {
            foreach (var move in afterMoves)
                _moveQueue.Enqueue(move);
        }
    }
}