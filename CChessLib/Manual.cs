using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CChess;

public enum FileExtType { xqf, cm, txt, pgnrc, pgniccs, pgnzh, }

public enum InfoKey
{
    source, title, game, date, site, black, rowCols, red, eccoSn, eccoName, win,
    opening, writer, author, type, version, FEN, moveString
}

public class Manual
{
    public Manual()
    {
        Info = new();
        ManualMove = new();
    }

    public Manual(Stream stream)
    {
        //文件标记'XQ'=$5158/版本/加密掩码/ProductId[4], 产品(厂商的产品号)
        // 棋谱评论员/文件的作者
        // 32个棋子的原始位置
        // 加密的钥匙和/棋子布局位置钥匙/棋谱起点钥匙/棋谱终点钥匙
        // 用单字节坐标表示, 将字节变为十进制, 十位数为X(0-8)个位数为Y(0-9),
        // 棋盘的左下角为原点(0, 0). 32个棋子的位置从1到32依次为:
        // 红: 车马相士帅士相马车炮炮兵兵兵兵兵 (位置从右到左, 从下到上)
        // 黑: 车马象士将士象马车炮炮卒卒卒卒卒 (位置从右到左,
        // 该谁下 0-红先, 1-黑先/最终结果 0-未知, 1-红胜 2-黑胜, 3-和棋
        // 从下到上)PlayStepNo[2],
        // 对局类型(开,中,残等)
        const int PIECENUM = 32;
        byte[] byteInfo = new byte[496];
        stream.Read(byteInfo, 0, 496);
        ReadOnlySpan<byte> Signature = byteInfo[0..2], ProductId = byteInfo[4..8],
            headQiziXY = byteInfo[16..48],
            PlayStepNo = byteInfo[48..50], PlayNodes = byteInfo[52..56],
            PTreePos = byteInfo[56..60], Reserved1 = byteInfo[60..64],
            headCodeA_H = byteInfo[64..80], TitleA = byteInfo[80..144], TitleB = byteInfo[144..208],
            Event = byteInfo[208..272], Date = byteInfo[272..288], Site = byteInfo[288..304],
            Red = byteInfo[304..320], Black = byteInfo[320..336],
            Opening = byteInfo[336..400], Redtime = byteInfo[400..416],
            Blktime = byteInfo[416..432], Reservedh = byteInfo[432..464],
            RMKWriter = byteInfo[464..480], Author = byteInfo[480..496]; //, Other[528]{}; 
        byte Version = byteInfo[2], headKeyMask = byteInfo[3],
            headKeyOrA = byteInfo[8], headKeyOrB = byteInfo[9], headKeyOrC = byteInfo[10], headKeyOrD = byteInfo[11],
            headKeysSum = byteInfo[12], headKeyXY = byteInfo[13], headKeyXYf = byteInfo[14], headKeyXYt = byteInfo[15],
            headWhoPlay = byteInfo[50], headPlayResult = byteInfo[51];

        if (Signature[0] != 0x58 || Signature[1] != 0x51)
            throw new Exception("文件标记不符。");
        if ((headKeysSum + headKeyXY + headKeyXYf + headKeyXYt) % 256 != 0)
            throw new Exception("检查密码校验和不对，不等于0。");
        if (Version > 18)
            throw new Exception("这是一个高版本的XQF文件，您需要更高版本的XQStudio来读取这个文件。");

        byte KeyXYf, KeyXYt;
        uint KeyRMKSize;
        byte[] F32Keys = new byte[PIECENUM];

        byte[] head_QiziXY = new byte[PIECENUM];
        headQiziXY.CopyTo(head_QiziXY);
        if (Version <= 10)
        {   // version <= 10 兼容1.0以前的版本
            KeyRMKSize = 0;
            KeyXYf = KeyXYt = 0;
        }
        else
        {
            byte KeyXY;
            byte __calkey(byte bKey, byte cKey)
            {
                // % 256; // 保持为<256
                return (byte)((((((bKey * bKey) * 3 + 9) * 3 + 8) * 2 + 1) * 3 + 8) * cKey);
            }

            KeyXY = __calkey(headKeyXY, headKeyXY);
            KeyXYf = __calkey(headKeyXYf, KeyXY);
            KeyXYt = __calkey(headKeyXYt, KeyXYf);
            KeyRMKSize = (uint)(((headKeysSum * 256 + headKeyXY) % 32000) + 767); // % 65536
            if (Version >= 12)
            {   // 棋子位置循环移动
                byte[] Qixy = new byte[PIECENUM];
                headQiziXY.CopyTo(Qixy);
                for (int i = 0; i != PIECENUM; ++i)
                    head_QiziXY[(i + KeyXY + 1) % PIECENUM] = Qixy[i];
            }
            for (int i = 0; i != PIECENUM; ++i)
                head_QiziXY[i] -= KeyXY; // 保持为8位无符号整数，<256
        }

        int[] KeyBytes = new int[]{
                    (headKeysSum & headKeyMask) | headKeyOrA,
                    (headKeyXY & headKeyMask) | headKeyOrB,
                    (headKeyXYf & headKeyMask) | headKeyOrC,
                    (headKeyXYt & headKeyMask) | headKeyOrD };
        string copyright = "[(C) Copyright Mr. Dong Shiwei.]";
        for (int i = 0; i != PIECENUM; ++i)
            F32Keys[i] = (byte)(copyright[i] & KeyBytes[i % 4]); // ord(c)

        // 取得棋子字符串
        StringBuilder pieceCharBuilder = new(new string('_', 90));
        const string pieChars = "RNBAKABNRCCPPPPPrnbakabnrccppppp"; // QiziXY设定的棋子顺序
        for (int i = 0; i != PIECENUM; ++i)
        {
            int xy = head_QiziXY[i];
            if (xy <= 89)
                // 用单字节坐标表示, 将字节变为十进制,
                // 十位数为X(0-8),个位数为Y(0-9),棋盘的左下角为原点(0, 0)
                pieceCharBuilder[(10 - 1 - xy % 10) * 9 + xy / 10] = pieChars[i];
        }
        string pieceChars = pieceCharBuilder.ToString();

        Info = new();
        // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding codec = Encoding.GetEncoding("gb2312"); // "gb2312"
        string[] result = { "未知", "红胜", "黑胜", "和棋" };
        string[] typestr = { "全局", "开局", "中局", "残局" };
        string GetInfoString(ReadOnlySpan<byte> name)
            => codec.GetString(name).Replace('\0', ' ').Trim();
        (new List<(InfoKey field, string value)>
            {
                (InfoKey.FEN, $"{Board.PieceCharsToFEN(pieceChars)} r - - 0 1"), // 可能存在不是红棋先走的情况？
                (InfoKey.version, Version.ToString()),
                (InfoKey.win, result[headPlayResult].Trim()),
                (InfoKey.type, typestr[headCodeA_H[0]].Trim()),
                (InfoKey.title, GetInfoString(TitleA)),
                (InfoKey.game, GetInfoString(Event)),
                (InfoKey.date, GetInfoString(Date)),
                (InfoKey.site, GetInfoString(Site)),
                (InfoKey.red, GetInfoString(Red)),
                (InfoKey.black, GetInfoString(Black)),
                (InfoKey.opening, GetInfoString(Opening)),
                (InfoKey.writer, GetInfoString(RMKWriter)),
                (InfoKey.author, GetInfoString(Author))
            }).ForEach(fieldValue
                => SetInfoValue(fieldValue.field, fieldValue.value));

        ManualMove = new(GetFEN(), stream, (Version, KeyXYf, KeyXYt, KeyRMKSize, F32Keys));
    }

    public Manual(byte[] bytes)
    {
        Info = new();
        using var reader = new BinaryReader(
            new MemoryStream(bytes), Encoding.UTF8, true);
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            string key = reader.ReadString();
            string value = reader.ReadString();
            Info[key] = value;
        }

        ManualMove = new(GetFEN(), bytes[(int)reader.BaseStream.Position..]);
    }

    public Manual(string manualString, FileExtType fileExtType = FileExtType.txt)
    {
        Info = new();
        int infoEndPos = manualString.IndexOf("\n\n");

        string infoString = manualString[..infoEndPos];
        var matches = Regex.Matches(infoString, @"\[(\S+) ""(.*)""\]");
        foreach (Match match in matches.Cast<Match>())
            Info[match.Groups[1].Value] = match.Groups[2].Value;


        var moveString = manualString[(infoEndPos + 2)..];
        ManualMove = new(GetFEN(), moveString, fileExtType);
    }

    public Manual(Dictionary<string, string> info)
    {
        Info = info;
        string moveString = GetInfoValue(InfoKey.moveString);
        ManualMove = (moveString.Length > 0
                ? new(GetFEN(), moveString, FileExtType.txt)
                : new(GetFEN(), GetInfoValue(InfoKey.rowCols)));
    }

    public Dictionary<string, string> Info { get; }
    public ManualMove ManualMove { get; }

    public static List<string> FileExtNames
        => Enumerable.Range(0, (int)FileExtType.pgnzh + 1)
            .Select(index => $".{((FileExtType)index)}").ToList();

    public static List<string> InfoKeys
        => Enumerable.Range(0, (int)InfoKey.moveString + 1)
            .Select(index => ((InfoKey)index).ToString()).ToList();

    public static Manual GetManual(string fileName)
    {
        if (!File.Exists(fileName))
            return new();

        using FileStream stream = File.OpenRead(fileName);
        FileExtType fileExtType = GetFileExtType(fileName);
        switch (fileExtType)
        {
            case FileExtType.xqf:
                return new(stream);
            case FileExtType.cm:
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Write(bytes);
                    return new(bytes);
                }
            default:
                {
                    string manualString = string.Empty;
                    using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
                    writer.Write(manualString);
                    return new(manualString, fileExtType);
                }
        }
    }

    public byte[] GetBytes()
    {
        MemoryStream stream = new();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write(Info.Count);
        foreach (var kv in Info)
        {
            writer.Write(kv.Key);
            writer.Write(kv.Value);
        }

        stream.Write(ManualMove.GetBytes());
        return stream.ToArray();
    }

    public static string GetInfoKey(InfoKey field) => InfoKeys[(int)field];
    public string GetInfoValue(InfoKey field)
        => Info.ContainsKey(GetInfoKey(field)) ? Info[GetInfoKey(field)] : string.Empty;

    public void SetInfoValue(InfoKey field, string value) => Info[GetInfoKey(field)] = value;

    private static FileExtType GetFileExtType(string fileName)
        => (FileExtType)(FileExtNames.IndexOf(Path.GetExtension(fileName)));

    public static string GetFileName(string fileName, FileExtType fileExtType)
        => $"{fileName}{FileExtNames[(int)fileExtType]}";

    private string GetFEN()
        => GetInfoValue(InfoKey.FEN).Split(' ')[0];

    private string GetInfoString()
        => string.Concat(Info.Select(keyValue
            => $"[{keyValue.Key} \"{keyValue.Value}\"]\n"));

    public string GetMoveString(FileExtType fileExtType = FileExtType.txt)
        => ManualMove.GetString(fileExtType);

    public string GetString(FileExtType fileExtType = FileExtType.txt)
        => $"{GetInfoString()}\n{GetMoveString(fileExtType)}";

    public string ToString(bool showMove = false, bool isOrder = false)
        => $"{GetInfoString()}{ManualMove.ToString(showMove, isOrder)}";

}