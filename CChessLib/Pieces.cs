namespace CChess;

public class Pieces
{
    private Piece[][][] _pieces;
    public static readonly Pieces pieces = new();

    private Pieces()
    {
        const int ColorNum = 2;
        const int KindNum = 7;
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
            Piece[][] colorPieces = new Piece[KindNum][];
            for (int k = 0; k < KindNum; k++)
                colorPieces[k] = getKindPieces(color, PieceType[k], KindPieceNums[k]);

            return colorPieces;
        }

        _pieces = new Piece[ColorNum][][];
        for (int c = 0; c < ColorNum; c++)
            _pieces[c] = getColorPieces((PieceColor)c);
    }

    public static Pieces ThePieces { get => pieces; }

    public Piece GetKing(PieceColor color) => _pieces[(int)color][(int)PieceKind.King][0];

    public List<Piece> GetSeatPieces(string pieceChars)
    {
        List<Piece> pieces = new();
        foreach (char ch in pieceChars)
        {
            if (ch == Piece.Null.Char)
                pieces.Add(Piece.Null);
            else
            {
                foreach (Piece piece in _pieces[(int)Piece.GetColor(ch)][(int)Piece.GetKind(ch)])
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

    public static List<Piece> GetSeatPieces() => Coord.Coords.Select(coord => Piece.Null).ToList();

    public List<Piece> GetPieces()
        => _pieces.SelectMany(colorPieces => colorPieces)
            .SelectMany(kindPieces => kindPieces)
            .ToList();

    public override string ToString()
       => string.Join(" ", GetPieces().Select(piece => $"{piece}{Coord.Null}"));
}