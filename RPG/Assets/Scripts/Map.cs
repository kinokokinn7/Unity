using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public Grid Grid { get => GetComponent<Grid>(); }
    Dictionary<string, Tilemap> _tilemaps;

    readonly static string BACKGROUND_TILEMAP_NAME = "Background";
    readonly static string NONE_OBJECTS_TILEMAP_NAME = "NoneObjects";
    readonly static string OBJECTS_TILEMAP_NAME = "Objects";
    readonly static string EVENT_BOX_TILEMAP_NAME = "EventBox";

    /// <summary>
    /// タイルマップメンバ変数の初期化
    /// </summary>
    private void Awake()
    {
        _tilemaps = new Dictionary<string, Tilemap>();
        foreach (var tilemap in Grid.GetComponentsInChildren<Tilemap>())
        {
            _tilemaps.Add(tilemap.name, tilemap);
        }

        // EventBoxを非表示にする
        _tilemaps[EVENT_BOX_TILEMAP_NAME].gameObject.SetActive(false);
    }

    /// <summary>
    /// Grid位置からワールド座標を取得
    /// </summary>
    /// <param name="vector3Int"></param>
    /// <returns></returns>
    public Vector3 GetWorldPos(Vector3Int pos)
    {
        return Grid.CellToWorld(pos);
    }

    /// <summary>
    /// マスのクラス
    /// </summary>
    public class Mass
    {
        public bool isMovable;
        public TileBase eventTile;
        public MassEvent massEvent;
    }

    /// <summary>
    /// 指定位置のマスデータ（イベント、通行可能/禁止）を取得する
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns></returns>
    public Mass GetMassData(Vector3Int pos)
    {
        var mass = new Mass();
        // イベントタイルマップの指定位置のタイルを取得
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos);
        // 通行可能
        mass.isMovable = true;

        // マスにイベントが存在する場合
        if (mass.eventTile != null)
        {
            // イベントタイルからイベントを検索してマスイベント格納変数にセットする
            mass.massEvent = FindMassEvent(mass.eventTile);
        }
        // オブジェクトタイルマップにも指定位置にタイルがある場合
        else if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))
        {
            // 通行禁止
            mass.isMovable = false;
        }
        // 背景タイルマップの指定位置にタイルがない場合
        else if (_tilemaps[BACKGROUND_TILEMAP_NAME].GetTile(pos) == null)
        {
            // 通行禁止
            mass.isMovable = false;
        }
        return mass;

    }

    // EventBoxレイヤーにマップチップがある場合は移動できるようにする
    [SerializeField] List<MassEvent> _massEvents;

    /// <summary>
    /// マスのイベントを検索する。
    /// </summary>
    /// <param name="tile">タイル</param>
    /// <returns></returns>
    public MassEvent FindMassEvent(TileBase tile)
    {
        return _massEvents.Find(_c => _c.Tile == tile);
    }

    /// <summary>
    /// 指定のタイルがタイルマップ上のどの位置に配置されているかを取得する
    /// </summary>
    /// <param name="tile">検索するタイル</param>
    /// <param name="pos">検索結果（タイルマップ上の配置位置）</param>
    /// <returns>指定のタイルがタイルマップ上に存在する場合はtrue、そうでない場合はfalse</returns>
    public bool FindMassEventPos(TileBase tile, out Vector3Int pos)
    {
        // イベントレイヤーの取得
        var eventLayer = _tilemaps[EVENT_BOX_TILEMAP_NAME];
        // タイルマップのレンダラーを取得
        var renderer = eventLayer.GetComponent<TilemapRenderer>();
        // イベントレイヤーの最小のローカル位置をセル位置に変換
        var min = eventLayer.LocalToCell(renderer.bounds.min);
        // イベントレイヤーの最大のローカル位置をセル位置に変換
        var max = eventLayer.LocalToCell(renderer.bounds.max);

        pos = Vector3Int.zero;
        for (pos.y = min.y; pos.y < max.y; pos.y++)
        {
            for (pos.x = min.x; pos.x < max.x; pos.x++)
            {
                TileBase t = eventLayer.GetTile(pos);
                if (t == tile) return true;
            }
        }
        return false;
    }


}
