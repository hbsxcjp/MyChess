<Query Kind="Program">
  <Connection>
    <ID>e354c45d-3638-4fe1-b1b7-bcfd776ad52b</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AttachFileName>D:\Program\hbsxcjp\MyChess\CChessLibUnitTest\bin\Debug\net6.0\data.db</AttachFileName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.Sqlite</EFProvider>
    </DriverData>
  </Connection>
  <Reference Relative="CChessLib\bin\Debug\net6.0\CChessLib.dll">D:\Program\hbsxcjp\MyChess\CChessLib\bin\Debug\net6.0\CChessLib.dll</Reference>
</Query>

using CChess;

void Main()
{
    //CChess.Manual.FileExtNames.Dump();
    //CChess.Manual.InfoKeys.Dump();
    
    //using var writer = File.CreateText(@"D:\Program\hbsxcjp\MyChess\CChessLibUnitTest\bin\Debug\net6.0\EccoHtmls.txt");
    //string eccoHtmlsString = CChess.Database.DownEccoHtmls();
    //writer.Write(eccoHtmlsString);
    //eccoHtmlsString.Dump();
    
    using var reader = File.OpenText(@"D:\Program\hbsxcjp\MyChess\CChessLibUnitTest\bin\Debug\net6.0\EccoHtmls.txt");    
    string eccoHtmlsString = reader.ReadToEnd();
    
    //eccoHtmlsString.Dump();    
    var eccoDict = CChess.Database.GetEccoRecords(eccoHtmlsString);
    //int index = 0;
    //foreach (var kv in eccoDict)
    //    $"{++index}.\t{kv.Key}\n{string.Concat(kv.Value.Select(str =>$"\t\t{str}\n"))}".Dump();
        
    var allBoutStrs = CChess.Database.GetBoutStrs(eccoDict);
    int no = 0;
    foreach (var boutStrs in allBoutStrs){
        $"{no++}. {boutStrs.Sn} - {boutStrs.Name}\n{boutStrs.MoveStr}".Dump();
        int color = 0;
        foreach(var boutStr in boutStrs.BoutStr){
            $"\t\t{color++}. {string.Join('\t', boutStr)}".Dump(); 
        }
        "".Dump();
    }
    
        //var rowCols_PreZhStrs = CChess.Database.GetRowCols(sn_strArray.BoutStr);
        //if(rowCols_PreZhStrs.PreZhStrs.Length > 0){
//            $"{no++}. {sn_strArray.Sn} - {sn_strArray.Name}\n{sn_strArray.MoveStr}".Dump();
//            int index = 0;
//            foreach(var str in sn_strArray.BoutStr)
//                $"\t\t{index++}. {str}".Dump();
//
//            $"\t\t{rowCols_PreZhStrs.RowCols}\n\t\t{rowCols_PreZhStrs.PreZhStrs}\n".Dump();
//        }
        //"".Dump();

}