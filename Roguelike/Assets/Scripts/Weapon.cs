using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内の武器を表すクラスです。マップオブジェクトに装備することができ、武器によって攻撃力が変化します。
/// </summary>
[CreateAssetMenu(menuName = "Weapon")]
class Weapon : ScriptableObject
{
    public string Name = ""; // 武器の名前
    public int Attack = 1; // 武器による攻撃力の増加分

    /// <summary>
    /// 武器をマップオブジェクトに装備します。攻撃力が増加します。
    /// </summary>
    /// <param name="obj">装備するマップオブジェクト。</param>
    public void Attach(MapObjectBase obj)
    {
        obj.Attack += Attack;
    }

    /// <summary>
    /// 武器をマップオブジェクトから外します。攻撃力が減少します。
    /// </summary>
    /// <param name="obj">装備を外すマップオブジェクト。</param>
    public void Detach(MapObjectBase obj)
    {
        obj.Attack -= Attack;
    }

    /// <summary>
    /// 他の武器と合成して新しい武器を作成します。攻撃力は両方の武器の攻撃力の合計となります。
    /// </summary>
    /// <param name="other">合成する他の武器。</param>
    /// <returns>合成された新しい武器。</returns>
    public Weapon Merge(Weapon other)
    {
        var newWeapon = ScriptableObject.CreateInstance<Weapon>();
        newWeapon.Name = Name; // 新しい武器の名前は元の武器の名前を引き継ぎます
        newWeapon.Attack = Attack + (other != null ? other.Attack : 0); // 両方の武器の攻撃力を合算
        return newWeapon;
    }

    /// <summary>
    /// 武器の情報を文字列で返します。主にUIでの表示用です。
    /// </summary>
    /// <returns>武器の名前と攻撃力を含む文字列。</returns>
    public override string ToString()
    {
        return $"{Name} Atk+{Attack}";
    }
}
