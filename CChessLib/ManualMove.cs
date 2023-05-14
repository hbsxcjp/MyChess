using System.Diagnostics;
using System.Collections.Concurrent;
// using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

namespace CChess;

public class ManualMove// : IEnumerable
{
    public ManualMove(string fen = "")
    {
        RootBoard = new(fen);
        RootMove = Move.RootMove();
        CurMove = RootMove;
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
        RootMove.Remark = __readDataAndGetRemark()?.Trim();

        if ((tag & 0x80) == 0) // 无左子树
            return;

        // 有左子树
        Stack<Move> beforeMoves = new();
        beforeMoves.Push(RootMove);
        bool isOther = false;
        Move beforeMove = RootMove;
        // 当前棋子为根，且有后继棋子时，表明深度搜索已经回退到根，已经没有后续棋子了
        while (!(beforeMove.Before == null && beforeMove.HasAfter))
        {
            var remark = __readDataAndGetRemark();
            //# 一步棋的起点和终点有简单的加密计算，读入时需要还原

            int fcolrow = __sub(frc, (byte)(0X18 + xQFKey.KeyXYf)),
                tcolrow = __sub(trc, (byte)(0X20 + xQFKey.KeyXYt));
            if (fcolrow > 89 || tcolrow > 89)
                throw new Exception("fcolrow > 89 || tcolrow > 89 ? ");

            int frow = 10 - 1 - fcolrow % 10, fcol = fcolrow / 10,
                trow = 10 - 1 - tcolrow % 10, tcol = tcolrow / 10;//

            CoordPair coordPair = new(frow, fcol, trow, tcol);
            bool hasNext = (tag & 0x80) != 0, hasOther = (tag & 0x40) != 0;

            if (beforeMove.CoordPair.Equals(coordPair))
            {
                Debug.WriteLine("Error: " + coordPair.ToString() + beforeMove.Remark);
            }
            else
            {
                if (isOther)
                    beforeMove = beforeMove.Before ?? beforeMove;

                beforeMove = beforeMove.AddAfter(coordPair, remark);
                if (hasNext && hasOther)
                    beforeMoves.Push(beforeMove);

                isOther = !hasNext;
                if (isOther && !hasOther && beforeMoves.Count > 0)
                {
                    beforeMove = beforeMoves.Pop(); // 最后时，将回退到根
                }
            }
        }

        List<Move> allMoves = RootMove.AllAfterMoves;
        allMoves.Insert(0, RootMove);
        allMoves.ForEach(move
            => move.AfterMoves?.RemoveAll(move
                => !RootBoard.WithMove(move.Before).CanMove(move.CoordPair)));
    }

    public ManualMove(string fen, byte[] bytes) : this(fen)
    {
        static (string? remark, int afterNum) readRemarkAfterNum(BinaryReader reader)
        {
            string? remark = null;
            if (reader.ReadBoolean())
                remark = reader.ReadString();

            int afterNum = reader.ReadByte();
            return (remark, afterNum);
        }

        using var reader = new BinaryReader(
            new MemoryStream(bytes), Encoding.UTF8, true);
        var (rootRemark, rootAfterNum) = readRemarkAfterNum(reader);
        RootMove.Remark = rootRemark;

        Queue<(Move, int)> moveAfterNumQueue = new();
        moveAfterNumQueue.Enqueue((RootMove, rootAfterNum));
        while (moveAfterNumQueue.Count > 0)
        {
            var (beforeMove, beforeAfterNum) = moveAfterNumQueue.Dequeue();
            for (int i = 0; i < beforeAfterNum; ++i)
            {
                bool visible = reader.ReadBoolean();
                CoordPair coordPair = Board.GetCoordPairFromRowCol(reader.ReadString());
                var (remark, afterNum) = readRemarkAfterNum(reader);

                var move = beforeMove.AddAfter(coordPair, remark, visible);
                if (afterNum > 0)
                    moveAfterNumQueue.Enqueue((move, afterNum));
            }
        }
    }

    public ManualMove(string fen, string moveString, FileExtType fileExtType) : this(fen)
    {
        if (fileExtType == FileExtType.txt)
            SetRootMoveFromPGNText(fen, moveString);
        else
            SetRootMoveFromPGNExt(fen, moveString, fileExtType);
    }

    public ManualMove(string fen, string rowCols) : this(fen)
    {
        int lenght = rowCols.Length;
        Move move = RootMove;
        for (int i = 0; i < lenght; i += CoordPair.RowColICCSLength)
            move = move.AddAfter(Board.GetCoordPairFromRowCol(rowCols[i..(i + CoordPair.RowColICCSLength)]));
    }

    public Board RootBoard { get; }
    public Move RootMove { get; }
    public Move CurMove { get; private set; }

    public PieceColor StartColor
    {
        get
        {
            if (RootMove.AfterMoves == null)
                return PieceColor.Red;

            return RootBoard[RootMove.AfterMoves[0].CoordPair.FromCoord.Index].Color;
        }
    }

    public bool AddMove(string zhStr, bool isOther = false)
    {
        if (isOther)
            Back();

        Board board = RootBoard.WithMove(CurMove);
        CoordPair coordPair = board.GetCoordPairFromZhStr(zhStr);
        bool success = board.CanMove(coordPair);
        if (success)
            CurMove = CurMove.AddAfter(coordPair);

        return success;
    }

    public bool Go() // 前进
    {
        var afterMoves = CurMove.AfterMoves;
        if (afterMoves == null)
            return false;

        CurMove = afterMoves[0];
        return true;
    }
    public bool GoOther(bool isLeft) // 变着
    {
        var otherMoves = CurMove.OtherMoves;
        if (otherMoves == null)
            return false;

        int index = otherMoves.IndexOf(CurMove);
        if ((isLeft && index == 0)
            || (!isLeft && index == otherMoves.Count - 1))
            return false;

        CurMove = otherMoves[index + (isLeft ? -1 : 1)];
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

        CurMove = CurMove.Before;
        return true;
    }
    public void BackStart() // 回退到开始
        => CurMove = RootMove;

    public byte[] GetBytes()
    {
        static void writeRemarkAfterNum(BinaryWriter writer, string? remark, int afterNum)
        {
            writer.Write(remark != null);
            if (remark != null)
                writer.Write(remark);
            writer.Write((byte)afterNum);
        }

        MemoryStream stream = new();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        writeRemarkAfterNum(writer, RootMove.Remark, RootMove.AfterNum);
        RootMove.AllAfterMoves.ForEach(move =>
        {
            writer.Write(move.Visible);
            writer.Write(move.CoordPair.RowCol);
            writeRemarkAfterNum(writer, move.Remark, move.AfterNum);
        });

        return stream.ToArray();
    }

    public string GetString(FileExtType fileExtType, ChangeType ct = ChangeType.NoChange)
    {
        if (fileExtType == FileExtType.txt)
        {
            string GetRemarkAfterNum(Move move)
                => $"{(move.Remark == null ? "" : $"{{{move.Remark}}}")}{(move.AfterNum == 0 ? "" : $"({move.AfterNum})")} ";

            return GetRemarkAfterNum(RootMove) +
                    string.Concat(RootMove.AllAfterMoves.Select(move
                        => $"{(move.Visible ? "+" : "-")}{(ct == ChangeType.Symmetry_V ? move.CoordPair.SymmetryVRowCol : move.CoordPair.RowCol)}{GetRemarkAfterNum(move)}"));
        }

        string GetPGNText(Move move, FileExtType fileExtType = FileExtType.pgnzh)
            => fileExtType switch
            {
                FileExtType.pgniccs => move.CoordPair.Iccs,
                FileExtType.pgnrc => ct == ChangeType.Symmetry_V ? move.CoordPair.SymmetryVRowCol : move.CoordPair.RowCol,
                _ => move.Before == null ? string.Empty
                    : RootBoard.WithMove(move.Before).GetZhStrFromCoordPair(move.CoordPair),
            };

        return ((RootMove.Remark != null && RootMove.Remark.Length > 0) ? $"{{{RootMove.Remark}}}\n" : "") +
            string.Concat(RootMove.AllAfterMoves.Select(move =>
                 move.Before?.Id.ToString() + "-"
                    + GetPGNText(move, fileExtType)
                    + (move.Visible ? "" : "_")
                    + (move.Remark == null ? " " : "{" + move.Remark + "} ")));
    }

    public string GetRowCols()
    {
        StringBuilder result = new();
        Queue<List<Move>> afterMovesQueue = new();
        if (RootMove.AfterMoves != null)
            afterMovesQueue.Enqueue(RootMove.AfterMoves);

        while (afterMovesQueue.Count > 0)
        {
            List<Move> afterMoves = afterMovesQueue.Dequeue();

            result.Append(afterMoves.Count == 1
                ? afterMoves[0].CoordPair.RowCol
                : $"(?:{string.Join('|', afterMoves.Select(move => move.CoordPair.RowCol))})")
                .Append('-');

            afterMoves.ForEach(move =>
            {
                if (move.AfterMoves != null)
                    afterMovesQueue.Enqueue(move.AfterMoves);
            });
        }

        return result.ToString();
    }

    public string GetFirstRowCols()
    {
        StringBuilder result = new();
        Move tempMove = RootMove;
        while (tempMove.AfterMoves != null)
        {
            tempMove = tempMove.AfterMoves[0];
            result.Append(tempMove.CoordPair.RowCol);
        }

        return result.ToString();
    }

    public List<(string fen, string rowCol)> GetFENRowCols()
    {
        List<(string fen, string rowCol)> aspects = new();
        string UniversalFEN(Move before)
        {
            Board board = RootBoard.WithMove(before);
            return Board.GetFEN(board.GetFEN(), board.IsBottom(PieceColor.Red)
                ? ChangeType.NoChange : ChangeType.Exchange);
        }

        RootMove.AllAfterMoves.ForEach(move =>
        {
            if (move.Before != null)
                aspects.Add((UniversalFEN(move.Before), move.CoordPair.RowCol));
        });

        return aspects;
    }

    private void SetRootMoveFromPGNText(string fen, string moveString)
    {
        string movePattern;
        MatchCollection matches;
        string remarkAfterNumPattern = @"(?:{([\s\S]+?)})?(?:\((\d+)\))? ";
        var rootMoveMatch = Regex.Match(moveString, "^" + remarkAfterNumPattern);
        if (!rootMoveMatch.Success)
            return;

        var rootRemark = rootMoveMatch.Groups[1].Value;
        if (rootRemark.Length > 0)
            RootMove.Remark = rootRemark;
        Queue<(Move, int)> moveAfterNumQueue = new();
        moveAfterNumQueue.Enqueue((RootMove, Convert.ToInt32(rootMoveMatch.Groups[2].Value)));

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
                CoordPair coordPair = Board.GetCoordPairFromRowCol(match.Groups[2].Value);
                string remark = match.Groups[3].Value, afterNumStr = match.Groups[4].Value;

                var move = beforeMove.AddAfter(coordPair, remark.Length > 0 ? remark : null, visible);
                if (afterNumStr.Length > 0)
                    moveAfterNumQueue.Enqueue((move, Convert.ToInt32(afterNumStr)));
            }
        }
    }

    private void SetRootMoveFromPGNExt(string fen, string moveString, FileExtType fileExtType)
    {
        CoordPair GetCoordPair(Board board, string pgnText, FileExtType fileExtType)
            => fileExtType switch
            {
                FileExtType.pgniccs => Board.GetCoordPairFromIccs(pgnText),
                FileExtType.pgnrc => Board.GetCoordPairFromRowCol(pgnText),
                _ => board.GetCoordPairFromZhStr(pgnText)
            };

        string remarkPattern = @"(?:{([\s\S]+?)})";
        var remarkMatch = Regex.Match(moveString, "^" + remarkPattern);
        if (remarkMatch.Success)
            RootMove.Remark = remarkMatch.Groups[1].Value;

        List<Move> allMoves = new() { RootMove };
        string pgnPattern = (fileExtType == FileExtType.pgniccs
            ? @$"(?:[{Coord.ColChars}]\d){{2}}"
            : (fileExtType == FileExtType.pgnrc ? @"\d{4}" : Board.PGNZHCharsPattern()));
        string movePattern = @$"(\d+)\-({pgnPattern})(_?){remarkPattern}?\s+";
        foreach (Match match in Regex.Matches(moveString, movePattern))
        {
            int id = Convert.ToInt32(match.Groups[1].Value);
            string pgnText = match.Groups[2].Value;
            bool visible = match.Groups[3].Value.Length == 0;
            string? remark = match.Groups[4].Success ? match.Groups[4].Value : null;

            Board board = RootBoard.WithMove(allMoves[id]);
            allMoves.Add(allMoves[id].AddAfter(GetCoordPair(board, pgnText, fileExtType), remark, visible));
        }
    }

    public string ToString(bool showMove = false, bool isOrder = false, ChangeType ct = ChangeType.NoChange)
    {
        int moveCount = 0, remarkCount = 0, maxRemarkCount = 0;
        StringBuilder result = new(ct == ChangeType.Symmetry_V ? RootMove.SymmetryVToString() : RootMove.ToString());
        List<Move> allMoves = RootMove.AllAfterMoves;
        // foreach (var move in this)
        allMoves.ForEach(move =>
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
                    result.Append(ct == ChangeType.Symmetry_V ? move.SymmetryVToString() : move.ToString());
                // else
                // allMoves.Add(move);
            }
        });

        if (showMove && !isOrder)
        {
            BlockingCollection<string> results = new();
            Parallel.ForEach<Move, string>(allMoves,
                () => "",
                (move, loop, subString) => subString += ct == ChangeType.Symmetry_V ? move.SymmetryVToString() : move.ToString(),
                (finalSubString) => results.Add(finalSubString));
            result.Append(string.Concat(results));
        }
        result.Append($"着法数量【{moveCount}】\t注解数量【{remarkCount}】\t注解最长【{maxRemarkCount}】\n\n");

        return RootBoard.ToString() + result;
    }

    // IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();

    // public ManualMoveEnum GetEnumerator() => new(this);
}
