using System.Collections;
using System.Collections.Generic;
using Roguelike.Window;
using UnityEngine;

/// <summary>
/// ゲーム内でプレイヤーが見つけることができる宝箱を表すクラスです。宝箱からは様々なアイテムが手に入ります。
/// </summary>
class Treasure : MapObjectBase
{
    [SerializeField]
    Item _item;

    /// <summary>
    /// 宝箱を開けたときの処理です。
    /// </summary>
    /// <param name="treasure">宝箱</param>
    /// <param name="mass">マス</param>
    /// <param name="movedPos">移動後の座標</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OpenTreasure(Player player, Map.Mass mass, Vector2Int movedPos)
    {
        // 持ち物リストに宝箱の中のアイテムを追加する
        var itemInventory = UnityEngine.Object.FindObjectOfType<ItemInventory>();
        var added = itemInventory.AddItem(_item);
        if (!added) return;

        SoundEffectManager.Instance.PlayItemGetSound();

        // 宝箱を開けたらマップから削除する
        mass.ExistTreasureOrTrap = null;
        mass.Type = MassType.Road;
        UnityEngine.Object.Destroy(gameObject);
    }
}
