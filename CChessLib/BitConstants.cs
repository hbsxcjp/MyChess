using System.Text;
using System.Numerics;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;

public class MoveEffect : IComparable
{
    public int score;
    public int frequency;

    public MoveEffect(int sco, int fre) { score = sco; frequency = fre; }

    private int CompareTo(MoveEffect otherEffect) => -score.CompareTo(otherEffect.score);
    public int CompareTo(object? obj) => obj == null ? -1 : this.CompareTo((MoveEffect)obj);
}

public static class BitConstants
{
    // 每次生成相同的随机数序列（但是，如果.Net版本不同，序列可能不同）
    private const int seedKey = 100;
    private const int seedLock = 200;
    public static readonly long[][][] ZobristKey = CreateZobrist(seedKey);
    public static readonly long[][][] ZobristLock = CreateZobrist(seedLock);

    // 走棋方设置(利用空闲键值)
    public static readonly long[] ColorZobristKey = ZobristKey[0][0][0..2];
    public static readonly long[] ColorZobristLock = ZobristLock[0][0][0..2];
    public static readonly long[] CollideZobristKey = ZobristKey[1][0][0..3];

    public static readonly BigInteger[] Mask = Coord.Coords.Select(
        coord => (BigInteger)1 << coord.Index).ToArray();
    public static readonly BigInteger[] RotateMask = Coord.Coords.Select(
        coord => (BigInteger)1 << coord.Col * Coord.RowCount + coord.Row).ToArray();

    // 根据所处的位置选取可放置的位置[isBottom:0-1]
    public static readonly BigInteger[] KingPut = CreateKingPut();
    public static readonly BigInteger[] AdvisorPut = CreateAdvisorPut();
    public static readonly BigInteger[] BishopPut = CreateBishopPut();
    public static readonly BigInteger KnightRookCannonPut = ((BigInteger)(0X3FFFFFFUL) << 64) | 0XFFFFFFFFFFFFFFFFUL;
    public static readonly BigInteger[] PawnPut = CreatePawnPut();

    // 帅仕根据所处的位置选取可移动位棋盘[index:0-89]
    public static readonly BigInteger[] KingMove = CreateKingMove();
    public static readonly BigInteger[] AdvisorMove = CreateAdvisorMove();

    // 马相根据憋马腿或田心组成的四个位置状态选取可移动位棋盘[state:0-0XF][index:0-89]
    private static readonly List<BigInteger[]> BishopMove = CreateBishopMove();
    private static readonly List<BigInteger[]> KnightMove = CreateKnightMove();

    // 车炮根据每行和每列的位置状态选取可移动位棋盘[state:0-0x1F,0X3F][index:0-89]
    private static readonly List<BigInteger[]> RookRowMove = CreateRookCannonMove(PieceKind.Rook, false);
    private static readonly List<BigInteger[]> RookColMove = CreateRookCannonMove(PieceKind.Rook, true);
    private static readonly List<BigInteger[]> CannonRowMove = CreateRookCannonMove(PieceKind.Cannon, false);
    private static readonly List<BigInteger[]> CannonColMove = CreateRookCannonMove(PieceKind.Cannon, true);

    // 兵根据本方处于上或下的二个位置状态选取可移动位棋盘[isBottom:0-1][index:0-89]
    public static readonly List<BigInteger[]> PawnMove = CreatePawnMove();

    // 求最低位非零位的序号，调用前判断参数非零
    private static int TrailingZeroCount(BigInteger bigInt)
    {
        int count = 0;
        ulong value = (ulong)(bigInt & 0XFFFFFFFFFFFFFFFFUL);// 00-63 位
        // int count = 89;
        // ulong value = (ulong)(bigInt >> 64);// 64-89 位
        if (value == 0)
        {
            value = (ulong)(bigInt >> 64); // 64-89 位
            count = 64;
            // value = (ulong)(bigInt & 0XFFFFFFFFFFFFFFFFUL);// 00-63 位
            // count = 63;
        }

        return count + BitOperations.TrailingZeroCount(value);
        // return count - BitOperations.LeadingZeroCount(value);
    }

    public static List<int> GetNonZeroIndexs(BigInteger bigInt)
    {
        List<int> indexs = new();
        while (!bigInt.IsZero)
        {
            int index = TrailingZeroCount(bigInt);
            indexs.Add(index);

            bigInt ^= BitConstants.Mask[index];
        }

        return indexs;
    }

    public static BigInteger MergeBitInt(IEnumerable<BigInteger> bigIntegers)
        => bigIntegers.Aggregate((BigInteger)0, (result, next) => result | next);

    private static long[][][] CreateZobrist(int seed)
    {
        long[][][] zobrist = new long[Piece.ColorCount][][];
        Random random = new Random(seed);
        for (int color = 0; color < Piece.ColorCount; ++color)
        {
            long[][] colorZobrist = new long[Piece.KindCount][];
            for (int kind = 0; kind < Piece.KindCount; ++kind)
            {
                long[] kindZobrist = new long[Coord.Count];
                foreach (int index in Coord.Indexs)
                {
                    kindZobrist[index] = random.NextInt64(long.MinValue, long.MaxValue);
                }
                colorZobrist[kind] = kindZobrist;
            }
            zobrist[color] = colorZobrist;
        }

        return zobrist;
    }

    private static BigInteger[] CreateKingPut()
    {
        BigInteger[] kingPut = { 0, 0 };
        foreach (int index in Coord.Indexs)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            if ((row < 3 || row > 6) && (col > 2 && col < 6))
                kingPut[(index >= Coord.Indexs.Count / 2) ? 1 : 0] |= Mask[index];
        }

        return kingPut;
    }

    private static BigInteger[] CreateAdvisorPut()
    {
        BigInteger[] advisorPut = { 0, 0 };
        foreach (int index in Coord.Indexs)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            if (((row == 0 || row == 2 || row == 7 || row == 9) && (col == 3 || col == 5))
                || ((row == 1 || row == 8) && col == 4))
                advisorPut[(index >= Coord.Indexs.Count / 2) ? 1 : 0] |= Mask[index];
        }

        return advisorPut;
    }

    private static BigInteger[] CreateBishopPut()
    {
        BigInteger[] bishopPut = { 0, 0 };
        foreach (int index in Coord.Indexs)
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;
            if (((row == 0 || row == 4 || row == 5 || row == 9) && (col == 2 || col == 6))
                || ((row == 2 || row == 7) && (col == 0 || col == 4 || col == 8)))
                bishopPut[(index >= Coord.Indexs.Count / 2) ? 1 : 0] |= Mask[index];
        }

        return bishopPut;
    }

    private static BigInteger[] CreatePawnPut()
    {
        BigInteger[] pawnPut = { 0, 0 };
        for (int isBottom = 0; isBottom < 2; ++isBottom)
        {
            foreach (int index in Coord.Indexs)
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;
                if (isBottom == 1 ? (row < 5 || ((row == 5 || row == 6) && (col == 0 || col == 2 || col == 4 || col == 6 || col == 8)))
                            : (row > 4 || ((row == 3 || row == 4) && (col == 0 || col == 2 || col == 4 || col == 6 || col == 8))))
                    pawnPut[isBottom] |= Mask[index];
            }
        }

        return pawnPut;
    }

    private static BigInteger[] CreateKingMove()
    {
        BigInteger[] kingMove = new BigInteger[Coord.Count];
        foreach (int index in GetNonZeroIndexs(KingPut[0] | KingPut[1]))
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;

            BigInteger match = 0;
            if (col > 3)
                match |= Mask[index - 1];
            if (col < 5)
                match |= Mask[index + 1];

            if (row == 1 || row == 2 || row == 8 || row == 9)
                match |= Mask[index - Coord.ColCount];
            if (row == 0 || row == 1 || row == 7 || row == 8)
                match |= Mask[index + Coord.ColCount];

            kingMove[index] = match;
        }

        return kingMove;
    }

    private static BigInteger[] CreateAdvisorMove()
    {
        BigInteger[] advisorMove = new BigInteger[Coord.Count];
        foreach (int index in GetNonZeroIndexs(AdvisorPut[0] | AdvisorPut[1]))
        {
            Coord fromCoord = Coord.Coords[index];
            int row = fromCoord.Row, col = fromCoord.Col;

            BigInteger match;
            if (col == 4)
                match = (Mask[index - Coord.ColCount - 1]
                    | Mask[index - Coord.ColCount + 1]
                    | Mask[index + Coord.ColCount - 1]
                    | Mask[index + Coord.ColCount + 1]);
            else
                match = Mask[row < 3 ? 13 : 76];

            advisorMove[index] = match;
        }

        return advisorMove;
    }

    private static List<BigInteger[]> CreateBishopMove()
    {
        List<BigInteger[]> bishopMove = new();
        for (int state = 0; state < 1 << (Piece.LegCount); ++state)
        {
            BigInteger[] allIndexMove = new BigInteger[Coord.Count];
            foreach (int index in GetNonZeroIndexs(BishopPut[0] | BishopPut[1]))
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;

                int realState = state;
                if (row == 0 || row == 5)
                    realState |= (1 << (Piece.LegCount - 1) | 1 << (Piece.LegCount - 2));
                else if (row == 4 || row == Coord.RowCount - 1)
                    realState |= (1 << (Piece.LegCount - 3) | 1 << (Piece.LegCount - 4));
                if (col == 0)
                    realState |= (1 << (Piece.LegCount - 1) | 1 << (Piece.LegCount - 3));
                else if (col == Coord.ColCount - 1)
                    realState |= (1 << (Piece.LegCount - 2) | 1 << (Piece.LegCount - 4));

                BigInteger match = 0;
                if (0 == (realState & (1 << (Piece.LegCount - 1))))
                    match |= Mask[index - 2 * Coord.ColCount - 2];

                if (0 == (realState & (1 << (Piece.LegCount - 2))))
                    match |= Mask[index - 2 * Coord.ColCount + 2];

                if (0 == (realState & (1 << (Piece.LegCount - 3))))
                    match |= Mask[index + 2 * Coord.ColCount - 2];

                if (0 == (realState & (1 << (Piece.LegCount - 4))))
                    match |= Mask[index + 2 * Coord.ColCount + 2];

                allIndexMove[index] = match;
            }

            bishopMove.Add(allIndexMove);
        }

        return bishopMove;
    }

    private static List<BigInteger[]> CreateKnightMove()
    {
        List<BigInteger[]> knightMove = new();
        for (int state = 0; state < 1 << (Piece.LegCount); ++state)
        {
            BigInteger[] allIndexMove = new BigInteger[Coord.Count];
            foreach (int index in Coord.Indexs)
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;
                int realState = state;
                if (row == 0)
                    realState |= 1 << (Piece.LegCount - 1);
                else if (row == Coord.RowCount - 1)
                    realState |= 1 << (Piece.LegCount - 4);
                if (col == 0)
                    realState |= 1 << (Piece.LegCount - 2);
                else if (col == Coord.ColCount - 1)
                    realState |= 1 << (Piece.LegCount - 3);

                BigInteger match = 0;
                if (0 == (realState & (1 << (Piece.LegCount - 1))) && row > 1)
                {
                    if (col > 0)
                        match |= Mask[index - 2 * Coord.ColCount - 1];
                    if (col < Coord.ColCount - 1)
                        match |= Mask[index - 2 * Coord.ColCount + 1];
                }
                if (0 == (realState & (1 << (Piece.LegCount - 2))) && col > 1)
                {
                    if (row > 0)
                        match |= Mask[index - Coord.ColCount - 2];
                    if (row < Coord.RowCount - 1)
                        match |= Mask[index + Coord.ColCount - 2];
                }
                if (0 == (realState & (1 << (Piece.LegCount - 3))) && col < Coord.ColCount - 2)
                {
                    if (row > 0)
                        match |= Mask[index - Coord.ColCount + 2];
                    if (row < Coord.RowCount - 1)
                        match |= Mask[index + Coord.ColCount + 2];
                }
                if (0 == (realState & (1 << (Piece.LegCount - 4))) && row < Coord.RowCount - 2)
                {
                    if (col > 0)
                        match |= Mask[index + 2 * Coord.ColCount - 1];
                    if (col < Coord.ColCount - 1)
                        match |= Mask[index + 2 * Coord.ColCount + 1];
                }

                allIndexMove[index] = match;
            }

            knightMove.Add(allIndexMove);
        }

        return knightMove;
    }

    private static List<BigInteger[]> CreateRookCannonMove(PieceKind kind, bool isCol)
    {
        static int getMatch(int state, int rowColIndex, bool isCannon, bool isRotate)
        {
            int match = 0;
            for (int isHigh = 0; isHigh < 2; ++isHigh)
            {
                int direction = isHigh == 1 ? 1 : -1,
                    endIndex = isHigh == 1 ? (isRotate ? Coord.RowCount : Coord.ColCount) - 1 : 0; // 每行列数或每列行数
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

        List<BigInteger[]> rookCanonRowColMove = new();
        bool isCannon = kind == PieceKind.Cannon;
        int bitLength = isCol ? Coord.RowCount : Coord.ColCount,
            stateTotal = 1 << bitLength;
        for (int rowColIndex = 0; rowColIndex < bitLength; ++rowColIndex)
        {
            BigInteger[] allStateMove = new BigInteger[stateTotal];
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
                    for (int row = 0; row < Coord.RowCount; ++row)
                    {
                        if (0 != (match & 1 << row))
                            colMatch |= Mask[row * Coord.ColCount]; // 每行的首列置位
                    }

                    allStateMove[state] = colMatch;
                }
                else
                    allStateMove[state] = match;
            }

            rookCanonRowColMove.Add(allStateMove);
        }

        return rookCanonRowColMove;
    }

    private static List<BigInteger[]> CreatePawnMove()
    {
        List<BigInteger[]> pawnMove = new();
        for (int isBottom = 0; isBottom < 2; ++isBottom)
        {
            BigInteger[] sidePawnMove = new BigInteger[Coord.Count];
            foreach (int index in GetNonZeroIndexs(PawnPut[isBottom]))
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;

                BigInteger match = 0;
                if ((isBottom == 0 && row > 4) || (isBottom == 1 && row < 5))
                {
                    if (col != 0)
                        match |= Mask[index - 1];
                    if (col != Coord.ColCount - 1)
                        match |= Mask[index + 1];
                }

                if (isBottom == 0 && row != Coord.RowCount - 1)
                    match |= Mask[index + Coord.ColCount];

                if (isBottom == 1 && row != 0)
                    match |= Mask[index - Coord.ColCount];

                sidePawnMove[index] = match;
            }
            pawnMove.Add(sidePawnMove);
        }

        return pawnMove;
    }

    public static BigInteger GetBishopMove(int fromIndex, BigInteger allPieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col;
        bool isTop = row == 0 || row == 5,
             isBottom = row == 4 || row == Coord.RowCount - 1,
             isLeft = col == 0,
             isRight = col == Coord.ColCount - 1;
        int state = ((isTop || isLeft || !(allPieces & Mask[fromIndex - Coord.ColCount - 1]).IsZero ? 1 << (Piece.LegCount - 1) : 0)
            | (isTop || isRight || !(allPieces & Mask[fromIndex - Coord.ColCount + 1]).IsZero ? 1 << (Piece.LegCount - 2) : 0)
            | (isBottom || isLeft || !(allPieces & Mask[fromIndex + Coord.ColCount - 1]).IsZero ? 1 << (Piece.LegCount - 3) : 0)
            | (isBottom || isRight || !(allPieces & Mask[fromIndex + Coord.ColCount + 1]).IsZero ? 1 << (Piece.LegCount - 4) : 0));

        return BishopMove[state][fromIndex];
    }

    public static BigInteger GetKnightMove(int fromIndex, BigInteger allPieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col;
        int state = ((row == 0 || !(allPieces & Mask[fromIndex - Coord.ColCount]).IsZero ? 1 << (Piece.LegCount - 1) : 0)
            | (col == 0 || !(allPieces & Mask[fromIndex - 1]).IsZero ? 1 << (Piece.LegCount - 2) : 0)
            | (col == Coord.ColCount - 1 || !(allPieces & Mask[fromIndex + 1]).IsZero ? 1 << (Piece.LegCount - 3) : 0)
            | (row == Coord.RowCount - 1 || !(allPieces & Mask[fromIndex + Coord.ColCount]).IsZero ? 1 << (Piece.LegCount - 4) : 0));

        return KnightMove[state][fromIndex];
    }

    public static BigInteger GetRookCannonMove(bool isCannon, int fromIndex, BigInteger allPieces, BigInteger rotatePieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col,
            rowOffset = row * Coord.ColCount;
        List<BigInteger[]> rowMove = isCannon ? CannonRowMove : RookRowMove,
            colMove = isCannon ? CannonColMove : RookColMove;

        return ((rowMove[col][(int)((allPieces >> rowOffset) & 0x1FF)] << rowOffset)
            | (colMove[row][(int)((rotatePieces >> col * Coord.RowCount) & 0x3FF)] << col)); // 每行首列置位全体移动数列
    }

    public static List<string> GetBigIntString(BigInteger bigInt, bool isRotate)
    {
        static string GetRowColString(int rowInt, int colNum)
        {
            StringBuilder result = new();
            for (int bitCol = 0; bitCol < colNum; ++bitCol)
            {
                result.Append((rowInt & (1 << bitCol)) == 0 ? '-' : '1');
            }
            result.Append(' ');

            return result.ToString();
        }

        int rowNum = isRotate ? Coord.ColCount : Coord.RowCount;
        int colNum = isRotate ? Coord.RowCount : Coord.ColCount;
        int mode = isRotate ? 0x3FF : 0x1FF;
        List<string> result = new();
        for (int row = 0; row < rowNum; ++row)
        {
            result.Add(GetRowColString((int)(bigInt & mode), colNum));
            bigInt = bigInt >> colNum;
        }

        return result;
    }

    public static string GetBigIntArrayString(BigInteger[] bigInts, int colNumPerRow, bool showZero, bool isRotate)
    {
        int rowNum = isRotate ? Coord.ColCount : Coord.RowCount;
        int length = bigInts.Length;
        colNumPerRow = Math.Min(length, colNumPerRow);
        string nullStr = "   ";
        for (int col = 0; col < colNumPerRow; ++col)
        {
            nullStr += isRotate ? "ABCDEFGHIJ " : "ABCDEFGHI ";
        }
        nullStr += "\n";

        if (!showZero)
        {
            int count = 0;
            BigInteger[] nonZeroBigInts = new BigInteger[length];
            for (int index = 0; index < length; ++index)
            {
                if (!bigInts[index].IsZero)
                    nonZeroBigInts[count++] = bigInts[index];
            }
            bigInts = nonZeroBigInts;
            length = count;
        }

        StringBuilder result = new();
        for (int index = 0; index < length; index += colNumPerRow)
        {
            List<List<string>> resultPerRow = new();
            for (int col = 0; col < colNumPerRow && index + col < length; ++col)
            {
                resultPerRow.Add(GetBigIntString(bigInts[index + col], isRotate));
            }

            StringBuilder rowResult = new();
            rowResult.Append(nullStr);
            for (int row = 0; row < rowNum; ++row)
            {
                rowResult.Append($"{row}: ");
                for (int col = 0; col < colNumPerRow && index + col < length; ++col)
                    rowResult.Append(resultPerRow[col][row]);

                rowResult.Append("\n");
            }

            result.Append(rowResult);
        }
        result.Append($"length: {length}\n");

        return result.ToString();
    }

    public static new string ToString()
    {
        StringBuilder result = new();
        result.Append($"Zobrist:\n");
        for (int color = 0; color < Piece.ColorCount; ++color)
        {
            result.Append($"Color: {color}\n");
            for (int kind = 0; kind < Piece.KindCount; ++kind)
            {
                result.Append($"Kind: {kind}\n");
                foreach (int index in Coord.Indexs)
                {
                    result.Append($"{ZobristKey[color][kind][index],18:X16}");
                    result.Append($"{ZobristLock[color][kind][index],18:X16}");
                }
                result.Append("\n");
            }
            result.Append("\n");
        }

        string bigIntsString = GetBigIntArrayString(Mask, Coord.ColCount, true, false);
        result.Append($"Mask: {Mask.Length}\n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(RotateMask, Coord.RowCount, true, true);
        result.Append($"RotateMask: {RotateMask.Length}\n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(KingPut, Coord.ColCount, false, false);
        result.Append($"KingPut: \n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(AdvisorPut, Coord.ColCount, false, false);
        result.Append($"AdvisorPut: \n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(BishopPut, Coord.ColCount, false, false);
        result.Append($"BishopPut: \n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(new BigInteger[] { KnightRookCannonPut }, Coord.ColCount, false, false);
        result.Append($"KnightRookCannonPut: \n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(PawnPut, Coord.ColCount, false, false);
        result.Append($"PawnPut: \n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(KingMove, Coord.ColCount, false, false);
        result.Append($"KingMove: \n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(AdvisorMove, 5, false, false);
        result.Append($"AdvisorMove: \n{bigIntsString}\n");

        for (int state = 0; state < BishopMove.Count; ++state)
        {
            bigIntsString = GetBigIntArrayString(BishopMove[state], 7, false, false);
            result.Append($"BishopMove State: {state:X}\n{bigIntsString}\n");
        }

        for (int state = 0; state < KnightMove.Count; ++state)
        {
            bigIntsString = GetBigIntArrayString(KnightMove[state], 8, false, false);
            result.Append($"KnightMove State: {state:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < RookRowMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(RookRowMove[index], Coord.ColCount, false, false);
            result.Append($"RookRowMove Col: {index:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < RookColMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(RookColMove[index], Coord.ColCount, false, false);
            result.Append($"RookColMove Row: {index:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < CannonRowMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(CannonRowMove[index], Coord.ColCount, false, false);
            result.Append($"CannonRowMove Col: {index:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < CannonColMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(CannonColMove[index], Coord.ColCount, false, false);
            result.Append($"CannonColMove Row: {index:X}\n{bigIntsString}\n");
        }

        for (int isBottom = 0; isBottom < PawnMove.Count; ++isBottom)
        {
            bigIntsString = GetBigIntArrayString(PawnMove[isBottom], Coord.RowCount, false, false);
            result.Append($"PawnMove IsBottom: {isBottom}\n{bigIntsString}\n");
        }

        return result.ToString();
    }
}
