using System.Collections.Concurrent;
using System.Text;

namespace CChess;

internal class Aspects
{
    private readonly Dictionary<string, Dictionary<string, List<int>>> _aspectDict;

    public Aspects() { _aspectDict = new(); }
    public Aspects(string fileName) : this()
    {
        if(!File.Exists(fileName))
            return;

        using var stream = File.Open(fileName, FileMode.Open);
        using var reader = new BinaryReader(stream, Encoding.UTF8, false);
        int fenCount = reader.ReadInt32();
        for(int i = 0;i < fenCount;i++)
        {
            string fen = reader.ReadString();
            int rowColCount = reader.ReadInt32();
            Dictionary<string, List<int>> aspectData = new();
            for(int j = 0;j < rowColCount;j++)
            {
                string rowCol = reader.ReadString();
                int valueCount = reader.ReadInt32();
                List<int> valueList = new();
                for(int k = 0;k < valueCount;k++)
                    valueList.Add(reader.ReadInt32());

                aspectData.TryAdd(rowCol, valueList);
            }
            _aspectDict.TryAdd(fen, aspectData);
        }
    }
    public void Write(string fileName)
    {
        using var stream = File.Open(fileName, FileMode.Create);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, false);
        writer.Write(_aspectDict.Count);
        foreach(var fenData in _aspectDict)
        {
            writer.Write(fenData.Key);
            writer.Write(fenData.Value.Count);
            foreach(var aspectData in fenData.Value)
            {
                writer.Write(aspectData.Key);
                writer.Write(aspectData.Value.Count);
                foreach(var x in aspectData.Value)
                    writer.Write(x);
            }
        }
    }

    public void Add(string fileName)
    {
        Manual manual = new(fileName);
        foreach(var aspect in manual.GetAspects())
            Join(aspect);

        // 以下同步方式与非同步方式的耗时基本相同，同步字典并行运行时被锁定？
        //var aspectList = (new Manual(fileName)).GetAspects();
        //Parallel.ForEach<(string fen, int data), bool>(
        //    aspectList,
        //    () => true,
        //    (aspect, loop, x) => Join(aspect),
        //    (x) => x = true);
    }

    public List<(string rowCol, List<int> valueList)>? GetAspectData(string fen)
    {
        var (finded, findCt, findFen) = FindCtFens(fen);
        if(!finded)
            return null;

        return _aspectDict[findFen].Select(
            rowColValue => (Coord.GetRowCol(rowColValue.Key, findCt), rowColValue.Value)).ToList();
    }

    private bool Join((string fen, string rowCol) aspect)
    {
        var (fen, rowCol) = aspect;
        Dictionary<string, List<int>> aspectData;
        var (finded, findCt, findFen) = FindCtFens(fen);
        if(finded)
            aspectData = _aspectDict[findFen];
        else
        {
            aspectData = new();
            _aspectDict.Add(fen, aspectData);
        }

        if(findCt != ChangeType.NoChange)
            rowCol = Coord.GetRowCol(rowCol, findCt);

        if(aspectData.ContainsKey(rowCol))
        {
            aspectData[rowCol][0]++; // 第一项计数，列表可添加功能
        }
        else
            aspectData.TryAdd(rowCol, new List<int>() { 1 });

        return true;
    }

    private (bool finded, ChangeType findCt, string findFen) FindCtFens(string fen)
    {
        if(_aspectDict.ContainsKey(fen))
            return (true, ChangeType.NoChange, fen);

        ChangeType ct = ChangeType.Symmetry_H;
        fen = Board.GetFEN(fen, ct);
        if(_aspectDict.ContainsKey(fen))
            return (true, ct, fen);

        return (false, ChangeType.NoChange, fen);
    }

    override public string ToString()
    {
        static string FenDataToString(KeyValuePair<string, Dictionary<string, List<int>>> fenData,
              ParallelLoopState loop, string subString)
        {
            subString += fenData.Key + " [";
            foreach(var aspectData in fenData.Value)
            {
                subString += aspectData.Key + "(";
                foreach(var value in aspectData.Value)
                    subString += value.ToString() + ' ';

                subString = subString.TrimEnd() + ") ";
            }

            return subString.TrimEnd() + "]\n";
        }

        // 非常有效地提升了速度! 
        BlockingCollection<string> subStringCollection = new();
        Parallel.ForEach(
            _aspectDict,
            () => "",
            FenDataToString,
            (finalSubString) => subStringCollection.Add(finalSubString));

        return string.Concat(subStringCollection);
    }
}

