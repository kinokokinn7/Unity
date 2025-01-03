using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string Type;
    public string Name;
    public string Description;
    public int Money;
    public bool Usable;
    public bool Consumable;

    public ItemData()
    { }

    public ItemData(Item item)
    {
        Type = item.GetType().Name;
        Name = item.Name;
        Description = item.Description;
        Money = item.Money;
        Usable = item.Usable;
        Consumable = item.Consumable;
    }

    /// <summary>
    /// ItemDataクラス（および継承クラス）からItemクラス（および継承クラス）へデシリアライズします。
    /// </summary>
    /// <returns></returns>
    public virtual Item ToItem()
    {
        Item item;
        switch (Type)
        {
            case "Weapon":
                item = ScriptableObject.CreateInstance<Weapon>();
                break;
            case "Armor":
                item = ScriptableObject.CreateInstance<Armor>();
                break;
            case "LifeRecoveryItem":
                item = ScriptableObject.CreateInstance<LifeRecoveryItem>();
                break;
            case "FoodRecoveryItem":
                item = ScriptableObject.CreateInstance<FoodRecoveryItem>();
                break;
            default:
                item = ScriptableObject.CreateInstance<Item>();
                break;
        }
        item.FromItemData(this);
        return item;
    }
}
