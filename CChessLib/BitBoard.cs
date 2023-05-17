using System.Text;
using System.Numerics;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public class BitBoard
{
    // 基本数据
    private PieceColor[] colors; // [index]
    private PieceKind[] kinds; // [index]
    private BigInteger[][] pieces; // [color][kind]

    // 计算中间存储数据(基本局面改动时更新)
    private BigInteger[] colorPieces; // [color]
    private BigInteger allPieces;
    private BigInteger rotatePieces;

    // 哈希局面数据
    private long hashkey;
    private long hashLock;

    private static HistoryRecord? historyRecord;

    public BitBoard(Board board)
    {
        colors = new PieceColor[Coord.Count];
        kinds = new PieceKind[Coord.Count];
        pieces = new BigInteger[Piece.ColorCount][];
        for (int color = 0; color < Piece.ColorCount; ++color)
            pieces[color] = new BigInteger[Piece.KindCount];

        colorPieces = new BigInteger[Piece.ColorCount];
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

            hashkey ^= BitConstants.ZobristKey[color][kind][index];
            hashLock ^= BitConstants.ZobristLock[color][kind][index];
        }

        BottomColor = board.BottomColor;
    }

    public static HistoryRecord HistoryRecord
    {
        get
        {
            if (historyRecord == null)
                historyRecord = Database.GetHistoryRecord();

            return historyRecord;
        }
    }

    public PieceColor BottomColor { get; }

    // 返回着法执行后的状态效果
    delegate MoveEffect GetEffect(int fromIndex, int toIndex, PieceKind eatKind);

    public PieceColor GetColor(int index) => colors[index];

    public long GetHashKey(PieceColor color) => hashkey ^ BitConstants.ColorZobristKey[(int)color];
    public long GetHashLock(PieceColor color) => hashLock ^ BitConstants.ColorZobristLock[(int)color];

    public PieceKind DoMove(int fromIndex, int toIndex, bool isBack = false, PieceKind eatKind = PieceKind.NoKind)
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
        kinds[startIndex] = PieceKind.NoKind;

        pieces[fromColorInt][fromKindInt] ^= moveBoard;
        colorPieces[fromColorInt] ^= moveBoard;

        hashkey ^= (BitConstants.ZobristKey[fromColorInt][fromKindInt][fromIndex] ^ BitConstants.ZobristKey[fromColorInt][fromKindInt][toIndex]);
        hashLock ^= (BitConstants.ZobristLock[fromColorInt][fromKindInt][fromIndex] ^ BitConstants.ZobristLock[fromColorInt][fromKindInt][toIndex]);

        if (eatKind != PieceKind.NoKind)
        {
            if (isBack)
            {
                colors[startIndex] = Piece.GetOtherColor(fromColor);
                kinds[startIndex] = eatKind;
            }
            int eatKindInt = (int)eatKind;
            pieces[toColorInt][eatKindInt] ^= toBoard;
            colorPieces[toColorInt] ^= toBoard;

            hashkey ^= BitConstants.ZobristKey[toColorInt][eatKindInt][toIndex];
            hashLock ^= BitConstants.ZobristLock[toColorInt][eatKindInt][toIndex];

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

    public bool CanMove(CoordPair coordPair)
        => GetCanToIndexs(coordPair.FromCoord.Index).Contains(coordPair.ToCoord.Index);

    public List<int> GetCanToIndexs(int fromIndex)
        => BitConstants.GetNonZeroIndexs(GetMove(fromIndex))
            .Where(toIndex => GetMoveEffect(fromIndex, toIndex, GetKilled).score >= 0).ToList();

    public List<(int, List<int>)> GetAllCanToIndexs(PieceColor color)
        => BitConstants.GetNonZeroIndexs(colorPieces[(int)color])
            .Select(fromIndex => (fromIndex, GetCanToIndexs(fromIndex))).ToList();

    public bool IsKilled(PieceColor color)
    {
        PieceColor otherColor = Piece.GetOtherColor(color);
        int fromColorInt = (int)color, kingInt = (int)PieceKind.King;
        bool kingFace()
        {
            int kingIndex = BitConstants.GetNonZeroIndexs(pieces[fromColorInt][kingInt])[0],
                otherKingIndex = BitConstants.GetNonZeroIndexs(pieces[(int)otherColor][kingInt])[0];
            int minKingIndex = Math.Min(kingIndex, otherKingIndex),
                maxKingIndex = Math.Max(kingIndex, otherKingIndex);
            if ((maxKingIndex - minKingIndex) % Coord.ColCount != 0)
                return false;

            for (int index = minKingIndex + Coord.ColCount; index < maxKingIndex; index += Coord.ColCount)
                if (!(allPieces & BitConstants.Mask[index]).IsZero)
                    return false;

            return true;
        }

        return kingFace() || !(GetColorMove(otherColor) & pieces[fromColorInt][kingInt]).IsZero;
    }

    public bool IsFailed(PieceColor color) => GetColorMove(color).IsZero;

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
                bitMove = BitConstants.PawnMove[BottomColor == fromColor ? 1 : 0][fromIndex];
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

    public MoveRecord? GetMoveRecord(PieceColor color)
    {
        long hashKey = GetHashKey(color);
        return HistoryRecord.GetMoveRecord(ref hashKey, GetHashLock(color));
    }

    public int GetFrequency(PieceColor color)
        => GetMoveRecord(color)?.frequency ?? 0;

    private MoveEffect GetKilled(int fromIndex, int toIndex, PieceKind eatKind)
    {
        // 如是对方将帅的位置则直接可走，不用判断是否被将军（如加以判断，则会直接走棋吃将帅）
        return new(eatKind != PieceKind.King && IsKilled(colors[toIndex]) ? -1 : 0, 0);
    }

    // 执行某一着后的效果(委托函数可叠加)
    private MoveEffect GetMoveEffect(int fromIndex, int toIndex, GetEffect getEffect)
    {

        PieceKind eatKind = DoMove(fromIndex, toIndex);

        MoveEffect effect = getEffect(fromIndex, toIndex, eatKind);

        DoMove(fromIndex, toIndex, true, eatKind);
        return effect;
    }

    public override string ToString()
    {
        List<BigInteger> moveBigInts = new();
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
                    moveBigInts.Add(BitConstants.MergeBitInt(
                        GetCanToIndexs(fromIndex).Select(toIndex => BitConstants.Mask[toIndex])));
                });
            }
        }
        boardStr.Append(BitConstants.GetBigIntArrayString(moveBigInts.ToArray(), 8, true, false)).Append("\n");
        // boardStr.Append($"historyRecord.Count: {HistoryRecord.historyDict.Count}");

        return boardStr.ToString();
    }


}