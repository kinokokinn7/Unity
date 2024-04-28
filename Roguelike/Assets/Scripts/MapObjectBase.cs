using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// マップ上のオブジェクトの基本クラスです。プレイヤーや敵など、マップ上に存在する全てのオブジェクトの共通機能を提供します。
/// </summary>
class MapObjectBase : MonoBehaviour
{
    [Range(0, 100)] public float MoveSecond = 0.1f; // 移動にかかる時間
    public int Exp = 0; // 経験値

    public bool IsNowMoving { get; private set; } = false; // 現在移動中かどうか
    public Vector2Int Pos; // 現在の位置
    public Vector2Int PrevPos { get; protected set; } // 前の位置
    public Direction Forward; // 現在の向き

    private Map _map; // 現在のマップへの参照
    public Map Map
    {
        get => _map != null ? _map : (_map = Object.FindObjectOfType<Map>());
    }

    public Hp Hp; // HP
    public Atk Attack; // 攻撃力

    /// <summary>
    /// オブジェクトの所属グループを表します。プレイヤー、敵、その他などがあります。
    /// </summary>
    public enum Group
    {
        Player,
        Enemy,
        Other,
    }
    public Group CurrentGroup = Group.Other; // 現在の所属グループ

    private bool _visible = true; // 可視状態
    public bool Visible // オブジェクトの可視状態を取得または設定します。
    {
        get => _visible;
        set
        {
            _visible = value;
            foreach (var renderer
                in GetComponents<Renderer>().Concat(GetComponentsInChildren<Renderer>()))
            {
                renderer.enabled = value;
            }
        }
    }

    /// <summary>
    /// 死亡しているか否かを表すフラグ。
    /// </summary>
    public bool IsDead { get; protected set; } = false;

    /// <summary>
    /// オブジェクトの位置と向きを設定します。
    /// </summary>
    /// <param name="pos">新しい位置。</param>
    /// <param name="forward">新しい向き。</param>
    public void SetPosAndForward(Vector2Int pos, Direction forward)
    {
        // PosとForwardを設定し、Transformに反映させる
        PrevPos = new Vector2Int(-1, -1);
        Pos = pos;
        Forward = forward;

        transform.position = Map.CalcMapPos(Pos);
    }

    /// <summary>
    /// 指定された方向にオブジェクトを移動させます。移動が可能な場合は移動し、それ以外の場合は何もしません。
    /// </summary>
    /// <param name="dir">移動する方向。</param>
    public virtual void Move(Direction dir)
    {
        IsNowMoving = false;
        var (movedMass, movedPos) = Map.GetMovePos(Pos, dir);
        if (movedMass == null) return;

        var massData = Map[movedMass.Type];
        if (movedMass.ExistObject)
        {
            MoveToExistObject(movedMass, movedPos);
        }
        else if (massData.IsRoad)
        {
            MoveToRoad(movedMass, movedPos);
        }
        else
        {
            MoveToNotMoving(movedMass, movedPos);
        }

        // 向き修正
        switch (dir)
        {
            case Direction.North:
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case Direction.South:
                this.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                break;
            case Direction.East:
                this.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                break;
            case Direction.West:
                this.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                break;
        }


    }

    /// <summary>
    /// マスに既に存在するオブジェクトに対してアクションを実行します。攻撃可能なオブジェクトが存在する場合は攻撃し、移動します。
    /// 攻撃が不可能な場合、または攻撃しても相手を倒せなかった場合は、移動せずに現在位置に留まります。
    /// </summary>
    /// <param name="mass">対象のマス。</param>
    /// <param name="movedPos">移動を試みる位置。</param>
    protected virtual void MoveToExistObject(Map.Mass mass, Vector2Int movedPos)
    {
        var otherObject = mass.ExistObject.GetComponent<MapObjectBase>();
        if (IsAttackableObject(this, otherObject))
        {
            AttackTo(otherObject);
        }

        StartCoroutine(NotMoveCoroutine(movedPos));
    }

    /// <summary>
    /// 二つのオブジェクトが互いに攻撃可能かどうかを判定します。
    /// </summary>
    /// <param name="self">判定を行うオブジェクト自身。</param>
    /// <param name="other">判定対象のオブジェクト。</param>
    /// <returns>攻撃可能な場合はtrue、そうでなければfalse。</returns>
    public static bool IsAttackableObject(MapObjectBase self, MapObjectBase other)
    {
        return self.CurrentGroup != other.CurrentGroup
            && (self.CurrentGroup != Group.Other && other.CurrentGroup != Group.Other);
    }

    /// <summary>
    /// 他のマップオブジェクトに対して攻撃を行います。
    /// </summary>
    /// <param name="other">攻撃対象のオブジェクト。</param>
    /// <returns>攻撃によって対象を倒した場合はtrue、倒せなかった場合はfalse。</returns>
    public virtual bool AttackTo(MapObjectBase other)
    {
        int damageAmount = Attack.GetCurrentValue();
        other.Hp.decreaseCurrentValue(damageAmount);
        other.Damaged(damageAmount);
        if (other.Hp.isZero())
        {
            other.Dead();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// オブジェクトがダメージを受けた際の処理を行います。オーバーライドして具体的なダメージ処理を実装します。
    /// </summary>
    /// <param name="damage">受けたダメージ量。</param>
    public virtual void Damaged(int damage)
    {
        // ダメージ値をポップアップ表示する
        DamagePopup damagePopup = GetComponent<DamagePopup>();
        damagePopup.ShowDamage(damage, transform.position, Color.white);

    }

    /// <summary>
    /// オブジェクトが死亡した際の処理を行います。オブジェクトの破棄などの処理を実装します。
    /// </summary>
    public virtual void Dead()
    {
        Object.Destroy(gameObject);
    }

    /// <summary>
    /// マスが道路の場合の移動処理を行います。移動先のマスにオブジェクトを移動させます。
    /// </summary>
    /// <param name="mass">移動先のマス。</param>
    /// <param name="movedPos">移動先の位置。</param>
    protected virtual void MoveToRoad(Map.Mass mass, Vector2Int movedPos)
    {
        StartCoroutine(MoveCoroutine(movedPos));
    }

    /// <summary>
    /// 移動先のマスが移動不可能な場合の処理を行います。オブジェクトは現在位置に留まります。
    /// </summary>
    /// <param name="mass">移動を試みたマス。</param>
    /// <param name="movedPos">移動を試みた位置。</param>
    protected virtual void MoveToNotMoving(Map.Mass mass, Vector2Int movedPos)
    {
        StartCoroutine(NotMoveCoroutine(movedPos));
    }

    /// <summary>
    /// オブジェクトの移動アニメーションを実行するコルーチンです。指定されたターゲット位置までオブジェクトを滑らかに移動させます。
    /// </summary>
    /// <param name="target">移動先のターゲット位置。</param>
    /// <returns>IEnumeratorを返し、コルーチンの実行を可能にします。</returns>
    private IEnumerator MoveCoroutine(Vector2Int target)
    {
        // マップ上の現在位置のマスの情報を更新
        var startMass = Map[Pos.x, Pos.y];
        startMass.ExistObject = null;

        // 移動先のマップ上の位置を計算
        var movedPos = Map.CalcMapPos(target);

        // 移動前の位置を保存し、新しい位置を設定
        PrevPos = Pos;
        Pos = target;

        // 移動先のマスの情報を更新
        var movedMass = Map[Pos.x, Pos.y];
        movedMass.ExistObject = gameObject;

        // 移動先のマスが可視状態になっているかを設定
        Visible = movedMass.Visible;

        // 実際のモデルの移動処理を開始
        IsNowMoving = true;
        var start = transform.position;
        var timer = 0f;
        while (timer < MoveSecond)
        {
            yield return null;
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(start, movedPos, timer / MoveSecond);
        }
        transform.position = movedPos;

        // 移動処理が完了したことを示す
        IsNowMoving = false;
    }

    /// <summary>
    /// オブジェクトが移動を試みるが移動できない場合のアニメーションを実行するコルーチンです。
    /// 移動先に壁がある場合や、別のオブジェクトが道を塞いでいる場合にこのメソッドが呼び出されます。
    /// オブジェクトはわずかに前に進むように見えますが、最終的には元の位置に戻ります。
    /// </summary>
    /// <param name="target">移動を試みたターゲット位置。</param>
    /// <returns>IEnumeratorを返し、コルーチンの実行を可能にします。</returns>
    protected IEnumerator NotMoveCoroutine(Vector2Int target)
    {
        // 移動先のマップ上の位置を計算
        var movedPos = Map.CalcMapPos(target);

        // 実際のモデルの移動アニメーション処理を開始
        IsNowMoving = true;
        var start = transform.position;
        var timer = 0f;
        movedPos = Vector3.Lerp(start, movedPos, 0.5f); // 移動先と元の位置の中間点を計算

        // オブジェクトがわずかに前に進むアニメーションを実行
        while (timer < MoveSecond)
        {
            yield return null;
            timer += Time.deltaTime;
            var t = 1f - Mathf.Abs(timer / MoveSecond * 2 - 1f); // アニメーションの進行度を計算
            transform.position = Vector3.Lerp(start, movedPos, t); // 現在の位置をアニメーションの進行度に基づいて更新
        }
        transform.position = start; // オブジェクトを元の位置に戻す

        // 移動処理が完了したことを示す
        IsNowMoving = false;
    }

    [SerializeField] Weapon _weapon; // シリアライズされた武器オブジェクト

    /// <summary>
    /// 現在装備している武器。装備する際にはプロパティを通じて設定し、必要な設定処理を行います。
    /// </summary>
    public Weapon CurrentWeapon
    {
        get => _weapon; // 現在装備している武器を取得
        set
        {
            if (_weapon != null)
            {
                // 既に武器を装備している場合は、古い武器を外す
                _weapon.Detach(this);
            }
            _weapon = value; // 新しい武器を設定
            if (_weapon != null)
            {
                // 新しい武器を装備する際の処理を行う
                _weapon.Attach(this);
            }
        }
    }

    /// <summary>
    /// オブジェクトが生成された際に呼び出されます。装備している武器があれば、その設定処理を行います。
    /// </summary>
    private void Awake()
    {
        if (CurrentWeapon != null)
        {
            // 起動時に装備している武器の設定処理を行う
            CurrentWeapon.Attach(this);
        }
    }



}
