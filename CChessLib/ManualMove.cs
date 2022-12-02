using System.Collections.Concurrent;
using System.Collections;
using System.Text.RegularExpressions;

namespace CChess;

internal class ManualMove: IEnumerable
{
    private readonly Board _board;
    private readonly Move _rootMove;

    public ManualMove()
    {
        _board = new();
        _rootMove = Move.CreateRootMove();
        CurMove = _rootMove;
        EnumMoveDone = false;
    }

    public Move CurMove { get; set; }
    public string? CurRemark { get { return CurMove.Remark; } set { CurMove.Remark = value?.Trim(); } }
    public bool EnumMoveDone { get; set; }

    public List<Coord> GetCanPutCoords(Piece piece) => piece.PutCoord(_board, _board.IsBottomColor(piece.Color));
    public List<Coord> GetCanMoveCoords(Coord fromCoord) => _board.CanMoveCoord(fromCoord);

    public bool AcceptCoordPair(CoordPair coordPair)
        => _board.CanMoveCoord(coordPair.FromCoord).Contains(coordPair.ToCoord);
    public bool SetBoard(string fen) => _board.SetFEN(fen.Split(' ')[0]);

    public void AddMove(CoordPair coordPair, string? remark = null, bool visible = true)
        => GoMove(CurMove.AddAfterMove(coordPair, remark, visible));
    public bool AddMove(string zhStr)
    {
        var coordPair = _board.GetCoordPairFromZhstr(zhStr);
        bool success = coordPair != CoordPair.Null;
        if(success)
            AddMove(coordPair);

        return success;
    }

    public CoordPair GetCoordPair(int frow, int fcol, int trow, int tcol)
        => new(_board[frow, fcol].Coord, _board[trow, tcol].Coord);

    public bool Go() // 前进
    {
        var afterMoves = CurMove.AfterMoves(VisibleType.True);
        if(afterMoves == null)
            return false;

        GoMove(afterMoves[0]);
        return true;
    }
    public bool GoOther(bool isLeft) // 变着
    {
        var otherMoves = CurMove.OtherMoves();
        if(otherMoves == null)
            return false;

        int index = otherMoves.IndexOf(CurMove);
        if((isLeft && index == 0)
            || (!isLeft && index == otherMoves.Count - 1))
            return false;

        CurMove.Undo(_board);
        GoMove(otherMoves[index + (isLeft ? -1 : 1)]);
        return true;
    }
    public void GoEnd() // 前进到底
    {
        while(Go())
            ;
    }
    public bool Back() // 回退
    {
        if(CurMove.Before == null)
            return false;

        CurMove.Undo(_board);
        CurMove = CurMove.Before;
        return true;
    }
    public void BackStart() // 回退到开始
    {
        while(Back())
            ;
    }
    public bool GoTo(Move? move) // 转至指定move
    {
        if(CurMove == move || move == null)
            return false;

        var beforeMoves = move.BeforeMoves();
        int index = -1;
        while(Back())
            if((index = beforeMoves.IndexOf(CurMove)) > -1)
                break;

        for(int i = index + 1;i < beforeMoves.Count;++i)
            beforeMoves[i].Done(_board);

        CurMove = move;
        return true;
    }

    public void ReadCM(BinaryReader reader)
    {
        static (string? remark, int afterNum) readRemarkAfterNum(BinaryReader reader)
        {
            string? remark = null;
            if(reader.ReadBoolean())
                remark = reader.ReadString();

            int afterNum = reader.ReadByte();
            return (remark, afterNum);
        }

        var (rootRemark, rootAfterNum) = readRemarkAfterNum(reader);
        _rootMove.Remark = rootRemark;

        Queue<(Move, int)> moveAfterNumQueue = new();
        moveAfterNumQueue.Enqueue((_rootMove, rootAfterNum));
        while(moveAfterNumQueue.Count > 0)
        {
            var (beforeMove, beforeAfterNum) = moveAfterNumQueue.Dequeue();
            for(int i = 0;i < beforeAfterNum;++i)
            {
                bool visible = reader.ReadBoolean();
                CoordPair coordPair = _board.GetCoordPairFromRowCol(reader.ReadString());
                var (remark, afterNum) = readRemarkAfterNum(reader);

                var move = beforeMove.AddAfterMove(coordPair, remark, visible);
                if(afterNum > 0)
                    moveAfterNumQueue.Enqueue((move, afterNum));
            }
        }
    }

    public void WriteCM(BinaryWriter writer)
    {
        static void writeRemarkAfterNum(BinaryWriter writer, string? remark, int afterNum)
        {
            writer.Write(remark != null);
            if(remark != null)
                writer.Write(remark);
            writer.Write((byte)afterNum);
        }

        writeRemarkAfterNum(writer, _rootMove.Remark, _rootMove.AfterNum);
        foreach(var move in this)
        {
            writer.Write(move.Visible);
            writer.Write(move.CoordPair.RowCol);
            writeRemarkAfterNum(writer, move.Remark, move.AfterNum);
        }
    }

    public void SetFromString(string moveString, FileExtType fileExtType)
    {
        string movePattern;
        MatchCollection matches;
        if(fileExtType == FileExtType.Text)
        {
            string remarkAfterNumPattern = @"(?:{([\s\S]+?)})?(?:\((\d+)\))? ";
            var rootMoveMatch = Regex.Match(moveString, "^" + remarkAfterNumPattern);
            if(!rootMoveMatch.Success)
                return;

            var rootRemark = rootMoveMatch.Groups[1].Value;
            if(rootRemark.Length > 0)
                _rootMove.Remark = rootRemark;
            Queue<(Move, int)> moveAfterNumQueue = new();
            moveAfterNumQueue.Enqueue((_rootMove, Convert.ToInt32(rootMoveMatch.Groups[2].Value)));

            int matchIndex = 0;
            movePattern = @"([+-])(\d{4})" + remarkAfterNumPattern;
            matches = Regex.Matches(moveString, movePattern);
            while(moveAfterNumQueue.Count > 0 && matchIndex < matches.Count)
            {
                var (beforeMove, beforeAfterNum) = moveAfterNumQueue.Dequeue();
                for(int i = 0;i < beforeAfterNum;++i)
                {
                    Match match = matches[matchIndex++];
                    bool visible = match.Groups[1].Value == "+";
                    CoordPair coordPair = _board.GetCoordPairFromRowCol(match.Groups[2].Value);
                    string remark = match.Groups[3].Value, afterNumStr = match.Groups[4].Value;

                    var move = beforeMove.AddAfterMove(coordPair, remark.Length > 0 ? remark : null, visible);
                    if(afterNumStr.Length > 0)
                        moveAfterNumQueue.Enqueue((move, Convert.ToInt32(afterNumStr)));
                }
            }
            return;
        }

        string remarkPattern = @"(?:{([\s\S]+?)})";
        var remarkMatch = Regex.Match(moveString, "^" + remarkPattern);
        if(remarkMatch.Success)
            _rootMove.Remark = remarkMatch.Groups[1].Value;

        List<Move> allMoves = new() { _rootMove };
        string pgnPattern = (fileExtType == FileExtType.PGNIccs
            ? @"(?:[" + Coord.ColChars + @"]\d){2}"
            : (fileExtType == FileExtType.PGNRowCol ? @"\d{4}" : "[" + Piece.PGNZHChars() + @"]{4}"));
        movePattern = @"(\d+)\-(" + pgnPattern + @")(_?)" + remarkPattern + @"?\s+";
        matches = Regex.Matches(moveString, movePattern);
        foreach(Match match in matches.Cast<Match>())
        {
            if(!match.Success)
                break;

            int id = Convert.ToInt32(match.Groups[1].Value);
            string pgnText = match.Groups[2].Value;
            bool visible = match.Groups[3].Value.Length == 0;
            string? remark = match.Groups[4].Success ? match.Groups[4].Value : null;
            if(fileExtType == FileExtType.PGNZh)
                GoTo(allMoves[id]);

            allMoves.Add(allMoves[id].AddAfterMove(GetCoordPair(pgnText, fileExtType), remark, visible));
        }
    }

    public string GetString(FileExtType fileExtType)
    {
        string result = "";
        if(fileExtType == FileExtType.Text)
        {
            static string GetRemarkAfterNum(Move move)
                => (move.Remark == null ? "" : "{" + move.Remark + "}") +
                 (move.AfterNum == 0 ? "" : "(" + move.AfterNum.ToString() + ")") + " ";

            static string GetMoveString(Move move)
                 => $"{(move.Visible ? "+" : "-")}{move.CoordPair.RowCol}";

            result = GetRemarkAfterNum(_rootMove);
            foreach(var move in this)
                result += GetMoveString(move) + GetRemarkAfterNum(move);

            return result;
        }

        if(_rootMove.Remark != null && _rootMove.Remark.Length > 0)
            result += "{" + _rootMove.Remark + "}\n";

        var oldEnumMoveDone = EnumMoveDone;
        if(fileExtType == FileExtType.PGNZh)
            EnumMoveDone = true;
        foreach(var move in this)
            result += move.Before?.Id.ToString() + "-"
                + GetPGNText(move.CoordPair, fileExtType)
                + (move.Visible ? "" : "_")
                + (move.Remark == null ? " " : "{" + move.Remark + "} ");

        if(fileExtType == FileExtType.PGNZh)
            EnumMoveDone = oldEnumMoveDone;

        return result;
    }

    public void SetFromRowCols(string rowCols)
    {
        int lenght = rowCols.Length;
        Move move = _rootMove;
        for(int i = 0;i < lenght;i += CoordPair.RowColICCSLength)
            move = move.AddAfterMove(GetCoordPair(rowCols[i..(i + CoordPair.RowColICCSLength)], FileExtType.PGNRowCol));
    }

    public string GetRowCols()
    {
        string rowCols = "";
        var afterMoves = _rootMove.AfterMoves();
        while(afterMoves != null && afterMoves.Count > 0)
        {
            rowCols += afterMoves[0].CoordPair.RowCol;
            afterMoves = afterMoves[0].AfterMoves();
        }

        return rowCols;
    }

    public List<(string fen, string rowCol)> GetAspects()
    {
        List<(string fen, string rowCol)> aspects = new();
        var oldEnumMoveDone = EnumMoveDone;
        EnumMoveDone = true;
        foreach(var move in this)
            aspects.Add((AspectFEN, move.CoordPair.RowCol));
        EnumMoveDone = oldEnumMoveDone;

        return aspects;
    }

    public string AspectFEN
    {
        get => Board.GetFEN(_board.GetFEN(), _board.IsBottomColor(PieceColor.Red) ? ChangeType.NoChange : ChangeType.Exchange);
    }

    public void ClearError()
    {
        var oldEnumMoveDone = EnumMoveDone;
        EnumMoveDone = true;
        _rootMove.ClearAfterMovesError(this);
        foreach(var move in this)
        {
            move.Done(_board);
            move.ClearAfterMovesError(this);
            move.Undo(_board);
        }
        EnumMoveDone = oldEnumMoveDone;
    }

    public string ToString(bool showMove = false, bool isOrder = false)
    {
        int moveCount = 0, remarkCount = 0, maxRemarkCount = 0;
        string moveString = _rootMove.ToString();
        List<Move> allMoves = new();
        foreach(var move in this)
        {
            ++moveCount;
            if(move.Remark != null)
            {
                remarkCount++;
                maxRemarkCount = Math.Max(maxRemarkCount, move.Remark.Length);
            }
            if(showMove)
            {
                if(isOrder)
                    moveString += move.ToString();
                else
                    allMoves.Add(move);
            }
        }

        if(showMove && !isOrder)
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

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();
    public ManualMoveEnum GetEnumerator() => new(this);

    private void GoMove(Move move) => (CurMove = move).Done(_board);

    private CoordPair GetCoordPair(string pgnText, FileExtType fileExtType)
        => fileExtType switch
        {
            FileExtType.PGNIccs => _board.GetCoordPairFromIccs(pgnText),
            FileExtType.PGNRowCol => _board.GetCoordPairFromRowCol(pgnText),
            _ => _board.GetCoordPairFromZhstr(pgnText),
        };
    
    private string GetPGNText(CoordPair coordPair, FileExtType fileExtType)
        => fileExtType switch
        {
            FileExtType.PGNIccs => coordPair.Iccs,
            FileExtType.PGNRowCol => coordPair.RowCol,
            _ => _board.GetZhStrFromCoordPair(coordPair),
        };
}

internal class ManualMoveEnum: IEnumerator
{
    private readonly Queue<Move> _moveQueue;
    private readonly ManualMove _manualMove;
    private Move _curMove;
    private int _id;

    public ManualMoveEnum(ManualMove manualMove)
    {
        _manualMove = manualMove;
        _moveQueue = new();
        _curMove = manualMove.CurMove; // 消除未赋值警示

        Reset();
    }

    public void Reset()
    {
        _manualMove.BackStart();
        _moveQueue.Clear();
        _id = 0;
        SetCurrentEnqueueAfterMoves(_manualMove.CurMove);
    }

    // 迭代不含根节点。如执行着法，棋局执行至当前之前着，当前着法未执行
    public bool MoveNext()
    {
        if(_moveQueue.Count == 0)
        {
            if(_manualMove.EnumMoveDone)
                _manualMove.BackStart();
            return false;
        }

        SetCurrentEnqueueAfterMoves(_moveQueue.Dequeue());
        return true;
    }

    object IEnumerator.Current { get { return Current; } }

    public Move Current { get { return _curMove; } }

    private void SetCurrentEnqueueAfterMoves(Move curMove)
    {
        _curMove = curMove;
        _curMove.Id = _id++;
        // 根据枚举特性判断是否执行着法
        if(_manualMove.EnumMoveDone)
            _manualMove.GoTo(_curMove.Before);

        var afterMoves = _curMove.AfterMoves();
        if(afterMoves != null)
        {
            foreach(var move in afterMoves)
                _moveQueue.Enqueue(move);
        }
    }
}
