using System.Text;
using System.Numerics;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;


public class BitBoard
{
    // 基本数据
    private PieceColor player;
    private BigInteger[][] pieces;

    // 计算中间存储数据(基本局面改动时更新)
    private PieceColor bottomColor;
    private BigInteger[] colorPieces;
    private BigInteger allPieces;
    private BigInteger rotatePieces;

    ulong zobrist;

    public BitBoard(Board board)
    {
        player = PieceColor.Red;
        pieces = new BigInteger[(int)BitNum.COLORNUM][];
        for (int color = 0; color < (int)BitNum.COLORNUM; ++color)
            pieces[color] = new BigInteger[(int)BitNum.KINDNUM];

        bottomColor = board.BottomColor;
        colorPieces = new BigInteger[(int)BitNum.COLORNUM];
        allPieces = 0;
        rotatePieces = 0;

        zobrist = player == PieceColor.Black ? BitConstants.ZobristBlack : 0;
        for (int index = 0; index < (int)BitNum.BOARDLENGTH; ++index)
        {
            Piece piece = board[index];
            if (piece != Piece.Null)
            {
                int color = (int)piece.Color,
                    kind = (int)piece.Kind;
                BigInteger turnBoard = BitConstants.Mask[index];

                pieces[color][kind] ^= turnBoard;
                colorPieces[color] ^= turnBoard;
                allPieces ^= turnBoard;
                rotatePieces ^= BitConstants.RotateMask[index];

                zobrist ^= BitConstants.Zobrist[color][kind][index];
            }
        }
    }

    delegate bool IsState(PieceColor color, PieceKind kind, int fromIndex, int toIndex);
    // delegate MoveEffect GetMoveEffect(PieceColor color, PieceKind kind, int fromIndex, int toIndex);

    public PieceKind DoMove(PieceColor color, PieceKind kind, int fromIndex, int toIndex, bool isBack, PieceKind eatKind = PieceKind.NoKind)
    {
        int toColor = color == PieceColor.Red ? 1 : 0;
        BigInteger fromBoard = BitConstants.Mask[fromIndex],
                 toBoard = BitConstants.Mask[toIndex],
                 moveBoard = fromBoard | toBoard;

        // 清除原位置，置位新位置
        pieces[(int)color][(int)kind] ^= moveBoard;
        colorPieces[(int)color] ^= moveBoard;
        zobrist ^= (BitConstants.Zobrist[(int)color][(int)kind][fromIndex] |
                    BitConstants.Zobrist[(int)color][(int)kind][toIndex]);

        bool backToBoardHasPiece = isBack && eatKind != PieceKind.NoKind,
            goToBoardHasPiece = !isBack && !(colorPieces[toColor] & toBoard).IsZero;
        if (backToBoardHasPiece || goToBoardHasPiece)
        {
            if (goToBoardHasPiece)
            {
                for (PieceKind toKind = PieceKind.Pawn; toKind > PieceKind.NoKind; --toKind)
                {
                    if (!(pieces[toColor][(int)toKind] & toBoard).IsZero)
                    {
                        eatKind = toKind;
                        break;
                    }
                }
            }

            pieces[toColor][(int)eatKind] ^= toBoard;
            colorPieces[toColor] ^= toBoard;
            allPieces ^= fromBoard;
            rotatePieces ^= BitConstants.RotateMask[fromIndex];
            zobrist ^= BitConstants.Zobrist[toColor][(int)eatKind][toIndex];
        }
        else
        {
            allPieces ^= moveBoard;
            rotatePieces ^= BitConstants.RotateMask[fromIndex] | BitConstants.RotateMask[toIndex];
        }

        return eatKind;
    }

    private BigInteger GetMove(PieceColor fromColor, PieceKind fromKind, int fromIndex)
    {
        BigInteger bitMove;
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
        => BitConstants.MergeBitInt(
                BitConstants.GetNonZeroIndexs(pieces[(int)fromColor][(int)fromKind])
                    .Select(fromIndex => GetMove(fromColor, fromKind, fromIndex)));

    private BigInteger GetColorMove(PieceColor fromColor)
        => BitConstants.MergeBitInt(Piece.PieceKinds.Select(fromKind => GetKindMove(fromColor, (PieceKind)fromKind)));

    private bool IsNonKilled(PieceColor color, PieceKind kind, int fromIndex, int toIndex)
        => (GetColorMove(Piece.GetOtherColor(color)) & pieces[(int)color][(int)PieceKind.King]).IsZero;

    // 执行某一着后的效果(委托函数可叠加)
    private MoveEffect GetMoveEffect(PieceColor color, PieceKind kind, int fromIndex, int toIndex, IsState isState)
    {
        MoveEffect effect = new MoveEffect { fromIndex = fromIndex, toIndex = toIndex, score = 0 };
        PieceKind eatKind = DoMove(color, kind, fromIndex, toIndex, false);

        effect.score += isState(color, kind, fromIndex, toIndex) ? 1 : 0;

        DoMove(color, kind, fromIndex, toIndex, true, eatKind);
        return effect;
    }

    public override string ToString()
    {
        List<BigInteger> moves = new();
        StringBuilder boardStr = new();
        boardStr.Append("－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n－－－－－－－－－\n");
        for (int color = 0; color < (int)BitNum.COLORNUM; ++color)
        {
            for (int kind = 0; kind < (int)BitNum.KINDNUM; ++kind)
            {
                BigInteger piece = pieces[color][kind];
                BitConstants.GetNonZeroIndexs(piece).ForEach(fromIndex =>
                {
                    Coord coord = Coord.Coords[fromIndex];
                    boardStr[coord.Row * ((int)BitNum.BOARDCOLNUM + 1) + coord.Col] = Piece.GetPrintName((PieceColor)color, (PieceKind)kind);

                    // 计算可移动位置
                    moves.Add(BitConstants.MergeBitInt(
                        BitConstants.GetNonZeroIndexs(GetMove((PieceColor)color, (PieceKind)kind, fromIndex))
                        .Where(toIndex => GetMoveEffect((PieceColor)color, (PieceKind)kind, fromIndex, toIndex, IsNonKilled).score > 0)
                        .Select(toIndex => BitConstants.Mask[toIndex])));
                });
            }
        }
        boardStr.Append(BitConstants.GetBigIntArrayString(moves.ToArray(), 8, true, false)).Append("\n");

        return boardStr.ToString();
    }


}

