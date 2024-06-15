using UnityEngine;
using System.Collections;
using System;

public class Item : ScriptableObject
{
    public readonly Guid id;
    public string Name;
    public string Description;
    public int Money;
    public bool Usable;

    public Item()
    {
        id = Guid.NewGuid();
    }

    /// <summary>
    /// アイテムを使用します。
    /// </summary>
    /// <param name="target">アイテム使用者</param>
    public virtual void Use(MapObjectBase target) { }
}
