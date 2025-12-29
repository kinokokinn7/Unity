using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Roguelike.Window;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

using Zenject;

public class Player : MapObjectBase
{
    public int Level = 1;   // レベル
    public Food FoodValue;  // 満腹度

    // 階層
    public int Floor
    {
        get => MapSceneManager.Instance.CurrentFloor;
        set => MapSceneManager.Instance.CurrentFloor = value;
    }

    public readonly int NumberOfStepsToReduceFoodValue = 5;
    private int _numberOfSteps = 0;

    [Range(1, 10)] public int VisibleRange = 5; // 周りのマスが見える範囲

    private PlayerInputProvider _inputProvider;
    private PlayerFoodHandler _foodHandler;
    private MessageWindow _messageWindow;

    private IPlayerLevelManager _levelManager;
    private IPlayerInteractionManager _interactionManager;
    private IPlayerEffectManager _effectManager;

    [Inject]
    public void Construct(
        PlayerInputProvider inputProvider, 
        PlayerFoodHandler foodHandler, 
        MessageWindow messageWindow,
        IPlayerLevelManager levelManager,
        IPlayerInteractionManager interactionManager,
        IPlayerEffectManager effectManager
        )
    {
        _inputProvider = inputProvider;
        _foodHandler = foodHandler;
        _messageWindow = messageWindow;
        _levelManager = levelManager;
        _interactionManager = interactionManager;
        _effectManager = effectManager;
    }

    public MessageWindow MessageWindow => _messageWindow;

    /// <summary>
    /// オブジェクトの初期化時に一度だけ呼ばれます。プレイヤーUIの設定、カメラの移動、アクションの開始、マスの可視性更新を行います。
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // プレイヤーの初期ステータス設定
        CriticalChance = 0.15f;

        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.Set(this);
        
        _foodHandler.Initialize(this, FoodValue);
        _levelManager.Initialize(this);
        _interactionManager.Initialize(this);
        _effectManager.Initialize(this);

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
        Defence = saveData.Defence;
        FoodValue.CurrentValue = saveData.Food;
        Exp = saveData.Exp;
        if (saveData.Weapon)
        {
            CurrentWeapon = saveData.Weapon;
            var itemInventory = UnityEngine.Object.FindObjectOfType<ItemInventory>();
            if (itemInventory != null)
            {
                itemInventory.ReplaceEquippedWeapon(CurrentWeapon);
                itemInventory.ReplaceEquippedArmor(CurrentArmor);
            }
        }
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
        UseItem,
        Attack
    }

    public Action NowAction
    {
        get => _inputProvider.NowAction;
        private set { } // InputProvider manages state, but we might need dummy setter for compatibility or change usages. 
        // actually NowAction was used as setter in WaitInput.
        // We will change WaitInput to not set this property but InputProvider's state.
        // For external assignments (if any).. check.
    }
    
    // We remove the setter logic from here effectively by redirecting to inputProvider, 
    // but the property syntax `get; private set;` creates a backing field if auto-prop.
    // I should change it to non-auto prop.
    // However, I can't easily change all usages if I don't expose setter.
    // But WaitInput is local. 
    // Let's implement NowAction as a proxy.
   
    public void SetNowActionUseItem()
    {
        _inputProvider.RequestUseItem();
    }

    public bool DoWaitEvent 
    { 
        get => _interactionManager.DoWaitEvent;
        set => _interactionManager.SetDoWaitEvent(value);
    }

    public bool CanMove { get; set; } = true;

    /// <summary>
    /// プレイヤーのアクションをコルーチンで管理します。
    /// 入力待ち、アクションの実行、食糧の更新、可視マスの更新、イベントの確認を行います。
    /// </summary>
    IEnumerator ActionCoroutine()
    {
        while (true)
        {
            // 入力待ちをコルーチンとして完全に待機
            yield return StartCoroutine(WaitInput());
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
                case Action.UseItem:
                    // アイテム使用時、エフェクト再生が完了するまで待機
                    yield return new WaitWhile(() => IsNowUsingItem);
                    break;
                case Action.Attack:
                    var enemy = FindEnemyInFront();
                    if (enemy != null)
                    {
                        StartCoroutine(AttackTo(enemy));
                        var (movedMass, movedPos) = Map.GetMovePos(Pos, this.Forward);
                        yield return StartCoroutine(NotMoveCoroutine(movedPos));
                    }
                    break;
                case Action.None:
                    // アクションがNoneの場合は何もしない
                    continue;
            }
            UpdateFood();
            _inputProvider.ResetAction();

            UpdateVisibleMass();
            CheckEvent(); // ここで敵ターンに移行
            _numberOfSteps++;
            yield return new WaitWhile(() => DoWaitEvent);
        }
    }

    /// <summary>
    /// プレイヤーの満腹度を更新します。満腹度が0になるとHPが減少します。
    /// </summary>
    void UpdateFood()
    {
        _foodHandler.UpdateFood(_numberOfSteps, NumberOfStepsToReduceFoodValue);
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
        _inputProvider.ResetAction();
        // 前フレームのキー状態
        // Handled by PlayerInputProvider

        while (NowAction == Action.None)
        {
            yield return null;
            _inputProvider.PollInput();
        }
    }


    /// <summary>
    /// イベントの確認を行い、存在する場合はそれを実行します。
    /// </summary>
    void CheckEvent()
    {
        _interactionManager.CheckEvent();
    }

    // RunEvents and Goal have been moved to PlayerInteractionManager

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
        if (playerUI != null)
        {
            if (playerUI.HpText != null)
                playerUI.HpText.text = "0";
            else
                Debug.LogWarning("PlayerUI.HpText が Inspector で設定されていません。");
        }
        else
        {
            Debug.LogWarning("PlayerUI がシーン上に見つかりません。");
        }

        var mapManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        if (mapManager == null)
        {
            Debug.LogError("MapSceneManager がシーン上に見つかりません。GameOver 表示に失敗しました。");
            return;
        }

        if (mapManager.GameOver == null)
        {
            Debug.LogError("MapSceneManager.GameOver が Inspector で割り当てられていません。");
            return;
        }
        var go = mapManager.GameOver;
        go.SetActive(true);

        mapManager.GameOver.SetActive(true);
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
            
            _levelManager.AddExperience(other.Exp.GetCurrentValue());
        }
    }

    // GetNextRequiredExpValue moved to PlayerLevelManager

    /// <summary>
    /// プレイヤーが満腹度のダメージを受けた際の処理を行います。
    /// </summary>
    /// <param name="damage">受けたダメージ量。</param>
    public void FoodValueDamaged(int damage)
    {
        // 空腹度を減らす
        FoodValue.CurrentValue = Mathf.Max(FoodValue.CurrentValue - damage, 0);

        SoundEffectManager.Instance.PlayFoodDamagedSound();

        // ダメージ値を黄色文字でポップアップ表示する
        DamagePopup damagePopup = GetComponent<DamagePopup>();
        damagePopup.ShowDamage(damage, transform.position, Color.blue);
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

    // LevelUp, RotateWithEffects, JumpWithEffects moved to PlayerLevelManager and PlayerEffectManager

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
            else if (otherTreasureOrTrap is FinalGoal)
            {
                var finalGoal = (otherTreasureOrTrap as FinalGoal);
                StartCoroutine(finalGoal.Execute());
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
        trap.SpawnEffect(transform.position);
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

    /// <summary>
    /// プレイヤーの前方にいる敵を探します。
    /// </summary>
    /// <returns>敵キャラ。</returns>
    Enemy FindEnemyInFront()
    {
        var (movedMass, movedPos) = Map.GetMovePos(Pos, this.Forward);
        var character = movedMass.ExistCharacter;
        if (character == null || character.Equals(null))
        {
            return null;
        }
        return character.GetComponent<Enemy>();
    }

    internal async Task PlayAnimationForFinalGoal()
    {
        // プレイヤーの移動を一時的に停止
        CanMove = false;

        await _effectManager.PlayGoalEffect();

        MessageWindow.AppendMessage("ゲームクリア！");
    }

    /// <summary>
    /// 広告報酬でプレイヤーを復活させる処理を行います。
    /// </summary>
    public void ReviveFromAd()
    {
        if (!IsDead) return;

        // HPを最大値まで回復
        Hp.SetCurrentValue(this.Hp.GetMaxValue());
        this.CanMove = true;

        // メッセージウィンドウに復活メッセージを表示
        MessageWindow.AppendMessage("広告を視聴して復活した！");
    }

    public void Destroy()
    {
        // プレイヤーオブジェクトを削除
        Destroy(gameObject);
    }
}
