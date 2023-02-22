using System.Collections.Concurrent;
using System.Text;

namespace CChess;

public class Aspects
{
    public Aspects() { FENRowCols = new(); }

    public Aspects(Stream stream) : this()
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);
        int fenCount = reader.ReadInt32();
        for (int i = 0; i < fenCount; i++)
        {
            string fen = reader.ReadString();
            int rowColCount = reader.ReadInt32();
            Dictionary<string, List<int>> aspectData = new();
            for (int j = 0; j < rowColCount; j++)
            {
                string rowCol = reader.ReadString();
                int valueCount = reader.ReadInt32();
                List<int> valueList = new();
                for (int k = 0; k < valueCount; k++)
                    valueList.Add(reader.ReadInt32());

                aspectData.TryAdd(rowCol, valueList);
            }
            FENRowCols.TryAdd(fen, aspectData);
        }
    }

    public Aspects(List<Manual> manuals) : this()
    {
        manuals.ForEach(manual => Add(manual));
    }

    public Dictionary<string, Dictionary<string, List<int>>> FENRowCols { get; }

    public static Aspects GetAspects(string fileName)
    {
        if (!File.Exists(fileName))
            return new();

        using var stream = File.Open(fileName, FileMode.Open);
        return new(stream);
    }

    public void Write(string fileName)
    {
        using var stream = File.Open(fileName, FileMode.Create);
        DataStream.CopyTo(stream);
    }

    public Stream DataStream
    {
        get
        {
            MemoryStream stream = new();
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
            writer.Write(FENRowCols.Count);
            foreach (var fenData in FENRowCols)
            {
                writer.Write(fenData.Key);
                writer.Write(fenData.Value.Count);
                foreach (var aspectData in fenData.Value)
                {
                    writer.Write(aspectData.Key);
                    writer.Write(aspectData.Value.Count);
                    foreach (var x in aspectData.Value)
                        writer.Write(x);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }

    public List<(string rowCol, List<int> valueList)>? GetAspectData(string fen)
    {
        var (finded, findCt, findFen) = FindCtFens(fen);
        if (!finded)
            return null;

        return FENRowCols[findFen].Select(rowColValue
             => (Coord.GetRowCol(rowColValue.Key, findCt), rowColValue.Value)).ToList();
    }

    public void Add(Manual manual)
        => manual.GetFENRowCols().ForEach(fenRowCol => Join(fenRowCol));

    private void Join((string fen, string rowCol) fenRowCol)
    {
        var (fen, rowCol) = fenRowCol;
        Dictionary<string, List<int>> aspectData;
        var (finded, findCt, findFen) = FindCtFens(fen);
        if (finded)
            aspectData = FENRowCols[findFen];
        else
        {
            aspectData = new();
            FENRowCols.Add(fen, aspectData);
        }

        rowCol = Coord.GetRowCol(rowCol, findCt);
        if (aspectData.ContainsKey(rowCol))
        {
            aspectData[rowCol][0]++; // 第一项计数，列表可添加功能
        }
        else
            aspectData.TryAdd(rowCol, new List<int>() { 1 });
    }

    private (bool finded, ChangeType findCt, string findFen) FindCtFens(string fen)
    {
        if (FENRowCols.ContainsKey(fen))
            return (true, ChangeType.NoChange, fen);

        ChangeType ct = ChangeType.Symmetry_H;
        fen = Board.GetFEN(fen, ct);
        if (FENRowCols.ContainsKey(fen))
            return (true, ct, fen);

        return (false, ChangeType.NoChange, fen);
    }

    public bool Equal(Aspects aspects)
    {
        var akeys = FENRowCols.Keys;
        var bkeys = aspects.FENRowCols.Keys;
        var avalues = FENRowCols.Values;
        var bvalues = aspects.FENRowCols.Values;

        int count = akeys.Count;
        if (count != bkeys.Count || count != avalues.Count || count != bvalues.Count)
            return false;

        for (int i = 0; i < count; ++i)
        {
            if (akeys.ElementAt(i) != bkeys.ElementAt(i))
                return false;

            var aRcInts = avalues.ElementAt(i);
            var bRcInts = bvalues.ElementAt(i);
            var aRcKeys = aRcInts.Keys;
            var bRcKeys = bRcInts.Keys;
            var aRcValues = aRcInts.Values;
            var bRcValues = bRcInts.Values;

            int rcCount = aRcKeys.Count;
            if (rcCount != bRcKeys.Count || rcCount != aRcValues.Count || rcCount != bRcValues.Count)
                return false;

            for (int j = 0; j < rcCount; ++j)
            {
                if (aRcKeys.ElementAt(j) != bRcKeys.ElementAt(j))
                    return false;

                var aints = aRcValues.ElementAt(j);
                var bints = bRcValues.ElementAt(j);
                aints.Sort();
                bints.Sort();
                if (aints.Count != bints.Count)
                    return false;

                for (int k = 0; k < aints.Count; ++k)
                    if (aints[k] != bints[k])
                        return false;
            }
        }

        return true;
    }

    override public string ToString()
    {
        static string FenDataToString(KeyValuePair<string, Dictionary<string, List<int>>> fenRowcol,
              ParallelLoopState loop, string subString)
            => $"{subString}{fenRowcol.Key} [" +
                string.Concat(fenRowcol.Value.Select(rowcolData =>
                    $"{rowcolData.Key}({string.Concat(rowcolData.Value.Select(value => $"{value} ")).TrimEnd()}) ")).TrimEnd() + "]\n";

        // 非常有效地提升了速度! 
        BlockingCollection<string> subStringCollection = new();
        Parallel.ForEach(
            FENRowCols,
            () => "",
            FenDataToString,
            (finalSubString) => subStringCollection.Add(finalSubString));

        return string.Concat(subStringCollection);
    }
}

