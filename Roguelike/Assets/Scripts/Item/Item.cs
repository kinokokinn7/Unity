using UnityEngine;
using System.Collections;

public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public int Money;

    /// <summary>
    /// アイテムを使用します。
    /// </summary>
    /// <param name="target">アイテム使用者</param>
    public virtual void Use(MapObjectBase target) { }
}
