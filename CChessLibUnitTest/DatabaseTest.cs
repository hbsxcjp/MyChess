using System.Text;

using CChess;

namespace CChessTest;

public class DatabaseTest
{
    private Manual GetManualFromId(int id)
    {
        List<Manual> ReadManuals() => Database.GetManuals($"id={id}");

        List<Manual> manuals = ReadManuals();
        if (manuals.Count == 0)
        {
            // 存储下载棋谱(重复添加)
            Database.DownXqbaseManual(1, 5);

            // 存储xqf文件棋谱(重复添加)
            List<Manual> xqfManuals = ManualTest.XqfManuals;
            for (int index = 0; index < xqfManuals.Count; ++index)
            {
                Manual manual = xqfManuals[index];
                manual.SetInfoValue(InfoKey.source, ManualTest.GetXqfFileName(index));
                manual.SetInfoValue(InfoKey.rowCols, manual.ManualMove.GetFirstRowCols());
                manual.SetInfoValue(InfoKey.moveString, manual.GetMoveString()); // 使用文本形式存储着法
            }
            Database.StorageManuals(xqfManuals);

            manuals = ReadManuals();
        }

        return manuals[0];
    }

    [Fact]
    public void TestAppendManuals()
    {
        // Database.DownXqbaseManual(12140, 12200); // 手工注释
        // Database.DownXqbaseManual(1, 12141); // 手工注释
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void TestGetManuals(int index)
    {
        string[] expectMoveStrings = {
            "(1) +2724(1) +9172(1) +0726(1) +7775(1) +0807(1) +9776(1) +3242(1) +6656(1) +2141(1) +9674(1) +0122(1) +7655(1) +0314(1) +9584(1) +2423(1) +7151(1) +0224(1) +9896(1) +0747(1) +6050(1) +2243(1) +5543(1) +4743(1) +5155(1) +2321(1) +7576(1) +4131(1) +9697(1) +4345(1) +5595(1) +4585(1) +6454(1) +0003(1) +9767(1) +8586(1) +7678(1) +0383(1) +9080(1) +3181(1) +8070(1) +8184(1) +7071(1) +8454(1) +6764(1) +3444(1) +7121(1) +8676(1) +7888(1) +7674(1) +9384(1) +7484(1) +7284(1) +8384(1) +9493(1) +8464(1) +2101(1) +1403(1) +0103(1) +0414(1) +0333(1) +6484(1) +8878(1) +8464(1) +7871(1) +5451(1) +9535(1) +4454(1) +3530(1) +5455(1) +5040(1) +6462(1) +3020(1) +6292(1) +9383(1) +9282(1) +8393(1) +8272(1) +7161(1) +7262(1) +6171(1) +2607(1) +4030(1) +6261(1) +7177(1) +6167(1) +7775(1) +6765(1) +7577(1) +4252(1) +7747(1) +2446(1) +4717(1) +5262(1) +2026(1) +6563(1) +3363(1) +6263 ",
            "(1) +2724(1) +9776(1) +0726(1) +9897(1) +0807(1) +6656(1) +0767(1) +9172(1) +3242(1) +9080(1) +0122(1) +8083(1) +2120(1) +7131(1) +3646(1) +5646(1) +6766(1) +7737(1) +6646(1) +3736(1) +0628(1) +7655(1) +3444(1) +9674(1) +0001(1) +8333(1) +2241(1) +3332(1) +4454(1) +6454(1) +3040(1) +9767(1) +0514(1) +9384(1) +4160(1) +7264(1) +2464(1) +6764(1) +2030(1) +3141(1) +6041(1) +3630(1) +4050(1) +5444(1) +0224(1) +5567(1) +4676(1) +4434(1) +2806(1) +3424(1) +0624(1) +6755(1) +7646(1) +6424(1) +4162(1) +5536(1) +2605(1) +2464(1) +6283(1) +6463(1) +8371(1) +6364(1) +7183(1) +6454(1) +8362(1) +5464(1) +6283(1) +6463(1) +8371(1) +6364(1) +4252(1) +3644(1) +4626(1) +3038(1) +2628(1) +3233(1) +0121(1) +3858(1) +0524(1) +5854(1) +2141(1) +4423(1) +0405(1) +3335(1) +2825(1) +5455(1) +4145(1) +3525(1) +1425(1) +6424(1) +0314(1) +2414 ",
            "(1) +2724(1) +9172(1) +0726(1) +7775(1) +0807(1) +9776(1) +3646(1) +6252(1) +0120(1) +9674(1) +2122(1) +9091(1) +0001(1) +7131(1) +3242(1) +5242(1) +4656(1) +6656(1) +0747(1) +3132(1) +0100(1) +7535(1) +4742(1) +7253(1) +4243(1) +5341(1) +2032(1) +4122(1) +0020(1) +3532(1) +2022(1) +3252(1) +3444(1) +9584(1) +4454(1) +6454(1) +4383(1) +9171(1) +2645(1) +9895(1) +4553(1) +7655(1) +5374(1) +5202(1) +2202(1) +5574(1) +0292(1) +9565(1) +9252(1) +5444(1) +8333(1) +7462(1) +3332(1) +6281(1) +5254(1) +7141(1) +5456(1) +4171(1) +5696(1) +6595(1) +9646(1) +7141(1) +3282(1) +9565(1) +0514(1) +6858(1) +2427(1) +6595(1) +2787(1) +4445(1) +4666(1) +4131(1) +8281(1) +3130(1) +8777(1) +9575(1) +7767(1) +3038 ",
            "(1) +2724(1) +9776(1) +0726(1) +9897(1) +0807(1) +9172(1) +3646(1) +6252(1) +2161(1) +9674(1) +6162(1) +7111(1) +0010(1) +9091(1) +0717(1) +1121(1) +2645(1) +9161(1) +6266(1) +6141(1) +1015(1) +4143(1) +4656(1) +2141(1) +4526(1) +9584(1) +1513(1) +4313(1) +1713(1) +7456(1) +1343(1) +4181(1) +0122(1) +8182(1) +2647(1) +7757(1) +2426(1) +9795(1) +0224(1) +9565(1) +6667(1) +6567(1) +2676(1) +6777(1) +7666(1) +7775(1) +4726(1) +5787(1) +2214(1) +5674(1) +4345(1) +7577(1) +4565(1) +7767(1) +2645(1) +8281(1) +3444(1) +7253(1) +4454(1) +5332(1) +1402(1) +6454(1) +6564(1) +5444(1) +6660(1) +6764(1) +4564(1) +8101(1) +6062(1) +3211(1) +0414(1) +8737(1) +6476(1) +3734(1) +1413(1) +3433(1) +1314(1) +5242(1) +6267(1) +4232(1) +6717(1) +1130(1) +0221(1) +3022(1) +1415(1) +2203(1) +1514(1) +0322(1) +1415(1) +3231(1) +2102(1) +4434(1) +2446(1) +3338(1) +7668(1) +3835(1) +0223(1) +3585(1) +1727(1) +2243(1) +2344(1) +0106(1) +6887(1) +4322(1) +0514(1) +3435(1) +4465(1) +2241(1) +2757(1) +0600(1) +1505(1) +4153(1) +5754(1) +0050(1) +5450(1) +5365(1) +8766(1) +8575(1) +4628(1) +3132(1) +0504(1) +3233(1) +5054(1) +6573(1) +5457(1) +7352(1) +6654(1) +5244(1) +5475(1) +8475 ",
            "(1) +2724(1) +9776(1) +0726(1) +9897(1) +0807(1) +9172(1) +3646(1) +6252(1) +0120(1) +9384(1) +2161(1) +9674(1) +0010(1) +6050(1) +6166(1) +5040(1) +3040(1) +9040(1) +4656(1) +7151(1) +1015(1) +5156(1) +0747(1) +4047(1) +2647(1) +7778(1) +4755(1) +7888(1) +2426(1) +9777(1) +2041(1) +8475(1) +2620(1) +7280(1) +4160(1) +9584(1) +5563(1) +8061(1) +6384(1) +5654(1) +0224(1) +6140(1) +1575(1) +9484(1) +6072(1) +8494(1) +7264(1) +5456(1) +6472(1) +8884(1) +6660(1) +8434(1) +0314(1) +5654(1) +6090(1) +9270(1) +2070 ",

            "(2) +2546{从相肩进马是取胜的正确途径。其它着法，均不能取胜。}(4) +2544(1) +0393(1) +0373(1) +8475(1) +6858(1) +0393(1) +4654{不怕黑炮平中拴链，进观的攻势含蓄双有诱惑性，是红方制胜的关键。}(1) +4654(2) +4667{叫杀得车。} +4654(1) +4456(1) +9394(1) +7374(1) +8887(1) +8868(1) +9394(1) +7675{弃车，与前着相联系，由此巧妙成杀。}(1) +5466 +7675(1) +7686 +1413(1) +8475(1) +8475(2) +8887(1) +5466 +5466 +5473 +5677(1) +8777(1) +7677(1) +8493(1) +7787(1) +8575{至此，形成少见的高将底炮双士和单车的局面。}(1) +8767(1) +9584(1) +6765(1) +7574(1) +6568(1) +7475(1) +6865(1) +7574(1) +1303(1) +9495{和棋。} ",
            "(1) +2724(1) +9172(1) +0726(1) +7775(1) +0807(1) +9776(1) +0120(1) +6656{红方左马屯边，是一种老式的攻法，优点是大子出动速度较快，不利之处是双马位置欠佳，易成弱点。\n\n黑方挺卒活马，应着稳正，是对付红方边马阵形的有效战术。}(1) +2122{平炮七线，意在加强对黑方3路线的压力，但阵营不够稳固，容易给黑方提供骚扰反击的机会。如改走炮八平六，则相对来讲要稳健一些。}(1) +7655{黑方迅即跃马，有随时马6进4入侵的手段，是一种牵制战术。此外另有车1平2的选择，以下形成车九平八，炮2进4，车二进六，各攻一翼之势。}(1) +0001(1) +7170{当然之着，可摆脱红车的牵制。如果走车1平2，则车八进四，黑方因单马护守中卒而不能炮2平1自然邀兑。红方占优。}(1) +3646{带有欺骗性的弃兵，意在强行打开局面，实施快攻战术。通常红方多走车八进四或车二进四。}(1) +5646{黑方去兵，当仁不让。如改走马6进4，红将兵三进一！马4进3，车八进二，炮6进5，马三进四，黑方得子受攻，形势不利。}(1) +0141{如图形势，面对红方的捉卒，黑方主要有两种应着：（甲）卒7进1；（乙）卒7平8。现分述如下：}(2) +4636{冲卒捉马，看起来是一步绝对先手，但却流于习俗，正为红方所算。}(1) +4647{平卒拦车，意在延缓红方攻势，取舍异常果断，有“四两拨千斤”之妙！}(1) +0757{！\n进车捉马，战术紧逼，乃预谋的攻着。}(1) +2647(1) +3626{另有两种选择：(1)马6退7，车二平三，车9进2，车三退二，红方主动；(2)马6退5，马三退一，黑方虽有一卒过河，但阵形呆滞，红方占有主动。}(1) +9897{佳着，可顺势抢先。}(1) +5755(1) +0727{高车生根，可立即迫兑黑方河口马，着法及时，否则纠缠下去于红方无益。}(1) +9384(1) +9747(1) +3242{依仗出子优势，红方继续贯彻强攻计划。若改走炮七平三，则象3进5，局面较为平稳，红方略占先手。}(1) +2747(1) +9274(1) +5547(1) +4252{！}(1) +4147(1) +6252{对黑方消极的象5进3，红有马九进七下伏马七进六或马七进五等手段，将全线出击。}(1) +9274{经过转换，烟消云散，双方趋于平稳。}(1) +2272(1) +3040(1) +7572(1) +9384{补士固防，稳正之着，当然不宜走卒3进1，否则红将兵七进一乘势进攻。}(1) +2464(1) +2262(1) +7202(1) +6858{细致的一手，不给红方炮七平一打卒的机会。}(1) +0314{红方持有中炮攻势，占有优势。} +2041(1) +9093{双方大致均势。\n\n\n［小结］对于红方所施的骗着，黑方（甲）变不够明智，遭到了红方的猛攻，处境不妙。（乙）变黑方妙用平卒巧着，有效地遏制了红方攻势，双方平分秋色。\n\n在本局中。红方的布局骗着具有快速突击的特点。对此，黑方愈是用强，红势则愈旺。黑若能冷静对待，并采取（乙）变着法，延缓红势的策略，可安然无恙。} ",
            "{这是一局炮斗车卒的范例。对车炮的运用颇有启迪，可资借鉴。}(1) +2242(3) +3430(2) +3433(1) +3484(1) +2425{献炮叫将，伏车八平四白脸将成杀，是获胜的关键。}(1) +2427(1) +4244{红车占中是获胜的休着。黑不敢平车邀兑，否则，红车五平四胜。}(1) +4247(1) +3534(1) +3034{将军。}(1) +9585(1) +8486(1) +4245(1) +2724{叫杀。}(1) +2425(1) +4797{红方升车再打将，使黑方车卒失去有机联系，是获胜的重要环节。}(1) +9594(1) +3430(1) +3534(1) +9585(1) +4544(2) +2427{“二打对一打”，红方不变作负。} +2526(1) +9737(3) +9495(1) +9493(1) +8595(1) +3536(1) +8636(1) +3525(1) +2526(1) +2535(1) +2636(1) +3747(1) +2425(1) +3735(1) +9585(1) +3010(1) +3424(1) +8595(1) +3525(1) +8584(1) +2636(1) +1404(1) +4424{红方胜定。} +4745(1) +3736 +3525{以下升车占中，海底捞月胜。} +3010{平炮再升炮打车，消灭小卒，催毁黑方中路屏障，是红方获胜的精华。}(1) +1013(1) +9594(1) +1404(1) +4434(1) +4544(1) +1015(1) +1323(1) +9493(1) +4434(1) +3494(1) +2423{红方胜定。} +1525(1) +9383(1) +3696(1) +3532{以下海底捞月红胜。} +2575(1) +3484(1) +8595(1) +8494(1) +9585(1) +9695(1) +7576(1) +9454(1) +8595(1) +5455 ",
            "(1) +0624(1) +7772(1) +0818(1) +9776(1) +1813(1) +9897(1) +1383(1) +9170(1) +8381(1) +7101(1) +0001(1) +9674(1) +2120(1) +9584(1) +0171(1) +7273(1) +0715(1) +9757(1) +3040(1) +7363(1) +8183(1) +6373(1) +2060(1) +7091(1) +6050(1) +5755(1) +1507(1) +6656(1) +8381{红得子大优} ",
            "(1) +3242(1) +7172(1) +2724(1) +9274(1) +0726(1) +6252(1) +0120(1) +5242(1) +0807(1) +9888(1) +0001(1) +8883(1) +0314(1) +9384(1) +0747(1) +6656(1) +4742(1) +9776(1) +3646(1) +5646(1) +4246(1) +6050(1) +2123(1) +9170(1) +0171(1) +7655(1) +7161(1) +8343(1) +4645(1) +7776(1) +0628(1) +4353(1) +2464(1) +5352(1) +0224(1) +9091(1) +6191(1) +7091(1) +2647(1) +5547(1) +4547(1) +7292(1) +4746(1) +9272(1) +3040(1) +9183(1) +6465(1) +5255(1) +6567(1) +8364(1) +4641(1) +5040(1) +4140(1) +6452(1) +4090(1) +7292(1) +2032(1) +5554(1) +3444(1) +5244 ",
        };
        string[] expectManualStrings = {
            "[id \"1\"]\n[source \"https://www.xqbase.com/xqbase/?gameid=1\"]\n[title \"农民体协 毕彬彬 先胜 广东 文静 - 象棋巫师棋谱仓库\"]\n[game \"2000年全国象棋个人锦标赛\"]\n[date \"2000年11月\"]\n[site \"\"]\n[black \"广东 文静\"]\n[rowCols \"7774012297762725989707266252364671510624917226459384051474732141927408069757304072534553575341457371252651610607535545055515344490930737151626289313001061111020111420211444373464542171162628182624031424142214131404031434719184939193948493633414182814342821444105655444656044454050343260703202031302121303122221312232312176975060323121273137272537352527524227577456578742327076353363333233\"]\n[red \"农民体协 毕彬彬\"]\n[eccoSn \"B35\"]\n[eccoName \"中炮巡河炮对反宫马\"]\n[win \"红胜\"]\n\n(1) +2724(1) +9172(1) +0726(1) +7775(1) +0807(1) +9776(1) +3242(1) +6656(1) +2141(1) +9674(1) +0122(1) +7655(1) +0314(1) +9584(1) +2423(1) +7151(1) +0224(1) +9896(1) +0747(1) +6050(1) +2243(1) +5543(1) +4743(1) +5155(1) +2321(1) +7576(1) +4131(1) +9697(1) +4345(1) +5595(1) +4585(1) +6454(1) +0003(1) +9767(1) +8586(1) +7678(1) +0383(1) +9080(1) +3181(1) +8070(1) +8184(1) +7071(1) +8454(1) +6764(1) +3444(1) +7121(1) +8676(1) +7888(1) +7674(1) +9384(1) +7484(1) +7284(1) +8384(1) +9493(1) +8464(1) +2101(1) +1403(1) +0103(1) +0414(1) +0333(1) +6484(1) +8878(1) +8464(1) +7871(1) +5451(1) +9535(1) +4454(1) +3530(1) +5455(1) +5040(1) +6462(1) +3020(1) +6292(1) +9383(1) +9282(1) +8393(1) +8272(1) +7161(1) +7262(1) +6171(1) +2607(1) +4030(1) +6261(1) +7177(1) +6167(1) +7775(1) +6765(1) +7577(1) +4252(1) +7747(1) +2446(1) +4717(1) +5262(1) +2026(1) +6563(1) +3363(1) +6263 ",
            "[id \"2\"]\n[source \"https://www.xqbase.com/xqbase/?gameid=2\"]\n[title \"农民体协 毕彬彬 先负 浙江 金海英 - 象棋巫师棋谱仓库\"]\n[game \"2000年全国象棋个人锦标赛\"]\n[date \"2000年11月\"]\n[site \"\"]\n[black \"浙江 金海英\"]\n[rowCols \"77740726977608079897364697370122625200109172101371702161665646563736276736566766967826456454062490911363725163625444344460500737958403145130223474343734706061513051666050404454927445375626546478966474967437452656347451324566769574343213343313213334211334441332443432133433132133345242665456766068767862639171684895744844715154739495636578754445515565758475347493847484\"]\n[red \"农民体协 毕彬彬\"]\n[eccoSn \"C33\"]\n[eccoName \"中炮过河车互进七兵对屏风马右横车\"]\n[win \"黑胜\"]\n\n(1) +2724(1) +9776(1) +0726(1) +9897(1) +0807(1) +6656(1) +0767(1) +9172(1) +3242(1) +9080(1) +0122(1) +8083(1) +2120(1) +7131(1) +3646(1) +5646(1) +6766(1) +7737(1) +6646(1) +3736(1) +0628(1) +7655(1) +3444(1) +9674(1) +0001(1) +8333(1) +2241(1) +3332(1) +4454(1) +6454(1) +3040(1) +9767(1) +0514(1) +9384(1) +4160(1) +7264(1) +2464(1) +6764(1) +2030(1) +3141(1) +6041(1) +3630(1) +4050(1) +5444(1) +0224(1) +5567(1) +4676(1) +4434(1) +2806(1) +3424(1) +0624(1) +6755(1) +7646(1) +6424(1) +4162(1) +5536(1) +2605(1) +2464(1) +6283(1) +6463(1) +8371(1) +6364(1) +7183(1) +6454(1) +8362(1) +5464(1) +6283(1) +6463(1) +8371(1) +6364(1) +4252(1) +3644(1) +4626(1) +3038(1) +2628(1) +3233(1) +0121(1) +3858(1) +0524(1) +5854(1) +2141(1) +4423(1) +0405(1) +3335(1) +2825(1) +5455(1) +4145(1) +3525(1) +1425(1) +6424(1) +0314(1) +2414 ",
            "[id \"3\"]\n[source \"https://www.xqbase.com/xqbase/?gameid=3\"]\n[title \"农民体协 毕彬彬 先胜 山西 黄芳 - 象棋巫师棋谱仓库\"]\n[game \"2000年全国象棋个人锦标赛\"]\n[date \"2000年11月\"]\n[site \"\"]\n[black \"山西 黄芳\"]\n[rowCols \"777401229776272598970726665632429170062471720001909121616252425256463646975761629190256557522243525343517062517290706562707262426454051454443444531301217655080555432645432442927292452492020535024244541363243263623211424421514446512146063505065621516212053595843848747735057717545556365161121161601727052527376068\"]\n[red \"农民体协 毕彬彬\"]\n[eccoSn \"B56\"]\n[eccoName \"五七炮互进三兵对反宫马 红弃双兵对黑右炮过河\"]\n[win \"红胜\"]\n\n(1) +2724(1) +9172(1) +0726(1) +7775(1) +0807(1) +9776(1) +3646(1) +6252(1) +0120(1) +9674(1) +2122(1) +9091(1) +0001(1) +7131(1) +3242(1) +5242(1) +4656(1) +6656(1) +0747(1) +3132(1) +0100(1) +7535(1) +4742(1) +7253(1) +4243(1) +5341(1) +2032(1) +4122(1) +0020(1) +3532(1) +2022(1) +3252(1) +3444(1) +9584(1) +4454(1) +6454(1) +4383(1) +9171(1) +2645(1) +9895(1) +4553(1) +7655(1) +5374(1) +5202(1) +2202(1) +5574(1) +0292(1) +9565(1) +9252(1) +5444(1) +8333(1) +7462(1) +3332(1) +6281(1) +5254(1) +7141(1) +5456(1) +4171(1) +5696(1) +6595(1) +9646(1) +7141(1) +3282(1) +9565(1) +0514(1) +6858(1) +2427(1) +6595(1) +2787(1) +4445(1) +4666(1) +4131(1) +8281(1) +3130(1) +8777(1) +9575(1) +7767(1) +3038 ",
            "[id \"4\"]\n[source \"https://www.xqbase.com/xqbase/?gameid=4\"]\n[title \"沈阳 卜风波 先负 沈阳 苗永鹏 - 象棋巫师棋谱仓库\"]\n[game \"2000年全国象棋个人锦标赛\"]\n[date \"2000年11月\"]\n[site \"\"]\n[black \"沈阳 苗永鹏\"]\n[rowCols \"7774072697760807989701226656324271310624313221819080000197878171765501313236315180855153564671515576051485835383878324468353511191721112765727477476070592740535363735377626372726362725577647177284462453552527553527377655121164542243544443628492344435344454363037345534119130326281948417673426676484836463838442523237526237878160927160728485729385849372848562617192546474566368263868659273651587777253735491963817537295846465543572517747969085955143474490404440433517361525567861629594626340443523444723423644425444251425\"]\n[red \"沈阳 卜风波\"]\n[eccoSn \"C98\"]\n[eccoName \"五八炮互进三兵对屏风马 红平炮压马\"]\n[win \"黑胜\"]\n\n(1) +2724(1) +9776(1) +0726(1) +9897(1) +0807(1) +9172(1) +3646(1) +6252(1) +2161(1) +9674(1) +6162(1) +7111(1) +0010(1) +9091(1) +0717(1) +1121(1) +2645(1) +9161(1) +6266(1) +6141(1) +1015(1) +4143(1) +4656(1) +2141(1) +4526(1) +9584(1) +1513(1) +4313(1) +1713(1) +7456(1) +1343(1) +4181(1) +0122(1) +8182(1) +2647(1) +7757(1) +2426(1) +9795(1) +0224(1) +9565(1) +6667(1) +6567(1) +2676(1) +6777(1) +7666(1) +7775(1) +4726(1) +5787(1) +2214(1) +5674(1) +4345(1) +7577(1) +4565(1) +7767(1) +2645(1) +8281(1) +3444(1) +7253(1) +4454(1) +5332(1) +1402(1) +6454(1) +6564(1) +5444(1) +6660(1) +6764(1) +4564(1) +8101(1) +6062(1) +3211(1) +0414(1) +8737(1) +6476(1) +3734(1) +1413(1) +3433(1) +1314(1) +5242(1) +6267(1) +4232(1) +6717(1) +1130(1) +0221(1) +3022(1) +1415(1) +2203(1) +1514(1) +0322(1) +1415(1) +3231(1) +2102(1) +4434(1) +2446(1) +3338(1) +7668(1) +3835(1) +0223(1) +3585(1) +1727(1) +2243(1) +2344(1) +0106(1) +6887(1) +4322(1) +0514(1) +3435(1) +4465(1) +2241(1) +2757(1) +0600(1) +1505(1) +4153(1) +5754(1) +0050(1) +5450(1) +5365(1) +8766(1) +8575(1) +4628(1) +3132(1) +0504(1) +3233(1) +5054(1) +6573(1) +5457(1) +7352(1) +6654(1) +5244(1) +5475(1) +8475 ",
            "[id \"5\"]\n[source \"https://www.xqbase.com/xqbase/?gameid=5\"]\n[title \"沈阳 卜风波 先胜 火车头 宋国强 - 象棋巫师棋谱仓库\"]\n[game \"2000年全国象棋个人锦标赛\"]\n[date \"2000年11月\"]\n[site \"\"]\n[black \"火车头 宋国强\"]\n[rowCols \"777407269776080798970122665632429170031471310624908030403136405060500050564621418085414697575057765727285745281874760727705114257670221051300514453310313314464492743150852504143022140422344446342218143630146493844644300002207020\"]\n[red \"沈阳 卜风波\"]\n[eccoSn \"C94\"]\n[eccoName \"五八炮互进三兵对屏风马 红左边马对黑上士\"]\n[win \"红胜\"]\n\n(1) +2724(1) +9776(1) +0726(1) +9897(1) +0807(1) +9172(1) +3646(1) +6252(1) +0120(1) +9384(1) +2161(1) +9674(1) +0010(1) +6050(1) +6166(1) +5040(1) +3040(1) +9040(1) +4656(1) +7151(1) +1015(1) +5156(1) +0747(1) +4047(1) +2647(1) +7778(1) +4755(1) +7888(1) +2426(1) +9777(1) +2041(1) +8475(1) +2620(1) +7280(1) +4160(1) +9584(1) +5563(1) +8061(1) +6384(1) +5654(1) +0224(1) +6140(1) +1575(1) +9484(1) +6072(1) +8494(1) +7264(1) +5456(1) +6472(1) +8884(1) +6660(1) +8434(1) +0314(1) +5654(1) +6090(1) +9270(1) +2070 ",

            "[id \"6\"]\n[source \"01.xqf\"]\n[title \"\u0006第01局\"]\n[game \"\"]\n[date \"\"]\n[site \"\"]\n[black \"\"]\n[rowCols \"7556930356440304262514254436\"]\n[red \"\"]\n[win \"红胜\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n[type \"残局\"]\n[version \"18\"]\n[FEN \"5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5 r - - 0 1\"]\n[moveString \"(2) +7556{从相肩进马是取胜的正确途径。其它着法，均不能取胜。}(4) +7554(1) +9303(1) +9323(1) +1425(1) +3848(1) +9303(1) +5644{不怕黑炮平中拴链，进观的攻势含蓄双有诱惑性，是红方制胜的关键。}(1) +5644(2) +5637{叫杀得车。} +5644(1) +5446(1) +0304(1) +2324(1) +1817(1) +1838(1) +0304(1) +2625{弃车，与前着相联系，由此巧妙成杀。}(1) +4436 +2625(1) +2616 +8483(1) +1425(1) +1425(2) +1817(1) +4436 +4436 +4423 +4627(1) +1727(1) +2627(1) +1403(1) +2717(1) +1525{至此，形成少见的高将底炮双士和单车的局面。}(1) +1737(1) +0514(1) +3735(1) +2524(1) +3538(1) +2425(1) +3835(1) +2524(1) +8393(1) +0405{和棋。} \"]\n\n(2) +7556{从相肩进马是取胜的正确途径。其它着法，均不能取胜。}(4) +7554(1) +9303(1) +9323(1) +1425(1) +3848(1) +9303(1) +5644{不怕黑炮平中拴链，进观的攻势含蓄双有诱惑性，是红方制胜的关键。}(1) +5644(2) +5637{叫杀得车。} +5644(1) +5446(1) +0304(1) +2324(1) +1817(1) +1838(1) +0304(1) +2625{弃车，与前着相联系，由此巧妙成杀。}(1) +4436 +2625(1) +2616 +8483(1) +1425(1) +1425(2) +1817(1) +4436 +4436 +4423 +4627(1) +1727(1) +2627(1) +1403(1) +2717(1) +1525{至此，形成少见的高将底炮双士和单车的局面。}(1) +1737(1) +0514(1) +3735(1) +2524(1) +3538(1) +2425(1) +3835(1) +2524(1) +8393(1) +0405{和棋。} ",
            "[id \"7\"]\n[source \"4四量拨千斤.xqf\"]\n[title \"四量拨千斤\"]\n[game \"\"]\n[date \"\"]\n[site \"\"]\n[black \"\"]\n[rowCols \"77740122977627259897072691703646717226459091212066564656915156669747667647450314625202245242324272222522743422929384\"]\n[red \"\"]\n[win \"未知\"]\n[opening \"\"]\n[writer \"阎文清 张强\"]\n[author \"\b橘子黄了\"]\n[type \"全局\"]\n[version \"10\"]\n[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[moveString \"(1) +7774(1) +0122(1) +9776(1) +2725(1) +9897(1) +0726(1) +9170(1) +3646{红方左马屯边，是一种老式的攻法，优点是大子出动速度较快，不利之处是双马位置欠佳，易成弱点。\n\n黑方挺卒活马，应着稳正，是对付红方边马阵形的有效战术。}(1) +7172{平炮七线，意在加强对黑方3路线的压力，但阵营不够稳固，容易给黑方提供骚扰反击的机会。如改走炮八平六，则相对来讲要稳健一些。}(1) +2645{黑方迅即跃马，有随时马6进4入侵的手段，是一种牵制战术。此外另有车1平2的选择，以下形成车九平八，炮2进4，车二进六，各攻一翼之势。}(1) +9091(1) +2120{当然之着，可摆脱红车的牵制。如果走车1平2，则车八进四，黑方因单马护守中卒而不能炮2平1自然邀兑。红方占优。}(1) +6656{带有欺骗性的弃兵，意在强行打开局面，实施快攻战术。通常红方多走车八进四或车二进四。}(1) +4656{黑方去兵，当仁不让。如改走马6进4，红将兵三进一！马4进3，车八进二，炮6进5，马三进四，黑方得子受攻，形势不利。}(1) +9151{如图形势，面对红方的捉卒，黑方主要有两种应着：（甲）卒7进1；（乙）卒7平8。现分述如下：}(2) +5666{冲卒捉马，看起来是一步绝对先手，但却流于习俗，正为红方所算。}(1) +5657{平卒拦车，意在延缓红方攻势，取舍异常果断，有“四两拨千斤”之妙！}(1) +9747{！\n进车捉马，战术紧逼，乃预谋的攻着。}(1) +7657(1) +6676{另有两种选择：(1)马6退7，车二平三，车9进2，车三退二，红方主动；(2)马6退5，马三退一，黑方虽有一卒过河，但阵形呆滞，红方占有主动。}(1) +0807{佳着，可顺势抢先。}(1) +4745(1) +9777{高车生根，可立即迫兑黑方河口马，着法及时，否则纠缠下去于红方无益。}(1) +0314(1) +0757(1) +6252{依仗出子优势，红方继续贯彻强攻计划。若改走炮七平三，则象3进5，局面较为平稳，红方略占先手。}(1) +7757(1) +0224(1) +4557(1) +5242{！}(1) +5157(1) +3242{对黑方消极的象5进3，红有马九进七下伏马七进六或马七进五等手段，将全线出击。}(1) +0224{经过转换，烟消云散，双方趋于平稳。}(1) +7222(1) +6050(1) +2522(1) +0314{补士固防，稳正之着，当然不宜走卒3进1，否则红将兵七进一乘势进攻。}(1) +7434(1) +7232(1) +2292(1) +3848{细致的一手，不给红方炮七平一打卒的机会。}(1) +9384{红方持有中炮攻势，占有优势。} +7051(1) +0003{双方大致均势。\n\n\n［小结］对于红方所施的骗着，黑方（甲）变不够明智，遭到了红方的猛攻，处境不妙。（乙）变黑方妙用平卒巧着，有效地遏制了红方攻势，双方平分秋色。\n\n在本局中。红方的布局骗着具有快速突击的特点。对此，黑方愈是用强，红势则愈旺。黑若能冷静对待，并采取（乙）变着法，延缓红势的策略，可安然无恙。} \"]\n\n(1) +7774(1) +0122(1) +9776(1) +2725(1) +9897(1) +0726(1) +9170(1) +3646{红方左马屯边，是一种老式的攻法，优点是大子出动速度较快，不利之处是双马位置欠佳，易成弱点。\n\n黑方挺卒活马，应着稳正，是对付红方边马阵形的有效战术。}(1) +7172{平炮七线，意在加强对黑方3路线的压力，但阵营不够稳固，容易给黑方提供骚扰反击的机会。如改走炮八平六，则相对来讲要稳健一些。}(1) +2645{黑方迅即跃马，有随时马6进4入侵的手段，是一种牵制战术。此外另有车1平2的选择，以下形成车九平八，炮2进4，车二进六，各攻一翼之势。}(1) +9091(1) +2120{当然之着，可摆脱红车的牵制。如果走车1平2，则车八进四，黑方因单马护守中卒而不能炮2平1自然邀兑。红方占优。}(1) +6656{带有欺骗性的弃兵，意在强行打开局面，实施快攻战术。通常红方多走车八进四或车二进四。}(1) +4656{黑方去兵，当仁不让。如改走马6进4，红将兵三进一！马4进3，车八进二，炮6进5，马三进四，黑方得子受攻，形势不利。}(1) +9151{如图形势，面对红方的捉卒，黑方主要有两种应着：（甲）卒7进1；（乙）卒7平8。现分述如下：}(2) +5666{冲卒捉马，看起来是一步绝对先手，但却流于习俗，正为红方所算。}(1) +5657{平卒拦车，意在延缓红方攻势，取舍异常果断，有“四两拨千斤”之妙！}(1) +9747{！\n进车捉马，战术紧逼，乃预谋的攻着。}(1) +7657(1) +6676{另有两种选择：(1)马6退7，车二平三，车9进2，车三退二，红方主动；(2)马6退5，马三退一，黑方虽有一卒过河，但阵形呆滞，红方占有主动。}(1) +0807{佳着，可顺势抢先。}(1) +4745(1) +9777{高车生根，可立即迫兑黑方河口马，着法及时，否则纠缠下去于红方无益。}(1) +0314(1) +0757(1) +6252{依仗出子优势，红方继续贯彻强攻计划。若改走炮七平三，则象3进5，局面较为平稳，红方略占先手。}(1) +7757(1) +0224(1) +4557(1) +5242{！}(1) +5157(1) +3242{对黑方消极的象5进3，红有马九进七下伏马七进六或马七进五等手段，将全线出击。}(1) +0224{经过转换，烟消云散，双方趋于平稳。}(1) +7222(1) +6050(1) +2522(1) +0314{补士固防，稳正之着，当然不宜走卒3进1，否则红将兵七进一乘势进攻。}(1) +7434(1) +7232(1) +2292(1) +3848{细致的一手，不给红方炮七平一打卒的机会。}(1) +9384{红方持有中炮攻势，占有优势。} +7051(1) +0003{双方大致均势。\n\n\n［小结］对于红方所施的骗着，黑方（甲）变不够明智，遭到了红方的猛攻，处境不妙。（乙）变黑方妙用平卒巧着，有效地遏制了红方攻势，双方平分秋色。\n\n在本局中。红方的布局骗着具有快速突击的特点。对此，黑方愈是用强，红势则愈旺。黑若能冷静对待，并采取（乙）变着法，延缓红势的策略，可安然无恙。} ",
            "[id \"8\"]\n[source \"第09局.xqf\"]\n[title \"\u0006第09局\"]\n[game \"\"]\n[date \"\"]\n[site \"\"]\n[black \"\"]\n[rowCols \"725264607475656452550504555404057576051576666080849480855464857566067525641415051404051506052526044415054445\"]\n[red \"\"]\n[win \"红胜\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n[type \"残局\"]\n[version \"18\"]\n[FEN \"5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9 r - - 0 1\"]\n[moveString \"{这是一局炮斗车卒的范例。对车炮的运用颇有启迪，可资借鉴。}(1) +7252(3) +6460(2) +6463(1) +6414(1) +7475{献炮叫将，伏车八平四白脸将成杀，是获胜的关键。}(1) +7477(1) +5254{红车占中是获胜的休着。黑不敢平车邀兑，否则，红车五平四胜。}(1) +5257(1) +6564(1) +6064{将军。}(1) +0515(1) +1416(1) +5255(1) +7774{叫杀。}(1) +7475(1) +5707{红方升车再打将，使黑方车卒失去有机联系，是获胜的重要环节。}(1) +0504(1) +6460(1) +6564(1) +0515(1) +5554(2) +7477{“二打对一打”，红方不变作负。} +7576(1) +0767(3) +0405(1) +0403(1) +1505(1) +6566(1) +1666(1) +6575(1) +7576(1) +7565(1) +7666(1) +6757(1) +7475(1) +6765(1) +0515(1) +6080(1) +6474(1) +1505(1) +6575(1) +1514(1) +7666(1) +8494(1) +5474{红方胜定。} +5755(1) +6766 +6575{以下升车占中，海底捞月胜。} +6080{平炮再升炮打车，消灭小卒，催毁黑方中路屏障，是红方获胜的精华。}(1) +8083(1) +0504(1) +8494(1) +5464(1) +5554(1) +8085(1) +8373(1) +0403(1) +5464(1) +6404(1) +7473{红方胜定。} +8575(1) +0313(1) +6606(1) +6562{以下海底捞月红胜。} +7525(1) +6414(1) +1505(1) +1404(1) +0515(1) +0605(1) +2526(1) +0444(1) +1505(1) +4445 \"]\n\n{这是一局炮斗车卒的范例。对车炮的运用颇有启迪，可资借鉴。}(1) +7252(3) +6460(2) +6463(1) +6414(1) +7475{献炮叫将，伏车八平四白脸将成杀，是获胜的关键。}(1) +7477(1) +5254{红车占中是获胜的休着。黑不敢平车邀兑，否则，红车五平四胜。}(1) +5257(1) +6564(1) +6064{将军。}(1) +0515(1) +1416(1) +5255(1) +7774{叫杀。}(1) +7475(1) +5707{红方升车再打将，使黑方车卒失去有机联系，是获胜的重要环节。}(1) +0504(1) +6460(1) +6564(1) +0515(1) +5554(2) +7477{“二打对一打”，红方不变作负。} +7576(1) +0767(3) +0405(1) +0403(1) +1505(1) +6566(1) +1666(1) +6575(1) +7576(1) +7565(1) +7666(1) +6757(1) +7475(1) +6765(1) +0515(1) +6080(1) +6474(1) +1505(1) +6575(1) +1514(1) +7666(1) +8494(1) +5474{红方胜定。} +5755(1) +6766 +6575{以下升车占中，海底捞月胜。} +6080{平炮再升炮打车，消灭小卒，催毁黑方中路屏障，是红方获胜的精华。}(1) +8083(1) +0504(1) +8494(1) +5464(1) +5554(1) +8085(1) +8373(1) +0403(1) +5464(1) +6404(1) +7473{红方胜定。} +8575(1) +0313(1) +6606(1) +6562{以下海底捞月红胜。} +7525(1) +6414(1) +1505(1) +1404(1) +0515(1) +0605(1) +2526(1) +0444(1) +1505(1) +4445 ",
            "[id \"9\"]\n[source \"布局陷阱--飞相局对金钩炮.xqf\"]\n[title \"\u0018布局陷阱--飞相局对金钩炮\"]\n[game \"\u0018布局陷阱--飞相局对金钩炮\"]\n[date \"\"]\n[site \"\"]\n[black \"\"]\n[rowCols \"96742722988807268883080783130120131121919091062471700514912122239785074760502333111333237030200130404745859736461311\"]\n[red \"\"]\n[win \"红胜\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n[type \"全局\"]\n[version \"12\"]\n[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[moveString \"(1) +9674(1) +2722(1) +9888(1) +0726(1) +8883(1) +0807(1) +8313(1) +0120(1) +1311(1) +2191(1) +9091(1) +0624(1) +7170(1) +0514(1) +9121(1) +2223(1) +9785(1) +0747(1) +6050(1) +2333(1) +1113(1) +3323(1) +7030(1) +2001(1) +3040(1) +4745(1) +8597(1) +3646(1) +1311{红得子大优} \"]\n\n(1) +9674(1) +2722(1) +9888(1) +0726(1) +8883(1) +0807(1) +8313(1) +0120(1) +1311(1) +2191(1) +9091(1) +0624(1) +7170(1) +0514(1) +9121(1) +2223(1) +9785(1) +0747(1) +6050(1) +2333(1) +1113(1) +3323(1) +7030(1) +2001(1) +3040(1) +4745(1) +8597(1) +3646(1) +1311{红得子大优} ",
            "[id \"10\"]\n[source \"- 北京张强 (和) 上海胡荣华 (1993.4.27于南京).xqf\"]\n[title \"挺兵对卒底炮\"]\n[game \"\u001093全国象棋锦标赛\"]\n[date \"1993.4.27\"]\n[site \"\u0004南京\"]\n[black \"上海胡荣华\"]\n[rowCols \"625221227774022497763242917042529897081890911813938403149757364657520726665646565256304071730120912126452131135356552726967853437434434292740001310120017657455755572202575602226050011334354245353713345651405051503442500022027062454464544254\"]\n[red \"\b北京张强\"]\n[win \"和棋\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n[type \"全局\"]\n[version \"13\"]\n[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[moveString \"(1) +6252(1) +2122(1) +7774(1) +0224(1) +9776(1) +3242(1) +9170(1) +4252(1) +9897(1) +0818(1) +9091(1) +1813(1) +9384(1) +0314(1) +9757(1) +3646(1) +5752(1) +0726(1) +6656(1) +4656(1) +5256(1) +3040(1) +7173(1) +0120(1) +9121(1) +2645(1) +2131(1) +1353(1) +5655(1) +2726(1) +9678(1) +5343(1) +7434(1) +4342(1) +9274(1) +0001(1) +3101(1) +2001(1) +7657(1) +4557(1) +5557(1) +2202(1) +5756(1) +0222(1) +6050(1) +0113(1) +3435(1) +4245(1) +3537(1) +1334(1) +5651(1) +4050(1) +5150(1) +3442(1) +5000(1) +2202(1) +7062(1) +4544(1) +6454(1) +4254 \"]\n\n(1) +6252(1) +2122(1) +7774(1) +0224(1) +9776(1) +3242(1) +9170(1) +4252(1) +9897(1) +0818(1) +9091(1) +1813(1) +9384(1) +0314(1) +9757(1) +3646(1) +5752(1) +0726(1) +6656(1) +4656(1) +5256(1) +3040(1) +7173(1) +0120(1) +9121(1) +2645(1) +2131(1) +1353(1) +5655(1) +2726(1) +9678(1) +5343(1) +7434(1) +4342(1) +9274(1) +0001(1) +3101(1) +2001(1) +7657(1) +4557(1) +5557(1) +2202(1) +5756(1) +0222(1) +6050(1) +0113(1) +3435(1) +4245(1) +3537(1) +1334(1) +5651(1) +4050(1) +5150(1) +3442(1) +5000(1) +2202(1) +7062(1) +4544(1) +6454(1) +4254 ",
        };

        Manual manual = GetManualFromId(index + 1);

        string result = manual.GetMoveString(FileExtType.txt, ChangeType.Symmetry_V);
        Assert.Equal(expectMoveStrings[index], result);

        result = manual.GetString(FileExtType.txt, index < 5 ? ChangeType.Symmetry_V : ChangeType.NoChange);
        Assert.Equal(expectManualStrings[index], result);
    }

    // [Fact]
    // public void TestEcco()
    // {
    // 下载、存储开局网页信息
    // using var writer = File.CreateText("EccoHtmls.txt");
    // string eccoHtmlsString = Database.DownEccoHtmls();
    // writer.Write(eccoHtmlsString);

    // 读取开局网页信息，解析成开局数据
    // using var reader = File.OpenText("EccoHtmls.txt");
    // using var writer = File.CreateText("eccoDict.txt");
    // string eccoHtmlsString = reader.ReadToEnd();
    // foreach (var kv in Database.GetEccoRecords(eccoHtmlsString))
    //     writer.Write($"{string.Concat(kv.Value.Select(str => str))}\n");

    //     db.InitEccoData();
    // }

}
