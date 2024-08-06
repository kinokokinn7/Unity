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
    /// 回復エフェクトのプレハブの参照。
    /// </summary>
    public GameObject _healingEffectPrefab;

    /// <summary>
    /// アイテムを使用してHPを回復します。
    /// ※回復後のHPが最大HPより大きい場合は最大HPに設定されます。
    /// </summary>
    /// <param name="target">回復対象のバトルパラメータ</param>
    public override void Use(MapObjectBase target)
    {
        MessageWindow.Instance.AppendMessage($"{this.Name}を使った！");
        MessageWindow.Instance.AppendMessage($"{target.Name}のHPが{RecoveryPower}回復した！");
        SpawnHealingEffect(target.transform.position);
        target.HpRecovered(RecoveryPower);
        target.Hp.Recover(RecoveryPower);
    }

    /// <summary>
    /// エフェクトを生成します。
    /// </summary>
    /// <param name="position">エフェクトの生成位置。</param>
    private void SpawnHealingEffect(Vector3 position)
    {
        //プレハブを指定した位置に生成
        Instantiate(_healingEffectPrefab, position, Quaternion.identity);
    }

    /// <summary>
    /// シリアライズ用のデータクラスのインスタンスを生成します。
    /// </summary>
    /// <returns>シリアライズ用のデータクラスのインスタンス。</returns>

    public override ItemData ToItemData()
    {
        return new LifeRecoveryItemData(this);
    }


    /// <summary>
    /// シリアライズ用のデータクラスをもとにデシリアライズします。
    /// </summary>    
    public override void FromItemData(ItemData data)
    {
        base.FromItemData(data);
        if (data is LifeRecoveryItemData recoveryItemData)
        {
            RecoveryPower = recoveryItemData.RecoveryPower;
            if (!string.IsNullOrEmpty(recoveryItemData.HealingEffectPrefabName))
            {
                _healingEffectPrefab = Resources.Load<GameObject>(recoveryItemData.HealingEffectPrefabName);
            }
        }
    }
}
