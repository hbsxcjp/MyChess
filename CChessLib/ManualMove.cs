using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

namespace CChess;

public class ManualMove : IEnumerable
{
    private Board _board;
    private readonly Move _rootMove;

    public ManualMove(string fen)
    {
        _board = new(fen);
        _rootMove = Move.RootMove();

        CurMove = _rootMove;
        EnumMoveDone = false;
    }

    public ManualMove(string fen, Stream stream, (byte Version, byte KeyXYf, byte KeyXYt, uint KeyRMKSize, byte[] F32Keys) xQFKey)
        : this(fen)
    {
        byte __sub(byte a, byte b) { return (byte)(a - b); }; // 保持为<256

        void __readBytes(byte[] bytes, int size)
        {
            int pos = (int)stream.Position;
            stream.Read(bytes, 0, size);
            if (xQFKey.Version > 10) // '字节解密'
                for (uint i = 0; i != size; ++i)
                    bytes[i] = __sub(bytes[i], xQFKey.F32Keys[(pos + i) % 32]);
        }

        uint __getRemarksize()
        {
            byte[] clen = new byte[4];
            __readBytes(clen, 4);
            return (uint)(clen[0] + (clen[1] << 8) + (clen[2] << 16) + (clen[3] << 24)) - xQFKey.KeyRMKSize;
        };

        Encoding codec = Encoding.GetEncoding("gb2312"); // "gb2312"
        byte[] data = new byte[4];
        byte frc = data[0], trc = data[1], tag = data[2];
        string? __readDataAndGetRemark()
        {
            __readBytes(data, 4);
            uint RemarkSize = 0;
            frc = data[0];
            trc = data[1];
            tag = data[2];
            if (xQFKey.Version <= 10)
            {
                tag = (byte)((((tag & 0xF0) != 0) ? 0x80 : 0) | (((tag & 0x0F) != 0) ? 0x40 : 0));
                RemarkSize = __getRemarksize();
            }
            else
            {
                tag &= 0xE0;
                if ((tag & 0x20) != 0)
                    RemarkSize = __getRemarksize();
            }

            if (RemarkSize == 0)
                return null;

            // # 有注解
            byte[] rem = new byte[2048 * 2];
            __readBytes(rem, (int)RemarkSize);
            var remark = codec.GetString(rem).Replace('\0', ' ')
                .Replace("\r\n", "\n").Trim();
            return remark.Length > 0 ? remark : null;
        }

        stream.Seek(1024, SeekOrigin.Begin);
        CurRemark = __readDataAndGetRemark();

        if ((tag & 0x80) == 0) // 无左子树
            return;

        // 有左子树
        Stack<Move> beforeMoves = new();
        beforeMoves.Push(CurMove);
        bool isOther = false;
        // 当前棋子为根，且有后继棋子时，表明深度搜索已经回退到根，已经没有后续棋子了
        while (!(CurMove.Before == null && CurMove.HasAfter))
        {
            var remark = __readDataAndGetRemark();
            //# 一步棋的起点和终点有简单的加密计算，读入时需要还原

            int fcolrow = __sub(frc, (byte)(0X18 + xQFKey.KeyXYf)),
                tcolrow = __sub(trc, (byte)(0X20 + xQFKey.KeyXYt));
            if (fcolrow > 89 || tcolrow > 89)
                throw new Exception("fcolrow > 89 || tcolrow > 89 ? ");

            int frow = fcolrow % 10, fcol = fcolrow / 10, trow = tcolrow % 10,
                tcol = tcolrow / 10;

            CoordPair coordPair = GetCoordPair(frow, fcol, trow, tcol);
            bool hasNext = (tag & 0x80) != 0, hasOther = (tag & 0x40) != 0;

            var curCoordPair = CurMove.CoordPair;
            if (curCoordPair.FromCoord.Row == frow && curCoordPair.FromCoord.Col == fcol
                && curCoordPair.ToCoord.Row == trow && curCoordPair.ToCoord.Col == tcol)
            {
                Debug.WriteLine("Error: " + coordPair.ToString() + CurRemark);
            }
            else
            {
                if (isOther)
                    Back();
                AddMove(coordPair, remark, true);
                //Debug.WriteLine("CurMove: " + CurMove.ToString());

                if (hasNext && hasOther)
                    beforeMoves.Push(CurMove);

                isOther = !hasNext;
                if (isOther && !hasOther && beforeMoves.Count > 0)
                {
                    var beforeMove = beforeMoves.Pop(); // 最后时，将回退到根
                    while (beforeMove != CurMove)
                        Back();
                }
            }
        }

        void ClearError()
        {
            bool AcceptCoordPair(CoordPair coordPair)
                => _board[coordPair.FromCoord].CanMoveCoord(_board).Contains(coordPair.ToCoord);

            void ClearAfterMovesError(Move move)
                => move.AfterMoves()?.RemoveAll(move => !AcceptCoordPair(move.CoordPair));

            EnumMoveDone = true;
            ClearAfterMovesError(_rootMove);
            foreach (var move in this)
            {
                DoMove(move);
                ClearAfterMovesError(move);
                UndoMove(move);
            }
        }
        ClearError(); // 清除XQF带来的错误着法
        // }
    }

    public ManualMove(string fen, BinaryReader reader) : this(fen)
    {
        static (string? remark, int afterNum) readRemarkAfterNum(BinaryReader reader)
        {
            string? remark = null;
            if (reader.ReadBoolean())
                remark = reader.ReadString();

            int afterNum = reader.ReadByte();
            return (remark, afterNum);
        }

        var (rootRemark, rootAfterNum) = readRemarkAfterNum(reader);
        _rootMove.Remark = rootRemark;

        Queue<(Move, int)> moveAfterNumQueue = new();
        moveAfterNumQueue.Enqueue((_rootMove, rootAfterNum));
        while (moveAfterNumQueue.Count > 0)
        {
            var (beforeMove, beforeAfterNum) = moveAfterNumQueue.Dequeue();
            for (int i = 0; i < beforeAfterNum; ++i)
            {
                bool visible = reader.ReadBoolean();
                CoordPair coordPair = _board.GetCoordPairFromRowCol(reader.ReadString());
                var (remark, afterNum) = readRemarkAfterNum(reader);

                var move = beforeMove.AddMove(coordPair, remark, visible);
                if (afterNum > 0)
                    moveAfterNumQueue.Enqueue((move, afterNum));
            }
        }
    }

    public ManualMove(string fen, string moveString, FileExtType fileExtType) : this(fen)
    {
        string movePattern;
        MatchCollection matches;
        if (fileExtType == FileExtType.Text)
        {
            string remarkAfterNumPattern = @"(?:{([\s\S]+?)})?(?:\((\d+)\))? ";
            var rootMoveMatch = Regex.Match(moveString, "^" + remarkAfterNumPattern);
            if (!rootMoveMatch.Success)
                return;

            var rootRemark = rootMoveMatch.Groups[1].Value;
            if (rootRemark.Length > 0)
                _rootMove.Remark = rootRemark;
            Queue<(Move, int)> moveAfterNumQueue = new();
            moveAfterNumQueue.Enqueue((_rootMove, Convert.ToInt32(rootMoveMatch.Groups[2].Value)));

            int matchIndex = 0;
            movePattern = @"([+-])(\d{4})" + remarkAfterNumPattern;
            matches = Regex.Matches(moveString, movePattern);
            while (moveAfterNumQueue.Count > 0 && matchIndex < matches.Count)
            {
                var (beforeMove, beforeAfterNum) = moveAfterNumQueue.Dequeue();
                for (int i = 0; i < beforeAfterNum; ++i)
                {
                    Match match = matches[matchIndex++];
                    bool visible = match.Groups[1].Value == "+";
                    CoordPair coordPair = _board.GetCoordPairFromRowCol(match.Groups[2].Value);
                    string remark = match.Groups[3].Value, afterNumStr = match.Groups[4].Value;

                    var move = beforeMove.AddMove(coordPair, remark.Length > 0 ? remark : null, visible);
                    if (afterNumStr.Length > 0)
                        moveAfterNumQueue.Enqueue((move, Convert.ToInt32(afterNumStr)));
                }
            }
            return;
        }

        string remarkPattern = @"(?:{([\s\S]+?)})";
        var remarkMatch = Regex.Match(moveString, "^" + remarkPattern);
        if (remarkMatch.Success)
            _rootMove.Remark = remarkMatch.Groups[1].Value;

        List<Move> allMoves = new() { _rootMove };
        string pgnPattern = (fileExtType == FileExtType.PGNIccs
            ? @"(?:[" + Coord.ColChars + @"]\d){2}"
            : (fileExtType == FileExtType.PGNRowCol ? @"\d{4}" : "[" + Board.PGNZHChars() + @"]{4}"));
        movePattern = @"(\d+)\-(" + pgnPattern + @")(_?)" + remarkPattern + @"?\s+";
        matches = Regex.Matches(moveString, movePattern);
        foreach (Match match in matches.Cast<Match>())
        {
            if (!match.Success)
                break;

            int id = Convert.ToInt32(match.Groups[1].Value);
            string pgnText = match.Groups[2].Value;
            bool visible = match.Groups[3].Value.Length == 0;
            string? remark = match.Groups[4].Success ? match.Groups[4].Value : null;
            if (fileExtType == FileExtType.PGNZh)
                GoTo(allMoves[id]);

            allMoves.Add(allMoves[id].AddMove(_board.GetCoordPair(pgnText, fileExtType), remark, visible));
        }
    }

    public ManualMove(string fen, string rowCols) : this(fen)
    {
        int lenght = rowCols.Length;
        Move move = _rootMove;
        for (int i = 0; i < lenght; i += CoordPair.RowColICCSLength)
            move = move.AddMove(_board.GetCoordPair(rowCols[i..(i + CoordPair.RowColICCSLength)], FileExtType.PGNRowCol));
    }

    public Move CurMove { get; set; }
    public bool EnumMoveDone { get; set; }
    public string? CurRemark { get => CurMove.Remark; set { CurMove.Remark = value?.Trim(); } }
    public string UniversalFEN
    {
        get => Board.GetFEN(_board.GetFEN(), _board.IsBottom(PieceColor.Red)
            ? ChangeType.NoChange : ChangeType.Exchange);
    }

    public void AddMove(CoordPair coordPair, string? remark = null, bool visible = true)
        => GoMove(CurMove.AddMove(coordPair, remark, visible));

    public bool AddMove(string zhStr)
    {
        var coordPair = _board.GetCoordPairFromZhStr(zhStr);
        if (coordPair.Equals(CoordPair.Null))
            return false;

        AddMove(coordPair);
        return true;
    }

    public CoordPair GetCoordPair(int frow, int fcol, int trow, int tcol)
        => _board.GetCoordPair(frow, fcol, trow, tcol);

    public bool Go() // 前进
    {
        var afterMoves = CurMove.AfterMoves();
        if (afterMoves == null)
            return false;

        GoMove(afterMoves[0]);
        return true;
    }
    public bool GoOther(bool isLeft) // 变着
    {
        var otherMoves = CurMove.OtherMoves();
        if (otherMoves == null)
            return false;

        int index = otherMoves.IndexOf(CurMove);
        if ((isLeft && index == 0)
            || (!isLeft && index == otherMoves.Count - 1))
            return false;

        UndoMove(CurMove);
        GoMove(otherMoves[index + (isLeft ? -1 : 1)]);
        return true;
    }
    public void GoEnd() // 前进到底
    {
        while (Go())
            ;
    }
    public bool Back() // 回退
    {
        if (CurMove.Before == null)
            return false;

        UndoMove(CurMove);
        CurMove = CurMove.Before;
        return true;
    }
    public void BackStart() // 回退到开始
    {
        while (Back())
            ;
    }
    public void GoTo(Move? move) // 转至指定move
    {
        if (CurMove == move || move == null)
            return;

        BackStart();
        CurMove = move;

        List<Move> beforeMoves = new();
        while (move.Before != null)
        {
            beforeMoves.Insert(0, move);
            move = move.Before;
        }
        beforeMoves.ForEach(move => DoMove(move));
    }

    private void GoMove(Move move) => DoMove(CurMove = move);

    private void DoMove(Move move)
    {
        CoordPair coordPair = move.CoordPair;
        move.EatPiece = _board[coordPair.ToCoord];

        _board = _board.MoveToBoard(coordPair.FromCoord, coordPair.ToCoord, Piece.Null);
    }
    private void UndoMove(Move move)
    {
        CoordPair coordPair = move.CoordPair;

        _board = _board.MoveToBoard(coordPair.ToCoord, coordPair.FromCoord, move.EatPiece);
    }

    public void WriteCM(BinaryWriter writer)
    {
        static void writeRemarkAfterNum(BinaryWriter writer, string? remark, int afterNum)
        {
            writer.Write(remark != null);
            if (remark != null)
                writer.Write(remark);
            writer.Write((byte)afterNum);
        }

        writeRemarkAfterNum(writer, _rootMove.Remark, _rootMove.AfterNum);
        foreach (var move in this)
        {
            writer.Write(move.Visible);
            writer.Write(move.CoordPair.RowCol);
            writeRemarkAfterNum(writer, move.Remark, move.AfterNum);
        }
    }

    public string GetString(FileExtType fileExtType)
    {
        string result = "";
        if (fileExtType == FileExtType.Text)
        {
            static string GetRemarkAfterNum(Move move)
                => (move.Remark == null ? "" : "{" + move.Remark + "}") +
                 (move.AfterNum == 0 ? "" : "(" + move.AfterNum.ToString() + ")") + " ";

            static string GetMoveString(Move move)
                 => $"{(move.Visible ? "+" : "-")}{move.CoordPair.RowCol}";

            result = GetRemarkAfterNum(_rootMove);
            foreach (var move in this)
                result += GetMoveString(move) + GetRemarkAfterNum(move);

            return result;
        }

        if (_rootMove.Remark != null && _rootMove.Remark.Length > 0)
            result += "{" + _rootMove.Remark + "}\n";

        EnumMoveDone = fileExtType == FileExtType.PGNZh;
        foreach (var move in this)
            result += move.Before?.Id.ToString() + "-"
                + GetPGNText(move.CoordPair, fileExtType)
                + (move.Visible ? "" : "_")
                + (move.Remark == null ? " " : "{" + move.Remark + "} ");

        return result;
    }

    public string GetRowCols()
    {
        StringBuilder rowCols = new();
        var afterMoves = _rootMove.AfterMoves();
        while (afterMoves != null && afterMoves.Count > 0)
        {
            rowCols.Append(afterMoves[0].CoordPair.RowCol);
            afterMoves = afterMoves[0].AfterMoves();
        }

        return rowCols.ToString();
    }

    public List<(string fen, string rowCol)> GetFENRowCols()
    {
        List<(string fen, string rowCol)> aspects = new();
        EnumMoveDone = true;
        foreach (var move in this)
            aspects.Add((UniversalFEN, move.CoordPair.RowCol));

        return aspects;
    }

    public string ToString(bool showMove = false, bool isOrder = false)
    {
        int moveCount = 0, remarkCount = 0, maxRemarkCount = 0;
        string moveString = _rootMove.ToString();
        List<Move> allMoves = new();
        foreach (var move in this)
        {
            ++moveCount;
            if (move.Remark != null)
            {
                remarkCount++;
                maxRemarkCount = Math.Max(maxRemarkCount, move.Remark.Length);
            }
            if (showMove)
            {
                if (isOrder)
                    moveString += move.ToString();
                else
                    allMoves.Add(move);
            }
        }

        if (showMove && !isOrder)
        {
            BlockingCollection<string> results = new();
            Parallel.ForEach<Move, string>(allMoves,
                () => "",
                (move, loop, subString) => subString += move.ToString(),
                (finalSubString) => results.Add(finalSubString));
            moveString += string.Concat(results);
        }
        moveString += $"着法数量【{moveCount}】\t注解数量【{remarkCount}】\t注解最长【{maxRemarkCount}】\n\n";

        return _board.ToString() + moveString;
    }

    private string GetPGNText(CoordPair coordPair, FileExtType fileExtType)
        => fileExtType switch
        {
            FileExtType.PGNIccs => coordPair.Iccs,
            FileExtType.PGNRowCol => coordPair.RowCol,
            _ => _board.GetZhStrFromCoordPair(coordPair),
        };

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();

    public ManualMoveEnum GetEnumerator() => new(this);
}
