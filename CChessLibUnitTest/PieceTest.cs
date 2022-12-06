using CChess;

namespace CChessTest;

public class PieceTest
{
    [Fact]
    public void TestToString()
    {
        string expect = "红帅K(-1,-1) 红仕A(-1,-1) 红仕A(-1,-1) 红相B(-1,-1) 红相B(-1,-1) 红马N(-1,-1) 红马N(-1,-1) 红车R(-1,-1) 红车R(-1,-1) 红炮C(-1,-1) 红炮C(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 黑将k(-1,-1) 黑士a(-1,-1) 黑士a(-1,-1) 黑象b(-1,-1) 黑象b(-1,-1) 黑馬n(-1,-1) 黑馬n(-1,-1) 黑車r(-1,-1) 黑車r(-1,-1) 黑砲c(-1,-1) 黑砲c(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1)";
        List<Piece> pieces = new();
        foreach (var colorPieces in Piece.CreatPieces())
            foreach (var kindPieces in colorPieces)
                foreach (var piece in kindPieces)
                    pieces.Add(piece);

        string actual = string.Join(" ", pieces.Select(seat => seat.ToString()));

        Assert.Equal(expect, actual);
    }
}
