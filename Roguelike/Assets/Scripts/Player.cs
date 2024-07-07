using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Roguelike.Window;

class Player : MapObjectBase
{
    public int Level = 1;   // レベル
    public Food FoodValue;  // 満腹度
    public int Floor = 1;   // 階層

    public readonly int NumberOfStepsToReduceFoodValue = 5;
    private int _numberOfSteps = 0;

    [Range(1, 10)] public int VisibleRange = 5; // 周りのマスが見える範囲

    MessageWindow _messageWindow;

    /// <summary>
    /// メッセージウィンドウインスタンスを取得または生成します。
    /// </summary>
    MessageWindow MessageWindow
    {
        get => _messageWindow != null ? _messageWindow : (_messageWindow = MessageWindow.Instance);
    }

    /// <summary>
    /// オブジェクトの初期化時に一度だけ呼ばれます。プレイヤーUIの設定、カメラの移動、アクションの開始、マスの可視性更新を行います。
    /// </summary>
    protected override void Start()
    {
        base.Start();
        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.Set(this);

        MessageWindow.Hide();

        StartCoroutine(FadeController.Instance.FadeIn());
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
        Hp = saveData.Hp;
        Attack = saveData.Attack;
        FoodValue.CurrentValue = saveData.Food;
        Exp = saveData.Exp;
        if (saveData.WeaponName != "")
        {
            var weapon = ScriptableObject.CreateInstance<Weapon>();
            weapon.Name = saveData.WeaponName;
            weapon.Attack.SetCurrentValue(saveData.WeaponAttack);
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

    public bool CanMove { get; set; } = true;

    /// <summary>
    /// プレイヤーのアクションをコルーチンで管理します。入力待ち、アクションの実行、食糧の更新、可視マスの更新、イベントの確認を行います。
    /// </summary>
    IEnumerator ActionCoroutine()
    {
        while (true)
        {
            StartCoroutine(WaitInput());
            yield return new WaitWhile(() => NowAction == Action.None);

            // プレイヤーが移動不可能な場合は移動処理を行わない
            if (!this.CanMove) continue;

            switch (NowAction)
            {
                case Action.MoveUp:
                case Action.MoveDown:
                case Action.MoveRight:
                case Action.MoveLeft:
                    Move(ToDirection(NowAction));
                    yield return new WaitWhile(() => IsNowMoving || IsNowAttacking);
                    break;
            }
            UpdateFood();
            NowAction = Action.None;

            UpdateVisibleMass();
            CheckEvent();
            _numberOfSteps++;
            yield return new WaitWhile(() => DoWaitEvent);
        }
    }

    /// <summary>
    /// プレイヤーの満腹度を更新します。満腹度が0になるとHPが減少します。
    /// </summary>
    void UpdateFood()
    {
        // 満腹度が1減る歩数を満たしていない場合は処理をスキップする
        if (_numberOfSteps % NumberOfStepsToReduceFoodValue != 0) return;

        FoodValue.CurrentValue--;
        if (FoodValue.CurrentValue <= 0)
        {
            FoodValue.CurrentValue = 0;
            MessageWindow.AppendMessage($"空腹で1ダメージ！");
            Damaged(1);
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
            if (Input.GetKey(KeyCode.UpArrow) || VirtualGamePad.Instance.IsUpPressed()) NowAction = Action.MoveUp;
            if (Input.GetKey(KeyCode.DownArrow) || VirtualGamePad.Instance.IsDownPressed()) NowAction = Action.MoveDown;
            if (Input.GetKey(KeyCode.RightArrow) || VirtualGamePad.Instance.IsRightPressed()) NowAction = Action.MoveRight;
            if (Input.GetKey(KeyCode.LeftArrow) || VirtualGamePad.Instance.IsLeftPressed()) NowAction = Action.MoveLeft;
        }
    }

    /// <summary>
    /// イベントの確認を行い、存在する場合はそれを実行します。
    /// </summary>
    void CheckEvent()
    {
        DoWaitEvent = true;
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
            UnityEngine.Object.FindObjectsOfType<Enemy>().Any(_e => _e.IsNowAttacking));

        var mass = Map[Pos.x, Pos.y];
        if (mass.Type == MassType.Goal)
        {
            StartCoroutine(Goal());
        }
        else
        {
            DoWaitEvent = false;
        }
    }

    /// <summary>
    /// ゴール時の処理を行います。
    /// 新しいマップの生成、プレイヤーのデータ引継ぎ、セーブデータの作成と保存を含みます。
    /// </summary>
    private IEnumerator Goal()
    {
        this.CanMove = false;

        SoundEffectManager.Instance.PlayStairSound();
        yield return StartCoroutine(FadeController.Instance.FadeOut());

        // マップを新規生成
        var mapSceneManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        mapSceneManager.GenerateMap();

        // プレイヤーのデータを引き継ぐ
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.Hp = Hp;
        player.FoodValue.CurrentValue = FoodValue.CurrentValue;
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
        saveData.Hp = Hp;
        saveData.Attack = Attack;
        saveData.Food = FoodValue.CurrentValue;
        saveData.Exp = Exp;
        if (CurrentWeapon != null)
        {
            saveData.Attack.DecreaseCurrentValue(CurrentWeapon.Attack.GetCurrentValue());
            saveData.WeaponName = CurrentWeapon.Name;
            saveData.WeaponAttack = CurrentWeapon.Attack.GetCurrentValue();
        }
        else
        {
            saveData.WeaponName = "";
            saveData.WeaponAttack = 0;
        }
        saveData.MapData = Map.MapData;
        saveData.Floor = Floor;
        saveData.Save();

        this.CanMove = true;
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
        base.Dead();

        this.CanMove = false;

        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.HpText.text = "0";

        var mapManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        mapManager.GameOver.SetActive(true);

        SaveData.Destroy();

    }

    /// <summary>
    /// プレイヤーが攻撃する際の処理を行います。ダメージの計算と敵の死亡判定を含みます。
    /// </summary>
    /// <param name="other">敵キャラ</param>
    public override IEnumerator AttackTo(MapObjectBase other)
    {
        yield return base.AttackTo(other);

        if (other.IsDead)
        {
            MessageWindow.AppendMessage($"{other.Name}を倒した！ {other.Exp.GetCurrentValue()}ポイントの経験値を手に入れた！");
            // 攻撃の結果、敵を倒したら、その敵のExp分自身のExpを上げる
            Exp.IncreaseCurrentValue(other.Exp.GetCurrentValue());

            // レベルアップ処理
            if (Exp.GetCurrentValue() >= 10)
            {
                LevelUp();
            }
        }
    }

    /// <summary>
    /// プレイヤーが満腹度のダメージを受けた際の処理を行います。
    /// </summary>
    /// <param name="damage">受けたダメージ量。</param>
    public void FoodValueDamaged(int damage)
    {
        // 空腹度を減らす
        FoodValue.CurrentValue -= damage;

        SoundEffectManager.Instance.PlayFoodDamagedSound();

        // ダメージ値を黄色文字でポップアップ表示する
        DamagePopup damagePopup = GetComponent<DamagePopup>();
        damagePopup.ShowDamage(damage, transform.position, Color.yellow);
    }

    /// <summary>
    /// プレイヤーの満腹度が回復した際の処理を行います。
    /// </summary>
    /// <param name="value">満腹度の回復量。</param>
    public void FoodValueRecovered(int value)
    {
        SoundEffectManager.Instance.PlayFoodRecoveredSound();

        // 回復値をシアン色文字でポップアップ表示する
        DamagePopup damagePopup = GetComponent<DamagePopup>();
        damagePopup.ShowDamage(value, transform.position, Color.cyan);
    }

    /// <summary>
    /// プレイヤーのレベルアップ処理を行います。
    /// </summary>
    public void LevelUp()
    {
        Level += 1;
        Hp.IncreaseMaxHp(5);
        Attack.IncreaseCurrentValue(1);
        Exp.Reset();

        SoundEffectManager.Instance.PlayLevelUpSound();
        MessageWindow.AppendMessage($"{this.Name}のレベルが{Level}に上がった！");
        MessageWindow.AppendMessage($"  HP +5  Atk + 1");
    }

    /// <summary>
    /// アイテムやトラップなどのオブジェクトが存在するマスへ移動します。
    /// </summary>
    /// <param name="mass">移動先のマス</param>
    /// <param name="movedPos">移動後の位置</param>
    protected override void MoveToExistObject(Map.Mass mass, Vector2Int movedPos, bool isPlayer)
    {
        var otherTreasureOrTrap = mass.ExistTreasureOrTrap?.GetComponent<MapObjectBase>();
        if (otherTreasureOrTrap != null)
        {
            if (otherTreasureOrTrap is Treasure)
            {
                var treasure = (otherTreasureOrTrap as Treasure);
                treasure.OpenTreasure(this, mass, movedPos);
                StartCoroutine(MoveCoroutine(movedPos));
                return;
            }
            else if (otherTreasureOrTrap is Trap)
            {
                var trap = (otherTreasureOrTrap as Trap);
                StampTrap(trap, mass, movedPos);
                StartCoroutine(NotMoveCoroutine(movedPos));
                return;
            }
        }
        base.MoveToExistObject(mass, movedPos, true);
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
                MessageWindow.AppendMessage($"{trap.Value}のダメージを受けた！");
                Damaged(trap.Value);
                break;
            case Trap.Type.FoodDown:
                MessageWindow.AppendMessage($"満腹度が{trap.Value}下がった！");
                FoodValueDamaged(trap.Value);
                break;
            default:
                throw new System.NotImplementedException();
        }

        // 罠はマップから削除する
        mass.ExistTreasureOrTrap = null;
        mass.Type = MassType.Road;
        UnityEngine.Object.Destroy(trap.gameObject);
    }
}
