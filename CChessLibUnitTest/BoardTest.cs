using System.Text;
using CChess;

namespace CChessTest;

public class BoardTest
{
    [Theory]
    [InlineData("rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR",
        "NoChange: \nrnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n車━馬━象━士━将━士━象━馬━車\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n车━马━相━仕━帅━仕━相━马━车\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(0,4) CanMoveCoord: (1,4)\n红仕A(0,3) CanMoveCoord: (1,4)\n红仕A(0,5) CanMoveCoord: (1,4)\n红相B(0,2) CanMoveCoord: (2,0)(2,4)\n红相B(0,6) CanMoveCoord: (2,4)(2,8)\n红马N(0,1) CanMoveCoord: (2,0)(2,2)\n红马N(0,7) CanMoveCoord: (2,6)(2,8)\n红车R(0,0) CanMoveCoord: (1,0)(2,0)\n红车R(0,8) CanMoveCoord: (1,8)(2,8)\n红炮C(2,1) CanMoveCoord: (1,1)(3,1)(4,1)(5,1)(6,1)(9,1)(2,0)(2,2)(2,3)(2,4)(2,5)(2,6)\n红炮C(2,7) CanMoveCoord: (1,7)(3,7)(4,7)(5,7)(6,7)(9,7)(2,6)(2,5)(2,4)(2,3)(2,2)(2,8)\n红兵P(3,0) CanMoveCoord: (4,0)\n红兵P(3,2) CanMoveCoord: (4,2)\n红兵P(3,4) CanMoveCoord: (4,4)\n红兵P(3,6) CanMoveCoord: (4,6)\n红兵P(3,8) CanMoveCoord: (4,8)\n黑将k(9,4) CanMoveCoord: (8,4)\n黑士a(9,3) CanMoveCoord: (8,4)\n黑士a(9,5) CanMoveCoord: (8,4)\n黑象b(9,2) CanMoveCoord: (7,0)(7,4)\n黑象b(9,6) CanMoveCoord: (7,4)(7,8)\n黑馬n(9,1) CanMoveCoord: (7,0)(7,2)\n黑馬n(9,7) CanMoveCoord: (7,6)(7,8)\n黑車r(9,0) CanMoveCoord: (8,0)(7,0)\n黑車r(9,8) CanMoveCoord: (8,8)(7,8)\n黑砲c(7,1) CanMoveCoord: (6,1)(5,1)(4,1)(3,1)(0,1)(8,1)(7,0)(7,2)(7,3)(7,4)(7,5)(7,6)\n黑砲c(7,7) CanMoveCoord: (6,7)(5,7)(4,7)(3,7)(0,7)(8,7)(7,6)(7,5)(7,4)(7,3)(7,2)(7,8)\n黑卒p(6,0) CanMoveCoord: (5,0)\n黑卒p(6,2) CanMoveCoord: (5,2)\n黑卒p(6,4) CanMoveCoord: (5,4)\n黑卒p(6,6) CanMoveCoord: (5,6)\n黑卒p(6,8) CanMoveCoord: (5,8)\nSymmetry_V: \nRNBAKABNR/9/1C5C1/P1P1P1P1P/9/9/p1p1p1p1p/1c5c1/9/rnbakabnr\n　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n车━马━相━仕━帅━仕━相━马━车\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n車━馬━象━士━将━士━象━馬━車\n９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n红帅K(9,4) CanMoveCoord: (8,4)\n红仕A(9,3) CanMoveCoord: (8,4)\n红仕A(9,5) CanMoveCoord: (8,4)\n红相B(9,2) CanMoveCoord: (7,0)(7,4)\n红相B(9,6) CanMoveCoord: (7,4)(7,8)\n红马N(9,1) CanMoveCoord: (7,0)(7,2)\n红马N(9,7) CanMoveCoord: (7,6)(7,8)\n红车R(9,0) CanMoveCoord: (8,0)(7,0)\n红车R(9,8) CanMoveCoord: (8,8)(7,8)\n红炮C(7,1) CanMoveCoord: (6,1)(5,1)(4,1)(3,1)(0,1)(8,1)(7,0)(7,2)(7,3)(7,4)(7,5)(7,6)\n红炮C(7,7) CanMoveCoord: (6,7)(5,7)(4,7)(3,7)(0,7)(8,7)(7,6)(7,5)(7,4)(7,3)(7,2)(7,8)\n红兵P(6,0) CanMoveCoord: (5,0)\n红兵P(6,2) CanMoveCoord: (5,2)\n红兵P(6,4) CanMoveCoord: (5,4)\n红兵P(6,6) CanMoveCoord: (5,6)\n红兵P(6,8) CanMoveCoord: (5,8)\n黑将k(0,4) CanMoveCoord: (1,4)\n黑士a(0,3) CanMoveCoord: (1,4)\n黑士a(0,5) CanMoveCoord: (1,4)\n黑象b(0,2) CanMoveCoord: (2,0)(2,4)\n黑象b(0,6) CanMoveCoord: (2,4)(2,8)\n黑馬n(0,1) CanMoveCoord: (2,0)(2,2)\n黑馬n(0,7) CanMoveCoord: (2,6)(2,8)\n黑車r(0,0) CanMoveCoord: (1,0)(2,0)\n黑車r(0,8) CanMoveCoord: (1,8)(2,8)\n黑砲c(2,1) CanMoveCoord: (1,1)(3,1)(4,1)(5,1)(6,1)(9,1)(2,0)(2,2)(2,3)(2,4)(2,5)(2,6)\n黑砲c(2,7) CanMoveCoord: (1,7)(3,7)(4,7)(5,7)(6,7)(9,7)(2,6)(2,5)(2,4)(2,3)(2,2)(2,8)\n黑卒p(3,0) CanMoveCoord: (4,0)\n黑卒p(3,2) CanMoveCoord: (4,2)\n黑卒p(3,4) CanMoveCoord: (4,4)\n黑卒p(3,6) CanMoveCoord: (4,6)\n黑卒p(3,8) CanMoveCoord: (4,8)\nSymmetry_H: \nrnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n車━馬━象━士━将━士━象━馬━車\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n车━马━相━仕━帅━仕━相━马━车\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(0,4) CanMoveCoord: (1,4)\n红仕A(0,3) CanMoveCoord: (1,4)\n红仕A(0,5) CanMoveCoord: (1,4)\n红相B(0,2) CanMoveCoord: (2,0)(2,4)\n红相B(0,6) CanMoveCoord: (2,4)(2,8)\n红马N(0,1) CanMoveCoord: (2,0)(2,2)\n红马N(0,7) CanMoveCoord: (2,6)(2,8)\n红车R(0,0) CanMoveCoord: (1,0)(2,0)\n红车R(0,8) CanMoveCoord: (1,8)(2,8)\n红炮C(2,1) CanMoveCoord: (1,1)(3,1)(4,1)(5,1)(6,1)(9,1)(2,0)(2,2)(2,3)(2,4)(2,5)(2,6)\n红炮C(2,7) CanMoveCoord: (1,7)(3,7)(4,7)(5,7)(6,7)(9,7)(2,6)(2,5)(2,4)(2,3)(2,2)(2,8)\n红兵P(3,0) CanMoveCoord: (4,0)\n红兵P(3,2) CanMoveCoord: (4,2)\n红兵P(3,4) CanMoveCoord: (4,4)\n红兵P(3,6) CanMoveCoord: (4,6)\n红兵P(3,8) CanMoveCoord: (4,8)\n黑将k(9,4) CanMoveCoord: (8,4)\n黑士a(9,3) CanMoveCoord: (8,4)\n黑士a(9,5) CanMoveCoord: (8,4)\n黑象b(9,2) CanMoveCoord: (7,0)(7,4)\n黑象b(9,6) CanMoveCoord: (7,4)(7,8)\n黑馬n(9,1) CanMoveCoord: (7,0)(7,2)\n黑馬n(9,7) CanMoveCoord: (7,6)(7,8)\n黑車r(9,0) CanMoveCoord: (8,0)(7,0)\n黑車r(9,8) CanMoveCoord: (8,8)(7,8)\n黑砲c(7,1) CanMoveCoord: (6,1)(5,1)(4,1)(3,1)(0,1)(8,1)(7,0)(7,2)(7,3)(7,4)(7,5)(7,6)\n黑砲c(7,7) CanMoveCoord: (6,7)(5,7)(4,7)(3,7)(0,7)(8,7)(7,6)(7,5)(7,4)(7,3)(7,2)(7,8)\n黑卒p(6,0) CanMoveCoord: (5,0)\n黑卒p(6,2) CanMoveCoord: (5,2)\n黑卒p(6,4) CanMoveCoord: (5,4)\n黑卒p(6,6) CanMoveCoord: (5,6)\n黑卒p(6,8) CanMoveCoord: (5,8)\nNoChange: \nrnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n車━馬━象━士━将━士━象━馬━車\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n车━马━相━仕━帅━仕━相━马━车\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(0,4) CanMoveCoord: (1,4)\n红仕A(0,3) CanMoveCoord: (1,4)\n红仕A(0,5) CanMoveCoord: (1,4)\n红相B(0,2) CanMoveCoord: (2,0)(2,4)\n红相B(0,6) CanMoveCoord: (2,4)(2,8)\n红马N(0,1) CanMoveCoord: (2,0)(2,2)\n红马N(0,7) CanMoveCoord: (2,6)(2,8)\n红车R(0,0) CanMoveCoord: (1,0)(2,0)\n红车R(0,8) CanMoveCoord: (1,8)(2,8)\n红炮C(2,1) CanMoveCoord: (1,1)(3,1)(4,1)(5,1)(6,1)(9,1)(2,0)(2,2)(2,3)(2,4)(2,5)(2,6)\n红炮C(2,7) CanMoveCoord: (1,7)(3,7)(4,7)(5,7)(6,7)(9,7)(2,6)(2,5)(2,4)(2,3)(2,2)(2,8)\n红兵P(3,0) CanMoveCoord: (4,0)\n红兵P(3,2) CanMoveCoord: (4,2)\n红兵P(3,4) CanMoveCoord: (4,4)\n红兵P(3,6) CanMoveCoord: (4,6)\n红兵P(3,8) CanMoveCoord: (4,8)\n黑将k(9,4) CanMoveCoord: (8,4)\n黑士a(9,3) CanMoveCoord: (8,4)\n黑士a(9,5) CanMoveCoord: (8,4)\n黑象b(9,2) CanMoveCoord: (7,0)(7,4)\n黑象b(9,6) CanMoveCoord: (7,4)(7,8)\n黑馬n(9,1) CanMoveCoord: (7,0)(7,2)\n黑馬n(9,7) CanMoveCoord: (7,6)(7,8)\n黑車r(9,0) CanMoveCoord: (8,0)(7,0)\n黑車r(9,8) CanMoveCoord: (8,8)(7,8)\n黑砲c(7,1) CanMoveCoord: (6,1)(5,1)(4,1)(3,1)(0,1)(8,1)(7,0)(7,2)(7,3)(7,4)(7,5)(7,6)\n黑砲c(7,7) CanMoveCoord: (6,7)(5,7)(4,7)(3,7)(0,7)(8,7)(7,6)(7,5)(7,4)(7,3)(7,2)(7,8)\n黑卒p(6,0) CanMoveCoord: (5,0)\n黑卒p(6,2) CanMoveCoord: (5,2)\n黑卒p(6,4) CanMoveCoord: (5,4)\n黑卒p(6,6) CanMoveCoord: (5,6)\n黑卒p(6,8) CanMoveCoord: (5,8)\nExchange: \nRNBAKABNR/9/1C5C1/P1P1P1P1P/9/9/p1p1p1p1p/1c5c1/9/rnbakabnr\n　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n车━马━相━仕━帅━仕━相━马━车\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─炮─┼─┼─┼─┼─┼─炮─┨\n┃　│　│　│　│　│　│　│　┃\n兵─┼─兵─┼─兵─┼─兵─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─卒─┼─卒─┼─卒─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─砲─┼─┼─┼─┼─┼─砲─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n車━馬━象━士━将━士━象━馬━車\n９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n红帅K(9,4) CanMoveCoord: (8,4)\n红仕A(9,3) CanMoveCoord: (8,4)\n红仕A(9,5) CanMoveCoord: (8,4)\n红相B(9,2) CanMoveCoord: (7,0)(7,4)\n红相B(9,6) CanMoveCoord: (7,4)(7,8)\n红马N(9,1) CanMoveCoord: (7,0)(7,2)\n红马N(9,7) CanMoveCoord: (7,6)(7,8)\n红车R(9,0) CanMoveCoord: (8,0)(7,0)\n红车R(9,8) CanMoveCoord: (8,8)(7,8)\n红炮C(7,1) CanMoveCoord: (6,1)(5,1)(4,1)(3,1)(0,1)(8,1)(7,0)(7,2)(7,3)(7,4)(7,5)(7,6)\n红炮C(7,7) CanMoveCoord: (6,7)(5,7)(4,7)(3,7)(0,7)(8,7)(7,6)(7,5)(7,4)(7,3)(7,2)(7,8)\n红兵P(6,0) CanMoveCoord: (5,0)\n红兵P(6,2) CanMoveCoord: (5,2)\n红兵P(6,4) CanMoveCoord: (5,4)\n红兵P(6,6) CanMoveCoord: (5,6)\n红兵P(6,8) CanMoveCoord: (5,8)\n黑将k(0,4) CanMoveCoord: (1,4)\n黑士a(0,3) CanMoveCoord: (1,4)\n黑士a(0,5) CanMoveCoord: (1,4)\n黑象b(0,2) CanMoveCoord: (2,0)(2,4)\n黑象b(0,6) CanMoveCoord: (2,4)(2,8)\n黑馬n(0,1) CanMoveCoord: (2,0)(2,2)\n黑馬n(0,7) CanMoveCoord: (2,6)(2,8)\n黑車r(0,0) CanMoveCoord: (1,0)(2,0)\n黑車r(0,8) CanMoveCoord: (1,8)(2,8)\n黑砲c(2,1) CanMoveCoord: (1,1)(3,1)(4,1)(5,1)(6,1)(9,1)(2,0)(2,2)(2,3)(2,4)(2,5)(2,6)\n黑砲c(2,7) CanMoveCoord: (1,7)(3,7)(4,7)(5,7)(6,7)(9,7)(2,6)(2,5)(2,4)(2,3)(2,2)(2,8)\n黑卒p(3,0) CanMoveCoord: (4,0)\n黑卒p(3,2) CanMoveCoord: (4,2)\n黑卒p(3,4) CanMoveCoord: (4,4)\n黑卒p(3,6) CanMoveCoord: (4,6)\n黑卒p(3,8) CanMoveCoord: (4,8)\n")]
    [InlineData("5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5",
        "NoChange: \n5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━┯━┯━士━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─士─将─┼─┼─車\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─车─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n相─╬─┼─┼─┼─马─┼─╬─相\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━砲━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(1,4) CanMoveCoord: (1,3)(1,5)(2,4)(0,4)\n红相B(2,0) CanMoveCoord: (4,2)(0,2)\n红相B(2,8) CanMoveCoord: (4,6)(0,6)\n红马N(2,5) CanMoveCoord: (0,4)(0,6)(1,3)(1,7)(3,3)(3,7)(4,4)(4,6)\n红车R(7,6) CanMoveCoord: (6,6)(5,6)(4,6)(3,6)(2,6)(1,6)(0,6)(8,6)(9,6)(7,5)(7,4)(7,3)(7,2)(7,1)(7,0)(7,7)(7,8)\n黑将k(8,5) CanMoveCoord: \n黑士a(8,4) CanMoveCoord: (7,3)(7,5)(9,3)\n黑士a(9,5) CanMoveCoord: \n黑車r(8,8) CanMoveCoord: (7,8)(9,8)(8,7)(8,6)\n黑砲c(0,3) CanMoveCoord: (1,3)(2,3)(3,3)(4,3)(5,3)(6,3)(7,3)(8,3)(9,3)(0,2)(0,1)(0,0)(0,4)(0,5)(0,6)(0,7)(0,8)\n黑卒p(6,8) CanMoveCoord: (5,8)\nSymmetry_V: \n3c5/4K4/B4N2B/9/9/9/8p/6R2/4ak2r/5a3\n　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n┏━┯━┯━砲━┯━┯━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n相─╬─┼─┼─┼─马─┼─╬─相\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─╬─┼─┼─┼─┼─车─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─士─将─┼─┼─車\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━士━┷━┷━┛\n９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n红帅K(8,4) CanMoveCoord: (8,3)(8,5)(9,4)(7,4)\n红相B(7,0) CanMoveCoord: (9,2)(5,2)\n红相B(7,8) CanMoveCoord: (9,6)(5,6)\n红马N(7,5) CanMoveCoord: (5,4)(5,6)(6,3)(6,7)(8,3)(8,7)(9,4)(9,6)\n红车R(2,6) CanMoveCoord: (1,6)(0,6)(3,6)(4,6)(5,6)(6,6)(7,6)(8,6)(9,6)(2,5)(2,4)(2,3)(2,2)(2,1)(2,0)(2,7)(2,8)\n黑将k(1,5) CanMoveCoord: \n黑士a(0,5) CanMoveCoord: \n黑士a(1,4) CanMoveCoord: (0,3)(2,3)(2,5)\n黑車r(1,8) CanMoveCoord: (0,8)(2,8)(1,7)(1,6)\n黑砲c(9,3) CanMoveCoord: (8,3)(7,3)(6,3)(5,3)(4,3)(3,3)(2,3)(1,3)(0,3)(9,2)(9,1)(9,0)(9,4)(9,5)(9,6)(9,7)(9,8)\n黑卒p(3,8) CanMoveCoord: (4,8)\nSymmetry_H: \n3a5/r2ka4/2R6/p8/9/9/9/B2N4B/4K4/5c3\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━士━┯━┯━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n車─┼─┼─将─士─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─车─┼─┼─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n卒─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n相─╬─┼─马─┼─┼─┼─╬─相\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━砲━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(1,4) CanMoveCoord: (1,3)(1,5)(2,4)(0,4)\n红相B(2,0) CanMoveCoord: (4,2)(0,2)\n红相B(2,8) CanMoveCoord: (4,6)(0,6)\n红马N(2,3) CanMoveCoord: (0,2)(0,4)(1,1)(1,5)(3,1)(3,5)(4,2)(4,4)\n红车R(7,2) CanMoveCoord: (6,2)(5,2)(4,2)(3,2)(2,2)(1,2)(0,2)(8,2)(9,2)(7,1)(7,0)(7,3)(7,4)(7,5)(7,6)(7,7)(7,8)\n黑将k(8,3) CanMoveCoord: \n黑士a(8,4) CanMoveCoord: (7,3)(7,5)(9,5)\n黑士a(9,3) CanMoveCoord: \n黑車r(8,0) CanMoveCoord: (7,0)(9,0)(8,1)(8,2)\n黑砲c(0,5) CanMoveCoord: (1,5)(2,5)(3,5)(4,5)(5,5)(6,5)(7,5)(8,5)(9,5)(0,4)(0,3)(0,2)(0,1)(0,0)(0,6)(0,7)(0,8)\n黑卒p(6,0) CanMoveCoord: (5,0)\nNoChange: \n5a3/4ak2r/6R2/8p/9/9/9/B4N2B/4K4/3c5\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━┯━┯━士━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─士─将─┼─┼─車\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─车─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─卒\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n相─╬─┼─┼─┼─马─┼─╬─相\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━砲━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(1,4) CanMoveCoord: (1,3)(1,5)(2,4)(0,4)\n红相B(2,0) CanMoveCoord: (4,2)(0,2)\n红相B(2,8) CanMoveCoord: (4,6)(0,6)\n红马N(2,5) CanMoveCoord: (0,4)(0,6)(1,3)(1,7)(3,3)(3,7)(4,4)(4,6)\n红车R(7,6) CanMoveCoord: (6,6)(5,6)(4,6)(3,6)(2,6)(1,6)(0,6)(8,6)(9,6)(7,5)(7,4)(7,3)(7,2)(7,1)(7,0)(7,7)(7,8)\n黑将k(8,5) CanMoveCoord: \n黑士a(8,4) CanMoveCoord: (7,3)(7,5)(9,3)\n黑士a(9,5) CanMoveCoord: \n黑車r(8,8) CanMoveCoord: (7,8)(9,8)(8,7)(8,6)\n黑砲c(0,3) CanMoveCoord: (1,3)(2,3)(3,3)(4,3)(5,3)(6,3)(7,3)(8,3)(9,3)(0,2)(0,1)(0,0)(0,4)(0,5)(0,6)(0,7)(0,8)\n黑卒p(6,8) CanMoveCoord: (5,8)\nExchange: \n5A3/4AK2R/6r2/8P/9/9/9/b4n2b/4k4/3C5\n　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n┏━┯━┯━┯━┯━仕━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─仕─帅─┼─┼─车\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─車─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─兵\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n象─╬─┼─┼─┼─馬─┼─╬─象\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─将─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━炮━┷━┷━┷━┷━┛\n９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n红帅K(8,5) CanMoveCoord: \n红仕A(8,4) CanMoveCoord: (7,3)(7,5)(9,3)\n红仕A(9,5) CanMoveCoord: \n红车R(8,8) CanMoveCoord: (7,8)(9,8)(8,7)(8,6)\n红炮C(0,3) CanMoveCoord: (1,3)(2,3)(3,3)(4,3)(5,3)(6,3)(7,3)(8,3)(9,3)(0,2)(0,1)(0,0)(0,4)(0,5)(0,6)(0,7)(0,8)\n红兵P(6,8) CanMoveCoord: (5,8)\n黑将k(1,4) CanMoveCoord: (1,3)(1,5)(2,4)(0,4)\n黑象b(2,0) CanMoveCoord: (4,2)(0,2)\n黑象b(2,8) CanMoveCoord: (4,6)(0,6)\n黑馬n(2,5) CanMoveCoord: (0,4)(0,6)(1,3)(1,7)(3,3)(3,7)(4,4)(4,6)\n黑車r(7,6) CanMoveCoord: (6,6)(5,6)(4,6)(3,6)(2,6)(1,6)(0,6)(8,6)(9,6)(7,5)(7,4)(7,3)(7,2)(7,1)(7,0)(7,7)(7,8)\n")]
    [InlineData("5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9",
        "NoChange: \n5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━┯━┯━将━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─車─卒─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─╬─车─┼─炮─┼─┼─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(1,4) CanMoveCoord: (1,3)(1,5)(0,4)\n红车R(2,2) CanMoveCoord: (1,2)(0,2)(3,2)(4,2)(5,2)(6,2)(7,2)(8,2)(9,2)(2,1)(2,0)(2,3)\n红炮C(2,4) CanMoveCoord: \n黑将k(9,5) CanMoveCoord: (8,5)\n黑車r(3,4) CanMoveCoord: (2,4)(4,4)(5,4)(6,4)(7,4)(8,4)(9,4)(3,3)(3,2)(3,1)(3,0)\n黑卒p(3,5) CanMoveCoord: (3,6)(2,5)\nSymmetry_V: \n9/4K4/2R1C4/4rp3/9/9/9/9/9/5k3\n　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n┏━┯━┯━┯━┯━┯━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─车─┼─炮─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─車─卒─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─╬─┼─┼─┼─┼─┼─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━将━┷━┷━┛\n９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n红帅K(8,4) CanMoveCoord: (8,3)(8,5)(9,4)\n红车R(7,2) CanMoveCoord: (6,2)(5,2)(4,2)(3,2)(2,2)(1,2)(0,2)(8,2)(9,2)(7,1)(7,0)(7,3)\n红炮C(7,4) CanMoveCoord: \n黑将k(0,5) CanMoveCoord: (1,5)\n黑車r(6,4) CanMoveCoord: (5,4)(4,4)(3,4)(2,4)(1,4)(0,4)(7,4)(6,3)(6,2)(6,1)(6,0)\n黑卒p(6,5) CanMoveCoord: (6,6)(7,5)\nSymmetry_H: \n3k5/9/9/9/9/9/3pr4/4C1R2/4K4/9\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━将━┯━┯━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─卒─車─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─╬─┼─┼─炮─┼─车─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(1,4) CanMoveCoord: (1,3)(1,5)(0,4)\n红车R(2,6) CanMoveCoord: (1,6)(0,6)(3,6)(4,6)(5,6)(6,6)(7,6)(8,6)(9,6)(2,5)(2,7)(2,8)\n红炮C(2,4) CanMoveCoord: \n黑将k(9,3) CanMoveCoord: (8,3)\n黑車r(3,4) CanMoveCoord: (2,4)(4,4)(5,4)(6,4)(7,4)(8,4)(9,4)(3,5)(3,6)(3,7)(3,8)\n黑卒p(3,3) CanMoveCoord: (3,2)(2,3)\nNoChange: \n5k3/9/9/9/9/9/4rp3/2R1C4/4K4/9\n　　　　　　　黑　方　　　　　　　\n１　２　３　４　５　６　７　８　９\n┏━┯━┯━┯━┯━将━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─車─卒─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─╬─车─┼─炮─┼─┼─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─帅─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━┷━┷━┷━┛\n九　八　七　六　五　四　三　二　一\n　　　　　　　红　方　　　　　　　\n红帅K(1,4) CanMoveCoord: (1,3)(1,5)(0,4)\n红车R(2,2) CanMoveCoord: (1,2)(0,2)(3,2)(4,2)(5,2)(6,2)(7,2)(8,2)(9,2)(2,1)(2,0)(2,3)\n红炮C(2,4) CanMoveCoord: \n黑将k(9,5) CanMoveCoord: (8,5)\n黑車r(3,4) CanMoveCoord: (2,4)(4,4)(5,4)(6,4)(7,4)(8,4)(9,4)(3,3)(3,2)(3,1)(3,0)\n黑卒p(3,5) CanMoveCoord: (3,6)(2,5)\nExchange: \n5K3/9/9/9/9/9/4RP3/2r1c4/4k4/9\n　　　　　　　红　方　　　　　　　\n一　二　三　四　五　六　七　八　九\n┏━┯━┯━┯━┯━帅━┯━┯━┓\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─╳─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┠─╬─┼─┼─┼─┼─┼─╬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─╬─┼─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┴─┴─┴─┴─┴─┴─┴─┨\n┃　　　　　　　　　　　　　　　┃\n┠─┬─┬─┬─┬─┬─┬─┬─┨\n┃　│　│　│　│　│　│　│　┃\n┠─┼─╬─┼─车─兵─╬─┼─┨\n┃　│　│　│　│　│　│　│　┃\n┠─╬─車─┼─砲─┼─┼─╬─┨\n┃　│　│　│╲│╱│　│　│　┃\n┠─┼─┼─┼─将─┼─┼─┼─┨\n┃　│　│　│╱│╲│　│　│　┃\n┗━┷━┷━┷━┷━┷━┷━┷━┛\n９　８　７　６　５　４　３　２　１\n　　　　　　　黑　方　　　　　　　\n红帅K(9,5) CanMoveCoord: (8,5)\n红车R(3,4) CanMoveCoord: (2,4)(4,4)(5,4)(6,4)(7,4)(8,4)(9,4)(3,3)(3,2)(3,1)(3,0)\n红兵P(3,5) CanMoveCoord: (3,6)(2,5)\n黑将k(1,4) CanMoveCoord: (1,3)(1,5)(0,4)\n黑車r(2,2) CanMoveCoord: (1,2)(0,2)(3,2)(4,2)(5,2)(6,2)(7,2)(8,2)(9,2)(2,1)(2,0)(2,3)\n黑砲c(2,4) CanMoveCoord: \n")]
    public void TestCanMoveCoord(string fen, string expected)
    {
        string CanMoveCoordString(Board board)
        {
            List<Piece> livePieces = board.GetLivePieces();
            livePieces.Sort(delegate (Piece apiece, Piece bpiece)
            {
                int compColor = apiece.Color.CompareTo(bpiece.Color);
                if (compColor != 0)
                    return compColor;

                int compKind = apiece.Kind.CompareTo(bpiece.Kind);
                if (compKind != 0)
                    return compKind;

                Coord acoord = board.GetCoord(apiece), bcoord = board.GetCoord(bpiece);
                int compRow = acoord.Row.CompareTo(bcoord.Row);
                return compRow != 0 ? -compRow : acoord.Col.CompareTo(bcoord.Col);
            });
            return string.Join("\n", livePieces.Select(piece =>
            {
                List<Coord> canMoveCoords = piece.CanMoveCoord(board);
                return $"{piece}{board.GetCoord(piece).SymmetryRowToString()} CanMoveCoord: " +
                               string.Join("", canMoveCoords.Select(coord => coord.SymmetryRowToString()));
            }));
        }

        StringBuilder result = new();
        Board board = new(fen);
        foreach (var ct in new List<ChangeType> {
                    ChangeType.NoChange,
                    ChangeType.Symmetry_V,
                    ChangeType.Symmetry_H,
                    ChangeType.NoChange,
                    ChangeType.Exchange})
        {
            Board ctBoard = new(Board.GetFEN(board.GetFEN(), ct));
            result.Append($"{ct}: \n{ctBoard.GetFEN()}\n{ctBoard}{CanMoveCoordString(ctBoard)}\n");
        }

        Assert.Equal(expected, result.ToString());
    }
}
