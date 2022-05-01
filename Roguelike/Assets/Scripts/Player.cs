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

    MessageWindow _messageWindow;
    MessageWindow MessageWindow
    {
        get => _messageWindow != null ? _messageWindow : (_messageWindow = MessageWindow.Find());
    }

    void Start()
    {
        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.Set(this);

        StartCoroutine(CameraMove());
        StartCoroutine(ActionCoroutine());
    }

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
    IEnumerator ActionCoroutine()
    {
        while (true)
        {
            // 入力待ち
            StartCoroutine(WaitInput());
            yield return new WaitWhile(() => NowAction == Action.None);
            // アクションの実行
            switch (NowAction)
            {
                case Action.MoveUp:
                case Action.MoveDown:
                case Action.MoveRight:
                case Action.MoveLeft:
                    Move(ToDirection(NowAction));
                    yield return new WaitWhile(() => IsNowMoving);  // アクション終わるまで待つ
                    break;
            }
            UpdateFood();
            NowAction = Action.None;

            // イベントを確認
            CheckEvent();
            yield return new WaitWhile(() => DoWaitEvent);
        }

    }

    void UpdateFood()
    {
        Food--;
        if (Food <= 0)
        {
            Food = 0;
            Hp--;
            MessageWindow.AppendMessage($"空腹で1ダメージ！");
            if (Hp <= 0)
            {
                Dead();
            }
        }
    }

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

    IEnumerator WaitInput()
    {
        NowAction = Action.None;
        // キー入力の確認
        while (NowAction == Action.None)
        {
            yield return null;
            // 入力されたキーの確認
            if (Input.GetKeyDown(KeyCode.UpArrow)) NowAction = Action.MoveUp;
            if (Input.GetKeyDown(KeyCode.DownArrow)) NowAction = Action.MoveDown;
            if (Input.GetKeyDown(KeyCode.RightArrow)) NowAction = Action.MoveRight;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) NowAction = Action.MoveLeft;
        }
    }

    void CheckEvent()
    {
        DoWaitEvent = false;
        StartCoroutine(RunEvents());
    }

    IEnumerator RunEvents()
    {
        // 敵の移動処理
        foreach (var enemy in UnityEngine.Object.FindObjectsOfType<Enemy>())
        {
            enemy.MoveStart();
        }
        // 全ての敵が移動完了するまで待つ
        yield return new WaitWhile(() =>
            UnityEngine.Object.FindObjectsOfType<Enemy>().All(_e => !_e.IsNowMoving));

        // ゴール判定
        var mass = Map[Pos.x, Pos.y];
        if (mass.Type == MassType.Goal)
        {
            StartCoroutine(Goal());
        }
        else
        {
            // 終了処理
            DoWaitEvent = true;
        }
    }

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
        player.Floor = Floor + 1;
    }

    [Range(0, 100)] public float CameraDistance;
    public Vector3 CameraDirection = new Vector3(0, 10, -3);

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

    public override void Dead()
    {
        var playerUI = UnityEngine.Object.FindObjectOfType<PlayerUI>();
        playerUI.HpText.text = "0";

        base.Dead();

        var mapManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        mapManager.GameOver.SetActive(true);

    }

    public override bool AttackTo(MapObjectBase other)
    {
        MessageWindow.AppendMessage($"プレイヤーのこうげき！　敵に{Attack}のダメージ！");
        other.Hp -= Attack;
        other.Damaged(Attack);  // 現段階ではEnemy.csでDamagedをオーバーライドしていないので何もしない

        if (other.Hp <= 0)
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

    public void LevelUp()
    {
        Level += 1;
        Hp += 5;
        Attack += 1;
        Exp = 0;

        MessageWindow.AppendMessage($"プレイヤーのレベルが{Level}に上がった！");
        MessageWindow.AppendMessage($"  HP +5  Atk + 1");
    }

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

    protected void StampTrap(Trap trap, Map.Mass mass, Vector2Int movedPos)
    {
        MessageWindow.AppendMessage($"トラップにひっかかった！！");
        switch (trap.CurrentType)
        {
            case Trap.Type.LifeDown:
                Hp -= trap.Value;
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

    protected void OpenTreasure(Treasure treasure, Map.Mass mass, Vector2Int movedPos)
    {
        MessageWindow.AppendMessage($"宝箱を開けた！");
        switch (treasure.CurrentType)
        {
            case Treasure.Type.LifeUp:
                Hp += treasure.Value;
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
