using System.Text;
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

public struct MoveEffect
{
    int score;

    PieceColor toColor;
    PieceKind toKind;
}

public static class BitConstants
{

    public static readonly ulong ZobristBlack = (ulong)(new Random(7)).NextInt64(long.MinValue, long.MaxValue);
    public static readonly ulong[,,] Zobrist = CreateZobrist();

    public static readonly BigInteger[] Mask = Coord.Coords.Select(
        coord => (BigInteger)1 << coord.Index).ToArray();
    public static readonly BigInteger[] RotateMask = Coord.Coords.Select(
        coord => (BigInteger)1 << coord.Col * (int)BitNum.BOARDROWNUM + coord.Row).ToArray();

    // 帅仕根据所处的位置选取可移动位棋盘
    public static readonly BigInteger[] KingMove = CreateKingMove();
    public static readonly BigInteger[] AdvisorMove = CreateAdvisorMove();

    // 马相根据憋马腿或田心组成的四个位置状态选取可移动位棋盘
    private static readonly List<BigInteger[]> BishopMove = CreateBishopMove();
    private static readonly List<BigInteger[]> KnightMove = CreateKnightMove();

    // 车炮根据每行和每列的位置状态选取可移动位棋盘
    private static readonly List<BigInteger[]> RookRowMove = CreateRookRowMove();
    private static readonly List<BigInteger[]> RookColMove = CreateRookColMove();
    private static readonly List<BigInteger[]> CannonRowMove = CreateCannonRowMove();
    private static readonly List<BigInteger[]> CannonColMove = CreateCannonColMove();

    // 兵根据本方处于上或下的二个位置状态选取可移动位棋盘
    public static readonly List<BigInteger[]> PawnMove = CreatePawnMove();

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

    private static ulong[,,] CreateZobrist()
    {
        ulong[,,] zobrist = new ulong[(int)BitNum.COLORNUM, (int)BitNum.KINDNUM, (int)BitNum.BOARDLENGTH];
        Random random = new Random();
        for (int color = 0; color < (int)BitNum.COLORNUM; ++color)
        {
            for (int kind = 0; kind < (int)BitNum.KINDNUM; ++kind)
            {
                for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
                {
                    zobrist[color, kind, index] = (ulong)random.NextInt64(long.MinValue, long.MaxValue);
                }
            }
        }

        return zobrist;
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

    private static List<BigInteger[]> CreateBishopMove()
    {
        static bool isValidBishop(int row, int col)
        {
            return (((row == 0 || row == 4 || row == 5 || row == 9) && (col == 2 || col == 6))
                || ((row == 2 || row == 7) && (col == 0 || col == 4 || col == 8)));
        }

        List<BigInteger[]> bishopMove = new();
        for (int state = 0; state < 1 << ((int)BitNum.LEGCOUNT); ++state)
        {
            BigInteger[] allIndexMove = new BigInteger[(int)BitNum.BOARDLENGTH];
            for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;
                if (!isValidBishop(row, col))
                    continue;

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

                allIndexMove[index] = match;
            }

            bishopMove.Add(allIndexMove);
        }

        return bishopMove;
    }

    private static List<BigInteger[]> CreateKnightMove()
    {
        List<BigInteger[]> knightMove = new();
        for (int state = 0; state < 1 << ((int)BitNum.LEGCOUNT); ++state)
        {
            BigInteger[] allIndexMove = new BigInteger[(int)BitNum.BOARDLENGTH];
            for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;
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

                allIndexMove[index] = match;
            }

            knightMove.Add(allIndexMove);
        }

        return knightMove;
    }

    private static void InitRookCannonMove(List<BigInteger[]> rookCanonRowColMove, PieceKind kind, bool isCol)
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
                    for (int row = 0; row < (int)BitNum.BOARDROWNUM; ++row)
                    {
                        if (0 != (match & 1 << row))
                            colMatch |= Mask[row * (int)BitNum.BOARDCOLNUM]; // 每行的首列置位
                    }

                    allStateMove[state] = colMatch;
                }
                else
                    allStateMove[state] = match;
            }

            rookCanonRowColMove.Add(allStateMove);
        }
    }

    private static List<BigInteger[]> CreateRookRowMove()
    {
        List<BigInteger[]> rookRowMove = new();
        InitRookCannonMove(rookRowMove, PieceKind.Rook, false);

        return rookRowMove;
    }

    private static List<BigInteger[]> CreateRookColMove()
    {
        List<BigInteger[]> rookColMove = new();
        InitRookCannonMove(rookColMove, PieceKind.Rook, true);

        return rookColMove;
    }

    private static List<BigInteger[]> CreateCannonRowMove()
    {
        List<BigInteger[]> cannonRowMove = new();
        InitRookCannonMove(cannonRowMove, PieceKind.Cannon, false);

        return cannonRowMove;
    }

    private static List<BigInteger[]> CreateCannonColMove()
    {
        List<BigInteger[]> cannonColMove = new();
        InitRookCannonMove(cannonColMove, PieceKind.Cannon, true);

        return cannonColMove;
    }

    private static List<BigInteger[]> CreatePawnMove()
    {
        static bool isValidPawn(int row, int col, bool isBottom)
        {
            return (isBottom ? (row < 5 || ((row == 5 || row == 6) && (col == 0 || col == 2 || col == 4 || col == 6 || col == 8)))
                             : (row > 4 || ((row == 3 || row == 4) && (col == 0 || col == 2 || col == 4 || col == 6 || col == 8))));
        }

        List<BigInteger[]> pawnMove = new();
        for (int isBottom = 0; isBottom < 2; ++isBottom)
        {
            BigInteger[] sidePawnMove = new BigInteger[(int)BitNum.BOARDLENGTH];
            for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
            {
                Coord fromCoord = Coord.Coords[index];
                int row = fromCoord.Row, col = fromCoord.Col;

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
             isBottom = row == 4 || row == (int)BitNum.BOARDROWNUM - 1,
             isLeft = col == 0,
             isRight = col == (int)BitNum.BOARDCOLNUM - 1;
        int state = ((isTop || isLeft || !(allPieces & Mask[fromIndex - (int)BitNum.BOARDCOLNUM - 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 1) : 0)
            | (isTop || isRight || !(allPieces & Mask[fromIndex - (int)BitNum.BOARDCOLNUM + 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 2) : 0)
            | (isBottom || isLeft || !(allPieces & Mask[fromIndex + (int)BitNum.BOARDCOLNUM - 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 3) : 0)
            | (isBottom || isRight || !(allPieces & Mask[fromIndex + (int)BitNum.BOARDCOLNUM + 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 4) : 0));

        return BishopMove[state][fromIndex];
    }

    public static BigInteger GetKnightMove(int fromIndex, BigInteger allPieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col;
        int state = ((row == 0 || !(allPieces & Mask[fromIndex - (int)BitNum.BOARDCOLNUM]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 1) : 0)
            | (col == 0 || !(allPieces & Mask[fromIndex - 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 2) : 0)
            | (col == (int)BitNum.BOARDCOLNUM - 1 || !(allPieces & Mask[fromIndex + 1]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 3) : 0)
            | (row == (int)BitNum.BOARDROWNUM - 1 || !(allPieces & Mask[fromIndex + (int)BitNum.BOARDCOLNUM]).IsZero ? 1 << ((int)BitNum.LEGCOUNT - 4) : 0));

        return KnightMove[state][fromIndex];
    }

    public static BigInteger GetRookCannonMove(bool isCannon, int fromIndex, BigInteger allPieces, BigInteger rotatePieces)
    {
        Coord fromCoord = Coord.Coords[fromIndex];
        int row = fromCoord.Row, col = fromCoord.Col,
            rowOffset = row * (int)BitNum.BOARDCOLNUM;
        List<BigInteger[]> rowMove = isCannon ? CannonRowMove : RookRowMove,
            colMove = isCannon ? CannonColMove : RookColMove;

        return ((rowMove[col][(int)((allPieces >> rowOffset) & 0x1FF)] << rowOffset)
            | (colMove[row][(int)((rotatePieces >> col * (int)BitNum.BOARDROWNUM) & 0x3FF)] << col)); // 每行首列置位全体移动数列
    }

    public static List<string> GetBigIntString(BigInteger bigInt, bool isRotate)
    {
        static string GetRowColString(int rowInt, int colNum)
        {
            string result = string.Empty;
            for (int bitCol = 0; bitCol < colNum; ++bitCol)
            {
                result += (rowInt & (1 << bitCol)) == 0 ? '-' : '1';
            }
            result += " ";

            return result;
        }

        int rowNum = isRotate ? (int)BitNum.BOARDCOLNUM : (int)BitNum.BOARDROWNUM;
        int colNum = isRotate ? (int)BitNum.BOARDROWNUM : (int)BitNum.BOARDCOLNUM;
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
        int rowNum = isRotate ? (int)BitNum.BOARDCOLNUM : (int)BitNum.BOARDROWNUM;
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
        result.Append($"ZobristBlack: {ZobristBlack,18:X16}\n");
        for (int color = 0; color < (int)BitNum.COLORNUM; ++color)
        {
            result.Append($"Color: {color}\n");
            for (int kind = 0; kind < (int)BitNum.KINDNUM; ++kind)
            {
                result.Append($"Kind: {kind}\n");
                for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
                {
                    result.Append($"{Zobrist[color, kind, index],18:X16}");
                }
                result.Append("\n");
            }
            result.Append("\n");
        }

        string bigIntsString = GetBigIntArrayString(Mask, (int)BitNum.BOARDCOLNUM, true, false);
        result.Append($"Mask: {Mask.Length}\n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(RotateMask, (int)BitNum.BOARDROWNUM, true, true);
        result.Append($"RotateMask: {RotateMask.Length}\n{bigIntsString}\n");

        bigIntsString = GetBigIntArrayString(KingMove, (int)BitNum.BOARDCOLNUM, false, false);
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
            bigIntsString = GetBigIntArrayString(RookRowMove[index], (int)BitNum.BOARDCOLNUM, false, false);
            result.Append($"RookRowMove Col: {index:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < RookColMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(RookColMove[index], (int)BitNum.BOARDCOLNUM, false, false);
            result.Append($"RookColMove Row: {index:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < CannonRowMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(CannonRowMove[index], (int)BitNum.BOARDCOLNUM, false, false);
            result.Append($"CannonRowMove Col: {index:X}\n{bigIntsString}\n");
        }

        for (int index = 0; index < CannonColMove.Count; ++index)
        {
            bigIntsString = GetBigIntArrayString(CannonColMove[index], (int)BitNum.BOARDCOLNUM, false, false);
            result.Append($"CannonColMove Row: {index:X}\n{bigIntsString}\n");
        }

        for (int isBottom = 0; isBottom < PawnMove.Count; ++isBottom)
        {
            bigIntsString = GetBigIntArrayString(PawnMove[isBottom], (int)BitNum.BOARDROWNUM, false, false);
            result.Append($"PawnMove IsBottom: {isBottom}\n{bigIntsString}\n");
        }

        return result.ToString();
    }
}
