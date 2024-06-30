using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// マップ上のマスの種類を表す列挙型です。
/// </summary>
public enum MassType
{
    Road,   // 通路
    Wall,   // 壁 
    Player, // プレイヤー
    Goal,   // ゴール
    Enemy,  // 敵
    Treasure,   // 宝箱（ライフ回復）
    FoodTreasure,   // 宝箱（食べ物）
    WeaponTreasure, // 宝箱（武器）
    Trap,           // 罠
    FoodTrap,       // 罠（食べ物）
}

/// <summary>
/// マップ上のマスのデータを保持するクラスです。
/// </summary>
[System.Serializable]
public class MassData
{
    public GameObject Prefab;
    public MassType Type;   // マスの種別
    public char MapChar;    // マスの文字
    public bool IsRoad;     // 通路かどうか
    public bool IsCharacter;// キャラクター（プレイヤー or 敵）かどうか？
}

public enum Direction
{
    North,  // 北
    South,  // 南
    East,   // 東
    West,   // 西
}

/// <summary>
/// マップを生成し、管理するクラスです。
/// </summary>
public class Map : MonoBehaviour
{
    /// <summary>
    /// マップを構成するマスを表すクラスです。
    /// </summary>
    public class Mass
    {
        public MassType Type;
        public GameObject MassGameObject;
        public GameObject ExistCharacter;  // キャラクター（プレイヤー or 敵）
        public GameObject ExistTreasureOrTrap;  // アイテム/罠

        /// <summary>
        /// マスの可視状態を取得または設定します。
        /// </summary>
        public bool Visible
        {
            get => MassGameObject.activeSelf;
            set
            {
                // 入力値が既定値と同じ場合は何もしない
                if (MassGameObject.activeSelf == value) return;

                MassGameObject.SetActive(value);
                if (ExistCharacter != null && ExistCharacter.TryGetComponent<MapObjectBase>(out var character))
                {
                    character.Visible = value;
                }
                if (ExistTreasureOrTrap != null && ExistTreasureOrTrap.TryGetComponent<MapObjectBase>(out var treasureOrTrap))
                {
                    treasureOrTrap.Visible = value;
                }
            }
        }
    }

    /// <summary>
    /// 利用可能な全マスデータ。
    /// </summary>
    public MassData[] massDataList;

    /// <summary>
    /// マスのオフセット。
    /// </summary>
    public Vector2 MassOffset = new Vector2(1, 1);

    /// <summary>
    /// マップのBGM。
    /// </summary>
    public AudioSource bgmSource;

    /// <summary>
    /// マップの生成中かどうかを示す値を取得します。
    /// </summary>
    public bool IsNowBuilding { get; private set; }

    /// <summary>
    /// マップの開始位置を設定または取得します。
    /// </summary>
    public Vector2Int StartPos { get; set; }

    /// <summary>
    /// プレイヤーの開始時の向きを設定または取得します。
    /// </summary>
    public Direction StartForward { get; set; }

    Dictionary<MassType, MassData> MassDataDict { get; set; }
    Dictionary<char, MassData> MapCharDict { get; set; }

    /// <summary>
    /// マップのサイズを取得します。
    /// </summary>
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

    public List<string> MapData { get; private set; }

    /// <summary>
    /// マップデータを基にマップを構築します。
    /// </summary>
    /// <param name="map">マップデータを表す文字列のリスト。</param>
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
                    mass.ExistCharacter = Object.Instantiate(massData.Prefab, transform);
                    var mapObject = mass.ExistCharacter.GetComponent<MapObjectBase>();
                    mapObject.SetPosAndForward(new Vector2Int(i, Data.Count), Direction.South);

                    // キャラクターの時は道も一緒に作成する
                    massData = this[MassType.Road];
                }
                else if (
                    massData.Type == MassType.Treasure ||
                    massData.Type == MassType.FoodTreasure ||
                    massData.Type == MassType.WeaponTreasure ||
                    massData.Type == MassType.Trap ||
                    massData.Type == MassType.FoodTrap)
                {
                    mass.ExistTreasureOrTrap = Object.Instantiate(massData.Prefab, transform);
                    var mapObject = mass.ExistTreasureOrTrap.GetComponent<MapObjectBase>();
                    mapObject.SetPosAndForward(new Vector2Int(i, Data.Count), Direction.South);

                    // 道も一緒に作成する
                    massData = this[MassType.Road];
                }

                if (massData.Type == MassType.Road)
                {
                    pos.y = -0.5f;
                }

                mass.Type = massData.Type;
                mass.MassGameObject = Object.Instantiate(massData.Prefab, transform);
                mass.MassGameObject.transform.position = pos;
                lineData.Add(mass);
                mass.Visible = false;   // 初期状態では非表示にする
            }
            Data.Add(lineData);

            // マップサイズの設定
            mapSize.x = Mathf.Max(mapSize.x, line.Length);
            mapSize.y++;
        }
        MapSize = mapSize;
        MapData = map;
    }

    /// <summary>
    /// マップの各マスデータを初期化します。
    /// </summary>
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

    /// <summary>
    /// 指定された座標に基づいてマップ上の位置を計算します。
    /// </summary>
    /// <param name="x">X座標。</param>
    /// <param name="y">Y座標。</param>
    /// <returns>マップ上の位置を表すVector3。</returns>
    public Vector3 CalcMapPos(int x, int y)
    {
        var pos = Vector3.zero;
        pos.x = x * MassOffset.x;
        pos.z = y * MassOffset.y * -1;
        return pos;
    }

    public Vector3 CalcMapPos(Vector2Int pos) => CalcMapPos(pos.x, pos.y);

    /// <summary>
    /// 指定された方向に基づいて座標のオフセットを計算します。
    /// </summary>
    /// <param name="dir">移動方向。</param>
    /// <returns>座標のオフセットを表すVector2Int。</returns>
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

    [System.Serializable]
    public class GenerateParam
    {
        public Vector2Int Size = new Vector2Int(20, 20);
        public int GoalMinDistance = 10;
        [Range(0, 1)] public float LimitMassPercent = 0.5f;
        [Range(0, 1)] public float RoadMassPercent = 0.7f;
    }
    public void DestroyMap()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// マップを生成し、初期化します。プレイヤーとゴールの位置を決定し、マップ上に障害物や特別なマスを配置します。
    /// </summary>
    /// <param name="generateParam">マップ生成のためのパラメータ。</param>
    public void GenerateMap(GenerateParam generateParam)
    {
        InitMassData();

        var mapData = new List<List<char>>();
        var wallData = this[MassType.Wall];
        var line = new List<char>();

        for (var x = 0; x < generateParam.Size.x; x++)
        {
            line.Add(wallData.MapChar);
        }
        for (var y = 0; y < generateParam.Size.y; y++)
        {
            mapData.Add(new List<char>(line));
        }

        PlacePlayerAndGoal(mapData, generateParam);
        PlaceMass(mapData, generateParam);

        BuildMap(mapData.Select(_l => _l.Aggregate("", (_s, _c) => _s + _c)).ToList());
    }

    /// <summary>
    /// プレイヤーとゴールの位置をマップ上に配置し、それらを結ぶ道を作成します。
    /// </summary>
    /// <param name="mapData">マップデータ。</param>
    /// <param name="generateParam">マップ生成のためのパラメータ。</param>
    void PlacePlayerAndGoal(List<List<char>> mapData, GenerateParam generateParam)
    {
        var rnd = new System.Random();
        var playerPos = new Vector2Int(rnd.Next(generateParam.Size.x), rnd.Next(generateParam.Size.y));

        var goalPos = playerPos;
        do
        {
            goalPos = new Vector2Int(rnd.Next(generateParam.Size.x), rnd.Next(generateParam.Size.y));
        } while ((int)(playerPos - goalPos).magnitude < generateParam.GoalMinDistance);

        // プレイヤーとゴールを結ぶ
        // その際、中間点を通るようにしている。
        var centerPos = playerPos;
        do
        {
            centerPos = new Vector2Int(rnd.Next(generateParam.Size.x), rnd.Next(generateParam.Size.y));
        } while ((playerPos == centerPos) || goalPos == centerPos);

        var roadData = this[MassType.Road];
        DrawLine(mapData, playerPos, centerPos, roadData.MapChar);
        DrawLine(mapData, centerPos, goalPos, roadData.MapChar);

        var playerData = this[MassType.Player];
        var goalData = this[MassType.Goal];
        mapData[playerPos.y][playerPos.x] = playerData.MapChar;
        mapData[goalPos.y][goalPos.x] = goalData.MapChar;
    }

    /// <summary>
    /// マップ上に線を描画します。このメソッドは、プレイヤーからゴールへの経路を作成する際に使用されます。
    /// </summary>
    /// <param name="mapData">マップデータ。</param>
    /// <param name="start">線の開始位置。</param>
    /// <param name="end">線の終了位置。</param>
    /// <param name="ch">線を描画するために使用する文字。</param>
    void DrawLine(List<List<char>> mapData, Vector2Int start, Vector2Int end, char ch)
    {
        var pos = start;
        var vec = (Vector2)(end - start);
        vec.Normalize();
        var diff = Vector2.zero;
        while (pos != end)
        {
            diff += vec;
            if (Mathf.Abs(diff.x) >= 1)
            {
                var offset = diff.x > 0 ? 1 : -1;
                pos.x += offset;
                diff.x -= offset;
                mapData[pos.y][pos.x] = ch;
            }
            if (Mathf.Abs(diff.y) >= 1)
            {
                var offset = diff.y > 0 ? 1 : -1;
                pos.y += offset;
                diff.y -= offset;
                mapData[pos.y][pos.x] = ch;
            }
        }
    }

    /// <summary>
    /// マップ上に特定のマスを配置します。このメソッドは、障害物や特別なアイテムが含まれるマスをランダムに配置するために使用されます。
    /// </summary>
    /// <param name="mapData">マップデータ。</param>
    /// <param name="generateParam">マップ生成のためのパラメータ。</param>
    void PlaceMass(List<List<char>> mapData, GenerateParam generateParam)
    {
        var rnd = new System.Random();
        var massSum = generateParam.Size.x * generateParam.Size.y;
        var placeMassCount = massSum * generateParam.LimitMassPercent;
        var wallData = this[MassType.Wall];
        var roadData = this[MassType.Road];
        var placeMassKeys = MassDataDict.Keys
            .Where(_k => _k != MassType.Wall
            && _k != MassType.Player
            && _k != MassType.Goal
            && _k != MassType.Road)
            .ToList();

        while (placeMassCount > 0)
        {
            var pos = Vector2Int.zero;
            var loopCount = placeMassCount * 10;
            do
            {
                // mapData[pos.y][pos.x] != wallData.MapChar条件の無限ループ回避用
                if (loopCount-- < 0)
                    break;
                pos = new Vector2Int(rnd.Next(generateParam.Size.x),
                    rnd.Next(generateParam.Size.y));
            } while (mapData[pos.y][pos.x] != wallData.MapChar);

            var t = rnd.Next(1000) / 1000f;
            if (t < generateParam.RoadMassPercent)
            {
                mapData[pos.y][pos.x] = roadData.MapChar;
            }
            else
            {
                var placeMassKey = placeMassKeys[rnd.Next(placeMassKeys.Count)];
                var placeMass = this[placeMassKey];
                mapData[pos.y][pos.x] = placeMass.MapChar;
            }
            placeMassCount--;

        }
    }

    /// <summary>
    /// マップのBGMを再生します。
    /// 再生中の場合は何も行いません。
    /// </summary>
    public void PlayBGM()
    {
        if (bgmSource == null) return;
        if (bgmSource.isPlaying) return;

        bgmSource.Play();
    }
}



