using System.Collections;
using System.Collections.Generic;
using Roguelike.Window;
using UnityEngine;

/// <summary>
/// プレイヤーが所持するアイテムの一覧を管理するクラスです。
/// </summary>
public class ItemInventory : MonoBehaviour
{
    /// <summary>
    /// プレイヤーが所持しているアイテムの一覧。
    /// </summary>
    [SerializeField]
    private List<Item> _items;
    public List<Item> Items
    {
        get { return _items; }
        private set
        {
            _items = value;
        }
    }

    /// <summary>
    /// 最大所持可能アイテム数。
    /// </summary>
    public readonly int MaxItems = 6;

    /// <summary>
    /// アイテムを所持アイテム一覧に追加します。
    /// </summary>
    /// <param name="item">追加対象のアイテム</param>
    public void AddItem(Item item)
    {
        if (Items.Count >= MaxItems)
        {
            MessageWindow.Instance.AppendMessage("これ以上アイテムを持てない！");
            return;
        }
        Items.Add(item);
    }

}
