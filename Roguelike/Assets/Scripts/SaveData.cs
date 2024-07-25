using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームのセーブデータを管理するクラスです。プレイヤーの状態やゲームの進行状況を保存します。
/// </summary>
[System.Serializable]
class SaveData
{
    public int Floor; // 現在のフロア
    public int Level; // プレイヤーのレベル
    public Hp Hp; // プレイヤーのHP
    public Atk Attack; // プレイヤーの攻撃力
    public int Food; // プレイヤーの満腹度

    public Exp Exp; // プレイヤーの経験値
    public Weapon Weapon; // 装備している武器
    public List<string> MapData; // マップデータ

    /// <summary>
    /// セーブデータを保存します。現在のオブジェクトの状態をJSON形式で保存します。
    /// </summary>
    public void Save()
    {
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("save", json);
    }

    /// <summary>
    /// セーブデータを回復します。保存されているJSONデータからセーブデータを復元します。
    /// </summary>
    /// <returns>復元されたセーブデータ。セーブデータが存在しない場合はnullを返します。</returns>
    public static SaveData Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            var json = PlayerPrefs.GetString("save");
            Debug.Log($"json:{json}");
            return JsonUtility.FromJson<SaveData>(json);
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
