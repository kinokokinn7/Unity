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
    public static MapSceneManager Instance { get; private set; }

    public int CurrentFloor { get; set; } = 1;  // 現在の階層

    public GameObject GameOver; // ゲームオーバー画面
    public bool IsAutoGenerate = true; // 自動でマップを生成するかどうか
    [SerializeField] Map.GenerateParam GenerateParam; // マップ生成のパラメータ

    public ItemInventory itemInventory;

    /// <summary>
    /// オブジェクトが生成された際に呼び出されます。
    /// ゲームオーバー画面を非表示にし、セーブデータがあればそれを用いてマップを復元します。
    /// なければ新しいマップを生成します。
    /// </summary>

    private void Awake()
    {
        // シングルトンパターンでMapSceneManagerを管理
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        GameOver.SetActive(false);
    }

    /// <summary>
    /// マップを生成します。既存のマップを破棄し、新しいマップを生成パラメータに基づいて作成します。
    /// </summary>
    public void GenerateMap()
    {
        var map = GetComponent<Map>();
        map.DestroyMap();

        // 階層のデータを取得
        var floorData = map.floorDataList.Where(
                floorData => floorData.FloorNumber == CurrentFloor
            ).FirstOrDefault();
        if (floorData?.MapData != null && floorData.MapData.Count > 0)
        {
            // 固有のマップデータを使用
            map.BuildMap(floorData.MapData);
        }
        else
        {
            // ランダム生成
            map.GenerateRandomMap(GenerateParam);
        }
    }

    [TextArea(3, 15)]
    public string mapData = // デフォルトのマップデータ
        "000P\n" +
        "0+++\n" +
        "0000\n" +
        "+++0\n" +
        "G000\n";

    // デバッグ用: スペースキーを押すとマップを再生成します。
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }
    }


    public void InitializeMapScene()
    {
        GenerateMap();
    }
    public void SetupMapSceneCommon()
    {
        var map = GetComponent<Map>();
        // BGMを再生する
        SoundEffectManager.Instance.PlayDungeonBGM();
    }

    public void LoadSavedMapScene(SaveData saveData)
    {
        var map = GetComponent<Map>();

        if (saveData != null)
        {
            try
            {
                // マップ情報のロード
                CurrentFloor = saveData.Floor;
                map.BuildMap(saveData.MapData);

                // プレイヤー情報のロード
                var player = Object.FindObjectOfType<Player>();
                player?.Recover(saveData);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("セーブデータの復元に失敗しました。新規マップを生成します。");
                // セーブデータが壊れていた場合など、通常のマップ生成処理にフォールバック
                GenerateMap();
            }
        }

        SetupMapSceneCommon();
    }
}
