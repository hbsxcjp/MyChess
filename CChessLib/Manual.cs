using System.Text;
using System.Text.RegularExpressions;
//using System.Text.Encoding.CodePages;

namespace CChess;

internal enum FileExtType
{
    Xqf,
    Cm,
    Text,
    PGNRowCol,
    PGNIccs,
    PGNZh,
}

internal class Manual
{
    private readonly static List<string> FileExtName = new() { ".XQF", ".cm", ".text", ".pgnrc", ".pgniccs", ".pgnzh" };
    private const string FEN = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR";

    private readonly Dictionary<string, string> _info;
    private readonly ManualMove _manualMove;

    public Manual()
    {
        _info = new();
        _manualMove = new();
    }

    public Manual(Dictionary<string, string> info)
    {
        _info = info;
        _manualMove = new();
        string moveStrKey = Database.GetInfoKey(ManualField.MoveString);
        if(_info.ContainsKey(moveStrKey))
            _manualMove.SetFromString(_info[moveStrKey], FileExtType.Text);
        else
            _manualMove.SetFromRowCols(GetInfoValue(ManualField.RowCols));
    }
    public Manual(string fileName) : this()
    {
        FileExtType fileExtType = GetFileExtType(fileName);
        switch(fileExtType)
        {
            case FileExtType.Xqf:
                ReadXQF(fileName);
                break;
            case FileExtType.Cm:
                ReadCM(fileName);
                break;
            default:
                ReadText(fileName, fileExtType);
                return;
        }
    }

    public ManualMove ManualMove { get { return _manualMove; } }
    public Dictionary<string, string> Info { get { return _info; } }

    public void Write(string fileName)
    {
        FileExtType fileExtType = GetFileExtType(fileName);
        switch(fileExtType)
        {
            case FileExtType.Xqf:
                //ReadXQF(fileName);
                break;
            case FileExtType.Cm:
                WriteCM(fileName);
                break;
            default:
                WriteText(fileName, fileExtType);
                return;
        }
    }

    public void Reset()
    {
        _manualMove.BackStart();
        SetBoard();
    }

    public List<(string fen, string rowCol)> GetAspects() => _manualMove.GetAspects();
    public bool InfoHas(ManualField field) => _info.ContainsKey(Database.GetInfoKey(field));
    public string GetInfoValue(ManualField field) => _info[Database.GetInfoKey(field)];
    public void SetInfoValue(ManualField field, string value) => _info[Database.GetInfoKey(field)] = value;
    public void SetDatabaseField(string fileName)
    {
        SetInfoValue(ManualField.Source, fileName);
        SetInfoValue(ManualField.RowCols, _manualMove.GetRowCols());
        SetInfoValue(ManualField.MoveString, GetMoveString());
    }

    public void SetFromString(string manualString, FileExtType fileExtType = FileExtType.Text)
    {
        int infoEndPos = manualString.IndexOf("\n\n");
        SetInfo(manualString[..infoEndPos]);
        SetBoard();

        var moveString = manualString[(infoEndPos + 2)..];
        _manualMove.SetFromString(moveString, fileExtType);
    }
    public string GetString(FileExtType fileExtType = FileExtType.Text)
        => GetInfoString() + "\n" + GetMoveString(fileExtType);

    public string GetMoveString(FileExtType fileExtType = FileExtType.Text)
        => _manualMove.GetString(fileExtType);

    public string ToString(bool showMove = false, bool isOrder = false)
        => GetInfoString() + _manualMove.ToString(showMove, isOrder);

    private void ReadXQF(string fileName)
    {
        using FileStream stream = File.OpenRead(fileName);
        //文件标记'XQ'=$5158/版本/加密掩码/ProductId[4], 产品(厂商的产品号)
        const int PIECENUM = 32;
        byte[] Signature = new byte[3], Version = new byte[1], headKeyMask = new byte[1],
            ProductId = new byte[4], headKeyOrA = new byte[1],
            headKeyOrB = new byte[1], headKeyOrC = new byte[1], headKeyOrD = new byte[1],
            headKeysSum = new byte[1], headKeyXY = new byte[1], headKeyXYf = new byte[1], headKeyXYt = new byte[1],
            headQiziXY = new byte[PIECENUM],
            PlayStepNo = new byte[2], headWhoPlay = new byte[1], headPlayResult = new byte[1], PlayNodes = new byte[4],
            PTreePos = new byte[4], Reserved1 = new byte[4],
            headCodeA_H = new byte[16], TitleA = new byte[64], TitleB = new byte[64],
            Event = new byte[64], Date = new byte[16], Site = new byte[16], Red = new byte[16], Black = new byte[16],
            Opening = new byte[64], Redtime = new byte[16], Blktime = new byte[16], Reservedh = new byte[32],
            RMKWriter = new byte[16], Author = new byte[16]; //, Other[528]{}; 
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

        stream.Read(Signature, 0, 2);
        stream.Read(Version, 0, 1);
        stream.Read(headKeyMask, 0, 1);
        stream.Read(ProductId, 0, 4); // = 8 bytes
        stream.Read(headKeyOrA, 0, 1);
        stream.Read(headKeyOrB, 0, 1);
        stream.Read(headKeyOrC, 0, 1);
        stream.Read(headKeyOrD, 0, 1);
        stream.Read(headKeysSum, 0, 1);
        stream.Read(headKeyXY, 0, 1);
        stream.Read(headKeyXYf, 0, 1);
        stream.Read(headKeyXYt, 0, 1); // = 16 bytes
        stream.Read(headQiziXY, 0, PIECENUM); // = 48 bytes
        stream.Read(PlayStepNo, 0, 2);
        stream.Read(headWhoPlay, 0, 1);
        stream.Read(headPlayResult, 0, 1);
        stream.Read(PlayNodes, 0, 4);
        stream.Read(PTreePos, 0, 4);
        stream.Read(Reserved1, 0, 4); // = 64 bytes
        stream.Read(headCodeA_H, 0, 16);
        stream.Read(TitleA, 0, 64);
        stream.Read(TitleB, 0, 64);
        stream.Read(Event, 0, 64);
        stream.Read(Date, 0, 16);
        stream.Read(Site, 0, 16);
        stream.Read(Red, 0, 16); // = 320 bytes
        stream.Read(Black, 0, 16);
        stream.Read(Opening, 0, 64);
        stream.Read(Redtime, 0, 16);
        stream.Read(Blktime, 0, 16);
        stream.Read(Reservedh, 0, 32);
        stream.Read(RMKWriter, 0, 16); // = 480 bytes
        stream.Read(Author, 0, 16); // = 496 bytes

        if(Signature[0] != 0x58 || Signature[1] != 0x51)
            throw new Exception("文件标记不符。");
        if((headKeysSum[0] + headKeyXY[0] + headKeyXYf[0] + headKeyXYt[0]) % 256 != 0)
            throw new Exception("检查密码校验和不对，不等于0。");
        if(Version[0] > 18)
            throw new Exception("这是一个高版本的XQF文件，您需要更高版本的XQStudio来读取这个文件。");

        byte[] KeyXY = new byte[1], KeyXYf = new byte[1], KeyXYt = new byte[1],
            F32Keys = new byte[PIECENUM], head_QiziXY = new byte[PIECENUM];
        uint KeyRMKSize = 0;

        headQiziXY.CopyTo(head_QiziXY, 0);
        if(Version[0] <= 10)
        {   // version <= 10 兼容1.0以前的版本
            KeyRMKSize = 0;
            KeyXYf[0] = KeyXYt[0] = 0;
        }
        else
        {
            byte __calkey(byte bKey, byte cKey)
            {
                // % 256; // 保持为<256
                return (byte)((((((bKey * bKey) * 3 + 9) * 3 + 8) * 2 + 1) * 3 + 8) * cKey);
            }

            KeyXY[0] = __calkey(headKeyXY[0], headKeyXY[0]);
            KeyXYf[0] = __calkey(headKeyXYf[0], KeyXY[0]);
            KeyXYt[0] = __calkey(headKeyXYt[0], KeyXYf[0]);
            KeyRMKSize = (uint)(((headKeysSum[0] * 256 + headKeyXY[0]) % 32000) + 767); // % 65536
            if(Version[0] >= 12)
            {   // 棋子位置循环移动
                byte[] Qixy = new byte[PIECENUM];
                headQiziXY.CopyTo(Qixy, 0);
                for(int i = 0;i != PIECENUM;++i)
                    head_QiziXY[(i + KeyXY[0] + 1) % PIECENUM] = Qixy[i];
            }
            for(int i = 0;i != PIECENUM;++i)
                head_QiziXY[i] -= KeyXY[0]; // 保持为8位无符号整数，<256
        }
        int[] KeyBytes = new int[]{
                    (headKeysSum[0] & headKeyMask[0]) | headKeyOrA[0],
                    (headKeyXY[0] & headKeyMask[0]) | headKeyOrB[0],
                    (headKeyXYf[0] & headKeyMask[0]) | headKeyOrC[0],
                    (headKeyXYt[0] & headKeyMask[0]) | headKeyOrD[0] };
        string copyright = "[(C) Copyright Mr. Dong Shiwei.]";
        for(int i = 0;i != PIECENUM;++i)
            F32Keys[i] = (byte)(copyright[i] & KeyBytes[i % 4]); // ord(c)

        // 取得棋子字符串
        StringBuilder pieceChars = new(new string('_', 90));
        string pieChars = "RNBAKABNRCCPPPPPrnbakabnrccppppp"; // QiziXY设定的棋子顺序
        for(int i = 0;i != PIECENUM;++i)
        {
            int xy = head_QiziXY[i];
            if(xy <= 89)
                // 用单字节坐标表示, 将字节变为十进制,
                // 十位数为X(0-8),个位数为Y(0-9),棋盘的左下角为原点(0, 0)
                pieceChars[xy % 10 * 9 + xy / 10] = pieChars[i];
        }

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding codec = Encoding.GetEncoding("gb18030"); // "gb2312"
        string[] result = { "未知", "红胜", "黑胜", "和棋" };
        string[] typestr = { "全局", "开局", "中局", "残局" };
        SetInfoValue(ManualField.FEN, $"{Board.GetFEN(pieceChars.ToString())} r - - 0 1"); // 可能存在不是红棋先走的情况？
        SetInfoValue(ManualField.Version, Version[0].ToString());
        SetInfoValue(ManualField.Win, result[headPlayResult[0]].Trim());
        SetInfoValue(ManualField.Type, typestr[headCodeA_H[0]].Trim());
        ManualField[] fields = new ManualField[] { ManualField.Title , ManualField.Event,
                ManualField.Date, ManualField.Site, ManualField.Red, ManualField.Black,
                ManualField.Opening, ManualField.Writer, ManualField.Author};
        byte[][] fieldBytes = new byte[][] { TitleA, Event, Date, Site, Red, Black, Opening, RMKWriter, Author };
        for(int i = 0;i < fields.Length;i++)
            SetInfoValue(fields[i], codec.GetString(fieldBytes[i]).Replace('\0', ' ').Trim());
        SetBoard();

        byte __sub(byte a, byte b) { return (byte)(a - b); }; // 保持为<256

        void __readBytes(byte[] bytes, int size)
        {
            int pos = (int)stream.Position;
            stream.Read(bytes, 0, size);
            if(Version[0] > 10) // '字节解密'
                for(uint i = 0;i != size;++i)
                    bytes[i] = __sub(bytes[i], F32Keys[(pos + i) % 32]);
        }

        uint __getRemarksize()
        {
            byte[] clen = new byte[4];
            __readBytes(clen, 4);
            return (uint)(clen[0] + (clen[1] << 8) + (clen[2] << 16) + (clen[3] << 24)) - KeyRMKSize;
            //if(BitConverter.IsLittleEndian)
            //    Array.Reverse(clen);
            //return BitConverter.ToUInt32(clen, 0) - KeyRMKSize;
        };

        byte[] data = new byte[4];
        byte frc = data[0], trc = data[1], tag = data[2];
        string? __readDataAndGetRemark()
        {
            __readBytes(data, 4);
            uint RemarkSize = 0;
            frc = data[0];
            trc = data[1];
            tag = data[2];
            if(Version[0] <= 10)
            {
                tag = (byte)((((tag & 0xF0) != 0) ? 0x80 : 0) | (((tag & 0x0F) != 0) ? 0x40 : 0));
                RemarkSize = __getRemarksize();
            }
            else
            {
                tag &= 0xE0;
                if((tag & 0x20) != 0)
                    RemarkSize = __getRemarksize();
            }

            if(RemarkSize == 0)
                return null;

            // # 有注解
            byte[] rem = new byte[2048 * 2];
            __readBytes(rem, (int)RemarkSize);
            var remark = codec.GetString(rem).Replace('\0', ' ').Replace("\r\n", "\n").Trim();
            return remark.Length > 0 ? remark : null;
        }

        stream.Seek(1024, SeekOrigin.Begin);
        _manualMove.CurRemark = __readDataAndGetRemark();

        if((tag & 0x80) == 0) // 无左子树
            return;

        // 有左子树
        Stack<Move> beforeMoves = new();
        beforeMoves.Push(_manualMove.CurMove);
        bool isOther = false;
        // 当前棋子为根，且有后继棋子时，表明深度搜索已经回退到根，已经没有后续棋子了
        while(!(_manualMove.CurMove.Before == null && _manualMove.CurMove.HasAfter))
        {
            var remark = __readDataAndGetRemark();
            //# 一步棋的起点和终点有简单的加密计算，读入时需要还原

            int fcolrow = __sub(frc, (byte)(0X18 + KeyXYf[0])),
                tcolrow = __sub(trc, (byte)(0X20 + KeyXYt[0]));
            if(fcolrow > 89 || tcolrow > 89)
                throw new Exception("fcolrow > 89 || tcolrow > 89 ? ");

            int frow = fcolrow % 10, fcol = fcolrow / 10, trow = tcolrow % 10,
                tcol = tcolrow / 10;

            CoordPair coordPair = _manualMove.GetCoordPair(frow, fcol, trow, tcol);
            bool hasNext = (tag & 0x80) != 0, hasOther = (tag & 0x40) != 0;

            var curCoordPair = _manualMove.CurMove.CoordPair;
            if(curCoordPair.FromCoord.row == frow && curCoordPair.FromCoord.col == fcol
                && curCoordPair.ToCoord.row == trow && curCoordPair.ToCoord.col == tcol)
            {
                Console.WriteLine("Error: " + fileName + coordPair.ToString() + _manualMove.CurRemark);
            }
            else
            {
                if(isOther)
                    _manualMove.Back();
                _manualMove.AddMove(coordPair, remark, true);
                //Console.WriteLine("_manualMove.CurMove: " + _manualMove.CurMove.ToString());

                if(hasNext && hasOther)
                    beforeMoves.Push(_manualMove.CurMove);

                isOther = !hasNext;
                if(isOther && !hasOther && beforeMoves.Count > 0)
                {
                    var beforeMove = beforeMoves.Pop(); // 最后时，将回退到根
                    while(beforeMove != _manualMove.CurMove)
                        _manualMove.Back();
                }
            }
        }

        _manualMove.ClearError(); // 清除XQF带来的错误着法
    }
    private void ReadCM(string fileName)
    {
        if(!File.Exists(fileName))
            return;

        using var stream = File.Open(fileName, FileMode.Open);
        using var reader = new BinaryReader(stream, Encoding.UTF8, false);
        int count = reader.ReadInt32();
        for(int i = 0;i < count;i++)
        {
            string key = reader.ReadString();
            string value = reader.ReadString();
            _info[key] = value;
        }
        SetBoard();

        _manualMove.ReadCM(reader);
    }
    private void WriteCM(string fileName)
    {
        using var stream = File.Open(fileName, FileMode.Create);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, false);

        writer.Write(_info.Count);
        foreach(var kv in _info)
        {
            writer.Write(kv.Key);
            writer.Write(kv.Value);
        }

        _manualMove.WriteCM(writer);
    }

    private void ReadText(string fileName, FileExtType fileExtType)
    {
        if(!File.Exists(fileName))
            return;

        using var stream = File.Open(fileName, FileMode.Open);
        using var reader = new StreamReader(stream);
        string text = reader.ReadToEnd();
        SetFromString(text, fileExtType);
    }
    private void WriteText(string fileName, FileExtType fileExtType)
    {
        using var stream = File.Open(fileName, FileMode.Create);
        using var writer = new StreamWriter(stream);
        writer.Write(GetString(fileExtType));
    }

    public static string GetExtName(FileExtType fileExtType) => FileExtName[(int)fileExtType];
    private static FileExtType GetFileExtType(string fileName)
        => (FileExtType)(FileExtName.IndexOf(new FileInfo(fileName).Extension));
    private bool SetBoard() => _manualMove.SetBoard(InfoHas(ManualField.FEN) ? GetInfoValue(ManualField.FEN) : FEN);

    private void SetInfo(string infoString)
    {
        var matches = Regex.Matches(infoString, @"\[(\S+) ""(.*)""\]");
        foreach(Match match in matches.Cast<Match>())
            _info[match.Groups[1].Value] = match.Groups[2].Value;
    }
    private string GetInfoString()
    {
        string result = "";
        foreach(var item in _info)
            result += "[" + item.Key + " \"" + item.Value + "\"]\n";

        return result;
    }
}

