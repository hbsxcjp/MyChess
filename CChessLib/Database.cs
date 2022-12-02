#define WRITERESULTTEXT

using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace CChess;

internal enum ManualField
{
    Source, Title, Event, Date, Site, Black, RowCols, Red, EccoSn, EccoName, Win,
    Opening, Writer, Author, Type, Version, FEN, MoveString
}
internal class Database
{
    private static readonly string[] _infoKeys = {
            "source", "title", "event", "date", "site", "black", "rowCols", "red", "eccoSn", "eccoName", "win",
            "opening", "writer", "author", "type", "version", "FEN", "moveString" };
    private readonly string _manualTableName = "manual";

    public List<Manual> GetManuals(string condition = "1")
    {
        static Dictionary<string, string> GetInfo(SqliteDataReader reader)
        {
            Dictionary<string, string> info = new();
            for(int index = 0;index < reader.FieldCount;++index)
                if(!reader.IsDBNull(index))
                    info[reader.GetName(index)] = reader.GetString(index);

            return info;
        }

        List<Manual> manuals = new();
        using SqliteConnection connection = GetSqliteConnection();
        SqliteCommand command = new(ManualSelectCommandText(condition), connection);
        using SqliteDataReader reader = command.ExecuteReader();
        while(reader.Read())
            manuals.Add(new(GetInfo(reader)));

        return manuals;
    }

    public void StorageFileManual(IEnumerable<string> fileNames)
    {
        IEnumerable<Manual> manuals = fileNames.Select(fileName =>
        {
            Manual manual = new(fileName);
            manual.SetDatabaseField(fileName);
            return manual;
        });

        InsertInfoList(manuals.Select(manual => manual.Info));
    }

    //总界限:1~12141
    public void DownXqbaseManual(int start = 1, int end = 5)
    {
        const int XqbaseInfoCount = 11;
        using HttpClient client = new();
        Encoding codec = Encoding.GetEncoding("gb2312");
        Dictionary<string, string> GetInfo(string uri)
        {
            string pattern = @"<title>(.*?)</title>.*?>([^>]+赛[^>]*?)<.*?>(\d+年\d+月(?:\d+日)?)(?: ([^<]*?))?<.*?>黑方 ([^<]*?)<.*?MoveList=(.*?)"".*?>红方 ([^<]*?)<.*?>([A-E]\d{2})\. ([^<]*?)<.*\((.*?)\)</pre>";
            var taskA = client.GetByteArrayAsync(uri);
            Match match = Regex.Match(codec.GetString(taskA.Result), pattern, RegexOptions.Singleline);
            if(!match.Success)
                return new();

            // "source", "title", "event", "date", "site", "black", "rowCols", "red", "eccoSn", "eccoName", "win"
            Dictionary<string, string> info = new() { { GetInfoKey(ManualField.Source), uri } };
            for(int i = 1;i < XqbaseInfoCount;i++)
                info[GetInfoKey((ManualField)i)] = i != 6 ? match.Groups[i].Value
                    : Coord.RowCols(match.Groups[i].Value.Replace("-", "").Replace("+", ""));

            return info;
        }

        Task<Dictionary<string, string>>[] taskArray = new Task<Dictionary<string, string>>[end - start + 1];
        for(int i = 0;i < taskArray.Length;i++)
        {
            string uri = string.Format(@"https://www.xqbase.com/xqbase/?gameid={0}", i + start);
            taskArray[i] = Task<Dictionary<string, string>>.Factory.StartNew(() => GetInfo(uri));
        }
        Task.WaitAll(taskArray);
        InsertInfoList(taskArray.Select(task => task.Result));
    }

    public static void InitEccoData()
    {
#if WRITERESULTTEXT
        using StreamWriter sw = File.CreateText("TestEccoData.text");
#endif
        static string DownHtmls()
        {
            using HttpClient client = new();
            Encoding codec = Encoding.GetEncoding("gb2312");
            string eccoChars = "abcde";
            Task<string>[] taskArray = new Task<string>[eccoChars.Length];
            for(int i = 0;i < taskArray.Length;i++)
            {
                string uri = string.Format(@"https://www.xqbase.com/ecco/ecco_{0}.htm", eccoChars[i]);
                taskArray[i] = Task<string>.Factory.StartNew(() =>
                {
                    var taskA = client.GetByteArrayAsync(uri);
                    var html = Regex.Replace(codec.GetString(taskA.Result),
                        @"</?(?:div|font|img|strong|center|meta|dl|dt|table|tr|td|em|p|li|dir|html|head|body|title|a|b|tbody|script|br|span)[^>]*>", "");

                    return Regex.Replace(html, @"(?:\n|\r)\s+", " ");
                });
            }
            Task.WaitAll(taskArray);
            return string.Join("", taskArray.Select(task => task.Result));
        }

        static SortedDictionary<string, string[]> GetEccoRecords(string htmls)
        {
            Regex[] regs = new Regex[] {
                    new(@"([A-E])．(\S+?)\((共[\s\S]+?局)\)"),
                    new(@"([A-E]\d)(?:．|\. )(?:空|([\S^\r]+?)\((共[\s\S]+?局)\)\s+([\s\S]*?))(?=[\s　]*[A-E]\d0．)"),
                    new(@"([A-E]\d{2})．(\S+)[\s　]+(?:(?![A-E]\d|上一)([\s\S]*?)[\s　]*(无|共[\s\S]+?局)[\s\S]*?(?=上|[A-E]\d{0,2}．))?"),
                    new(@"([A-E]\d)\d局面 =([\s\S]*?)(?=[\s　]*[A-E]\d{2}．)"),
                };

            SortedDictionary<string, string[]> records = new();
            for(int gindex = 0;gindex < regs.Length;++gindex)
            {
                var matches = regs[gindex].Matches(htmls);
                foreach(Match match in matches.Cast<Match>())
                {
                    if(gindex == 3)
                    {
                        // C20 C30 C61 C72局面字符串, 供设置前置着法字符串
                        records[match.Groups[1].Value][3] = match.Groups[2].Value;
                    }
                    else
                    {
                        // field: SN_I, NAME_I, NUMS_I, MVSTRS_I, PRE_MVSTRS_I, REGSTR_I
                        string[] record = new string[] { "", "", "", "", "", "" };
                        for(int i = 0;i < match.Groups.Count - 1;i++)
                            record[(gindex == 2 && i > 1) ? (i == 3 ? 2 : 3) : i] = match.Groups[i + 1].Value;

                        records[match.Groups[1].Value] = record;
                    }
                }
            }

            foreach(var record in records)
            {
                string sn = record.Value[0], mvstrs = record.Value[3];
                if(sn.Length != 3 || mvstrs.Length < 3)
                    continue;

                string sn_pre = "";
                if(mvstrs[0] == '从')
                    // 三级局面的 C2 C3_C4 C61 C72局面 有40项
                    sn_pre = mvstrs[1..3];
                else if(mvstrs[0] != '1')
                {
                    // 前置省略的着法 有75项
                    sn_pre = sn[..2]; //  截断为两个字符长度
                    if(sn_pre[0] == 'C')
                        sn_pre = "C0"; // C0/C1/C5/C6/C7/C8/C9 => C0
                    else if(sn_pre == "D5")
                        sn_pre = "D51"; // D5 => D51
                }
                else
                    continue;

                record.Value[4] = sn_pre;
            }

            return records;
        }

        void SetEccoRecordRegstr(SortedDictionary<string, string[]> records)
        {
            char UnorderChar = '|', OrderChar = '*', SeparateChar = '/', OroneChar = '?', ExceptChar = '除';

            // 获取回合着法字符串
            void SetBoutStrs(SortedDictionary<char, List<string>> boutStrs, string sn, string mvstrs, bool isPreMvstrs)
            {
                // 删除此前已加入的相同着法
                //void DeleteSameMvstr(int color, string qstr)
                //{
                //    foreach(var boutNo in boutStrs.Keys)
                //    {
                //        string mvstrs = boutStrs[boutNo][color];
                //        int orderPos = mvstrs.LastIndexOf(OrderChar), mvstrPos = mvstrs.IndexOf(qstr);
                //        // 存在相同着法，且位于最后顺序标记位之前
                //        if(mvstrPos != -1 && mvstrPos < orderPos)
                //            boutStrs[boutNo][color] = mvstrs.Replace(qstr, "");
                //    }
                //}

                // 处理回合着法
                void HandleMvstr(char boutNo, int color, string qstr)
                {
                    if((sn == "A53" && color == 1 && boutNo == '2')
                        || (sn == "C98" && boutNo == 'n' && !isPreMvstrs))
                        return;

                    // B21 棋子文字错误
                    if(sn == "B21" && !isPreMvstrs)
                    {
                        if(qstr.Contains("象七进九"))
                            qstr = qstr.Replace('象', '相');
                        else if(qstr.Contains("车８进５"))
                            boutNo = 'r';
                    }
                    // C46 优化黑色，根据实例微调红色
                    else if(boutNo == '9' && sn == "C46")
                    {
                        string insertMv = color == 1 ? "炮９平７" : "车九平八";
                        qstr = insertMv + OrderChar + qstr;
                        if(color == 1)
                            boutStrs['8'][color] = "";
                    }
                    else if(sn == "D01" && boutNo == '2' && color == 1
                        && boutStrs.ContainsKey(boutNo) && boutStrs[boutNo][color].Length > 0)
                    {
                        boutStrs[boutNo][color] += OroneChar;
                        return;
                    }
                    else if((new string[] { "D03", "D04", "D05" }).Contains(sn)
                        && color == 0 && boutNo == '2' && boutStrs.ContainsKey(boutNo))
                    {
                        boutStrs[boutNo][color] = (qstr += OroneChar);
                        return;
                    }
                    else if((new string[] { "D23", "D24", "D26", "D27" }).Contains(sn)
                                    && boutNo == '4' && boutStrs.ContainsKey(boutNo))
                    {
                        boutStrs[boutNo][color] += OroneChar;
                        return;
                    }
                    else if(sn == "D34" && !isPreMvstrs)
                        boutNo = '5';

                    //            if (qstr.length() == MOVESTR_LEN)
                    //                deleteSameMvstr_(color, qstr);

                    // 已存在序号为boutNo的着法字符串
                    if(boutStrs.ContainsKey(boutNo))
                    {
                        string mvstr = boutStrs[boutNo][color];
                        // 已存在着法字符串尾部不匹配待处理字符串
                        if(!mvstr.EndsWith(qstr))
                        {
                            if(mvstr.Length > 0)
                            {
                                // 最后顺序着法字符串的长度等于待处理字符串长度，则添加无顺序字符标记；否则，添加有顺序标记
                                bool isUnorder = mvstr.Length - (mvstr.LastIndexOf(OrderChar) + 1) == qstr.Length;
                                // D41 着法m添加时需有顺序
                                if(sn == "D41" && !isPreMvstrs && boutNo == 'm')
                                    isUnorder = false;

                                boutStrs[boutNo][color] += isUnorder ? SeparateChar : OrderChar;
                            }
                            boutStrs[boutNo][color] += qstr;
                        }
                    }
                    else
                    {
                        List<string> mvstrs = new() { "", "" };
                        mvstrs[color] = qstr;
                        boutStrs.Add(boutNo, mvstrs);
                        // B32 黑棋着法有误
                        if(sn == "B32" && boutNo == '3')
                        {
                            boutStrs['1'][1] = "马２进３";
                            boutStrs[boutNo][1] = "马８进７";
                        }
                    }
                }

                // 处理不分先后标记
                void HandleUnorder(char boutNo, int color)
                {
                    bool isStart = true;
                    while(boutStrs.ContainsKey(boutNo))
                    {
                        var mvstrsList = boutStrs[boutNo];
                        for(int i = 0;i < mvstrsList.Count;++i)
                        {
                            if(mvstrsList[i].Length > 0)
                                mvstrsList[i] += UnorderChar;
                            if(isStart && color == 0)
                                break;
                        }

                        boutNo = (char)(boutNo - 1);
                        isStart = false;
                    }
                }

                // 处理"此前..."着法描述字符串
                void InsertMvstr(char boutNo, int color, string qstr)
                {
                    if(color == 0)
                    {
                        if(sn == "B40")
                            qstr += "车一平二";
                        else if(sn == "D43")
                            qstr += "马二进三";
                    }
                    else
                    {
                        if(!isPreMvstrs && sn == "D41")
                            color = 0;
                        // D29 先处理红棋着法 (此前红可走马八进七，黑可走马２进３、车９平４)
                        else if(sn == "D29")
                        {
                            string redStr = qstr[..4];
                            qstr = qstr[8..];
                            InsertMvstr(boutNo, 0, redStr);
                        }
                    }

                    while(boutStrs.ContainsKey(boutNo))
                    {
                        string mvstr = boutStrs[boutNo][color];
                        if(mvstr.Length > 0 && !mvstr.Contains(qstr))
                            boutStrs[boutNo][color] = qstr + OrderChar + mvstr;

                        boutNo = (char)(boutNo - 1);
                    }
                }

                // 捕捉一步着法: 1.着法，2.可能的“不分先后”标识，3.可能的“此前...”着法
                string mv = @"[" + Piece.PGNZHChars() + @"]{4}",
                    moveStr = @"(?:" + mv + @"(?:／" + mv + @")*)",
                    premStr = @"(?:" + mv + @"(?:[、／和，黑可走]{1,4}" + mv + @")*)",
                    rich_mvStr = "(" + moveStr + @"|……)(\s?\(不.先后[)，])?(?:[^\da-z]*?此前.*?走((?:过除)?" + premStr + @").*?\))?";

                // 捕捉一个回合着法：1.序号，2-4.首着、“不分先后”、“此前...”，5-7.着法、“不分先后”、“此前...”
                var matches = Regex.Matches(mvstrs, @"([\da-z]).\s?" + rich_mvStr + @"(?:，" + rich_mvStr + @")?");
                foreach(var match in matches.Cast<Match>())
                {
                    char boutNo = match.Groups[1].Value[0];
                    if(isPreMvstrs && sn.StartsWith("C9"))
                        boutNo = char.ToUpper(boutNo);
                    for(int i = 2;i < match.Groups.Count;++i)
                    {
                        string qstr = match.Groups[i].Value;
                        if(qstr.Length < 1 || qstr == "……")
                            continue;

                        int color = i / 5, capIndex = (i - 2) % 3;
                        switch(capIndex)
                        {
                            case 0: // i == 2 || i == 5 // 处理回合着法
                                HandleMvstr(boutNo, color, qstr);
#if WRITERESULTTEXTa
                                    sw.WriteLine(string.Format($"\tBoutMoveStr:{qstr}")); // BoutMoveStr:1520
#endif
                                break;
                            case 1: // i == 3 || i == 6 // "不分先后"
                                HandleUnorder(boutNo, color);
#if WRITERESULTTEXTa
                                    sw.WriteLine(string.Format($"\tNotOrder:{qstr}")); // NotOrder:26
#endif
                                break;
                            default: // i == 4 || i == 7 // "此前..."
                                InsertMvstr(boutNo, color, qstr);
#if WRITERESULTTEXTa
                                    // PreInsertBout:53个     \tInsertBout:103个
                                    sw.WriteLine(string.Format($"\t{(isPreMvstrs ? "Pre" : "")}InsertBout:{qstr}"));
#endif
                                break;
                        }
                    }
                }
            }

            string GetRegstr(SortedDictionary<char, List<string>> boutStrs, string sn, Manual manual)
            {
                // 处理不分先后标记（最后回合，或本回合已无标记时调用）
                void HandleUnorderChar(ref string groupRowcols, ref int boutNotOrderCount)
                {
                    // regStr的最后字符有标记，则去掉最后一个标记
                    if(groupRowcols.Length > 0 && groupRowcols.Last() == UnorderChar)
                        groupRowcols = groupRowcols[..(groupRowcols.Length - 1)];
                    // 如果存在有一个以上标记，则包裹处理
                    if(boutNotOrderCount > 1)
                        groupRowcols = $"(?:{groupRowcols}){{{boutNotOrderCount}}}";

                    boutNotOrderCount = 0;
                }

                string GetRowcol(string zhStr, bool isGo)
                {
                    Dictionary<string, string> zhStr_preZhStr = new(){
                        { "车一平二", "马二进三" }, { "车二进六", "车一平二" },
                        { "车八进四", "车九平八" }, { "车九平八", "马八进七" },
                        { "马八进七", "马七退八" }, { "马七进六", "兵七进一" },
                        { "马三进四", "兵三进一" }, { "马二进三", "马三退二" },
                        { "炮八平七", "卒３进１" }, { "车８进５", "炮８平９" },
                        { "车９平８", "马８进７" }, { "炮８进２", "车二退六" },
                        { "炮８平９", "车９平８" }
                    };

                    bool success = manual.ManualMove.AddMove(zhStr);
                    if(success)
                    {
                        if(!isGo)
                            manual.ManualMove.Back();
                    }
                    else
                    {
                        List<string> preZhStrs = new() { zhStr };
                        while(!success && zhStr_preZhStr.ContainsKey(preZhStrs.First()))
                        {
                            string preZhStr = zhStr_preZhStr[preZhStrs.First()];
#if WRITERESULTTEXT
                            sw.Write($"\t\tpremv:{preZhStr}");
#endif
                            success = manual.ManualMove.AddMove(preZhStr);
                            preZhStrs.Insert(0, preZhStr);
                        }

                        if(success)
                        {
                            for(int i = 1;i < preZhStrs.Count;++i)
                                success = manual.ManualMove.AddMove(preZhStrs[i]);
                            for(int i = 0;i < preZhStrs.Count;++i)
                                manual.ManualMove.Back();
                        }
                    }

                    string rowCol = success ? manual.ManualMove.CurMove.CoordPair.RowCol : "";
#if WRITERESULTTEXT
                    sw.Write($"{(success ? "失败" : "OK ")}:{zhStr} {isGo}: {rowCol}");
#endif

                    return rowCol;
                }

                List<string> GetRowcolList(string mvstr, bool isOrder)
                {
                    string reg_mv = @$"([{Piece.PGNZHChars()}]{4})",
                        reg_UnOrderMv = "马二进一/车九平八|马二进三/马八进七|马八进七／马八进九|炮八平六/马八进九|" +
                            "兵三进一.兵七进一|相三进五／相七进五|仕四进五／仕六进五|马８进７/车９进１|" +
                            "马８进７/马２进３|炮８进４/炮２进４|炮８平５／炮２平５|炮８平６／炮２平４／炮８平４／炮２平６|" +
                            "卒３进１/马２进３|象３进５／象７进５|象７进５／象３进５|士６进５／士４进５";

                    List<string> rowcolsList = new();
                    var someMatch = Regex.Match(mvstr, reg_UnOrderMv);
                    bool isFirst = true, isUnorderMvstr = someMatch.Success;
                    var matches = Regex.Matches(mvstr, reg_mv);
                    foreach(Match match in matches.Cast<Match>())
                    {
                        // 着法非强制按顺序，且非前进着法，且为首着，则为变着
                        bool isGo = isOrder || (!isUnorderMvstr && isFirst);
                        string zhStr = match.Groups[1].Value;
                        string rowcol = GetRowcol(zhStr, isGo);
                        if(rowcol.Length > 0)
                            rowcolsList.Add(rowcol);

                        isFirst = false;
                    }

                    return rowcolsList;
                }

                // 处理有顺序的的连续着法标记
                string GetColorRowcol(string mvstrs, string moveRegStr)
                {
                    string colorRowcols = "";
                    var mvstrArray = mvstrs.Split(OrderChar);
                    int count = mvstrArray.Length;
                    for(int i = 0;i < count;++i)
                    {
                        var mvstr = mvstrArray[i];
                        // 包含"红/黑方："的前置着法，强制按顺序走棋
                        bool isOrder = mvstr.Contains("方：");
                        var rowcolsList = GetRowcolList(mvstr, isOrder);
                        int num = rowcolsList.Count;
                        string rowcols = string.Join(UnorderChar, rowcolsList),
                            indexStr = isOrder ? "" : (i == count - 1 ? "1," : "0,"); // 是否本回合的最后顺序着法
                        if(mvstr.Last() == OroneChar)
                            rowcols = @"(?:" + rowcols + @")?";
                        // 着法数量大于1，或属于多顺序着法的非最后着法
                        else if(num > 1 || (count > 1 && i != count - 1))
                            // B30 "走过除...以外的着法" // =>(?!%1) 否定顺序环视 // 见《精通正则表达式》P66.
                            rowcols = (mvstr.Contains(ExceptChar)
                                    ? moveRegStr + @"*?"
                                    : @"(?:" + rowcols + "){" + indexStr + num.ToString() + "}");

                        // 按顺序添加着法组
                        colorRowcols += rowcols;
                    }

                    return colorRowcols;
                }

                char preBoutNo = '1';
                string moveRegStr = @"(?:\d{4})";
                int[] boutNotOrderCount = new int[] { 0, 0 };
                string[] regStr = new string[] { "", "" };
                string[] boutGroupRowcols = new string[] { "", "" };
                foreach(char boutNo in boutStrs.Keys)
                {
                    for(int color = 0;color < 2;++color)
                    {
                        string mvstrs = boutStrs[boutNo][color];
                        if(mvstrs.Length > 0)
                        {
                            bool endIsUnorderChar = mvstrs.Last() == UnorderChar;
                            // 处理不分先后标记（本回合已无标记）
                            if(!endIsUnorderChar)
                                HandleUnorderChar(ref boutGroupRowcols[color], ref boutNotOrderCount[color]);

                            // 取得某种颜色的着法结果
                            string colorRowcols = GetColorRowcol(mvstrs, moveRegStr);
                            // 处理不分先后标记（本回合有标记）
                            if(endIsUnorderChar)
                            {
                                colorRowcols += UnorderChar;
                                ++boutNotOrderCount[color];
                            }

                            boutGroupRowcols[color] += colorRowcols;
                        }

                        // 处理不分先后标记（最后回合）
                        if(boutNo == boutStrs.Last().Key)
                            HandleUnorderChar(ref boutGroupRowcols[color], ref boutNotOrderCount[color]);

                        // 存入结果字符串
                        if(boutNotOrderCount[color] == 0 && boutGroupRowcols[color].Length > 0)
                        {
                            // 回合序号有跳跃
                            if(boutNo - preBoutNo > 1 && sn == "B45")
                                regStr[color] += moveRegStr + "?";

                            regStr[color] += boutGroupRowcols[color];
                            boutGroupRowcols[color] = "";
                        }
                    }

                    preBoutNo = boutNo;
                }

                return "~" + regStr[0] + ")" + moveRegStr + @"*?-" + regStr[1] + "}";
            }

            Manual manual = new();
            // 设置正则字符串
            foreach(var record in records)
            {
                string sn = record.Value[0], mvstrs = record.Value[3];
                if(sn.Length != 3 || mvstrs.Length < 1)
                    continue;

                SortedDictionary<char, List<string>> boutStrs = new();
                // 处理前置着法字符串
                string pre_sn = record.Value[4];
                if(pre_sn.Length > 0)
                {
                    string pre_mvstrs = records[pre_sn][3];
                    var match = Regex.Match(pre_mvstrs, @"(红方：.+)\s+(黑方：.+)");
                    if(match.Success)
                    {
                        string redStr = match.Groups[1].Value,
                           blackStr = match.Groups[2].Value;
#if WRITERESULTTEXTa
                            sw.WriteLine(string.Format($"\tRedBlack@:{redStr}{blackStr}"));
#endif
                        // 处理前置着法描述字符串————"红方：黑方：" // RedBlack@:40个
                        boutStrs.Add('1', new List<string> { redStr, blackStr });
                    }
                    else
                    {
#if WRITERESULTTEXTa
                            sw.WriteLine(string.Format($"\tOterPre@:{pre_mvstrs}"));
#endif
                        // 处理其他的前置着法描述字符串 // OterPre@:75个
                        SetBoutStrs(boutStrs, sn, pre_mvstrs, true);
                    }
                }

                // 处理着法字符串
                SetBoutStrs(boutStrs, sn, mvstrs, false);
#if WRITERESULTTEXTa
                    foreach(var b in boutStrs)
                        sw.WriteLine(string.Format($"\t{b.Key}. {string.Join("\t\t", b.Value)}"));
#endif

                // 设置着法正则描述字符串
                manual.Reset();
                record.Value[5] = GetRegstr(boutStrs, sn, manual);
#if WRITERESULTTEXT
                sw.WriteLine(@"\t\tRegstr: " + record.Value[5] + "\n");
#endif
            }
        }

#if WRITERESULTTEXTa
            string htmls = DownHtmls();
            using StreamWriter htmlsSw = File.CreateText("EccoHtmls.text");
            htmlsSw.WriteLine(htmls);
#else
        using StreamReader htmlsSr = File.OpenText("EccoHtmls.text");
        string htmls = htmlsSr.ReadToEnd();
#endif
        var records = GetEccoRecords(htmls);
        SetEccoRecordRegstr(records);

#if WRITERESULTTEXTa
            int recIndex = 0;
            foreach(var record in records)
                sw.WriteLine(string.Format($"{++recIndex}.") + string.Join("\n\t", record.Value));
#endif
    }

    public static string GetInfoKey(ManualField field) => _infoKeys[(int)field];

    private void InsertInfoList(IEnumerable<Dictionary<string, string>> infoList, bool unequal = true)
    {
        using SqliteConnection connection = GetSqliteConnection();
        using var transaction = connection.BeginTransaction();

        var command = connection.CreateCommand();
        // 要求：所有Info的Keys都相同
        var infoKeys = infoList.First().Keys;
        static string ParamName(string key) => "$" + key;
        foreach(var key in infoKeys)
            command.Parameters.Add(new() { ParameterName = ParamName(key) });

        static string JoinEnumableString(IEnumerable<string> strings) => string.Join(", ", strings);
        var fields = JoinEnumableString(infoKeys.Select(key => $"'{key}'"));
        command.CommandText = $"INSERT INTO {_manualTableName} ({fields}) " +
            $"VALUES ({JoinEnumableString(infoKeys.Select(key => ParamName(key)))})";

        foreach(var info in infoList)
        {
            if(unequal && ExistsManual(connection,
                FieldEqualCondition(ManualField.Source, info[GetInfoKey(ManualField.Source)])))
                continue;

            foreach(var key in infoKeys)
                command.Parameters[ParamName(key)].Value = info[key];

            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }
    private bool ExistsManual(SqliteConnection connection, string condition)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = ManualSelectCommandText(condition);
        using var reader = command.ExecuteReader();
        return reader.Read();
    }
    private string ManualSelectCommandText(string condition) => $"SELECT * FROM {_manualTableName} WHERE {condition}";
    private static string FieldEqualCondition(ManualField field, string value) => $" {GetInfoKey(field)} == '{value}'";
    private SqliteConnection GetSqliteConnection()
    {
        string databaseFileName = "data.db";
        bool fileExists = File.Exists(databaseFileName);
        if(!fileExists)
            using(File.Create(databaseFileName)) { };

        SqliteConnection connection = new("Data Source=" + databaseFileName);
        connection.Open();
        if(!fileExists)
        {
            string[] commandString = new string[]{
                        $"CREATE TABLE {_manualTableName} (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        $"{string.Join(",", _infoKeys.Select(field => field + " TEXT"))})", };
            SqliteCommand command = connection.CreateCommand();
            foreach(var str in commandString)
            {
                command.CommandText = str;
                command.ExecuteNonQuery();
            }
        }

        return connection;
    }
}

