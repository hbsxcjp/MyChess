namespace CChess;

public class Pieces
{
    private Piece[][][] _pieces;
    private const int ColorNum = 2;

    public static readonly Pieces ThePieces = new();

    private Pieces()
    {
        static Piece[] getKindPieces(PieceColor color, Type type, int num)
        {
            var kindPieces = new Piece[num];
            var constructorInfo = type.GetConstructor(new Type[] { typeof(PieceColor) });
            if (constructorInfo != null)
                for (int i = 0; i < num; i++)
                    kindPieces[i] = (Piece)constructorInfo.Invoke(new object[] { color });

            return kindPieces;
        }

        static Piece[][] getColorPieces(PieceColor color)
        {
            Type[] pieceType = { typeof(King), typeof(Advisor), typeof(Bishop),
                    typeof(Knight), typeof(Rook), typeof(Cannon), typeof(Pawn) };
            int[] KindNums = { 1, 2, 2, 2, 2, 2, 5 };
            Piece[][] colorPieces = new Piece[Piece.KindNum][];
            for (int k = 0; k < Piece.KindNum; k++)
                colorPieces[k] = getKindPieces(color, pieceType[k], KindNums[k]);

            return colorPieces;
        }

        _pieces = new Piece[ColorNum][][];
        for (int c = 0; c < ColorNum; c++)
            _pieces[c] = getColorPieces((PieceColor)c);
    }

    public List<Piece> GetPieces(char ch)
        => _pieces[(int)Piece.GetColor(ch)][(int)Piece.GetKind(ch)].ToList();

    public List<Piece> GetPieces()
        => _pieces.SelectMany(colorPieces => colorPieces)
            .SelectMany(kindPieces => kindPieces)
            .ToList();

    public override string ToString()
       => string.Join(" ",
            GetPieces().Select(piece => $"{piece}{Coord.Null}"));
}