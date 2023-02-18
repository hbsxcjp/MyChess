// using System.Collections;

// namespace CChess;

// public class ManualMoveEnum : IEnumerator
// {
//     private readonly Queue<Move> _moveQueue;
//     private readonly ManualMove _manualMove;
//     private int _id; // 设置ID的临时变量

//     public ManualMoveEnum(ManualMove manualMove)
//     {
//         _moveQueue = new();
//         _manualMove = manualMove;

//         Current = manualMove.RootMove; // 消除未赋值警示
//         Reset();
//     }

//     object IEnumerator.Current { get => Current; }

//     public Move Current { get; set; }

//     public void Reset()
//     {
//         _moveQueue.Clear();
//         _id = 0;
//         _manualMove.RootMove.AfterMoves()?.ForEach(move => _moveQueue.Enqueue(move));
//     }

//     // 迭代不含根节点
//     public bool MoveNext()
//     {
//         if (_moveQueue.Count == 0)
//             return false;

//         Current = _moveQueue.Dequeue();
//         Current.Id = ++_id;
//         Current.AfterMoves()?.ForEach(move => _moveQueue.Enqueue(move));
//         return true;
//     }
// }