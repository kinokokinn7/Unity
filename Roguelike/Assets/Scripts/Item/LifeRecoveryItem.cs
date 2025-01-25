using UnityEngine;
using System.Collections;
using Roguelike.Window;
using System.Threading.Tasks;
using System;

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
    public override async void Use(MapObjectBase target)
    {
        var player = FindAnyObjectByType<Player>();
        player.IsNowUsingItem = true;

        MessageWindow.Instance.AppendMessage($"{this.Name}を使った！");
        MessageWindow.Instance.AppendMessage($"{target.Name}のHPが{RecoveryPower}回復した！");
        SpawnHealingEffect(target.transform.position);

        target.HpRecovered(RecoveryPower);
        target.Hp.Recover(RecoveryPower);

        await Task.Delay(1000);

        player.IsNowUsingItem = false;
    }
    /// <summary>
    /// エフェクトを生成します。
    /// </summary>
    /// <param name="position">エフェクトの生成位置。</param>
    /// <returns>生成されたエフェクトのParticleSystem。</returns>
    private ParticleSystem SpawnHealingEffect(Vector3 position)
    {
        //プレハブを指定した位置に生成
        var effectInstance = Instantiate(_healingEffectPrefab, position, Quaternion.identity);
        return effectInstance.GetComponent<ParticleSystem>();

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
        if (data.Type == "LifeRecoveryItem")
        {
            var recoveryItemData = data as LifeRecoveryItemData;
            RecoveryPower = recoveryItemData.RecoveryPower;
            if (!string.IsNullOrEmpty(recoveryItemData.HealingEffectPrefabName))
            {
                _healingEffectPrefab = Resources.Load<GameObject>(recoveryItemData.HealingEffectPrefabName);
            }
        }
    }
}
