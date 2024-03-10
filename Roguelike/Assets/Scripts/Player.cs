using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

class Player : MapObjectBase
{
    public int Level = 1;   // レベル
    public int Food = 99;   // 満腹度
    public int Floor = 1;   // 階層

    [Range(1, 10)] public int VisibleRange = 5; // 周りのマスが見える範囲

    MessageWindow _messageWindow;

    /// <summary>
    /// メッセージウィンドウインスタンスを取得または生成します。
    /// </summary>
    MessageWindow MessageWindow
    {
        get => _messageWindow != null ? _messageWindow : (_messageWindow = MessageWindow.Find());
    }

    public Player()
    {
        this.Hp = new Hp(10);
    }

    /// <summary>
    /// オブジェクトの初期化時に一度だけ呼ばれます。プレイヤーUIの設定、カメラの移動、アクションの開始、マスの可視性更新を行います。
    /// </summary>
    void Start()
    {
        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.Set(this);

        StartCoroutine(CameraMove());
        StartCoroutine(ActionCoroutine());

        UpdateVisibleMass();
    }

    /// <summary>
    /// プレイヤーの周囲のマスを可視状態に更新します。
    /// </summary>
    private void UpdateVisibleMass()
    {
        var map = Map;
        var startPos = Pos - Vector2Int.one * VisibleRange;
        var endPos = Pos + Vector2Int.one * VisibleRange;

        for (var y = startPos.y; y <= endPos.y; y++)
        {
            if (y < 0) continue;
            if (y >= Map.MapSize.y) break;

            for (var x = startPos.x; x <= endPos.x; x++)
            {
                if (x < 0) continue;
                if (x >= Map.MapSize.x) break;
                map[x, y].Visible = true;
            }
        }
    }

    /// <summary>
    /// セーブデータからプレイヤーの状態を復元します。
    /// </summary>
    /// <param name="saveData">復元するためのセーブデータ。</param>
    public void Recover(SaveData saveData)
    {
        CurrentWeapon = null;
        Level = saveData.Level;
        Hp.SetCurrentValue(saveData.Hp);
        Hp.SetMaxValue(saveData.MaxHp);
        Attack = saveData.Attack;
        Exp = saveData.Exp;
        if (saveData.WeaponName != "")
        {
            var weapon = ScriptableObject.CreateInstance<Weapon>();
            weapon.Name = saveData.WeaponName;
            weapon.Attack = saveData.WeaponAttack;
            CurrentWeapon = weapon;
        }
        Floor = saveData.Floor;
    }

    /// <summary>
    /// プレイヤーの可能なアクションを列挙します。
    /// </summary>
    public enum Action
    {
        None,
        MoveUp,
        MoveDown,
        MoveRight,
        MoveLeft,
    }

    public Action NowAction { get; private set; } = Action.None;
    public bool DoWaitEvent { get; set; } = false;

    /// <summary>
    /// プレイヤーのアクションをコルーチンで管理します。入力待ち、アクションの実行、食糧の更新、可視マスの更新、イベントの確認を行います。
    /// </summary>
    IEnumerator ActionCoroutine()
    {
        while (true)
        {
            StartCoroutine(WaitInput());
            yield return new WaitWhile(() => NowAction == Action.None);

            switch (NowAction)
            {
                case Action.MoveUp:
                case Action.MoveDown:
                case Action.MoveRight:
                case Action.MoveLeft:
                    Move(ToDirection(NowAction));
                    yield return new WaitWhile(() => IsNowMoving);
                    break;
            }
            UpdateFood();
            NowAction = Action.None;

            UpdateVisibleMass();

            CheckEvent();
            yield return new WaitWhile(() => DoWaitEvent);
        }
    }

    /// <summary>
    /// プレイヤーの食糧を更新します。食糧が0になるとHPが減少します。
    /// </summary>
    void UpdateFood()
    {
        Food--;
        if (Food <= 0)
        {
            Food = 0;
            Hp.decreaseCurrentValue(1);
            MessageWindow.AppendMessage($"空腹で1ダメージ！");
            if (Hp.isZero())
            {
                Dead();
            }
        }
    }

    /// <summary>
    /// 指定されたアクションに基づいて移動方向を決定します。
    /// </summary>
    /// <param name="action">移動方向を決定するためのアクション。</param>
    /// <returns>決定された移動方向。</returns>
    Direction ToDirection(Action action)
    {
        switch (NowAction)
        {
            case Action.MoveUp: return Direction.North;
            case Action.MoveDown: return Direction.South;
            case Action.MoveRight: return Direction.East;
            case Action.MoveLeft: return Direction.West;
            default: throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// プレイヤーの入力を待ちます。キー入力があるまでアクションはNoneに設定されます。
    /// </summary>
    IEnumerator WaitInput()
    {
        NowAction = Action.None;
        while (NowAction == Action.None)
        {
            yield return null;
            if (Input.GetKey(KeyCode.UpArrow)) NowAction = Action.MoveUp;
            if (Input.GetKey(KeyCode.DownArrow)) NowAction = Action.MoveDown;
            if (Input.GetKey(KeyCode.RightArrow)) NowAction = Action.MoveRight;
            if (Input.GetKey(KeyCode.LeftArrow)) NowAction = Action.MoveLeft;
        }
    }

    /// <summary>
    /// イベントの確認を行い、存在する場合はそれを実行します。
    /// </summary>
    void CheckEvent()
    {
        DoWaitEvent = false;
        StartCoroutine(RunEvents());
    }

    /// <summary>
    /// イベントの実行を行います。
    /// 敵の移動、敵の移動完了の待機、ゴール判定、ゴール時の処理を含みます。

    IEnumerator RunEvents()
    {
        foreach (var enemy in UnityEngine.Object.FindObjectsOfType<Enemy>())
        {
            enemy.MoveStart();
        }
        yield return new WaitWhile(() =>
            UnityEngine.Object.FindObjectsOfType<Enemy>().All(_e => !_e.IsNowMoving));

        var mass = Map[Pos.x, Pos.y];
        if (mass.Type == MassType.Goal)
        {
            StartCoroutine(Goal());
        }
        else
        {
            DoWaitEvent = true;
        }
    }

    /// <summary>
    /// ゴール時の処理を行います。
    /// 新しいマップの生成、プレイヤーのデータ引継ぎ、セーブデータの作成と保存を含みます。
    /// </summary>
    private IEnumerator Goal()
    {
        yield return new WaitForSeconds(1.0f);  // ゴール時にウェイトを入れる

        // マップを新規生成
        var mapSceneManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        mapSceneManager.GenerateMap();

        // プレイヤーのデータを引き継ぐ
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.Hp = Hp;
        player.Food = Food;
        player.Exp = Exp;
        player.Level = Level;
        player.CurrentWeapon = CurrentWeapon;
        player.Attack = Attack;
        // CurrentWeapon呼び出し時にAttackが上昇してしまうので
        // Attack代入はCurrentPosition代入の後

        // 階層を1つ下げる
        Floor += 1;
        player.Floor = Floor;

        var saveData = new SaveData();
        saveData.Level = Level;
        saveData.Hp = Hp.GetCurrentValue();
        saveData.MaxHp = Hp.GetMaxValue();
        saveData.Attack = Attack;
        saveData.Exp = Exp;
        if (CurrentWeapon != null)
        {
            saveData.Attack -= CurrentWeapon.Attack;
            saveData.WeaponName = CurrentWeapon.Name;
            saveData.WeaponAttack = CurrentWeapon.Attack;
        }
        else
        {
            saveData.WeaponName = "";
            saveData.WeaponAttack = 0;
        }
        saveData.MapData = Map.MapData;
        saveData.Floor = Floor;
        saveData.Save();
    }

    [Range(0, 100)] public float CameraDistance;
    public Vector3 CameraDirection = new Vector3(0, 10, -3);

    /// <summary>
    /// カメラをプレイヤーに追従させる処理を行います。
    /// </summary>
    /// <returns></returns>
    IEnumerator CameraMove()
    {
        var camera = Camera.main;
        while (true)
        {
            // カメラの位置をプレイヤーからの相対位置に設定する
            camera.transform.position = transform.position + CameraDirection.normalized * CameraDistance;
            camera.transform.LookAt(transform.position);
            yield return null;
        }
    }

    /// <summary>
    /// プレイヤーの死亡時の処理を行います。UIの更新とゲームオーバー画面の表示を含みます。
    /// </summary>
    public override void Dead()
    {
        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.HpText.text = "0";

        base.Dead();

        var mapManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        mapManager.GameOver.SetActive(true);

        SaveData.Destroy();

    }

    /// <summary>
    /// プレイヤーが攻撃する際の処理を行います。ダメージの計算と敵の死亡判定を含みます。
    /// </summary>
    /// <param name="other">敵キャラ</param>
    /// <returns></returns>
    public override bool AttackTo(MapObjectBase other)
    {
        MessageWindow.AppendMessage($"プレイヤーのこうげき！　敵に{Attack}のダメージ！");
        other.Hp.decreaseCurrentValue(Attack);
        other.Damaged(Attack);  // 現段階ではEnemy.csでDamagedをオーバーライドしていないので何もしない

        if (other.Hp.isZero())
        {
            MessageWindow.AppendMessage($"敵を倒した！ {other.Exp}ポイントの経験値を手に入れた！");
            other.Dead();
            // 攻撃の結果、敵を倒したら、その敵のExp分自身のExpを上げる
            Exp += other.Exp;

            // レベルアップ処理
            if (Exp >= 10)
            {
                LevelUp();
            }
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// プレイヤーのレベルアップ処理を行います。
    /// </summary>
    public void LevelUp()
    {
        Level += 1;
        Hp.IncreaseMaxHp(5);
        Attack += 1;
        Exp = 0;

        MessageWindow.AppendMessage($"プレイヤーのレベルが{Level}に上がった！");
        MessageWindow.AppendMessage($"  HP +5  Atk + 1");
    }

    /// <summary>
    /// アイテムやトラップなどのオブジェクトが存在するマスへ移動します。
    /// </summary>
    /// <param name="mass">移動先のマス</param>
    /// <param name="movedPos">移動後の位置</param>
    protected override void MoveToExistObject(Map.Mass mass, Vector2Int movedPos)
    {
        var otherObject = mass.ExistObject.GetComponent<MapObjectBase>();
        if (otherObject is Treasure)
        {
            var treasure = (otherObject as Treasure);
            OpenTreasure(treasure, mass, movedPos);
            StartCoroutine(NotMoveCoroutine(movedPos));
            return;
        }
        else if (otherObject is Trap)
        {
            var trap = (otherObject as Trap);
            StampTrap(trap, mass, movedPos);
            StartCoroutine(NotMoveCoroutine(movedPos));
            return;
        }
        base.MoveToExistObject(mass, movedPos);
    }

    /// <summary>
    /// トラップに引っかかった時の処理です。
    /// </summary>
    /// <param name="trap">トラップ</param>
    /// <param name="mass">マス</param>
    /// <param name="movedPos">移動後の座標</param>
    /// <exception cref="System.NotImplementedException"></exception>
    protected void StampTrap(Trap trap, Map.Mass mass, Vector2Int movedPos)
    {
        MessageWindow.AppendMessage($"トラップにひっかかった！！");
        switch (trap.CurrentType)
        {
            case Trap.Type.LifeDown:
                Hp.decreaseCurrentValue(trap.Value);
                MessageWindow.AppendMessage($"{trap.Value}のダメージを受けた！");
                break;
            case Trap.Type.FoodDown:
                Food -= trap.Value;
                MessageWindow.AppendMessage($"満腹度が{trap.Value}下がった！");
                break;
            default:
                throw new System.NotImplementedException();
        }

        // 罠はマップから削除する
        mass.ExistObject = null;
        mass.Type = MassType.Road;
        UnityEngine.Object.Destroy(trap.gameObject);
    }

    /// <summary>
    /// 宝箱を開けたときの処理です。
    /// </summary>
    /// <param name="treasure">宝箱</param>
    /// <param name="mass">マス</param>
    /// <param name="movedPos">移動後の座標</param>
    /// <exception cref="System.NotImplementedException"></exception>
    protected void OpenTreasure(Treasure treasure, Map.Mass mass, Vector2Int movedPos)
    {
        switch (treasure.CurrentType)
        {
            case Treasure.Type.LifeUp:
                Hp.recover(treasure.Value);
                MessageWindow.AppendMessage($"  HPが回復した！ +{treasure.Value}");
                break;
            case Treasure.Type.FoodUp:
                Food += treasure.Value;
                MessageWindow.AppendMessage($"  満腹度が回復した！ +{treasure.Value}");
                break;
            case Treasure.Type.Weapon:
                // 装備中の武器の攻撃力に足し合わせるようにしている
                MessageWindow.AppendMessage($"  新しい武器を手に入れた！ +{treasure.CurrentWeapon}");
                var newWeapon = treasure.CurrentWeapon.Merge(CurrentWeapon);
                CurrentWeapon = newWeapon;
                break;
            default:
                throw new System.NotImplementedException();
        }

        // 宝箱を開けたらマップから削除する
        mass.ExistObject = null;
        mass.Type = MassType.Road;
        UnityEngine.Object.Destroy(treasure.gameObject);
    }
}
