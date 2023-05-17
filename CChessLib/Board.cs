using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CChess;

public class Board
{
    public const string FEN = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR";
    private const char FENSplitChar = '/';
    private static readonly string[] NumChars = { "一二三四五六七八九", "１２３４５６７８９" };
    private const string PositionChars = "前中后";
    private const string MoveChars = "退平进";

    public Board(List<Piece> seatPieces)
    {
        SeatPieces = seatPieces;
        BottomColor = SeatPieces.FindLast(piece => piece.Kind == PieceKind.King)?.Color ?? PieceColor.Red;

        BitBoard = new(this);
    }

    public Board(string fen = "") : this(GetSeatPieces(fen.Length == 0 ? FEN : fen))
    {
    }

    public Piece this[int index] => SeatPieces[index];
    public Piece this[Coord coord] => SeatPieces[coord.Index];

    public List<Piece> SeatPieces { get; }
    public PieceColor BottomColor { get; }
    public BitBoard BitBoard { get; }

    public bool IsBottom(PieceColor color) => BottomColor == color;
    public bool IsNull(int row, int col) => this[Coord.Get(row, col)] == Piece.Null;

    public string GetFEN() => PieceCharsToFEN(GetPieceChars());

    public static string PieceCharsToFEN(string pieceChars)
        => Regex.Replace(
            string.Join(FENSplitChar,
                Enumerable.Range(0, Coord.RowCount).Select(row => pieceChars.Substring(row * Coord.ColCount, Coord.ColCount))),
            $"{Piece.NullCh}+",
            match => match.Value.Length.ToString());

    private static string FENToPieceChars(string fen)
        => Regex.Replace(
            string.Concat(fen.Split(FENSplitChar)),
            @"\d",
            match => new string(Piece.NullCh, int.Parse(match.Captures[0].Value)));

    public static string GetFEN(string fen, ChangeType ct)
    {
        if (ct == ChangeType.NoChange)
            return fen;

        if (ct == ChangeType.Exchange)
            return string.Concat(fen.Select(
                ch => char.IsLower(ch) ? char.ToUpper(ch) : char.ToLower(ch)));

        string[] fenArray = fen.Split(FENSplitChar);
        if (fenArray.Length != Coord.RowCount)
            return fen;

        return string.Join(FENSplitChar, ct switch
        {
            ChangeType.Symmetry_H => fenArray.Select(line => string.Concat(line.Reverse())),
            ChangeType.Symmetry_V => fenArray.Reverse(),
            //ChangeType.Rotate
            _ => fenArray.Reverse().Select(line => string.Concat(line.Reverse()))
        });
    }

    private string GetPieceChars() => string.Concat(SeatPieces.Select(piece => piece.Char));

    public Coord GetCoord(Piece piece)
    {
        int index = SeatPieces.IndexOf(piece);
        return index < 0 ? Coord.Null : Coord.Coords[index];
    }

    public List<Piece> GetLivePieces()
        => SeatPieces.Where(piece => piece != Piece.Null).ToList();

    public List<Piece> GetLivePieces(PieceColor color)
        => GetLivePieces().Where(piece => piece.Color == color).ToList();

    private List<Piece> GetLivePieces(PieceColor color, PieceKind kind)
        => GetLivePieces(color).Where(piece => piece.Kind == kind).ToList();

    private List<Piece> GetLivePieces(PieceColor color, PieceKind kind, int col)
        => GetLivePieces(color, kind).Where(piece => GetCoord(piece).Col == col).ToList();

    public string GetZhStrFromCoordPair(CoordPair coordPair)
    {
        Coord fromCoord = coordPair.FromCoord, toCoord = coordPair.ToCoord;
        Debug.Assert(!IsNull(fromCoord.Row, fromCoord.Col));

        string zhStr;
        Piece fromPiece = this[fromCoord];
        PieceColor color = fromPiece.Color;
        PieceKind kind = fromPiece.Kind;
        char name = fromPiece.Name;
        int fromRow = fromCoord.Row, fromCol = fromCoord.Col,
            toRow = toCoord.Row, toCol = toCoord.Col;
        bool isSameRow = fromRow == toRow, isBottomColor = IsBottom(color);

        List<Piece> livePieces = GetLivePieces(color, kind, fromCol);
        if (livePieces.Count > 1 && kind > PieceKind.Bishop)
        {
            // 该列有多兵时，需检查多列多兵的情况
            if (kind == PieceKind.Pawn)
                livePieces = GetLivePieces_MultiPawns(color);

            SortPieces(livePieces, isBottomColor);
            zhStr = $"{PreChars(livePieces.Count)[livePieces.IndexOf(fromPiece)]}{name}";
        }
        else
        {  //将帅, 仕(士),相(象): 不用“前”和“后”区别，因为能退的一定在前，能进的一定在后
            zhStr = $"{name}{GetColChar(color, Coord.GetCol(fromCol, isBottomColor))}";
        }

        int numOrCol = !isSameRow && Piece.IsLinePiece(kind)
            ? Math.Abs(fromRow - toRow) - 1 : Coord.GetCol(toCol, isBottomColor);
        zhStr += $"{MoveChar(isSameRow, isBottomColor == toRow > fromRow)}{GetColChar(color, numOrCol)}";

        Debug.Assert(GetCoordPairFromZhStr(zhStr).Equals(coordPair));
        return zhStr;
    }

    public CoordPair GetCoordPairFromZhStr(string zhStr)
    {
        // Debug.Assert(zhStr.Length == 4);
        if (zhStr.Length != 4)
            return CoordPair.Null;

        PieceColor color = GetColor_Num(zhStr[3]);
        bool isBottomColor = IsBottom(color);
        int index = 0, movDir = MoveDir(zhStr[2]),
            absMovDir = movDir * (isBottomColor ? 1 : -1);

        List<Piece> livePieces;
        PieceKind kind = Piece.GetKind_Name(zhStr[0]);
        if (kind != PieceKind.NoKind)
        {   // 首字符为棋子名
            int col = Coord.GetCol(GetCol(color, zhStr[1]), isBottomColor);
            livePieces = GetLivePieces(color, kind, col);
            // Debug.Assert(livePieces.Count > 0);
            if (livePieces.Count == 0)
                return CoordPair.Null;

            // 士、象同列时不分前后，以进、退区分棋子。移动方向为退时，修正index
            if (livePieces.Count == 2)
                index = (movDir == 1) ? 1 : 0;
        }
        else
        {
            kind = Piece.GetKind_Name(zhStr[1]);
            livePieces = kind == PieceKind.Pawn ? GetLivePieces_MultiPawns(color) : GetLivePieces(color, kind);
            // Debug.Assert(livePieces.Count > 1);

            if (livePieces.Count < 2)
                return CoordPair.Null;

            index = PreChars(livePieces.Count).IndexOf(zhStr[0]);
        }

        if (livePieces.Count <= index)
            return CoordPair.Null;

        SortPieces(livePieces, isBottomColor);
        Coord fromCoord = GetCoord(livePieces[index]);

        int toNum = GetCol(color, zhStr[3]) + 1,
            toRow = fromCoord.Row, toCol = Coord.GetCol(toNum - 1, isBottomColor);
        if (Piece.IsLinePiece(kind))
        {
            if (absMovDir != 0)
            {
                toRow += absMovDir * toNum;
                toCol = fromCoord.Col;
            }
        }
        else
        {   // 斜线走子：仕、相、马
            int colAway = Math.Abs(toCol - fromCoord.Col);
            //  相距1或2列
            int rowInc = (kind == PieceKind.Advisor || kind == PieceKind.Bishop)
                ? colAway : (colAway == 1 ? 2 : 1);
            toRow += absMovDir * rowInc;
        }

        if (!Coord.IsValid(toRow, toCol))
            return CoordPair.Null;

        return new(fromCoord, Coord.Get(toRow, toCol));
    }

    private void SortPieces(List<Piece> pieces, bool isBottomColor)
        => pieces.Sort(delegate (Piece piece1, Piece piece2)
        {
            Coord coord1 = GetCoord(piece1), coord2 = GetCoord(piece2);
            int comp = coord1.Col.CompareTo(coord2.Col);
            if (comp == 0)
                comp = coord1.Row.CompareTo(coord2.Row);

            return isBottomColor ? -comp : comp;
        });

    private List<Piece> GetLivePieces_MultiPawns(PieceColor color)
        => GetLivePieces(color, PieceKind.Pawn)
            .GroupBy(piece => GetCoord(piece).Col)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.AsEnumerable())
            .ToList();

    public static CoordPair GetCoordPairFromRowCol(string rowCol)
        => new(
            int.Parse(rowCol[0].ToString()),
            int.Parse(rowCol[1].ToString()),
            int.Parse(rowCol[2].ToString()),
            int.Parse(rowCol[3].ToString()));

    public static CoordPair GetCoordPairFromIccs(string iccs)
        => new(
            int.Parse(iccs[1].ToString()),
            Coord.ColChars.IndexOf(iccs[0]),
            int.Parse(iccs[3].ToString()),
            Coord.ColChars.IndexOf(iccs[2]));

    public static char GetColChar(PieceColor color, int col) => NumChars[(int)color][col];
    public static int GetCol(PieceColor color, char colChar) => NumChars[(int)color].IndexOf(colChar);

    public static PieceColor GetColor_Num(char numChar)
        => NumChars[(int)PieceColor.Red].Contains(numChar) ? PieceColor.Red : PieceColor.Black;

    public static string PreChars(int count)
        => (count == 2 ? $"{PositionChars[0]}{PositionChars[2]}"
            : (count == 3 ? PositionChars : NumChars[0][0..4]));

    public static char MoveChar(bool isSameRow, bool isGo) => MoveChars[isSameRow ? 1 : (isGo ? 2 : 0)];
    public static int MoveDir(char movCh) => MoveChars.IndexOf(movCh) - 1;

    public static string PGNZHCharsPattern()
        => string.Join('|', Piece.PieceColors.Select(color => PGNZHCharsPattern(color)));

    public static string PGNZHCharsPattern(PieceColor color)
        => @$"(?:[{Piece.NameChars[(int)color]}{NumChars[(int)color]}{PositionChars}]{{2}}[{MoveChars}][{NumChars[(int)color]}])";

    private static List<Piece> GetSeatPieces(string fen)
    {
        List<Piece> seatPieces = new(Coord.Count);
        foreach (char ch in FENToPieceChars(fen))
        {
            if (ch == Piece.NullCh)
            {
                seatPieces.Add(Piece.Null);
            }
            else
            {
                foreach (Piece piece in Piece.PieceArray[(int)Piece.GetColor(ch)][(int)Piece.GetKind(ch)])
                    if (!seatPieces.Contains(piece))
                    {
                        seatPieces.Add(piece);
                        break;
                    }
            }
        }

        return seatPieces;
    }

    public override string ToString()
    {
        // 棋盘上边标识字符串
        string[] preStr = {
                "　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n",
                "　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n"
            };

        // 棋盘下边标识字符串
        string[] sufStr = {
                "九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n",
                "９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n"
            };

        // 文本空棋盘
        StringBuilder textBlankBoard =
            new(
@"┏━┯━┯━┯━┯━┯━┯━┯━┓
┃　│　│　│╲│╱│　│　│　┃
┠─┼─┼─┼─╳─┼─┼─┼─┨
┃　│　│　│╱│╲│　│　│　┃
┠─╬─┼─┼─┼─┼─┼─╬─┨
┃　│　│　│　│　│　│　│　┃
┠─┼─╬─┼─╬─┼─╬─┼─┨
┃　│　│　│　│　│　│　│　┃
┠─┴─┴─┴─┴─┴─┴─┴─┨
┃　　　　　　　　　　　　　　　┃
┠─┬─┬─┬─┬─┬─┬─┬─┨
┃　│　│　│　│　│　│　│　┃
┠─┼─╬─┼─╬─┼─╬─┼─┨
┃　│　│　│　│　│　│　│　┃
┠─╬─┼─┼─┼─┼─┼─╬─┨
┃　│　│　│╲│╱│　│　│　┃
┠─┼─┼─┼─╳─┼─┼─┼─┨
┃　│　│　│╱│╲│　│　│　┃
┗━┷━┷━┷━┷━┷━┷━┷━┛
"
            );
        // 边框粗线

        GetLivePieces().ForEach(piece =>
            textBlankBoard[Coord.GetDoubleIndex(GetCoord(piece))] = piece.PrintName);

        int index = IsBottom(PieceColor.Red) ? 0 : 1;
        return $"{preStr[index]}{textBlankBoard.ToString()}{sufStr[index]}";
    }
}

