using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内でプレイヤーが見つけることができる宝箱を表すクラスです。宝箱からは様々なアイテムが手に入ります。
/// </summary>
class Treasure : MapObjectBase
{
    /// <summary>
    /// 宝箱から得られるアイテムの種類を定義します。
    /// LifeUpはプレイヤーのHPを増加させ、FoodUpは満腹度を増加させ、Weaponは新しい武器を提供します。
    /// </summary>
    public enum Type
    {
        LifeUp,   // プレイヤーのHPを増加させる
        FoodUp,   // プレイヤーの満腹度を増加させる
        Weapon,   // 新しい武器を提供する
    }

    public Type CurrentType = Type.LifeUp; // 現在の宝箱が提供するアイテムの種類
    public int Value = 5; // アイテムの効果の強さまたは武器の攻撃力
}
