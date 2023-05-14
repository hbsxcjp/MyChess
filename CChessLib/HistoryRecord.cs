using System.Text;
using System.Numerics;
using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public class MoveRecord
{
    public long hashLock;
    public int frequency;

    public MoveRecord(long alock, int afre = 1)
    {
        hashLock = alock; frequency = afre;
    }

    public override string? ToString()
    {
        return $"{hashLock,16:X16} {frequency}";
    }
}

public class HistoryRecord
{
    public Dictionary<long, MoveRecord> historyDict;

    public HistoryRecord(Dictionary<long, MoveRecord> kvDict)
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
        BitBoard bitBoard = manual.ManualMove.RootBoard.BitBoard;
        PieceColor curColor = manual.ManualMove.StartColor;
        void AddAfter(Move move)
        {
            move.AfterMoves?.ForEach(aMove =>
            {
                try
                {
                    int fromIndex = aMove.CoordPair.FromCoord.Index,
                        toIndex = aMove.CoordPair.ToCoord.Index;
                    PieceKind eatKind = bitBoard.DoMove(fromIndex, toIndex);

                    long hashKey = bitBoard.GetHashKey(curColor);
                    long hashLock = bitBoard.GetHashLock(curColor);
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

    public MoveRecord? GetMoveRecord(ref long hashKey, long hashLock)
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