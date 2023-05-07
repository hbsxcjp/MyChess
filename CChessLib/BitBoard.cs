using System.Text;
using System.Numerics;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public class BitBoard
{
    // 基本数据
    private PieceColor[] colors;
    private PieceKind[] kinds;
    private BigInteger[][] pieces;

    // 计算中间存储数据(基本局面改动时更新)
    private PieceColor bottomColor;
    private BigInteger[] colorPieces;
    private BigInteger allPieces;
    private BigInteger rotatePieces;

    public BitBoard(Board board)
    {
        colors = new PieceColor[Coord.Count];
        kinds = new PieceKind[Coord.Count];
        pieces = new BigInteger[Piece.ColorCount][];
        for (int color = 0; color < Piece.ColorCount; ++color)
            pieces[color] = new BigInteger[Piece.KindCount];

        bottomColor = board.BottomColor;
        colorPieces = new BigInteger[Piece.ColorCount];
        allPieces = 0;
        rotatePieces = 0;

        foreach (int index in Coord.Indexs)
        {
            Piece piece = board[index];
            if (piece == Piece.Null)
            {
                colors[index] = PieceColor.NoColor;
                kinds[index] = PieceKind.NoKind;
                continue;
            }

            int color = (int)piece.Color, kind = (int)piece.Kind;
            BigInteger turnBoard = BitConstants.Mask[index];

            colors[index] = piece.Color;
            kinds[index] = piece.Kind;
            pieces[color][kind] ^= turnBoard;
            colorPieces[color] ^= turnBoard;
            allPieces ^= turnBoard;
            rotatePieces ^= BitConstants.RotateMask[index];

            HashKey ^= BitConstants.ZobristKey[color][kind][index];
            HashLock ^= BitConstants.ZobristLock[color][kind][index];
        }
    }

    public ulong HashKey { get; private set; }
    public ulong HashLock { get; private set; }

    delegate bool DoneState(int fromIndex, int toIndex);
    // delegate MoveEffect GetMoveEffect(PieceColor color, PieceKind kind, int fromIndex, int toIndex);

    public PieceColor GetColor(int index) => colors[index];

    public PieceKind DoMove(int fromIndex, int toIndex, bool isBack, PieceKind eatKind = PieceKind.NoKind)
    {
        int startIndex = isBack ? toIndex : fromIndex, endIndex = isBack ? fromIndex : toIndex;
        PieceColor fromColor = colors[startIndex];
        PieceKind fromKind = kinds[startIndex];
        int fromColorInt = (int)fromColor, toColorInt = fromColorInt == 0 ? 1 : 0, fromKindInt = (int)fromKind;
        BigInteger fromBoard = BitConstants.Mask[fromIndex], toBoard = BitConstants.Mask[toIndex],
                moveBoard = fromBoard | toBoard;
        if (!isBack)
            eatKind = kinds[toIndex];

        // 清除原位置，置位新位置
        colors[endIndex] = fromColor;
        kinds[endIndex] = fromKind;
        colors[startIndex] = PieceColor.NoColor;
        kinds[startIndex] = eatKind;

        pieces[fromColorInt][fromKindInt] ^= moveBoard;
        colorPieces[fromColorInt] ^= moveBoard;

        HashKey ^= (BitConstants.ZobristKey[fromColorInt][fromKindInt][fromIndex] ^ BitConstants.ZobristKey[fromColorInt][fromKindInt][toIndex]);
        HashLock ^= (BitConstants.ZobristLock[fromColorInt][fromKindInt][fromIndex] ^ BitConstants.ZobristLock[fromColorInt][fromKindInt][toIndex]);

        if (eatKind != PieceKind.NoKind)
        {
            if (isBack)
                colors[startIndex] = Piece.GetOtherColor(fromColor);

            int eatKindInt = (int)eatKind;
            pieces[toColorInt][eatKindInt] ^= toBoard;
            colorPieces[toColorInt] ^= toBoard;

            HashKey ^= BitConstants.ZobristKey[toColorInt][eatKindInt][toIndex];
            HashLock ^= BitConstants.ZobristLock[toColorInt][eatKindInt][toIndex];

            allPieces ^= fromBoard;
            rotatePieces ^= BitConstants.RotateMask[fromIndex];
        }
        else
        {
            allPieces ^= moveBoard;
            rotatePieces ^= BitConstants.RotateMask[fromIndex] | BitConstants.RotateMask[toIndex];
        }

        return eatKind;
    }

    private BigInteger GetMove(int fromIndex)
    {
        BigInteger bitMove;
        PieceColor fromColor = colors[fromIndex];
        PieceKind fromKind = kinds[fromIndex];
        switch (fromKind)
        {
            case PieceKind.King:
                bitMove = BitConstants.KingMove[fromIndex];
                break;
            case PieceKind.Advisor:
                bitMove = BitConstants.AdvisorMove[fromIndex];
                break;
            case PieceKind.Bishop:
                bitMove = BitConstants.GetBishopMove(fromIndex, allPieces);
                break;
            case PieceKind.Knight:
                bitMove = BitConstants.GetKnightMove(fromIndex, allPieces);
                break;
            case PieceKind.Pawn:
                bitMove = BitConstants.PawnMove[bottomColor == fromColor ? 1 : 0][fromIndex];
                break;
            default: // PieceKind.Rook PieceKind.Cannon
                bitMove = BitConstants.GetRookCannonMove(fromKind == PieceKind.Cannon, fromIndex, allPieces, rotatePieces);
                break;
        }

        // 去掉同色棋子
        return bitMove ^ (bitMove & colorPieces[(int)fromColor]);
    }

    private BigInteger GetKindMove(PieceColor fromColor, PieceKind fromKind)
        => BitConstants.MergeBitInt(BitConstants.GetNonZeroIndexs(pieces[(int)fromColor][(int)fromKind])
                .Select(fromIndex => GetMove(fromIndex)));

    private BigInteger GetColorMove(PieceColor fromColor)
        => BitConstants.MergeBitInt(Piece.PieceKinds.Select(fromKind => GetKindMove(fromColor, fromKind)));

    private bool DoneNonKilled(int fromIndex, int toIndex)
    {
        PieceColor color = colors[toIndex]; // 此时着法已执行
        return (GetColorMove(Piece.GetOtherColor(color)) & pieces[(int)color][(int)PieceKind.King]).IsZero;
    }

    // 执行某一着后的效果(委托函数可叠加)
    private MoveEffect GetMoveEffect(int fromIndex, int toIndex, DoneState doneState)
    {
        MoveEffect effect = new MoveEffect { fromIndex = fromIndex, toIndex = toIndex, score = 0 };
        PieceKind eatKind = DoMove(fromIndex, toIndex, false);

        effect.score += doneState(fromIndex, toIndex) ? 1 : 0;

        DoMove(fromIndex, toIndex, true, eatKind);
        return effect;
    }

    public override string ToString()
    {
        List<BigInteger> moves = new();
        StringBuilder boardStr = new();
        boardStr.Append("－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n");
        for (int color = 0; color < Piece.ColorCount; ++color)
        {
            for (int kind = 0; kind < Piece.KindCount; ++kind)
            {
                BigInteger piece = pieces[color][kind];
                BitConstants.GetNonZeroIndexs(piece).ForEach(fromIndex =>
                {
                    Coord coord = Coord.Coords[fromIndex];
                    boardStr[coord.Row * (Coord.ColCount + 1) + coord.Col] = Piece.GetPrintName((PieceColor)color, (PieceKind)kind);

                    // 计算可移动位置
                    moves.Add(BitConstants.MergeBitInt(
                        BitConstants.GetNonZeroIndexs(GetMove(fromIndex))
                        .Where(toIndex => GetMoveEffect(fromIndex, toIndex, DoneNonKilled).score > 0)
                        .Select(toIndex => BitConstants.Mask[toIndex])));
                });
            }
        }
        boardStr.Append(BitConstants.GetBigIntArrayString(moves.ToArray(), 8, true, false)).Append("\n");

        return boardStr.ToString();
    }


}

public class HistoryDictionary
{
    private class MoveZobrist
    {
        public ulong hashLock;
        public int frequency;

        public MoveZobrist? next;

        public MoveZobrist(ulong alock) { hashLock = alock; frequency = 1; next = null; }
    }

    private MoveZobrist NullMoveZobrist = new(0);

    private Dictionary<ulong, MoveZobrist> historyDict;

    public HistoryDictionary(List<Manual> manuals)
    {
        historyDict = new();
        manuals.ForEach(manual => Append(manual));
    }

    public void Append(Manual manual)
    {
        BitBoard bitBoard = new(manual.ManualMove.RootBoard);
        int player = (int)manual.ManualMove.StartColor;
        void AddAfter(Move move)
        {
            if (move.AfterMoves == null)
                return;

            move.AfterMoves.ForEach(aMove =>
            {
                int fromIndex = aMove.CoordPair.FromCoord.Index,
                    toIndex = aMove.CoordPair.ToCoord.Index;
                PieceKind eatKind = bitBoard.DoMove(fromIndex, toIndex, false);

                // 走棋方设置(利用空闲键值)
                ulong hashKey = bitBoard.HashKey ^ BitConstants.ZobristKey[player][(int)PieceKind.King][0];
                ulong hashLock = bitBoard.HashKey ^ BitConstants.ZobristLock[player][(int)PieceKind.King][0];
                player = player == 0 ? 1 : 0;

                if (historyDict.ContainsKey(hashKey))
                {
                    MoveZobrist? zobrist = historyDict[hashKey];
                    while (zobrist.hashLock != hashLock)
                    {
                        Console.WriteLine($"zobrist.hashLock != hashLock\n");
                        if (zobrist.next == null)
                        {
                            zobrist.next = new(hashLock);
                            break;
                        }

                        zobrist = zobrist.next;
                    }
                    if (zobrist.hashLock == hashLock)
                        zobrist.frequency++;
                }
                else
                    historyDict.Add(hashKey, new(hashLock));

                // 深度优先递归调用
                AddAfter(aMove);

                bitBoard.DoMove(fromIndex, toIndex, true, eatKind);
            });
        }

        AddAfter(manual.ManualMove.RootMove);
    }

    public int GetFrequency(ulong hashKey, ulong hashLock)
    {
        if (!historyDict.ContainsKey(hashKey))
            return 0;

        MoveZobrist zobrist = historyDict[hashKey];
        while (zobrist.hashLock != hashLock)
        {
            Console.WriteLine($"zobrist.hashLock != hashLock\n");
            if (zobrist.next == null)
                return 0;

            zobrist = zobrist.next;
        }

        return zobrist.frequency;
    }

    public override string? ToString()
    {
        return string.Concat(historyDict.Keys.Select(key => $"{key}\n")) + $"{historyDict.Keys.Count}";
    }

}