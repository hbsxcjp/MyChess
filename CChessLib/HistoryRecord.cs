using System.Text;
using System.Numerics;
using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public class HistoryRecord
{
    private class MoveRecord
    {
        public ulong hashLock;
        public int frequency;

        public MoveRecord(ulong alock) { hashLock = alock; frequency = 1; }

        public override string? ToString()
        {
            return $"{hashLock,16:X16} {frequency}";
        }
    }

    private Dictionary<ulong, MoveRecord> historyRec;

    public HistoryRecord(List<Manual> manuals)
    {
        historyRec = new();
        Append(manuals);
    }

    public void Append(List<Manual> manuals) => manuals.ForEach(manual => Append(manual));

    public void Append(Manual manual)
    {
        BitBoard bitBoard = new(manual.ManualMove.RootBoard);
        int player = (int)manual.ManualMove.StartColor;
        void AddAfter(Move move)
        {
            move.AfterMoves?.ForEach(aMove =>
            {
                try
                {
                    int fromIndex = aMove.CoordPair.FromCoord.Index,
                        toIndex = aMove.CoordPair.ToCoord.Index;
                    PieceKind eatKind = bitBoard.DoMove(fromIndex, toIndex, false);

                    ulong hashKey = bitBoard.HashKey ^ BitConstants.ColorZobristKey[player];
                    ulong hashLock = bitBoard.HashKey ^ BitConstants.ColorZobristLock[player];
                    player = player == 0 ? 1 : 0;

                    MoveRecord? zobrist = GetMoveRecord(ref hashKey, hashLock);
                    if (zobrist == null)
                        historyRec.Add(hashKey, new(hashLock));
                    else
                        zobrist.frequency++;

                    // 深度优先递归调用
                    AddAfter(aMove);

                    bitBoard.DoMove(fromIndex, toIndex, true, eatKind);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}\nmanual:\n{manual.ToString()}\n\naMove: {aMove}");
                }
            });
        }

        AddAfter(manual.ManualMove.RootMove);
    }

    public int GetFrequency(ulong hashKey, ulong hashLock)
        => GetMoveRecord(ref hashKey, hashLock)?.frequency ?? 0;

    private MoveRecord? GetMoveRecord(ref ulong hashKey, ulong hashLock)
    {
        int index = 0;
        while (historyRec.ContainsKey(hashKey))
        {
            MoveRecord zobrist = historyRec[hashKey];
            if (zobrist.hashLock == hashLock)
                return zobrist;

            if (index > 2)
            {
                Debug.Assert(false, "重复碰撞最多允许3次!");
                return null;
            }

            hashKey ^= BitConstants.CollideZobristKey[index++];
            Console.WriteLine($"zobrist.hashLock != hashLock index: {index}\n");
        }

        return null;
    }

    public override string? ToString()
    {
        return string.Concat(historyRec.Where(keyValue => keyValue.Value.frequency > 600)
                .Select(keyValue => $"Key: {keyValue.Key,16:X16}  Record: {keyValue.Value}\n")) + $"Count: {historyRec.Count}";
    }

}