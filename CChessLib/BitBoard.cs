using System.Text;
using System.Numerics;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

namespace CChess;


public class BitBoard
{
    // 基本数据
    private PieceColor player;
    private BigInteger[,] pieces;

    // 计算中间存储数据(基本局面改动时更新)
    private PieceColor bottomColor;
    private BigInteger[] colorPieces;
    private BigInteger allPieces;
    private BigInteger rotatePieces;

    ulong zobrist;

    public BitBoard(Board board)
    {
        player = PieceColor.Red;
        pieces = new BigInteger[(int)BitNum.COLORNUM, (int)BitNum.KINDNUM];

        bottomColor = board.BottomColor;
        colorPieces = new BigInteger[(int)BitNum.COLORNUM];
        allPieces = 0;
        rotatePieces = 0;

        zobrist = player == PieceColor.Black ? BitConstants.ZobristBlack : 0;

        Enumerable.Range(0, (int)BitNum.BOARDLENGTH).Select(index => (index, board[index]))
                .Where(indexPiece => indexPiece.Item2 != Piece.Null).ToList()
                .ForEach(indexPiece =>
                        {
                            (int index, Piece piece) = indexPiece;
                            int color = (int)piece.Color,
                                kind = (int)piece.Kind;
                            BigInteger turnBoard = BitConstants.Mask[index];

                            pieces[color, kind] ^= turnBoard;
                            colorPieces[color] ^= turnBoard;
                            allPieces ^= turnBoard;
                            rotatePieces ^= BitConstants.RotateMask[index];

                            zobrist ^= BitConstants.Zobrist[color, kind, index];
                        });
    }

    delegate MoveEffect GetMoveEffect(PieceColor color, PieceKind kind, int fromIndex, int toIndex);

    public PieceKind DoMove(PieceColor color, PieceKind kind, int fromIndex, int toIndex, bool isBack, PieceKind eatKind = PieceKind.NoKind)
    {
        int toColor = color == PieceColor.Red ? 1 : 0;
        BigInteger fromBoard = BitConstants.Mask[fromIndex],
                 toBoard = BitConstants.Mask[toIndex],
                 moveBoard = fromBoard | toBoard;

        // 清除原位置，置位新位置
        pieces[(int)color, (int)kind] ^= moveBoard;
        colorPieces[(int)color] ^= moveBoard;
        zobrist ^= (BitConstants.Zobrist[(int)color, (int)kind, fromIndex] |
                    BitConstants.Zobrist[(int)color, (int)kind, toIndex]);

        bool backToBoardHasPiece = isBack && eatKind != PieceKind.NoKind,
            goToBoardHasPiece = !isBack && !(colorPieces[toColor] & toBoard).IsZero;
        if (backToBoardHasPiece || goToBoardHasPiece)
        {
            if (goToBoardHasPiece)
            {
                for (PieceKind toKind = PieceKind.Pawn; toKind > PieceKind.NoKind; --toKind)
                {
                    if (!(pieces[toColor, (int)toKind] & toBoard).IsZero)
                    {
                        eatKind = toKind;
                        break;
                    }
                }
            }

            pieces[toColor, (int)eatKind] ^= toBoard;
            colorPieces[toColor] ^= toBoard;
            allPieces ^= fromBoard;
            rotatePieces ^= BitConstants.RotateMask[fromIndex];
            zobrist ^= BitConstants.Zobrist[toColor, (int)eatKind, toIndex];
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

    private BigInteger GetMove(PieceColor fromColor, PieceKind fromKind)
    {
        BigInteger kindMove = 0;
        BigInteger piece = pieces[(int)fromColor, (int)fromKind];
        while (!piece.IsZero)
        {
            int fromIndex = BitConstants.TrailingZeroCount(piece);
            kindMove |= GetMove(fromColor, fromKind, fromIndex);

            piece ^= BitConstants.Mask[fromIndex];
        }

        return kindMove;
    }

    private BigInteger GetMove(PieceColor fromColor)
    {
        BigInteger colorMove = 0;
        for (PieceKind fromKind = PieceKind.King; fromKind <= PieceKind.Pawn; ++fromKind)
            colorMove |= GetMove(fromColor, fromKind);

        return colorMove;
    }

    // delegate MoveEffect GetMoveEffect(PieceColor color, PieceKind kind, int fromIndex, int toIndex);

    // 执行某一着后的效果
    private bool DoMoveIsKilled(PieceColor color, PieceKind kind, int fromIndex, int toIndex)
    {
        bool isKilled = false;
        PieceKind eatKind = DoMove(color, kind, fromIndex, toIndex, false);
        if (!(GetMove(Piece.GetOtherColor(color)) & pieces[(int)color, (int)PieceKind.King]).IsZero)
            isKilled = true;

        DoMove(color, kind, fromIndex, toIndex, true, eatKind);
        return isKilled;
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
                BigInteger piece = pieces[color, kind];
                while (!piece.IsZero)
                {
                    int index = BitConstants.TrailingZeroCount(piece);
                    Coord coord = Coord.Coords[index];
                    int row = coord.Row;//(int)BitNum.BOARDROWNUM - 1 - 
                    boardStr[row * ((int)BitNum.BOARDCOLNUM + 1) + coord.Col] = Piece.NameChars[color][kind];

                    // 计算可移动位置
                    BigInteger validMove = 0;
                    BigInteger move = GetMove((PieceColor)color, (PieceKind)kind, index);
                    while (!move.IsZero)
                    {
                        int toIndex = BitConstants.TrailingZeroCount(move);
                        if (!DoMoveIsKilled((PieceColor)color, (PieceKind)kind, index, toIndex))
                            validMove |= BitConstants.Mask[toIndex];

                        move ^= BitConstants.Mask[toIndex];
                    }
                    moves.Add(validMove);

                    piece ^= BitConstants.Mask[index];
                }
            }
        }
        boardStr.Append(BitConstants.GetBigIntArrayString(moves.ToArray(), 8, true, false));

        boardStr.Append("\n");

        return boardStr.ToString();
    }


}

