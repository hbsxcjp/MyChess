namespace CChess;

public class Pieces
{
    private Piece[][][] _pieces;
    private static readonly Pieces pieces = new();

    private Pieces()
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
            Piece[][] colorPieces = new Piece[(int)BitNum.KINDNUM][];
            for (int kind = 0; kind < (int)BitNum.KINDNUM; kind++)
                colorPieces[kind] = getKindPieces(color, PieceType[kind], KindPieceNums[kind]);

            return colorPieces;
        }

        _pieces = new Piece[(int)BitNum.COLORNUM][][];
        for (int color = 0; color < (int)BitNum.COLORNUM; color++)
            _pieces[color] = getColorPieces((PieceColor)color);
    }

    public static Pieces ThePieces { get => pieces; }

    public Piece GetKing(PieceColor color) => _pieces[(int)color][(int)PieceKind.King][0];

    public List<Piece> GetSeatPieces(string pieceChars)
    {
        List<Piece> pieces = new(pieceChars.Length);
        foreach (char ch in pieceChars)
        {
            if (ch == Piece.NullCh)
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