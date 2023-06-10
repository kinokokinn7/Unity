using UnityEngine;
using System.Collections;

/// <summary>
/// 回復用のアイテムクラス
/// </summary>
[CreateAssetMenu(menuName = "Item/Life Recovery Item")]
public class LifeRecoveryItem : Item
{
    /// <summary>
    /// 回復量。
    /// </summary>
    public int RecoveryPower;

    /// <summary>
    /// アイテムを使用してHPを回復します。
    /// ※回復後のHPの上限は最大HPとなります。
    /// </summary>
    /// <param name="target">回復対象のバトルパラメータ</param>
    public override void Use(BattleParameterBase target)
    {
        target.HP += RecoveryPower;
        if (target.MaxHP < target.HP) target.HP = target.MaxHP;
    }
}
