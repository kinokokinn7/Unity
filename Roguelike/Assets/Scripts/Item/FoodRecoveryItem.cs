using UnityEngine;
using System.Collections;
using Roguelike.Window;

/// <summary>
/// 回復用のアイテムクラス
/// </summary>
[CreateAssetMenu(menuName = "Item/Food Recovery Item")]
public class FoodRecoveryItem : Item
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
        if (target is not Player)
        {
            Debug.LogWarning("プレイヤー以外のキャラはこのアイテムを使用できません。");
            return;
        }
        var player = target as Player;
        MessageWindow.Instance.AppendMessage($"{this.Name}を使った！");
        MessageWindow.Instance.AppendMessage($"{target.Name}の満腹度が{RecoveryPower}回復した！");
        SpawnHealingEffect(target.transform.position);
        player.FoodValue.Recover(RecoveryPower);
    }

    /// <summary>
    /// エフェクトを生成する。
    /// </summary>
    /// <param name="position">エフェクトの生成位置。</param>
    private void SpawnHealingEffect(Vector3 position)
    {
    }
}
