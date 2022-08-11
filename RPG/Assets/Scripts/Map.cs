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
    }

    public Mass GetMassData(Vector3Int pos)
    {
        var mass = new Mass();
        // イベントタイルマップの指定位置のタイルを取得
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos);
        // 通行可能
        mass.isMovable = true;

        // オブジェクトタイルマップにも指定位置にタイルがある場合
        if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))
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


}
