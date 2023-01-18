using System.Text;

namespace CChess;

public class Board
{
    private readonly Pieces pieces;
    private readonly Seats seats;

    public Board()
    {
        pieces = new();
        seats = new();
    }

    public Piece Done(CoordPair coordPair) => seats.Done(coordPair);
    public void Undo(CoordPair coordPair, Piece toPiece) => seats.Undo(coordPair, toPiece);

    public List<Coord> GetAllCoords() => seats.GetAllCoords();

    public bool IsNull(int row, int col) => seats[row, col].Piece.IsNull;
    public bool IsBottom(PieceColor color) => pieces.IsBottom(color);
    public bool IsKilled(PieceColor color) => seats.IsKilled(color, pieces);
    public bool IsFailed(PieceColor color) => seats.IsFailed(color, pieces);

    public List<Coord> CanMoveCoord(Coord fromCoord) => seats.CanMoveCoord(fromCoord, pieces);
    public bool CanMove(Coord fromCoord, Coord toCoord) => seats.CanMove(fromCoord, toCoord, pieces);

    public void Reset() => seats.Reset();

    public string GetFEN() => seats.GetFEN();
    public bool SetFEN(string fen) => seats.SetFEN(fen, pieces);

    public bool ChangeLayout(ChangeType ct) => SetFEN(Seats.GetFEN(GetFEN(), ct));

    public string GetZhStrFromCoordPair(CoordPair coordPair) => seats.GetZhStr(coordPair, pieces);
    public CoordPair GetCoordPairFromZhstr(string zhStr) => seats.GetCoordPair(zhStr, pieces);
    public CoordPair GetCoordPairFromRowCol(string rowCol)
        => GetCoordPair(int.Parse(rowCol[0].ToString()), int.Parse(rowCol[1].ToString()),
            int.Parse(rowCol[2].ToString()), int.Parse(rowCol[3].ToString()));
    public CoordPair GetCoordPairFromIccs(string iccs)
        => GetCoordPair(int.Parse(iccs[1].ToString()), Coord.ColChars.IndexOf(iccs[0]),
            int.Parse(iccs[3].ToString()), Coord.ColChars.IndexOf(iccs[2]));
    public CoordPair GetCoordPair(int frow, int fcol, int trow, int tcol)
        => new(seats[frow, fcol].Coord, seats[trow, tcol].Coord);

    public CoordPair GetCoordPair(string pgnText, FileExtType fileExtType)
        => fileExtType switch
        {
            FileExtType.PGNIccs => GetCoordPairFromIccs(pgnText),
            FileExtType.PGNRowCol => GetCoordPairFromRowCol(pgnText),
            _ => GetCoordPairFromZhstr(pgnText)
        };

    public List<Piece> LivePieces(PieceColor color) => pieces.GetLivePieces(color);

    override public string ToString()
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

        var livePieces = pieces.GetLivePieces(PieceColor.Red);
        livePieces.AddRange(pieces.GetLivePieces(PieceColor.Black));
        foreach (var piece in livePieces)
            textBlankBoard[Coord.GetDoubleIndex(piece.Coord)] = piece.PrintName;

        int index = IsBottom(PieceColor.Red) ? 0 : 1;
        return preStr[index] + textBlankBoard.ToString() + sufStr[index];
    }
}

