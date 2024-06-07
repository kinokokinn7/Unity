using UnityEngine;
using System.Collections;
using Roguelike.Window;

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
    /// ※回復後のHPが最大HPより大きい場合は最大HPに設定されます。
    /// </summary>
    /// <param name="target">回復対象のバトルパラメータ</param>
    public override void Use(MapObjectBase target)
    {
        MessageWindow.Instance.AppendMessage($"{this.Name}を使った！");
        MessageWindow.Instance.AppendMessage($"{target.Name}のHPが{RecoveryPower}回復した！");
        target.Hp.Recover(RecoveryPower);
    }
}
