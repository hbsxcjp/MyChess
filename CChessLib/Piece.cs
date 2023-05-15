using System.Numerics;
namespace CChess;

public enum PieceColor { Red, Black, NoColor = -1 }

public enum PieceKind { King, Advisor, Bishop, Knight, Rook, Cannon, Pawn, NoKind = -1 }

public class Piece
{
    public static readonly Piece Null = new(PieceColor.NoColor, PieceKind.NoKind);
    private static Piece[][][] PieceArray = CreatePieceArray();

    public const int ColorCount = 2;
    public const int KindCount = 7;
    public const int LegCount = 4;

    public const char NullCh = '_';
    public const char NullName = '空';

    public static readonly PieceColor[] PieceColors = { PieceColor.Red, PieceColor.Black };
    public static readonly PieceKind[] PieceKinds = { PieceKind.King, PieceKind.Advisor,
        PieceKind.Bishop, PieceKind.Knight, PieceKind.Rook, PieceKind.Cannon, PieceKind.Pawn };

    public static readonly string[] NameChars = { "帅仕相马车炮兵", "将士象马车炮卒" };
    private static readonly string[] ChChars = { "KABNRCP", "kabnrcp" };
    private static readonly string ColorChars = "无红黑";

    private Piece(PieceColor color, PieceKind kind) { Color = color; Kind = kind; }

    public PieceColor Color { get; }
    public PieceKind Kind { get; }

    public char Char { get => Kind == PieceKind.NoKind ? Piece.NullCh : ChChars[(int)Color][(int)Kind]; }
    public char Name { get => GetName(Color, Kind); }
    public char PrintName { get => GetPrintName(Color, Kind); }

    public List<Coord> PutCoord(bool isBottom)
    {
        List<Coord> getCoords(BigInteger bigInt)
            => BitConstants.GetNonZeroIndexs(bigInt).Select(index => Coord.Coords[index]).ToList();

        int isBottomInt = isBottom ? 1 : 0;
        return Kind switch
        {
            PieceKind.NoKind => new(),
            PieceKind.King => getCoords(BitConstants.KingPut[isBottomInt]),
            PieceKind.Advisor => getCoords(BitConstants.AdvisorPut[isBottomInt]),
            PieceKind.Bishop => getCoords(BitConstants.BishopPut[isBottomInt]),
            PieceKind.Pawn => getCoords(BitConstants.PawnPut[isBottomInt]),
            _ => Coord.Coords
        };
    }

    public static char GetName(PieceColor color, PieceKind kind)
        => kind == PieceKind.NoKind ? Piece.NullName : NameChars[(int)color][(int)kind];

    public static char GetPrintName(PieceColor color, PieceKind kind)
    {
        const string nrcChars = "馬車砲";
        List<PieceKind> nrcKinds = new List<PieceKind> { PieceKind.Knight, PieceKind.Rook, PieceKind.Cannon };
        return (color == PieceColor.Black && nrcKinds.Contains(kind) ? nrcChars[nrcKinds.IndexOf(kind)] : GetName(color, kind));
    }

    public static PieceColor GetOtherColor(PieceColor color) => color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;
    public static PieceColor GetColor(char ch) => char.IsUpper(ch) ? PieceColor.Red : PieceColor.Black;

    public static PieceKind GetKind(char ch) => (PieceKind)ChChars[(int)GetColor(ch)].IndexOf(ch);

    public static PieceKind GetKind_Name(char name)
        => (PieceKind)NameChars[NameChars[0].Contains(name) ? 0 : 1].IndexOf(name);

    public static bool IsLinePiece(PieceKind kind)
        => (kind == PieceKind.King || kind == PieceKind.Rook || kind == PieceKind.Cannon || kind == PieceKind.Pawn);

    private static Piece[][][] CreatePieceArray()
    {
        int[] KindPieceNums = { 1, 2, 2, 2, 2, 2, 5 };
        Piece[] getKindPieces(PieceColor color, PieceKind kind, int num)
        {
            Piece[] kindPieces = new Piece[num];
            for (int i = 0; i < num; i++)
                kindPieces[i] = new(color, kind);

            return kindPieces;
        }

        Piece[][] getColorPieces(PieceColor color)
        {
            Piece[][] colorPieces = new Piece[Piece.KindCount][];
            for (int kind = 0; kind < Piece.KindCount; kind++)
                colorPieces[kind] = getKindPieces(color, (PieceKind)kind, KindPieceNums[kind]);

            return colorPieces;
        }

        Piece[][][] pieceArray = new Piece[Piece.ColorCount][][];
        for (int color = 0; color < Piece.ColorCount; color++)
            pieceArray[color] = getColorPieces((PieceColor)color);

        return pieceArray;
    }

    public static List<Piece> GetSeatPieces(string pieceChars)
    {
        List<Piece> pieces = new(pieceChars.Length);
        foreach (char ch in pieceChars)
        {
            if (ch == Piece.NullCh)
                pieces.Add(Piece.Null);
            else
            {
                foreach (Piece piece in PieceArray[(int)Piece.GetColor(ch)][(int)Piece.GetKind(ch)])
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
        => PieceArray.SelectMany(colorPieces => colorPieces)
            .SelectMany(kindPieces => kindPieces)
            .ToList();

    public static string AllPieceString()
       => string.Join(" ", GetAllPiece().Select(piece => $"{piece}{Coord.Null}"));

    override public string ToString() => $"{ColorChars[(int)Color + 1]}{PrintName}{Char}";
}