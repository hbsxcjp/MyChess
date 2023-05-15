using System.Text;
using CChess;

namespace CChessTest;

public class BoardTest
{
    [Theory]
    [InlineData("rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR")]
    [InlineData("5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5")]
    [InlineData("5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9")]
    public void TestCanMoveCoord(string fen)
    {
        string CanMoveCoordString(Board board)
        {
            var fromToIndexs = board.BitBoard.GetAllCanToIndexs(PieceColor.Red);
            fromToIndexs.AddRange(board.BitBoard.GetAllCanToIndexs(PieceColor.Black));
            return string.Join("\n", fromToIndexs.Select(fromToIndexs =>
            {
                // List<Coord> canMoveCoords = piece.CanMoveCoord(board);
                Piece piece = board[fromToIndexs.Item1];
                List<Coord> canMoveCoords = fromToIndexs.Item2.Select(toIndex => Coord.Coords[toIndex]).ToList();
                return $"{piece}{board.GetCoord(piece)} CanMoveCoord: " +
                               string.Join("", canMoveCoords.Select(coord => coord.ToString()));
            }));
        }

        StringBuilder result = new();
        Board board = new(fen);
        foreach (var ct in new List<ChangeType> {
                    ChangeType.NoChange,
                    ChangeType.Symmetry_V,
                    ChangeType.Symmetry_H,
                    ChangeType.NoChange,
                    ChangeType.Exchange})
        {
            Board ctBoard = new(Board.GetFEN(board.GetFEN(), ct));
            result.Append($"{ct}: \n{ctBoard.GetFEN()}\n{ctBoard}{CanMoveCoordString(ctBoard)}\n");
        }

        // Assert.Equal(expected, result.ToString());

        string fenFirstLine = fen.Split('/')[0];
        using StreamWriter writer = File.CreateText($"../../../TestBoard_{fenFirstLine}.txt");
        writer.Write(result.ToString());
    }
}
