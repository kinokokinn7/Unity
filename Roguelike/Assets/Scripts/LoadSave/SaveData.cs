using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// ゲームのセーブデータを管理するクラスです。プレイヤーの状態やゲームの進行状況を保存します。
/// </summary>
[System.Serializable]
public class SaveData
{
    public int Floor; // 現在のフロア
    public int Level; // プレイヤーのレベル
    public Hp Hp; // プレイヤーのHP
    public Atk Attack; // プレイヤーの攻撃力
    public int Food; // プレイヤーの満腹度

    public Exp Exp; // プレイヤーの経験値
    public Weapon Weapon; // 装備している武器
    public List<string> MapData; // マップデータ
    public List<ItemData> Items; // アイテムリスト

    /// <summary>
    /// セーブデータを保存します。現在のオブジェクトの状態をJSON形式で保存します。
    /// </summary>
    public void Save(string filePath)
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        Debug.Log($"jsonfilePath:{filePath}");
        Debug.Log($"json:{json}");

        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// セーブデータを回復します。保存されているJSONデータからセーブデータを復元します。
    /// </summary>
    /// <returns>復元されたセーブデータ。セーブデータが存在しない場合はnullを返します。</returns>
    public static SaveData Load(string filePath)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = new List<JsonConverter> { new ItemDataConverter() }
        };

        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            Debug.Log($"jsonfilePath:{filePath}");
            Debug.Log($"json:{json}");
            return JsonConvert.DeserializeObject<SaveData>(json, settings);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 保存されているすべてのセーブデータを破棄します。
    /// </summary>
    public static void Destroy()
    {
        PlayerPrefs.DeleteAll();
    }
}
