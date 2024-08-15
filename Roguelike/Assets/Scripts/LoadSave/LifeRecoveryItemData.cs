using UnityEngine;

[System.Serializable]
public class LifeRecoveryItemData : ItemData
{
    public int RecoveryPower;
    public string HealingEffectPrefabName;

    /// <summary>
    /// [シリアライズ用] 回復アイテム。
    /// </summary>
    public LifeRecoveryItemData() : base()
    { }

    /// <summary>
    /// [シリアライズ用] 回復アイテム。
    /// </summary>
    /// <param name="item"></param>
    public LifeRecoveryItemData(LifeRecoveryItem item) : base(item)
    {
        RecoveryPower = item.RecoveryPower;
        HealingEffectPrefabName = "Prefabs/Particle/" + item._healingEffectPrefab?.name;
    }

    public override Item ToItem()
    {
        var item = base.ToItem() as LifeRecoveryItem;
        item.RecoveryPower = RecoveryPower;
        if (!string.IsNullOrEmpty(HealingEffectPrefabName))
        {
            item._healingEffectPrefab = Resources.Load<GameObject>(HealingEffectPrefabName);
        }
        return item;
    }
}