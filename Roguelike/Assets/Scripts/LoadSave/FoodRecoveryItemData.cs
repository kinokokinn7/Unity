using UnityEngine;

[System.Serializable]
public class FoodRecoveryItemData : ItemData
{
    public int RecoveryPower;

    public FoodRecoveryItemData(FoodRecoveryItem item) : base(item)
    {
        RecoveryPower = item.RecoveryPower;
    }

    public override Item ToItem()
    {
        var item = base.ToItem() as FoodRecoveryItem;
        item.RecoveryPower = RecoveryPower;
        return item;
    }
}