using System.Text;
using System.Numerics;
using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public class HistoryRecord
{
    public class MoveRecord
    {
        public ulong hashLock;
        public int frequency;

        public MoveRecord(ulong alock, int afre = 1)
        {
            hashLock = alock; frequency = afre;
        }

        public override string? ToString()
        {
            return $"{hashLock,16:X16} {frequency}";
        }
    }

    public Dictionary<ulong, MoveRecord> historyDict;

    public HistoryRecord(Dictionary<ulong, MoveRecord> kvDict)
    {
        historyDict = kvDict;
    }

    public HistoryRecord(List<Manual> manuals)
    {
        historyDict = new();
        manuals.ForEach(manual => Append(manual));
    }

    public void Append(Manual manual)
    {
        BitBoard bitBoard = new(manual.ManualMove.RootBoard);
        PieceColor curColor = manual.ManualMove.StartColor;
        void AddAfter(Move move)
        {
            move.AfterMoves?.ForEach(aMove =>
            {
                try
                {
                    int fromIndex = aMove.CoordPair.FromCoord.Index,
                        toIndex = aMove.CoordPair.ToCoord.Index;
                    PieceKind eatKind = bitBoard.DoMove(fromIndex, toIndex, false);

                    ulong hashKey = bitBoard.GetHashKey(curColor);
                    ulong hashLock = bitBoard.GetHashLock(curColor);
                    curColor = Piece.GetOtherColor(curColor);

                    MoveRecord? zobrist = GetMoveRecord(ref hashKey, hashLock);
                    if (zobrist == null)
                        historyDict.Add(hashKey, new(hashLock));
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
        while (historyDict.ContainsKey(hashKey))
        {
            MoveRecord zobrist = historyDict[hashKey];
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
        return string.Concat(historyDict.Where(keyValue => keyValue.Value.frequency > 900)
                .Select(keyValue => $"Key: {keyValue.Key,16:X16}  Record: {keyValue.Value}\n")) + $"Count: {historyDict.Count}";
    }

}