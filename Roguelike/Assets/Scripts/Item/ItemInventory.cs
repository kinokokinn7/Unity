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
    public readonly int MaxItems = 30;

    /// <summary>
    /// ゲーム開始時にアイテムリストの各アイテムを複製（インスタンス化）します。
    /// </summary>
    private void Awake()
    {
        // アイテムリストを複製
        var originalItems = new List<Item>(_items);
        _items = new List<Item>();
        foreach (var item in originalItems)
        {
            var newItem = Instantiate(item);
            _items.Add(newItem);
        }
    }

    /// <summary>
    /// アイテムを所持アイテム一覧に追加します。
    /// </summary>
    /// <param name="item">追加対象のアイテム。</param>
    /// <returns>アイテムを追加できた場合は `true` 。</returns>
    public bool AddItem(Item item)
    {
        // アイテム数上限を設けるか、上限に達した時の処理を検討
        if (Items.Count >= MaxItems)
        {
            MessageWindow.Instance.AppendMessage("アイテムの上に乗った。");
            return false;
        }
        // Itemのインスタンスを複製する
        var newItem = Instantiate(item);
        Items.Add(newItem);
        MessageWindow.Instance.AppendMessage($"{newItem.Name}を手に入れた！");
        return true;
    }

    /// <summary>
    /// アイテムを所持アイテム一覧から削除します。
    /// </summary>
    /// <param name="item">削除対象のアイテム。</param>
    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

}
