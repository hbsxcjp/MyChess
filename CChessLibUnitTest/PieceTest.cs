using CChess;

namespace CChessTest;

public class PieceTest
{
    [Fact]
    public void TestToString()
    {
        string expected = "红帅K(-1,-1) 红仕A(-1,-1) 红仕A(-1,-1) 红相B(-1,-1) 红相B(-1,-1) 红马N(-1,-1) 红马N(-1,-1) 红车R(-1,-1) 红车R(-1,-1) 红炮C(-1,-1) 红炮C(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 红兵P(-1,-1) 黑将k(-1,-1) 黑士a(-1,-1) 黑士a(-1,-1) 黑象b(-1,-1) 黑象b(-1,-1) 黑馬n(-1,-1) 黑馬n(-1,-1) 黑車r(-1,-1) 黑車r(-1,-1) 黑砲c(-1,-1) 黑砲c(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1) 黑卒p(-1,-1)";
        List<Piece> allPieces = new Pieces().GetPieces();

        string actual = string.Join(" ", allPieces.Select(seat => seat.ToString()));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestPutCoord()
    {
        string expected = "红帅K(-1,-1) PutCoord: (0,3)(0,4)(0,5)(1,3)(1,4)(1,5)(2,3)(2,4)(2,5)\n红仕A(-1,-1) PutCoord: (0,3)(0,5)(2,3)(2,5)(1,4)\n红仕A(-1,-1) PutCoord: (0,3)(0,5)(2,3)(2,5)(1,4)\n红相B(-1,-1) PutCoord: (0,2)(0,6)(4,2)(4,6)(2,0)(2,4)(2,8)\n红相B(-1,-1) PutCoord: (0,2)(0,6)(4,2)(4,6)(2,0)(2,4)(2,8)\n红马N(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红马N(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红车R(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红车R(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红炮C(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红炮C(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红兵P(-1,-1) PutCoord: (3,0)(3,2)(3,4)(3,6)(3,8)(4,0)(4,2)(4,4)(4,6)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红兵P(-1,-1) PutCoord: (3,0)(3,2)(3,4)(3,6)(3,8)(4,0)(4,2)(4,4)(4,6)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红兵P(-1,-1) PutCoord: (3,0)(3,2)(3,4)(3,6)(3,8)(4,0)(4,2)(4,4)(4,6)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红兵P(-1,-1) PutCoord: (3,0)(3,2)(3,4)(3,6)(3,8)(4,0)(4,2)(4,4)(4,6)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n红兵P(-1,-1) PutCoord: (3,0)(3,2)(3,4)(3,6)(3,8)(4,0)(4,2)(4,4)(4,6)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑将k(-1,-1) PutCoord: (7,3)(7,4)(7,5)(8,3)(8,4)(8,5)(9,3)(9,4)(9,5)\n黑士a(-1,-1) PutCoord: (7,3)(7,5)(9,3)(9,5)(8,4)\n黑士a(-1,-1) PutCoord: (7,3)(7,5)(9,3)(9,5)(8,4)\n黑象b(-1,-1) PutCoord: (5,2)(5,6)(9,2)(9,6)(7,0)(7,4)(7,8)\n黑象b(-1,-1) PutCoord: (5,2)(5,6)(9,2)(9,6)(7,0)(7,4)(7,8)\n黑馬n(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑馬n(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑車r(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑車r(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑砲c(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑砲c(-1,-1) PutCoord: (0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑卒p(-1,-1) PutCoord: (5,0)(5,2)(5,4)(5,6)(5,8)(6,0)(6,2)(6,4)(6,6)(6,8)(0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)\n黑卒p(-1,-1) PutCoord: (5,0)(5,2)(5,4)(5,6)(5,8)(6,0)(6,2)(6,4)(6,6)(6,8)(0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)\n黑卒p(-1,-1) PutCoord: (5,0)(5,2)(5,4)(5,6)(5,8)(6,0)(6,2)(6,4)(6,6)(6,8)(0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)\n黑卒p(-1,-1) PutCoord: (5,0)(5,2)(5,4)(5,6)(5,8)(6,0)(6,2)(6,4)(6,6)(6,8)(0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)\n黑卒p(-1,-1) PutCoord: (5,0)(5,2)(5,4)(5,6)(5,8)(6,0)(6,2)(6,4)(6,6)(6,8)(0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)";
        Seats seats = new();
        Pieces pieces = new();
        List<Piece> allPieces = pieces.GetPieces();

        string actual = string.Join("\n", allPieces.Select(piece => piece.ToString() + " PutCoord: " +
            string.Concat(seats.PutCoord(piece, pieces.IsBottom(piece.Color)).Select(coord => coord.ToString()))));

        Assert.Equal(expected, actual);
    }
}
