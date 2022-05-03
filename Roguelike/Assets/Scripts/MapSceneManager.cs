using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapSceneManager : MonoBehaviour
{
    public GameObject GameOver;
    public bool IsAutoGenerate = true;
    [SerializeField] Map.GenerateParam GenerateParam;
    public void GenerateMap()
    {
        var map = GetComponent<Map>();
        map.DestroyMap();
        map.GenerateMap(GenerateParam);
    }


    [TextArea(3, 15)]
    public string mapData =
        "000P\n" +
        "0+++\n" +
        "0000\n" +
        "+++0\n" +
        "G000\n";

    void Awake()
    {
        GameOver.SetActive(false);

        var map = GetComponent<Map>();

        var saveData = SaveData.Recover();
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

    // デバッグ用のコード
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }

    }
}
