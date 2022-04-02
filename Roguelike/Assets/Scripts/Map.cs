using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

enum MassType
{
    Road,   // 通路
    Wall,   // 壁
    Player, // プレイヤー
    Goal,   // ゴール
    Enemy,  // 敵
    Treasure,   // 宝箱（ライフ回復）
    FoodTreasure,   // 宝箱（食べ物）
    WeaponTreasure, // 宝箱（武器）
}

[System.Serializable]
class MassData
{
    public GameObject Prefab;
    public MassType Type;   // マスの種別
    public char MapChar;    // マスの文字
    public bool IsRoad;     // 通路かどうか
    public bool IsCharacter;// プレイヤーかどうか？
}

enum Direction
{
    North,  // 北
    South,  // 南
    East,   // 東
    West,   // 西
}

class Map : MonoBehaviour
{
    public class Mass
    {
        public MassType Type;
        public GameObject MassGameObject;
        public GameObject ExistObject;
    }

    public MassData[] massDataList;
    public Vector2 MassOffset = new Vector2(1, 1);

    public bool IsNowBuilding { get; private set; }
    public Vector2Int StartPos { get; set; }        // マップ上の開始位置
    public Direction StartForward { get; set; }     // プレイヤーの開始時の向き

    Dictionary<MassType, MassData> MassDataDict { get; set; }
    Dictionary<char, MassData> MapCharDict { get; set; }

    public Vector2Int MapSize { get; private set; }
    List<List<Mass>> Data { get; set; }
    public Mass this[int x, int y]
    {
        get => Data[y][x];
        set => Data[y][x] = value;
    }

    public MassData this[MassType type]
    {
        get => MassDataDict[type];
    }

    public void BuildMap(List<string> map)
    {
        InitMassData();

        var mapSize = Vector2Int.zero;
        Data = new List<List<Mass>>();
        foreach (var line in map)
        {
            var lineData = new List<Mass>();
            for (var i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (!MapCharDict.ContainsKey(ch))
                {
                    Debug.LogWarning("どのマスか分からない文字がマップデータに存在しています。 ch=" + ch);
                    ch = MapCharDict.First().Key;   // 一応、始めのデータで代用する
                }

                var massData = MapCharDict[ch];         // これから生成するマスのデータを格納 
                var mass = new Mass();
                var pos = CalcMapPos(i, Data.Count);    // Count:Lengthと同じ
                if (massData.IsCharacter)
                {
                    mass.ExistObject = Object.Instantiate(massData.Prefab, transform);
                    var mapObject = mass.ExistObject.GetComponent<MapObjectBase>();
                    mapObject.SetPosAndForward(new Vector2Int(i, Data.Count), Direction.South);

                    // キャラクターの時は道も一緒に作成する
                    massData = this[MassType.Road];

                }

                if (massData.IsCharacter)
                {
                    massData = this[MassType.Road];
                }
                mass.Type = massData.Type;
                mass.MassGameObject = Object.Instantiate(massData.Prefab, transform);
                mass.MassGameObject.transform.position = pos;
                lineData.Add(mass);
            }
            Data.Add(lineData);

            // マップサイズの設定
            mapSize.x = Mathf.Max(mapSize.x, line.Length);
            mapSize.y++;
        }
        MapSize = mapSize;
    }

    private void InitMassData()
    {
        MassDataDict = new Dictionary<MassType, MassData>();
        MapCharDict = new Dictionary<char, MassData>();
        foreach (var massData in massDataList)
        {
            // マスデータのディクショナリ作成
            MassDataDict.Add(massData.Type, massData);
            MapCharDict.Add(massData.MapChar, massData);
        }
    }

    public Vector3 CalcMapPos(int x, int y)
    {
        var pos = Vector3.zero;
        pos.x = x * MassOffset.x;
        pos.z = y * MassOffset.y * -1;
        return pos;
    }

    public Vector3 CalcMapPos(Vector2Int pos) => CalcMapPos(pos.x, pos.y);

    public Vector2Int CalcDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return new Vector2Int(0, -1);
            case Direction.South: return new Vector2Int(0, 1);
            case Direction.East: return new Vector2Int(1, 0);
            case Direction.West: return new Vector2Int(-1, 0);
            default: throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 移動後位置座標の取得
    /// </summary>
    /// <param name="currentPos">現在位置座標</param>
    /// <param name="moveDir">移動方向</param>
    /// <returns></returns>
    public (Mass mass, Vector2Int movedPos) GetMovePos(Vector2Int currentPos,
        Direction moveDir)
    {
        var offset = CalcDirection(moveDir);
        var movedPos = currentPos + offset;

        // 移動後位置座標の範囲外チェック
        if (movedPos.x < 0 || movedPos.y < 0) return (null, currentPos);
        if (movedPos.y >= MapSize.y) return (null, currentPos);
        var line = Data[movedPos.y];
        if (movedPos.x >= line.Count) return (null, currentPos);
        var mass = line[movedPos.x];

        return (mass, movedPos);

    }

    public static Direction TurnLeftDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.East;
            case Direction.South: return Direction.West;
            case Direction.East: return Direction.South;
            case Direction.West: return Direction.North;
            default: throw new System.NotImplementedException();
        }
    }

    public static Direction TurnRightDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.West;
            case Direction.South: return Direction.East;
            case Direction.East: return Direction.North;
            case Direction.West: return Direction.South;
            default: throw new System.NotImplementedException();
        }
    }
}
