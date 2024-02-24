using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ゲームのマップデータを定義するクラスです。マップは文字列で表現され、各文字がマップ上の異なる要素を表します。
/// </summary>
public class MapData
{
    // マップデータ
    // 0: 道
    // +: 壁
    // S: 開始位置
    // G: ゴールの位置
    string map =
        "000P\n" +  // マップの1行目
        "0+++\n" +  // マップの2行目
        "0000\n" +  // マップの3行目
        "+++0\n" +  // マップの4行目
        "G000\n";   // マップの5行目
}
