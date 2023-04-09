#define DEBUGZOBRIST

using System.Numerics;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public enum BitNum
{
    COLORNUM = 2,
    KINDNUM = 7,
    BOARDROWNUM = 10,
    BOARDCOLNUM = 9,
    BOARDLENGTH = (BOARDROWNUM * BOARDCOLNUM),
    LEGCOUNT = 4,
    // MOVEBOARDMAXCOUNT = 256,
}

public static class BitConstants
{
    public static readonly BigInteger[] Mask = Coord.Coords.Select(
        coord => (BigInteger)1 << coord.Index).ToArray();
    public static readonly BigInteger[] RotateMask = Coord.Coords.Select(
        coord => (BigInteger)1 << coord.Col * (int)BitNum.BOARDROWNUM + coord.Row).ToArray();

    // 帅仕根据所处的位置选取可移动位棋盘
    public static readonly BigInteger[] KingMove = CreateKingMove();
    public static readonly BigInteger[] AdvisorMove = CreateAdvisorMove();

    // 马相根据憋马腿或田心组成的四个位置状态选取可移动位棋盘
    private static readonly BigInteger[,] BishopMove = CreateBishopMove();
    private static readonly BigInteger[,] KnightMove = CreateKnightMove();

    // 车炮根据每行和每列的位置状态选取可移动位棋盘
    private static readonly BigInteger[,] RookRowMove = CreateRookRowMove();
    private static readonly BigInteger[,] RookColMove = CreateRookColMove();
    private static readonly BigInteger[,] CannonRowMove = CreateCannonRowMove();
    private static readonly BigInteger[,] CannonColMove = CreateCannonColMove();

    // 兵根据本方处于上或下的二个位置状态选取可移动位棋盘
    public static readonly BigInteger[,] PawnMove = CreatePawnMove();

    public static readonly ulong ZobristBlack = (ulong)(new Random(7)).NextInt64(long.MinValue, long.MaxValue);
    public static readonly ulong[,,] Zobrist = CreateZobrist();

    // 求最低位非零位的序号，调用前判断参数非零
    public static int TrailingZeroCount(BigInteger bigInt)
    {
        int count = 0;
        ulong value = (ulong)(bigInt & 0XFFFFFFFFFFFFFFFFUL);// 00-63 位
        if (value == 0)
        {
            value = (ulong)(bigInt >> 64); // 64-89 位
            count = 64;
        }

        return count + BitOperations.TrailingZeroCount(value);
    }

    private static BigInteger[] CreateKingMove()
    {
        static bool isValidKing(int row, int col)
        {
            return (row < 3 || row > 6) && (col > 2 && col < 6);
        }

        BigInteger[] kingMove = new BigInteger[(int)BitNum.BOARDLENGTH];
        for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            if (!isValidKing(row, col))
                continue;

            BigInteger match = 0;
            if (col > 3)
                match |= Mask[index - 1];
            if (col < 5)
                match |= Mask[index + 1];
            if (row == 0 || row == 1 || row == 7 || row == 8)
                match |= Mask[index + (int)BitNum.BOARDCOLNUM];
            if (row == 1 || row == 2 || row == 8 || row == 9)
                match |= Mask[index - (int)BitNum.BOARDCOLNUM];

            kingMove[index] = match;
        }

        return kingMove;
    }

    private static BigInteger[] CreateAdvisorMove()
    {
        static bool isValidAdvisor(int row, int col)
        {
            return (((row == 0 || row == 2 || row == 7 || row == 9) && (col == 3 || col == 5))
                || ((row == 1 || row == 8) && col == 4));
        }

        BigInteger[] advisorMove = new BigInteger[(int)BitNum.BOARDLENGTH];
        for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            if (!isValidAdvisor(row, col))
                continue;

            BigInteger match;
            if (col == 4)
                match = (Mask[index - (int)BitNum.BOARDCOLNUM - 1]
                    | Mask[index - (int)BitNum.BOARDCOLNUM + 1]
                    | Mask[index + (int)BitNum.BOARDCOLNUM - 1]
                    | Mask[index + (int)BitNum.BOARDCOLNUM + 1]);
            else
                match = Mask[row < 3 ? 13 : 76];

            advisorMove[index] = match;
        }

        return advisorMove;
    }

    private static BigInteger[,] CreateBishopMove()
    {
        static bool isValidBishop(int row, int col)
        {
            return (((row == 0 || row == 4 || row == 5 || row == 9) && (col == 2 || col == 6))
                || ((row == 2 || row == 7) && (col == 0 || col == 4 || col == 8)));
        }

        BigInteger[,] bishopMove = new BigInteger[(int)BitNum.BOARDLENGTH, 1 << (int)BitNum.LEGCOUNT];
        for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            if (!isValidBishop(row, col))
                continue;

            for (int state = 0; state < 1 << ((int)BitNum.LEGCOUNT); ++state)
            {
                int realState = state;
                if (row == 0 || row == 5)
                    realState |= (1 << ((int)BitNum.LEGCOUNT - 1) | 1 << ((int)BitNum.LEGCOUNT - 2));
                else if (row == 4 || row == (int)BitNum.BOARDROWNUM - 1)
                    realState |= (1 << ((int)BitNum.LEGCOUNT - 3) | 1 << ((int)BitNum.LEGCOUNT - 4));
                if (col == 0)
                    realState |= (1 << ((int)BitNum.LEGCOUNT - 1) | 1 << ((int)BitNum.LEGCOUNT - 3));
                else if (col == (int)BitNum.BOARDCOLNUM - 1)
                    realState |= (1 << ((int)BitNum.LEGCOUNT - 2) | 1 << ((int)BitNum.LEGCOUNT - 4));

                BigInteger match = 0;
                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 1))))
                    match |= Mask[index - 2 * (int)BitNum.BOARDCOLNUM - 2];

                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 2))))
                    match |= Mask[index - 2 * (int)BitNum.BOARDCOLNUM + 2];

                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 3))))
                    match |= Mask[index + 2 * (int)BitNum.BOARDCOLNUM - 2];

                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 4))))
                    match |= Mask[index + 2 * (int)BitNum.BOARDCOLNUM + 2];

                bishopMove[index, state] = match;
            }
        }

        return bishopMove;
    }

    private static BigInteger[,] CreateKnightMove()
    {
        BigInteger[,] knightMove = new BigInteger[(int)BitNum.BOARDLENGTH, 1 << (int)BitNum.LEGCOUNT];
        for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            for (int state = 0; state < 1 << ((int)BitNum.LEGCOUNT); ++state)
            {
                int realState = state;
                if (row == 0)
                    realState |= 1 << ((int)BitNum.LEGCOUNT - 1);
                else if (row == (int)BitNum.BOARDROWNUM - 1)
                    realState |= 1 << ((int)BitNum.LEGCOUNT - 4);
                if (col == 0)
                    realState |= 1 << ((int)BitNum.LEGCOUNT - 2);
                else if (col == (int)BitNum.BOARDCOLNUM - 1)
                    realState |= 1 << ((int)BitNum.LEGCOUNT - 3);

                BigInteger match = 0;
                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 1))) && row > 1)
                {
                    if (col > 0)
                        match |= Mask[index - 2 * (int)BitNum.BOARDCOLNUM - 1];
                    if (col < (int)BitNum.BOARDCOLNUM - 1)
                        match |= Mask[index - 2 * (int)BitNum.BOARDCOLNUM + 1];
                }
                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 2))) && col > 1)
                {
                    if (row > 0)
                        match |= Mask[index - (int)BitNum.BOARDCOLNUM - 2];
                    if (row < (int)BitNum.BOARDROWNUM - 1)
                        match |= Mask[index + (int)BitNum.BOARDCOLNUM - 2];
                }
                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 3))) && col < (int)BitNum.BOARDCOLNUM - 2)
                {
                    if (row > 0)
                        match |= Mask[index - (int)BitNum.BOARDCOLNUM + 2];
                    if (row < (int)BitNum.BOARDROWNUM - 1)
                        match |= Mask[index + (int)BitNum.BOARDCOLNUM + 2];
                }
                if (0 == (realState & (1 << ((int)BitNum.LEGCOUNT - 4))) && row < (int)BitNum.BOARDROWNUM - 2)
                {
                    if (col > 0)
                        match |= Mask[index + 2 * (int)BitNum.BOARDCOLNUM - 1];
                    if (col < (int)BitNum.BOARDCOLNUM - 1)
                        match |= Mask[index + 2 * (int)BitNum.BOARDCOLNUM + 1];
                }

                knightMove[index, state] = match;
            }

        }

        return knightMove;
    }

    private static void InitRookCannonMove(BigInteger[,] rookCanonRowColMove, PieceKind kind, bool isCol)
    {
        static int getMatch(int state, int rowColIndex, bool isCannon, bool isRotate)
        {
            int match = 0;
            for (int isHigh = 0; isHigh < 2; ++isHigh)
            {
                int direction = isHigh == 1 ? 1 : -1,
                    endIndex = isHigh == 1 ? (isRotate ? (int)BitNum.BOARDROWNUM : (int)BitNum.BOARDCOLNUM) - 1 : 0; // 每行列数或每列行数
                bool skip = false; // 炮是否已跳
                for (int i = direction * (rowColIndex + direction); i <= endIndex; ++i)
                {
                    int index = direction * i;
                    bool hasPiece = (state & 1 << index) != 0;
                    if (isCannon)
                    {
                        if (!skip)
                        {
                            if (hasPiece)
                                skip = true;
                            else
                                match |= (1 << index);
                        }
                        else if (hasPiece)
                        {
                            match |= (1 << index);
                            break;
                        }
                    }
                    else
                    {
                        match |= (1 << index);
                        if (hasPiece) // 遇到棋子
                            break;
                    }
                }
            }

            return match;
        }

        bool isCannon = kind == PieceKind.Cannon;
        int bitLength = isCol ? (int)BitNum.BOARDROWNUM : (int)BitNum.BOARDCOLNUM,
            stateTotal = 1 << bitLength;
        for (int rowColIndex = 0; rowColIndex < bitLength; ++rowColIndex)
        {
            for (int state = 0; state < stateTotal; ++state)
            {
                // 本状态当前行或列位置无棋子
                if (0 == (state & 1 << rowColIndex))
                    continue;

                int match = getMatch(state, rowColIndex, isCannon, isCol);
                if (match == 0)
                    continue;

                if (isCol)
                {
                    BigInteger colMatch = 0;
                    for (int row = 0; row < (int)BitNum.BOARDROWNUM; ++row)
                    {
                        if (0 != (match & 1 << row))
                            colMatch |= Mask[row * (int)BitNum.BOARDCOLNUM]; // 每行的首列置位
                    }

                    rookCanonRowColMove[rowColIndex, state] = colMatch;
                }
                else
                    rookCanonRowColMove[rowColIndex, state] = match;
            }
        }
    }

    private static BigInteger[,] CreateRookRowMove()
    {
        BigInteger[,] rookRowMove = new BigInteger[(int)BitNum.BOARDCOLNUM, 1 << (int)BitNum.BOARDCOLNUM];
        InitRookCannonMove(rookRowMove, PieceKind.Rook, false);

        return rookRowMove;
    }

    private static BigInteger[,] CreateRookColMove()
    {
        BigInteger[,] rookColMove = new BigInteger[(int)BitNum.BOARDROWNUM, 1 << (int)BitNum.BOARDROWNUM];
        InitRookCannonMove(rookColMove, PieceKind.Rook, true);

        return rookColMove;
    }

    private static BigInteger[,] CreateCannonRowMove()
    {
        BigInteger[,] cannonRowMove = new BigInteger[(int)BitNum.BOARDCOLNUM, 1 << (int)BitNum.BOARDCOLNUM];
        InitRookCannonMove(cannonRowMove, PieceKind.Cannon, false);

        return cannonRowMove;
    }

    private static BigInteger[,] CreateCannonColMove()
    {
        BigInteger[,] cannonColMove = new BigInteger[(int)BitNum.BOARDROWNUM, 1 << (int)BitNum.BOARDROWNUM];
        InitRookCannonMove(cannonColMove, PieceKind.Cannon, true);

        return cannonColMove;
    }

    private static BigInteger[,] CreatePawnMove()
    {
        static bool isValidPawn(int row, int col, bool isBottom)
        {
            return (isBottom ? (row < 5 || ((row == 5 || row == 6) && (col == 0 || col == 2 || col == 4 || col == 6 || col == 8)))
                             : (row > 4 || ((row == 3 || row == 4) && (col == 0 || col == 2 || col == 4 || col == 6 || col == 8))));
        }

        BigInteger[,] pawnMove = new BigInteger[(int)BitNum.BOARDLENGTH, 1 << ((int)BitNum.COLORNUM - 1)];
        for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            for (int isBottom = 0; isBottom < 2; ++isBottom)
            {

                if (!isValidPawn(row, col, isBottom == 1))
                    continue;

                BigInteger match = 0;
                if ((isBottom == 0 && row > 4) || (isBottom == 1 && row < 5))
                {
                    if (col != 0)
                        match |= Mask[index - 1];
                    if (col != (int)BitNum.BOARDCOLNUM - 1)
                        match |= Mask[index + 1];
                }

                if (isBottom == 0 && row != (int)BitNum.BOARDROWNUM - 1)
                    match |= Mask[index + (int)BitNum.BOARDCOLNUM];

                if (isBottom == 1 && row != 0)
                    match |= Mask[index - (int)BitNum.BOARDCOLNUM];

                pawnMove[index, isBottom] = match;
            }
        }

        return pawnMove;
    }

    public static BigInteger GetBishopMove(int fromIndex, BigInteger allPieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col;
        bool isTop = row == 0 || row == 5,
             isBottom = row == 4 || row == (int)BitNum.BOARDROWNUM - 1,
             isLeft = col == 0,
             isRight = col == (int)BitNum.BOARDCOLNUM - 1;
        int state = ((isTop || isLeft || !(allPieces & Mask[fromIndex - (int)BitNum.BOARDCOLNUM - 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 1) : 0)
            | (isTop || isRight || !(allPieces & Mask[fromIndex - (int)BitNum.BOARDCOLNUM + 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 2) : 0)
            | (isBottom || isLeft || !(allPieces & Mask[fromIndex + (int)BitNum.BOARDCOLNUM - 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 3) : 0)
            | (isBottom || isRight || !(allPieces & Mask[fromIndex + (int)BitNum.BOARDCOLNUM + 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 4) : 0));

        return BishopMove[fromIndex, state];
    }

    public static BigInteger GetKnightMove(int fromIndex, BigInteger allPieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col;
        int state = ((row == 0 || !(allPieces & Mask[fromIndex - (int)BitNum.BOARDCOLNUM]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 1) : 0)
            | (col == 0 || !(allPieces & Mask[fromIndex - 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 2) : 0)
            | (col == (int)BitNum.BOARDCOLNUM - 1 || !(allPieces & Mask[fromIndex + 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 3) : 0)
            | (row == (int)BitNum.BOARDROWNUM - 1 || !(allPieces & Mask[fromIndex + (int)BitNum.BOARDCOLNUM]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 4) : 0));

        return KnightMove[fromIndex, state];
    }

    public static BigInteger GetRookCannonMove(bool isCannon, int fromIndex, BigInteger allPieces, BigInteger rotatePieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col,
            rowOffset = row * (int)BitNum.BOARDCOLNUM;
        BigInteger[,] rowMove = isCannon ? CannonRowMove : RookRowMove,
            colMove = isCannon ? CannonColMove : RookColMove;

        return ((rowMove[col, (int)((allPieces >> rowOffset) & 0x1FF)] << rowOffset)
            | (colMove[row, (int)((rotatePieces >> col * (int)BitNum.BOARDROWNUM) & 0x3FF)] << col)); // 每行首列置位全体移动数列
    }

    private static ulong[,,] CreateZobrist()
    {
        ulong[,,] zobrist = new ulong[(int)BitNum.COLORNUM, (int)BitNum.KINDNUM, (int)BitNum.BOARDLENGTH];
        Random random = new Random();
        for (int color = 0; color < (int)BitNum.COLORNUM; ++color)
        {
#if DEBUGZOBRIST
            Console.WriteLine($"Color: {color}");
#endif
            for (int kind = 0; kind < (int)BitNum.KINDNUM; ++kind)
            {
#if DEBUGZOBRIST
                Console.WriteLine($"Kind: {kind}");
#endif
                for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
                {
                    ulong value = (ulong)random.NextInt64(long.MinValue, long.MaxValue);
                    zobrist[color, kind, index] = value;
#if DEBUGZOBRIST
                    Console.Write($"{value,20:X}\t");
#endif
                }
#if DEBUGZOBRIST
                Console.WriteLine("\n");
#endif
            }
#if DEBUGZOBRIST
            Console.WriteLine("\n");
#endif
        }
        return zobrist;
    }
}

public class BitBoard
{
    private PieceColor player;
    private BigInteger[,] pieces;

    // 计算中间存储数据(基本局面改动时更新)
    private PieceColor bottomColor;
    private BigInteger[] colorPieces;
    private BigInteger allPieces;
    private BigInteger rotatePieces;

    public BitBoard()
    {
        player = PieceColor.Red;
        pieces = new BigInteger[(int)BitNum.COLORNUM, (int)BitNum.KINDNUM];

        bottomColor = PieceColor.Red;
        colorPieces = new BigInteger[(int)BitNum.COLORNUM];
        allPieces = 0;
        rotatePieces = 0;
    }

    private ulong GetZobristFromBoard()
    {
        ulong zobrist = player == PieceColor.Black ? BitConstants.ZobristBlack : 0;
        for (int color = 0; color < (int)BitNum.COLORNUM; ++color)
        {
            for (int kind = 0; kind < (int)BitNum.KINDNUM; ++kind)
            {
                BigInteger piece = pieces[color, kind];
                while (!piece.IsZero)
                {
                    int index = BitConstants.TrailingZeroCount(piece);
                    zobrist ^= BitConstants.Zobrist[color, kind, index];

                    piece ^= BitConstants.Mask[index];
                }
            }
        }

        return zobrist;
    }

    public BigInteger GetMove(PieceColor color, PieceKind kind, int fromIndex)
    {
        BigInteger bitMove;
        switch (kind)
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
                bitMove = BitConstants.PawnMove[fromIndex, bottomColor == color ? 1 : 0];
                break;
            default: // PieceKind.Rook PieceKind.Cannon
                bitMove = BitConstants.GetRookCannonMove(kind == PieceKind.Cannon, fromIndex, allPieces, rotatePieces);
                break;
        }

        // 去掉同色棋子
        return bitMove ^ (bitMove & colorPieces[(int)color]);
    }

    public PieceKind DoMove(PieceColor color, PieceKind kind, int fromIndex, int toIndex, bool isBack, PieceKind eatKind)
    {
        PieceKind thisEatKind = PieceKind.NoKind;
        int toColor = color == PieceColor.Red ? 1 : 0;
        BigInteger fromBoard = BitConstants.Mask[fromIndex],
                 toBoard = BitConstants.Mask[toIndex],
                 moveBoard = fromBoard | toBoard;

        // 清除原位置，置位新位置
        pieces[(int)color, (int)kind] ^= moveBoard;
        colorPieces[(int)color] ^= moveBoard;

        if (isBack)
        {
            if (eatKind != PieceKind.NoKind)
            {
                allPieces ^= fromBoard;
                rotatePieces ^= BitConstants.RotateMask[fromIndex];
            }
        }
        else
        {
            if ((colorPieces[toColor] & toBoard).IsZero)
            {
                allPieces ^= moveBoard;
                rotatePieces ^= BitConstants.RotateMask[fromIndex] | BitConstants.RotateMask[toIndex];
            }
            else// 新位置有对方棋子时
            {
                allPieces ^= fromBoard;
                rotatePieces ^= BitConstants.RotateMask[fromIndex];
                for (PieceKind toKind = PieceKind.Pawn; toKind > PieceKind.NoKind; ++toKind)
                {
                    if (!(pieces[toColor, (int)toKind] & toBoard).IsZero)
                    {
                        thisEatKind = toKind;
                        pieces[toColor, (int)toKind] ^= toBoard;
                        colorPieces[toColor] ^= toBoard;
                        break;
                    }
                }
            }
        }

        return thisEatKind;
    }

}

