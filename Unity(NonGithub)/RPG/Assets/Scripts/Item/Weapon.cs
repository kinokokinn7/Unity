using UnityEngine;
using System.Collections;

/// <summary>
/// 武器/防具の分類。
/// </summary>
public enum WeaponKind
{
    Attack,     // 武器
    Defense    // 防具
}

[CreateAssetMenu(menuName = "Item/Weapon")]
public class Weapon : Item
{
    /// <summary>
    /// 武器/防具の分類。
    /// </summary>
    public WeaponKind Kind;

    /// <summary>
    /// 攻撃力 または 守備力。
    /// </summary>
    public int Power;
}
