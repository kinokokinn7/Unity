using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// マップのシーン管理を担当するクラスです。
/// マップの生成、ゲームオーバー画面の表示、セーブデータの復元などを行います。
/// </summary>
public class MapSceneManager : MonoBehaviour
{
    public GameObject GameOver; // ゲームオーバー画面
    public bool IsAutoGenerate = true; // 自動でマップを生成するかどうか
    [SerializeField] Map.GenerateParam GenerateParam; // マップ生成のパラメータ

    /// <summary>
    /// マップを生成します。既存のマップを破棄し、新しいマップを生成パラメータに基づいて作成します。
    /// </summary>
    public void GenerateMap()
    {
        var map = GetComponent<Map>();
        map.DestroyMap();
        map.GenerateMap(GenerateParam);
    }


    [TextArea(3, 15)]
    public string mapData = // デフォルトのマップデータ
        "000P\n" +
        "0+++\n" +
        "0000\n" +
        "+++0\n" +
        "G000\n";

    /// <summary>
    /// オブジェクトが生成された際に呼び出されます。
    /// ゲームオーバー画面を非表示にし、セーブデータがあればそれを用いてマップを復元します。
    /// なければ新しいマップを生成します。
    /// </summary>
    void Awake()
    {
        GameOver.SetActive(false);

        var map = GetComponent<Map>();

        var saveData = SaveData.Load();
        if (saveData != null)
        {
            try
            {
                map.BuildMap(saveData.MapData);

                mapData = saveData.MapData.Aggregate("", (_s, _c) => _s + _c + '\n');
                var player = Object.FindObjectOfType<Player>();
                player.Recover(saveData);
                return;
            }
            catch (System.Exception e)
            {
                // エラー処理。復元に失敗したら普通のマップ生成を行う
                Debug.LogWarning($"Fail to Recover Save Data...");
            }
        }

        if (IsAutoGenerate)
        {
            map.GenerateMap(GenerateParam);
        }
        else
        {
            var lines = mapData.Split('\n').ToList();
            map.BuildMap(lines);
        }
    }

    // デバッグ用: スペースキーを押すとマップを再生成します。
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }

    }
}
