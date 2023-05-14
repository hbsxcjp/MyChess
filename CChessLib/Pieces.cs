namespace CChess;

public static class Pieces
{
    private static Piece[][][] pieceArray = GetPieceArray();

    private static Piece[][][] GetPieceArray()
    {
        int[] KindPieceNums = { 1, 2, 2, 2, 2, 2, 5 };
        List<Type> PieceType = new List<Type> {
            typeof(King), typeof(Advisor), typeof(Bishop), typeof(Knight), typeof(Rook), typeof(Cannon), typeof(Pawn) };

        Piece[] getKindPieces(PieceColor color, Type type, int num)
        {
            var kindPieces = new Piece[num];
            var constructorInfo = type.GetConstructor(new Type[] { typeof(PieceColor) });
            if (constructorInfo != null)
                for (int i = 0; i < num; i++)
                    kindPieces[i] = (Piece)constructorInfo.Invoke(new object[] { color });

            return kindPieces;
        }

        Piece[][] getColorPieces(PieceColor color)
        {
            Piece[][] colorPieces = new Piece[Piece.KindCount][];
            for (int kind = 0; kind < Piece.KindCount; kind++)
                colorPieces[kind] = getKindPieces(color, PieceType[kind], KindPieceNums[kind]);

            return colorPieces;
        }

        Piece[][][] pieceArray = new Piece[Piece.ColorCount][][];
        for (int color = 0; color < Piece.ColorCount; color++)
            pieceArray[color] = getColorPieces((PieceColor)color);

        return pieceArray;
    }

    public static Piece GetKing(PieceColor color) => pieceArray[(int)color][(int)PieceKind.King][0];

    public static List<Piece> GetSeatPieces(string pieceChars)
    {
        List<Piece> pieces = new(pieceChars.Length);
        foreach (char ch in pieceChars)
        {
            if (ch == Piece.NullCh)
                pieces.Add(Piece.Null);
            else
            {
                foreach (Piece piece in pieceArray[(int)Piece.GetColor(ch)][(int)Piece.GetKind(ch)])
                {
                    if (!pieces.Contains(piece))
                    {
                        pieces.Add(piece);
                        break;
                    }
                }
            }
        }

        return pieces;
    }

    public static List<Piece> GetAllPiece()
        => pieceArray.SelectMany(colorPieces => colorPieces)
            .SelectMany(kindPieces => kindPieces)
            .ToList();

    public new static string ToString()
       => string.Join(" ", GetAllPiece().Select(piece => $"{piece}{Coord.Null}"));
}