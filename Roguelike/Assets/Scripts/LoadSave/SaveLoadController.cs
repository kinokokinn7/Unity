using UnityEngine;
using System.IO;

public class SaveLoadController : MonoBehaviour
{

    private static string filePath;

    /// <summary>
    /// 起動時に保存先となるファイルパスを取得します。
    /// </summary>
    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "savedata.json");
    }

    /// <summary>
    /// 現在のプレイデータをセーブします。
    /// </summary>
    /// <param name="player">プレイヤー情報。</param>
    /// <param name="map">マップ情報。</param>
    /// <param name="itemInventory">アイテムリスト情報。</param>
    public void Save(Player player, Map map, ItemInventory itemInventory)
    {
        var saveData = new SaveData();

        // プレイヤー情報
        saveData.Level = player.Level;
        saveData.Hp = player.Hp;
        saveData.Attack = player.Attack;
        saveData.Food = player.FoodValue.CurrentValue;
        saveData.Exp = player.Exp;
        if (player.CurrentWeapon != null)
        {
            saveData.Weapon = player.CurrentWeapon;
        }

        // マップデータ
        saveData.MapData = map.MapData;
        saveData.Floor = player.Floor;

        // アイテムリスト
        saveData.Items = itemInventory.GetItemDataList();

        saveData.Save(filePath);
    }

    /// <summary>
    /// セーブデータをロードします。
    /// </summary>
    /// <returns></returns>
    public SaveData Load()
    {
        var saveData = SaveData.Load(filePath);
        if (saveData == null) return null;

        var itemInventory = Object.FindObjectOfType<ItemInventory>();
        itemInventory?.Recover(saveData.Items);

        return saveData;
    }


}
