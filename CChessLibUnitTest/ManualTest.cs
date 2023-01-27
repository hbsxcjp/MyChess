using CChess;

namespace CChessTest;

public class ManualTest
{
    //"中炮对屏风马",
    //"中炮【马8进7】",
    //"黑用开局库",
    //"仙人指路全集（史上最全最新版）",
    //"飞相局【卒7进1】",
    //"中炮【马2进3】"

    private static string[,] manualStrings =  {
        {"01",
        "[FEN \"5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5 r - - 0 1\"]\n[version \"18\"]\n[win \"红胜\"]\n[type \"残局\"]\n[title \"\u0006第01局\"]\n[event \"\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n\n(2) +2546{从相肩进马是取胜的正确途径。其它着法，均不能取胜。}(4) +2544(1) +0393(1) +0373(1) +8475(1) +6858(1) +0393(1) +4654{不怕黑炮平中拴链，进观的攻势含蓄双有诱惑性，是红方制胜的关键。}(1) +4654(2) +4667{叫杀得车。} +4654(1) +4456(1) +9394(1) +7374(1) +8887(1) +8868(1) +9394(1) +7675{弃车，与前着相联系，由此巧妙成杀。}(1) +5466 +7675(1) +7686 +1413(1) +8475(1) +8475(2) +8887(1) +5466 +5466 +5473 +5677(1) +8777(1) +7677(1) +8493(1) +7787(1) +8575{至此，形成少见的高将底炮双士和单车的局面。}(1) +8767(1) +9584(1) +6765(1) +7574(1) +6568(1) +7475(1) +6865(1) +7574(1) +1303(1) +9495{和棋。} ",
        "[FEN \"5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5 r - - 0 1\"]\n[version \"18\"]\n[win \"红胜\"]\n[type \"残局\"]\n[title \"\u0006第01局\"]\n[event \"\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━┯━┯━士━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─士─将─┼─┼─車\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─车─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n相─╬─┼─┼─┼─马─┼─╬─相\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━砲━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n-[(-1,-1),(-1,-1)].0 \n\t0-[(2,5),(4,6)].1 从相肩进马是取胜的正确途径。其它着法，均不能取胜。\n\t0-[(2,5),(4,4)].2 \n\t\t1-[(0,3),(9,3)].3 \n\t\t1-[(0,3),(7,3)].4 \n\t\t1-[(8,4),(7,5)].5 \n\t\t1-[(6,8),(5,8)].6 \n\t\t2-[(0,3),(9,3)].7 \n\t\t\t3-[(4,6),(5,4)].8 不怕黑炮平中拴链，进观的攻势含蓄双有诱惑性，是红方制胜的关键。\n\t\t\t4-[(4,6),(5,4)].9 \n\t\t\t5-[(4,6),(6,7)].10 叫杀得车。\n\t\t\t6-[(4,6),(5,4)].11 \n\t\t\t7-[(4,4),(5,6)].12 \n\t\t\t\t8-[(9,3),(9,4)].13 \n\t\t\t\t9-[(7,3),(7,4)].14 \n\t\t\t\t9-[(8,8),(8,7)].15 \n\t\t\t\t11-[(8,8),(6,8)].16 \n\t\t\t\t12-[(9,3),(9,4)].17 \n\t\t\t\t\t13-[(7,6),(7,5)].18 弃车，与前着相联系，由此巧妙成杀。\n\t\t\t\t\t14-[(5,4),(6,6)].19 \n\t\t\t\t\t15-[(7,6),(7,5)].20 \n\t\t\t\t\t16-[(7,6),(8,6)].21 \n\t\t\t\t\t17-[(1,4),(1,3)].22 \n\t\t\t\t\t\t18-[(8,4),(7,5)].23 \n\t\t\t\t\t\t20-[(8,4),(7,5)].24 \n\t\t\t\t\t\t22-[(8,8),(8,7)].25 \n\t\t\t\t\t\t\t23-[(5,4),(6,6)].26 \n\t\t\t\t\t\t\t24-[(5,4),(6,6)].27 \n\t\t\t\t\t\t\t24-[(5,4),(7,3)].28 \n\t\t\t\t\t\t\t25-[(5,6),(7,7)].29 \n\t\t\t\t\t\t\t\t29-[(8,7),(7,7)].30 \n\t\t\t\t\t\t\t\t\t30-[(7,6),(7,7)].31 \n\t\t\t\t\t\t\t\t\t\t31-[(8,4),(9,3)].32 \n\t\t\t\t\t\t\t\t\t\t\t32-[(7,7),(8,7)].33 \n\t\t\t\t\t\t\t\t\t\t\t\t33-[(8,5),(7,5)].34 至此，形成少见的高将底炮双士和单车的局面。\n\t\t\t\t\t\t\t\t\t\t\t\t\t34-[(8,7),(6,7)].35 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t35-[(9,5),(8,4)].36 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t36-[(6,7),(6,5)].37 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t37-[(7,5),(7,4)].38 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t38-[(6,5),(6,8)].39 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t39-[(7,4),(7,5)].40 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t40-[(6,8),(6,5)].41 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t41-[(7,5),(7,4)].42 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t42-[(1,3),(0,3)].43 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t43-[(9,4),(9,5)].44 和棋。\n着法数量【44】\t注解数量【6】\t注解最长【31】\n\n"
        },
        {"4四量拨千斤",
        "[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[version \"10\"]\n[win \"未知\"]\n[type \"全局\"]\n[title \"四量拨千斤\"]\n[event \"\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"阎文清 张强\"]\n[author \"\b橘子黄了\"]\n\n(1) +2724(1) +9172(1) +0726(1) +7775(1) +0807(1) +9776(1) +0120(1) +6656{红方左马屯边，是一种老式的攻法，优点是大子出动速度较快，不利之处是双马位置欠佳，易成弱点。\n\n黑方挺卒活马，应着稳正，是对付红方边马阵形的有效战术。}(1) +2122{平炮七线，意在加强对黑方3路线的压力，但阵营不够稳固，容易给黑方提供骚扰反击的机会。如改走炮八平六，则相对来讲要稳健一些。}(1) +7655{黑方迅即跃马，有随时马6进4入侵的手段，是一种牵制战术。此外另有车1平2的选择，以下形成车九平八，炮2进4，车二进六，各攻一翼之势。}(1) +0001(1) +7170{当然之着，可摆脱红车的牵制。如果走车1平2，则车八进四，黑方因单马护守中卒而不能炮2平1自然邀兑。红方占优。}(1) +3646{带有欺骗性的弃兵，意在强行打开局面，实施快攻战术。通常红方多走车八进四或车二进四。}(1) +5646{黑方去兵，当仁不让。如改走马6进4，红将兵三进一！马4进3，车八进二，炮6进5，马三进四，黑方得子受攻，形势不利。}(1) +0141{如图形势，面对红方的捉卒，黑方主要有两种应着：（甲）卒7进1；（乙）卒7平8。现分述如下：}(2) +4636{冲卒捉马，看起来是一步绝对先手，但却流于习俗，正为红方所算。}(1) +4647{平卒拦车，意在延缓红方攻势，取舍异常果断，有“四两拨千斤”之妙！}(1) +0757{！\n进车捉马，战术紧逼，乃预谋的攻着。}(1) +2647(1) +3626{另有两种选择：(1)马6退7，车二平三，车9进2，车三退二，红方主动；(2)马6退5，马三退一，黑方虽有一卒过河，但阵形呆滞，红方占有主动。}(1) +9897{佳着，可顺势抢先。}(1) +5755(1) +0727{高车生根，可立即迫兑黑方河口马，着法及时，否则纠缠下去于红方无益。}(1) +9384(1) +9747(1) +3242{依仗出子优势，红方继续贯彻强攻计划。若改走炮七平三，则象3进5，局面较为平稳，红方略占先手。}(1) +2747(1) +9274(1) +5547(1) +4252{！}(1) +4147(1) +6252{对黑方消极的象5进3，红有马九进七下伏马七进六或马七进五等手段，将全线出击。}(1) +9274{经过转换，烟消云散，双方趋于平稳。}(1) +2272(1) +3040(1) +7572(1) +9384{补士固防，稳正之着，当然不宜走卒3进1，否则红将兵七进一乘势进攻。}(1) +2464(1) +2262(1) +7202(1) +6858{细致的一手，不给红方炮七平一打卒的机会。}(1) +0314{红方持有中炮攻势，占有优势。} +2041(1) +9093{双方大致均势。\n\n\n［小结］对于红方所施的骗着，黑方（甲）变不够明智，遭到了红方的猛攻，处境不妙。（乙）变黑方妙用平卒巧着，有效地遏制了红方攻势，双方平分秋色。\n\n在本局中。红方的布局骗着具有快速突击的特点。对此，黑方愈是用强，红势则愈旺。黑若能冷静对待，并采取（乙）变着法，延缓红势的策略，可安然无恙。} ",
        "[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[version \"10\"]\n[win \"未知\"]\n[type \"全局\"]\n[title \"四量拨千斤\"]\n[event \"\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"阎文清 张强\"]\n[author \"\b橘子黄了\"]\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n車━馬━象━士━将━士━象━馬━車\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n车━马━相━仕━帅━仕━相━马━车\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n-[(-1,-1),(-1,-1)].0 \n\t0-[(2,7),(2,4)].1 \n\t\t1-[(9,1),(7,2)].2 \n\t\t\t2-[(0,7),(2,6)].3 \n\t\t\t\t3-[(7,7),(7,5)].4 \n\t\t\t\t\t4-[(0,8),(0,7)].5 \n\t\t\t\t\t\t5-[(9,7),(7,6)].6 \n\t\t\t\t\t\t\t6-[(0,1),(2,0)].7 \n\t\t\t\t\t\t\t\t7-[(6,6),(5,6)].8 红方左马屯边，是一种老式的攻法，优点是大子出动速度较快，不利之处是双马位置欠佳，易成弱点。\n\n黑方挺卒活马，应着稳正，是对付红方边马阵形的有效战术。\n\t\t\t\t\t\t\t\t\t8-[(2,1),(2,2)].9 平炮七线，意在加强对黑方3路线的压力，但阵营不够稳固，容易给黑方提供骚扰反击的机会。如改走炮八平六，则相对来讲要稳健一些。\n\t\t\t\t\t\t\t\t\t\t9-[(7,6),(5,5)].10 黑方迅即跃马，有随时马6进4入侵的手段，是一种牵制战术。此外另有车1平2的选择，以下形成车九平八，炮2进4，车二进六，各攻一翼之势。\n\t\t\t\t\t\t\t\t\t\t\t10-[(0,0),(0,1)].11 \n\t\t\t\t\t\t\t\t\t\t\t\t11-[(7,1),(7,0)].12 当然之着，可摆脱红车的牵制。如果走车1平2，则车八进四，黑方因单马护守中卒而不能炮2平1自然邀兑。红方占优。\n\t\t\t\t\t\t\t\t\t\t\t\t\t12-[(3,6),(4,6)].13 带有欺骗性的弃兵，意在强行打开局面，实施快攻战术。通常红方多走车八进四或车二进四。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t13-[(5,6),(4,6)].14 黑方去兵，当仁不让。如改走马6进4，红将兵三进一！马4进3，车八进二，炮6进5，马三进四，黑方得子受攻，形势不利。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t14-[(0,1),(4,1)].15 如图形势，面对红方的捉卒，黑方主要有两种应着：（甲）卒7进1；（乙）卒7平8。现分述如下：\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t15-[(4,6),(3,6)].16 冲卒捉马，看起来是一步绝对先手，但却流于习俗，正为红方所算。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t15-[(4,6),(4,7)].17 平卒拦车，意在延缓红方攻势，取舍异常果断，有“四两拨千斤”之妙！\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t16-[(0,7),(5,7)].18 ！\n进车捉马，战术紧逼，乃预谋的攻着。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t17-[(2,6),(4,7)].19 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t18-[(3,6),(2,6)].20 另有两种选择：(1)马6退7，车二平三，车9进2，车三退二，红方主动；(2)马6退5，马三退一，黑方虽有一卒过河，但阵形呆滞，红方占有主动。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t19-[(9,8),(9,7)].21 佳着，可顺势抢先。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t20-[(5,7),(5,5)].22 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t21-[(0,7),(2,7)].23 高车生根，可立即迫兑黑方河口马，着法及时，否则纠缠下去于红方无益。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t22-[(9,3),(8,4)].24 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t23-[(9,7),(4,7)].25 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t24-[(3,2),(4,2)].26 依仗出子优势，红方继续贯彻强攻计划。若改走炮七平三，则象3进5，局面较为平稳，红方略占先手。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t25-[(2,7),(4,7)].27 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t26-[(9,2),(7,4)].28 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t27-[(5,5),(4,7)].29 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t28-[(4,2),(5,2)].30 ！\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t29-[(4,1),(4,7)].31 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t30-[(6,2),(5,2)].32 对黑方消极的象5进3，红有马九进七下伏马七进六或马七进五等手段，将全线出击。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t31-[(9,2),(7,4)].33 经过转换，烟消云散，双方趋于平稳。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t32-[(2,2),(7,2)].34 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t33-[(3,0),(4,0)].35 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t34-[(7,5),(7,2)].36 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t35-[(9,3),(8,4)].37 补士固防，稳正之着，当然不宜走卒3进1，否则红将兵七进一乘势进攻。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t36-[(2,4),(6,4)].38 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t37-[(2,2),(6,2)].39 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t38-[(7,2),(0,2)].40 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t39-[(6,8),(5,8)].41 细致的一手，不给红方炮七平一打卒的机会。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t40-[(0,3),(1,4)].42 红方持有中炮攻势，占有优势。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t41-[(2,0),(4,1)].43 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t43-[(9,0),(9,3)].44 双方大致均势。\n\n\n［小结］对于红方所施的骗着，黑方（甲）变不够明智，遭到了红方的猛攻，处境不妙。（乙）变黑方妙用平卒巧着，有效地遏制了红方攻势，双方平分秋色。\n\n在本局中。红方的布局骗着具有快速突击的特点。对此，黑方愈是用强，红势则愈旺。黑若能冷静对待，并采取（乙）变着法，延缓红势的策略，可安然无恙。\n着法数量【44】\t注解数量【21】\t注解最长【152】\n\n"
        },
        {"第09局",
        "[FEN \"5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9 r - - 0 1\"]\n[version \"18\"]\n[win \"红胜\"]\n[type \"残局\"]\n[title \"\u0006第09局\"]\n[event \"\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n\n{这是一局炮斗车卒的范例。对车炮的运用颇有启迪，可资借鉴。}(1) +2242(3) +3430(2) +3433(1) +3484(1) +2425{献炮叫将，伏车八平四白脸将成杀，是获胜的关键。}(1) +2427(1) +4244{红车占中是获胜的休着。黑不敢平车邀兑，否则，红车五平四胜。}(1) +4247(1) +3534(1) +3034{将军。}(1) +9585(1) +8486(1) +4245(1) +2724{叫杀。}(1) +2425(1) +4797{红方升车再打将，使黑方车卒失去有机联系，是获胜的重要环节。}(1) +9594(1) +3430(1) +3534(1) +9585(1) +4544(2) +2427{“二打对一打”，红方不变作负。} +2526(1) +9737(3) +9495(1) +9493(1) +8595(1) +3536(1) +8636(1) +3525(1) +2526(1) +2535(1) +2636(1) +3747(1) +2425(1) +3735(1) +9585(1) +3010(1) +3424(1) +8595(1) +3525(1) +8584(1) +2636(1) +1404(1) +4424{红方胜定。} +4745(1) +3736 +3525{以下升车占中，海底捞月胜。} +3010{平炮再升炮打车，消灭小卒，催毁黑方中路屏障，是红方获胜的精华。}(1) +1013(1) +9594(1) +1404(1) +4434(1) +4544(1) +1015(1) +1323(1) +9493(1) +4434(1) +3494(1) +2423{红方胜定。} +1525(1) +9383(1) +3696(1) +3532{以下海底捞月红胜。} +2575(1) +3484(1) +8595(1) +8494(1) +9585(1) +9695(1) +7576(1) +9454(1) +8595(1) +5455 ",
        "[FEN \"5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9 r - - 0 1\"]\n[version \"18\"]\n[win \"红胜\"]\n[type \"残局\"]\n[title \"\u0006第09局\"]\n[event \"\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━┯━┯━将━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─車─卒─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─╬─车─┼─炮─┼─┼─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n-[(-1,-1),(-1,-1)].0 这是一局炮斗车卒的范例。对车炮的运用颇有启迪，可资借鉴。\n\t0-[(2,2),(4,2)].1 \n\t\t1-[(3,4),(3,0)].2 \n\t\t1-[(3,4),(3,3)].3 \n\t\t1-[(3,4),(8,4)].4 \n\t\t\t2-[(2,4),(2,5)].5 献炮叫将，伏车八平四白脸将成杀，是获胜的关键。\n\t\t\t2-[(2,4),(2,7)].6 \n\t\t\t3-[(4,2),(4,4)].7 红车占中是获胜的休着。黑不敢平车邀兑，否则，红车五平四胜。\n\t\t\t4-[(4,2),(4,7)].8 \n\t\t\t\t5-[(3,5),(3,4)].9 \n\t\t\t\t6-[(3,0),(3,4)].10 将军。\n\t\t\t\t7-[(9,5),(8,5)].11 \n\t\t\t\t8-[(8,4),(8,6)].12 \n\t\t\t\t\t9-[(4,2),(4,5)].13 \n\t\t\t\t\t10-[(2,7),(2,4)].14 叫杀。\n\t\t\t\t\t11-[(2,4),(2,5)].15 \n\t\t\t\t\t12-[(4,7),(9,7)].16 红方升车再打将，使黑方车卒失去有机联系，是获胜的重要环节。\n\t\t\t\t\t\t13-[(9,5),(9,4)].17 \n\t\t\t\t\t\t14-[(3,4),(3,0)].18 \n\t\t\t\t\t\t15-[(3,5),(3,4)].19 \n\t\t\t\t\t\t16-[(9,5),(8,5)].20 \n\t\t\t\t\t\t\t17-[(4,5),(4,4)].21 \n\t\t\t\t\t\t\t18-[(2,4),(2,7)].22 “二打对一打”，红方不变作负。\n\t\t\t\t\t\t\t19-[(2,5),(2,6)].23 \n\t\t\t\t\t\t\t20-[(9,7),(3,7)].24 \n\t\t\t\t\t\t\t\t21-[(9,4),(9,5)].25 \n\t\t\t\t\t\t\t\t21-[(9,4),(9,3)].26 \n\t\t\t\t\t\t\t\t23-[(8,5),(9,5)].27 \n\t\t\t\t\t\t\t\t24-[(3,5),(3,6)].28 \n\t\t\t\t\t\t\t\t24-[(8,6),(3,6)].29 \n\t\t\t\t\t\t\t\t24-[(3,5),(2,5)].30 \n\t\t\t\t\t\t\t\t\t25-[(2,5),(2,6)].31 \n\t\t\t\t\t\t\t\t\t26-[(2,5),(3,5)].32 \n\t\t\t\t\t\t\t\t\t27-[(2,6),(3,6)].33 \n\t\t\t\t\t\t\t\t\t28-[(3,7),(4,7)].34 \n\t\t\t\t\t\t\t\t\t29-[(2,4),(2,5)].35 \n\t\t\t\t\t\t\t\t\t30-[(3,7),(3,5)].36 \n\t\t\t\t\t\t\t\t\t\t31-[(9,5),(8,5)].37 \n\t\t\t\t\t\t\t\t\t\t32-[(3,0),(1,0)].38 \n\t\t\t\t\t\t\t\t\t\t33-[(3,4),(2,4)].39 \n\t\t\t\t\t\t\t\t\t\t34-[(8,5),(9,5)].40 \n\t\t\t\t\t\t\t\t\t\t35-[(3,5),(2,5)].41 \n\t\t\t\t\t\t\t\t\t\t36-[(8,5),(8,4)].42 \n\t\t\t\t\t\t\t\t\t\t\t37-[(2,6),(3,6)].43 \n\t\t\t\t\t\t\t\t\t\t\t38-[(1,4),(0,4)].44 \n\t\t\t\t\t\t\t\t\t\t\t39-[(4,4),(2,4)].45 红方胜定。\n\t\t\t\t\t\t\t\t\t\t\t40-[(4,7),(4,5)].46 \n\t\t\t\t\t\t\t\t\t\t\t41-[(3,7),(3,6)].47 \n\t\t\t\t\t\t\t\t\t\t\t42-[(3,5),(2,5)].48 以下升车占中，海底捞月胜。\n\t\t\t\t\t\t\t\t\t\t\t\t43-[(3,0),(1,0)].49 平炮再升炮打车，消灭小卒，催毁黑方中路屏障，是红方获胜的精华。\n\t\t\t\t\t\t\t\t\t\t\t\t44-[(1,0),(1,3)].50 \n\t\t\t\t\t\t\t\t\t\t\t\t46-[(9,5),(9,4)].51 \n\t\t\t\t\t\t\t\t\t\t\t\t\t49-[(1,4),(0,4)].52 \n\t\t\t\t\t\t\t\t\t\t\t\t\t50-[(4,4),(3,4)].53 \n\t\t\t\t\t\t\t\t\t\t\t\t\t51-[(4,5),(4,4)].54 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t52-[(1,0),(1,5)].55 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t53-[(1,3),(2,3)].56 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t54-[(9,4),(9,3)].57 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t55-[(4,4),(3,4)].58 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t56-[(3,4),(9,4)].59 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t57-[(2,4),(2,3)].60 红方胜定。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t58-[(1,5),(2,5)].61 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t59-[(9,3),(8,3)].62 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t61-[(3,6),(9,6)].63 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t62-[(3,5),(3,2)].64 以下海底捞月红胜。\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t63-[(2,5),(7,5)].65 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t65-[(3,4),(8,4)].66 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t66-[(8,5),(9,5)].67 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t67-[(8,4),(9,4)].68 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t68-[(9,5),(8,5)].69 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t69-[(9,6),(9,5)].70 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t70-[(7,5),(7,6)].71 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t71-[(9,4),(5,4)].72 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t72-[(8,5),(9,5)].73 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t73-[(5,4),(5,5)].74 \n着法数量【74】\t注解数量【11】\t注解最长【31】\n\n"
        },
        {"布局陷阱--飞相局对金钩炮",
        "[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[version \"12\"]\n[win \"红胜\"]\n[type \"全局\"]\n[title \"\u0018布局陷阱--飞相局对金钩炮\"]\n[event \"\u0018布局陷阱--飞相局对金钩炮\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n\n(1) +0624(1) +7772(1) +0818(1) +9776(1) +1813(1) +9897(1) +1383(1) +9170(1) +8381(1) +7101(1) +0001(1) +9674(1) +2120(1) +9584(1) +0171(1) +7273(1) +0715(1) +9757(1) +3040(1) +7363(1) +8183(1) +6373(1) +2060(1) +7091(1) +6050(1) +5755(1) +1507(1) +6656(1) +8381{红得子大优} ",
        "[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[version \"12\"]\n[win \"红胜\"]\n[type \"全局\"]\n[title \"\u0018布局陷阱--飞相局对金钩炮\"]\n[event \"\u0018布局陷阱--飞相局对金钩炮\"]\n[date \"\"]\n[site \"\"]\n[red \"\"]\n[black \"\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n車━馬━象━士━将━士━象━馬━車\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n车━马━相━仕━帅━仕━相━马━车\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n-[(-1,-1),(-1,-1)].0 \n\t0-[(0,6),(2,4)].1 \n\t\t1-[(7,7),(7,2)].2 \n\t\t\t2-[(0,8),(1,8)].3 \n\t\t\t\t3-[(9,7),(7,6)].4 \n\t\t\t\t\t4-[(1,8),(1,3)].5 \n\t\t\t\t\t\t5-[(9,8),(9,7)].6 \n\t\t\t\t\t\t\t6-[(1,3),(8,3)].7 \n\t\t\t\t\t\t\t\t7-[(9,1),(7,0)].8 \n\t\t\t\t\t\t\t\t\t8-[(8,3),(8,1)].9 \n\t\t\t\t\t\t\t\t\t\t9-[(7,1),(0,1)].10 \n\t\t\t\t\t\t\t\t\t\t\t10-[(0,0),(0,1)].11 \n\t\t\t\t\t\t\t\t\t\t\t\t11-[(9,6),(7,4)].12 \n\t\t\t\t\t\t\t\t\t\t\t\t\t12-[(2,1),(2,0)].13 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t13-[(9,5),(8,4)].14 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t14-[(0,1),(7,1)].15 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t15-[(7,2),(7,3)].16 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t16-[(0,7),(1,5)].17 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t17-[(9,7),(5,7)].18 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t18-[(3,0),(4,0)].19 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t19-[(7,3),(6,3)].20 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t20-[(8,1),(8,3)].21 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t21-[(6,3),(7,3)].22 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t22-[(2,0),(6,0)].23 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t23-[(7,0),(9,1)].24 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t24-[(6,0),(5,0)].25 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t25-[(5,7),(5,5)].26 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t26-[(1,5),(0,7)].27 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t27-[(6,6),(5,6)].28 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t28-[(8,3),(8,1)].29 红得子大优\n着法数量【29】\t注解数量【1】\t注解最长【5】\n\n"
        },
        {"- 北京张强 (和) 上海胡荣华 (1993.4.27于南京)",
        "[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[version \"13\"]\n[win \"和棋\"]\n[type \"全局\"]\n[title \"挺兵对卒底炮\"]\n[event \"\u001093全国象棋锦标赛\"]\n[date \"1993.4.27\"]\n[site \"\u0004南京\"]\n[red \"\b北京张强\"]\n[black \"上海胡荣华\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n\n(1) +3242(1) +7172(1) +2724(1) +9274(1) +0726(1) +6252(1) +0120(1) +5242(1) +0807(1) +9888(1) +0001(1) +8883(1) +0314(1) +9384(1) +0747(1) +6656(1) +4742(1) +9776(1) +3646(1) +5646(1) +4246(1) +6050(1) +2123(1) +9170(1) +0171(1) +7655(1) +7161(1) +8343(1) +4645(1) +7776(1) +0628(1) +4353(1) +2464(1) +5352(1) +0224(1) +9091(1) +6191(1) +7091(1) +2647(1) +5547(1) +4547(1) +7292(1) +4746(1) +9272(1) +3040(1) +9183(1) +6465(1) +5255(1) +6567(1) +8364(1) +4641(1) +5040(1) +4140(1) +6452(1) +4090(1) +7292(1) +2032(1) +5554(1) +3444(1) +5244 ",
        "[FEN \"rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR r - - 0 1\"]\n[version \"13\"]\n[win \"和棋\"]\n[type \"全局\"]\n[title \"挺兵对卒底炮\"]\n[event \"\u001093全国象棋锦标赛\"]\n[date \"1993.4.27\"]\n[site \"\u0004南京\"]\n[red \"\b北京张强\"]\n[black \"上海胡荣华\"]\n[opening \"\"]\n[writer \"\"]\n[author \"\"]\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n車━馬━象━士━将━士━象━馬━車\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n车━马━相━仕━帅━仕━相━马━车\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n-[(-1,-1),(-1,-1)].0 \n\t0-[(3,2),(4,2)].1 \n\t\t1-[(7,1),(7,2)].2 \n\t\t\t2-[(2,7),(2,4)].3 \n\t\t\t\t3-[(9,2),(7,4)].4 \n\t\t\t\t\t4-[(0,7),(2,6)].5 \n\t\t\t\t\t\t5-[(6,2),(5,2)].6 \n\t\t\t\t\t\t\t6-[(0,1),(2,0)].7 \n\t\t\t\t\t\t\t\t7-[(5,2),(4,2)].8 \n\t\t\t\t\t\t\t\t\t8-[(0,8),(0,7)].9 \n\t\t\t\t\t\t\t\t\t\t9-[(9,8),(8,8)].10 \n\t\t\t\t\t\t\t\t\t\t\t10-[(0,0),(0,1)].11 \n\t\t\t\t\t\t\t\t\t\t\t\t11-[(8,8),(8,3)].12 \n\t\t\t\t\t\t\t\t\t\t\t\t\t12-[(0,3),(1,4)].13 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t13-[(9,3),(8,4)].14 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t14-[(0,7),(4,7)].15 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t15-[(6,6),(5,6)].16 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t16-[(4,7),(4,2)].17 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t17-[(9,7),(7,6)].18 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t18-[(3,6),(4,6)].19 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t19-[(5,6),(4,6)].20 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t20-[(4,2),(4,6)].21 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t21-[(6,0),(5,0)].22 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t22-[(2,1),(2,3)].23 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t23-[(9,1),(7,0)].24 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t24-[(0,1),(7,1)].25 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t25-[(7,6),(5,5)].26 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t26-[(7,1),(6,1)].27 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t27-[(8,3),(4,3)].28 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t28-[(4,6),(4,5)].29 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t29-[(7,7),(7,6)].30 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t30-[(0,6),(2,8)].31 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t31-[(4,3),(5,3)].32 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t32-[(2,4),(6,4)].33 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t33-[(5,3),(5,2)].34 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t34-[(0,2),(2,4)].35 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t35-[(9,0),(9,1)].36 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t36-[(6,1),(9,1)].37 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t37-[(7,0),(9,1)].38 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t38-[(2,6),(4,7)].39 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t39-[(5,5),(4,7)].40 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t40-[(4,5),(4,7)].41 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t41-[(7,2),(9,2)].42 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t42-[(4,7),(4,6)].43 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t43-[(9,2),(7,2)].44 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t44-[(3,0),(4,0)].45 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t45-[(9,1),(8,3)].46 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t46-[(6,4),(6,5)].47 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t47-[(5,2),(5,5)].48 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t48-[(6,5),(6,7)].49 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t49-[(8,3),(6,4)].50 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t50-[(4,6),(4,1)].51 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t51-[(5,0),(4,0)].52 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t52-[(4,1),(4,0)].53 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t53-[(6,4),(5,2)].54 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t54-[(4,0),(9,0)].55 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t55-[(7,2),(9,2)].56 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t56-[(2,0),(3,2)].57 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t57-[(5,5),(5,4)].58 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t58-[(3,4),(4,4)].59 \n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t59-[(5,2),(4,4)].60 \n着法数量【60】\t注解数量【0】\t注解最长【0】\n\n"
        }
    };

    private static readonly List<Manual> Manuals = GetManuals();

    private static List<Manual> GetManuals()
    {
        List<Manual> manuals = new();
        for (int i = 0; i < manualStrings.GetLength(0); i++)
            manuals.Add(new(Manual.GetFileName(manualStrings[i, 0], FileExtType.Xqf)));

        return manuals;
    }

    private Manual GetManual(int index, FileExtType fileExtType)
    {
        if (fileExtType == FileExtType.Xqf)
            return Manuals[index];

        Manual manual = new();
        MemoryStream stream = new();

        Manuals[index].SetStream(stream, fileExtType);
        stream.Seek(0, SeekOrigin.Begin);
        manual.SetFromStream(stream, fileExtType);

        return manual;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void TestXQFType(int index)
    {
        Manual manual = GetManual(index, FileExtType.Xqf);

        string result = manual.GetString();
        Assert.Equal(manualStrings[index, 1], result);

        string detailResult = manual.ToString(true, true);
        Assert.Equal(manualStrings[index, 2], detailResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void TestCmType(int index)
    {
        Manual manual = GetManual(index, FileExtType.Cm);

        string result = manual.GetString();
        Assert.Equal(manualStrings[index, 1], result);

        string detailResult = manual.ToString(true, true);
        Assert.Equal(manualStrings[index, 2], detailResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void TestTextType(int index)
    {
        Manual manual = GetManual(index, FileExtType.Text);

        string result = manual.GetString();
        Assert.Equal(manualStrings[index, 1], result);

        string detailResult = manual.ToString(true, true);
        Assert.Equal(manualStrings[index, 2], detailResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void TestPGNRowColType(int index)
    {
        Manual manual = GetManual(index, FileExtType.PGNRowCol);

        string result = manual.GetString();
        Assert.Equal(manualStrings[index, 1], result);

        string detailResult = manual.ToString(true, true);
        Assert.Equal(manualStrings[index, 2], detailResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void TestPGNIccsType(int index)
    {
        Manual manual = GetManual(index, FileExtType.PGNIccs);

        string result = manual.GetString();
        Assert.Equal(manualStrings[index, 1], result);

        string detailResult = manual.ToString(true, true);
        Assert.Equal(manualStrings[index, 2], detailResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void TestPGNZhType(int index)
    {
        Manual manual = GetManual(index, FileExtType.PGNZh);

        string result = manual.GetString();
        Assert.Equal(manualStrings[index, 1], result);

        string detailResult = manual.ToString(true, true);
        Assert.Equal(manualStrings[index, 2], detailResult);
    }
}
