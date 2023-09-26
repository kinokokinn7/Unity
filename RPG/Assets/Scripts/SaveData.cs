using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セーブ用のクラス。
///
/// このクラスでは次のデータを保存します。
/// 　・プレイヤーのパラメータ
/// 　・現在のマップとプレイヤーやNPCの位置
/// 　・宝箱を開けたかどうか(マップ単位)
/// 　・ボスの撃破状況
/// </summary>
public class SaveData : MonoBehaviour
{
    [SerializeField] List<Map> _maps;

    public void Save(RPGSceneManager manager)
    {
        // プレイヤーのパラメータを保存
        PlayerPrefs.SetString("player", JsonUtility.ToJson(manager.Player.GetSaveData()));

        // 現在のマップを保存
        var mapName = manager.ActiveMap.name;
        mapName = mapName.Replace("(Clone)", "");
        PlayerPrefs.SetString("activeMap", mapName);

        var instantMapData = manager.ActiveMap.GetInstantSaveData();
        PlayerPrefs.SetString("instantMapData", JsonUtility.ToJson(instantMapData));

        var activeMapKey = $"map_{manager.ActiveMap.name.Replace("(Clone)", "")}";
        foreach (var map in _maps)
        {
            var key = $"map_{map.name.Replace("(Clone)", "")}";
            if (key == activeMapKey)
            {
                Save(manager.ActiveMap);
            }
            else
            {
                SaveWithTemporary(map);
            }
        }
    }

    /// <summary>
    /// マップのセーブデータを保存します。
    /// </summary>
    /// <param name="map">保存対象のマップ</param>
    void Save(Map map)
    {
        var key = $"map_{map.name.Replace("(Clone)", "")}";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(map.GetSaveData()));
    }

    void SaveWithTemporary(Map map)
    {
        var key = $"map_{map.name.Replace("(Clone)", "")}";
        var tempKey = "temp_" + key;
        if (PlayerPrefs.HasKey(tempKey))
        {
            PlayerPrefs.SetString(key, PlayerPrefs.GetString(tempKey));
            PlayerPrefs.DeleteKey(tempKey);
        }
    }

    public void SaveTemporary(Map map)
    {
        var tempKey = $"temp_map_{map.name.Replace("(Clone)", "")}";
        var saveData = map.GetSaveData();
        PlayerPrefs.SetString(tempKey, JsonUtility.ToJson(saveData)
        );
    }
}
