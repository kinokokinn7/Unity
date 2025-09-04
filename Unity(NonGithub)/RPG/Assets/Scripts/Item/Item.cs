using UnityEngine;
using System.Collections;

public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public int Money;

    /// <summary>
    /// アイテムを使用する処理です。
    /// </summary>
    /// <param name="target">アイテム使用者のバトルパラメータ</param>
    public virtual void Use(BattleParameterBase target) { }
}
