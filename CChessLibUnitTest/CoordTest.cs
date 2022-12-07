using CChess;

namespace CChessTest;

public class CoordTest
{
    private List<Coord> allCoord = Coord.CreatCoords();

    [Fact]
    public void TestToString()
    {
        string expected = "(0,0)(0,1)(0,2)(0,3)(0,4)(0,5)(0,6)(0,7)(0,8)(1,0)(1,1)(1,2)(1,3)(1,4)(1,5)(1,6)(1,7)(1,8)(2,0)(2,1)(2,2)(2,3)(2,4)(2,5)(2,6)(2,7)(2,8)(3,0)(3,1)(3,2)(3,3)(3,4)(3,5)(3,6)(3,7)(3,8)(4,0)(4,1)(4,2)(4,3)(4,4)(4,5)(4,6)(4,7)(4,8)(5,0)(5,1)(5,2)(5,3)(5,4)(5,5)(5,6)(5,7)(5,8)(6,0)(6,1)(6,2)(6,3)(6,4)(6,5)(6,6)(6,7)(6,8)(7,0)(7,1)(7,2)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)(8,0)(8,1)(8,2)(8,3)(8,4)(8,5)(8,6)(8,7)(8,8)(9,0)(9,1)(9,2)(9,3)(9,4)(9,5)(9,6)(9,7)(9,8)";

        string actual = string.Concat(allCoord.Select(coord => coord.ToString()));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestRowCol()
    {
        string expected = "00 01 02 03 04 05 06 07 08 10 11 12 13 14 15 16 17 18 20 21 22 23 24 25 26 27 28 30 31 32 33 34 35 36 37 38 40 41 42 43 44 45 46 47 48 50 51 52 53 54 55 56 57 58 60 61 62 63 64 65 66 67 68 70 71 72 73 74 75 76 77 78 80 81 82 83 84 85 86 87 88 90 91 92 93 94 95 96 97 98";

        string actual = string.Join(" ", allCoord.Select(coord => coord.RowCol));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestIccs()
    {
        string expected = "A0 B0 C0 D0 E0 F0 G0 H0 I0 A1 B1 C1 D1 E1 F1 G1 H1 I1 A2 B2 C2 D2 E2 F2 G2 H2 I2 A3 B3 C3 D3 E3 F3 G3 H3 I3 A4 B4 C4 D4 E4 F4 G4 H4 I4 A5 B5 C5 D5 E5 F5 G5 H5 I5 A6 B6 C6 D6 E6 F6 G6 H6 I6 A7 B7 C7 D7 E7 F7 G7 H7 I7 A8 B8 C8 D8 E8 F8 G8 H8 I8 A9 B9 C9 D9 E9 F9 G9 H9 I9";

        string actual = string.Join(" ", allCoord.Select(coord => coord.Iccs));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestComparer()
    {
        string expected = "(9,8)(9,7)(9,6)(9,5)(9,4)(9,3)(9,2)(9,1)(9,0)(8,8)(8,7)(8,6)(8,5)(8,4)(8,3)(8,2)(8,1)(8,0)(7,8)(7,7)(7,6)(7,5)(7,4)(7,3)(7,2)(7,1)(7,0)(6,8)(6,7)(6,6)(6,5)(6,4)(6,3)(6,2)(6,1)(6,0)(5,8)(5,7)(5,6)(5,5)(5,4)(5,3)(5,2)(5,1)(5,0)(4,8)(4,7)(4,6)(4,5)(4,4)(4,3)(4,2)(4,1)(4,0)(3,8)(3,7)(3,6)(3,5)(3,4)(3,3)(3,2)(3,1)(3,0)(2,8)(2,7)(2,6)(2,5)(2,4)(2,3)(2,2)(2,1)(2,0)(1,8)(1,7)(1,6)(1,5)(1,4)(1,3)(1,2)(1,1)(1,0)(0,8)(0,7)(0,6)(0,5)(0,4)(0,3)(0,2)(0,1)(0,0)";
        allCoord.Reverse(); // 逆序

        string actual = string.Concat(allCoord.Select(coord => coord.ToString()));

        Assert.Equal(expected, actual);
    }
}
