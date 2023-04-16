using System.Text;
using CChess;

namespace CChessTest;

public class BitBoardTest
{
    [Fact]
    public void TestBitConstants()
    {
        using StreamWriter writer = File.CreateText("TestBitConstants.txt");
        writer.Write(BitConstants.ToString());
    }

    [Theory]
    [InlineData(Board.FEN)]
    [InlineData("5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5")]
    [InlineData("2b1kab2/4a4/4c4/9/9/3R5/9/1C7/4r4/2BK2B2")]
    [InlineData("4kab2/4a4/4b4/3N5/9/4N4/4n4/4B4/4A4/3AK1B2")]
    public void TestBitBoard(string fen)
    {
        Board board = new(fen);
        BitBoard bitBoard = new(board);
        using StreamWriter writer = File.AppendText("TestBitBoard.txt");

        writer.Write(bitBoard);
    }

}
