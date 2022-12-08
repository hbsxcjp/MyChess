using CChess;

namespace CChessTest;

public class PieceTest
{
    [Fact]
    public void TestToString()
    {
        string expected = "红帅K(-1,-1) 红仕A(-1,-1) 红仕A(-1,-1) 红相B(-1,-1) 红相B(-1,-1) 红马N(-1,-1) 红马N(-1,-1) 红车R(-1,-1) 红车R(-1,-1) 红炮C(-1,-1) 红炮C(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 黑将k(-1,-1) 黑士a(-1,-1) 黑士a(-1,-1) 黑象b(-1,-1) 黑象b(-1,-1) 黑馬n(-1,-1) 黑馬n(-1,-1) 黑車r(-1,-1) 黑車r(-1,-1) 黑砲c(-1,-1) 黑砲c(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1)";
        var pieces = new Pieces();
        List<Piece> allPieces = pieces.GetPieces(PieceColor.Red);
        allPieces.AddRange(pieces.GetPieces(PieceColor.Black));

        string actual = string.Join(" ", allPieces.Select(seat => seat.ToString()));

        Assert.Equal(expected, actual);
    }
}
