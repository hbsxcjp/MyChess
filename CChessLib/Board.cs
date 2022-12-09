using System.Diagnostics;
using System.Text;

namespace CChess;

public class Board
{
    private readonly Pieces pieces;
    private readonly Seats seats;

    public Board()
    {
        pieces = new Pieces();
        seats = new Seats();
    }

    public Seat this[int row, int col] { get { return seats[row, col]; } }

    public Piece Done(CoordPair coordPair) => seats.Done(coordPair);
    public void Undo(CoordPair coordPair, Piece toPiece) => seats.Undo(coordPair, toPiece);

    public List<Coord> GetAllCoords() => seats.GetAllCoords();

    public bool IsNull(int row, int col) => seats[row, col].Piece.IsNull;
    public bool IsBottom(PieceColor color) => pieces.IsBottom(color);
    public bool IsKilled(PieceColor color)
    {
        Coord kingCoord = GetKing(color);
        var otherColor = color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;

        // 将帅是否对面
        int col = kingCoord.Col;
        Coord otherKingCoord = GetKing(otherColor);
        if (col == otherKingCoord.Col)
        {
            int thisRow = kingCoord.Row, otherRow = otherKingCoord.Row,
                lowRow = Math.Min(thisRow, otherRow) + 1, count = Math.Abs(thisRow - otherRow) - 1;
            if (Enumerable.Range(lowRow, count).All(row => this.IsNull(row, col)))
                return true;
        }

        // 是否正被将军
        return LivePieces(otherColor).Any(piece => piece.MoveCoord(this).Contains(kingCoord));
    }
    private Coord GetKing(PieceColor color) => pieces.GetKing(color).Seat.Coord;

    public bool IsFailed(PieceColor color)
        => LivePieces(color).All(piece => CanMoveCoord(piece.Coord).Count == 0);

    /// <summary>
    /// 可移动位置, 排除将帅对面、被将军的位置
    /// </summary>
    /// <param name="fromCoord"></param>
    /// <returns></returns>
    public List<Coord> CanMoveCoord(Coord fromCoord)
    {
        List<Coord> coords = seats[fromCoord].Piece.MoveCoord(this);
        coords.RemoveAll(toCoord => !CanMove(fromCoord, toCoord));
        return coords;
    }

    /// <summary>
    /// 检测是否可移动, 包括直接杀将、移动后将帅未对面、未被将军
    /// </summary>
    /// <param name="fromCoord"></param>
    /// <param name="toCoord"></param>
    /// <returns></returns>
    public bool CanMove(Coord fromCoord, Coord toCoord)
    {
        Seat fromSeat = seats[fromCoord];
        Debug.Assert(!fromSeat.Piece.IsNull);

        var color = fromSeat.Piece.Color;
        Seat toSeat = seats[toCoord];
        Piece toPiece = toSeat.Piece;

        // 如是对方将帅的位置则直接可走，不用判断是否被将军（如加以判断，则会直接走棋吃将帅）
        if (toPiece.Kind == PieceKind.King)
            return true;

        fromSeat.MoveTo(toSeat, Piece.Null);
        bool killed = IsKilled(color);
        toSeat.MoveTo(fromSeat, toPiece);
        return !killed;
    }

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

    // public List<Piece> Pieces(PieceColor color) => pieces.GetPieces(color);

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

        // foreach (var seat in _seats)
        //     if (!seat.Piece.IsNull)
        //         textBlankBoard[Coord.GetDoubleIndex(seat.Coord)] = seat.Piece.PrintName;
        var livePieces = pieces.GetLivePieces(PieceColor.Red);
        livePieces.AddRange(pieces.GetLivePieces(PieceColor.Black));
        foreach (var piece in livePieces)
            textBlankBoard[Coord.GetDoubleIndex(piece.Seat.Coord)] = piece.PrintName;

        int index = IsBottom(PieceColor.Red) ? 0 : 1;
        return preStr[index] + textBlankBoard.ToString() + sufStr[index];
    }
}

