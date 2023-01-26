using CChess;

namespace CChessTest;

public class AspectTest
{
    private static List<Manual> GetManuals()
    {
        List<Manual> manuals = new();
        foreach (string fileName in new string[] {
            "01",
            "4四量拨千斤",
            "第09局",
            "布局陷阱--飞相局对金钩炮",
            "- 北京张强 (和) 上海胡荣华 (1993.4.27于南京)"})
            manuals.Add(new(Manual.GetFileName(fileName, FileExtType.Xqf)));

        return manuals;
    }

    [Fact]
    public void TestManualsAspects()
    {
        Aspects aspects = new(GetManuals());

        MemoryStream stream = new();
        aspects.SetStream(stream);
        stream.Seek(0, SeekOrigin.Begin);

        Aspects aspects2 = new();
        aspects2.SetFromStream(stream);

        Assert.True(aspects.Equal(aspects2));
    }
}
