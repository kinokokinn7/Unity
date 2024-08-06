using UnityEngine;
using System.Collections;
using System;

public abstract class Item : ScriptableObject
{
    public string Name;

    public string Description;

    public int Money;

    public bool Usable;

    public bool Consumable;

    /// <summary>
    /// アイテムを使用します。
    /// </summary>
    /// <param name="target">アイテム使用者</param>
    public virtual void Use(MapObjectBase target) { }

    public abstract ItemData ToItemData();

    public virtual void FromItemData(ItemData data)
    {
        Name = data.Name;
        Description = data.Description;
        Money = data.Money;
        Usable = data.Usable;
        Consumable = data.Consumable;
    }
}
