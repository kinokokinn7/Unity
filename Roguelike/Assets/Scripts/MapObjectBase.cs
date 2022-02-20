using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class MapObjectBase : MonoBehaviour
{
    [Range(0, 100)] public float MoveSecond = 0.1f;
    public int Exp = 0;

    public bool IsNowMoving { get; private set; } = false;
    public Vector2Int Pos;
    public Vector2Int PrevPos { get; protected set; }
    public Direction Forward;   // 向き
    Map _map;
    public Map Map
    {
        get => _map != null ? _map : (_map = Object.FindObjectOfType<Map>());
    }

    public int Hp = 5;
    public int Attack = 2;
    public enum Group
    {
        Player,
        Enemy,
        Other,
    }
    public Group CurrentGroup = Group.Other;


    // 位置と前方向を設定するメソッド
    public void SetPosAndForward(Vector2Int pos, Direction forward)
    {
        // PosとForwardを設定し、Transformに反映させる
        PrevPos = new Vector2Int(-1, -1);
        Pos = pos;
        Forward = forward;

        transform.position = Map.CalcMapPos(Pos);
    }

    // 移動処理
    public virtual void Move(Direction dir)
    {
        IsNowMoving = false;
        var (movedMass, movedPos) = Map.GetMovePos(Pos, dir);
        if (movedMass == null) return;

        var massData = Map[movedMass.Type];
        if (movedMass.ExistObject)
        {
            MoveToExistObjct(movedMass, movedPos);
        }
        else if (massData.IsRoad)
        {
            MoveToRoad(movedMass, movedPos);
        }
        else
        {
            MoveToNotMoving(movedMass, movedPos);
        }


    }

    protected virtual void MoveToExistObjct(Map.Mass mass, Vector2Int movedPos)
    {
        var otherObject = mass.ExistObject.GetComponent<MapObjectBase>();
        if (IsAttackableObject(this, otherObject))
        {
            if (AttackTo(otherObject))
            {
                // 攻撃の結果相手を倒したらそのマスに移動する
                StartCoroutine(MoveCoroutine(movedPos));
                return;
            }
        }

        StartCoroutine(NotMoveCoroutine(movedPos));
    }

    public static bool IsAttackableObject(MapObjectBase self, MapObjectBase other)
    {
        return self.CurrentGroup != other.CurrentGroup
            && (self.CurrentGroup != Group.Other && other.CurrentGroup != Group.Other);
    }

    public virtual bool AttackTo(MapObjectBase other)
    {
        other.Hp -= Attack;
        other.Damaged(Attack);
        if (other.Hp <= 0)
        {
            other.Dead();
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Damaged(int damage)
    {
    }

    public virtual void Dead()
    {
        Object.Destroy(gameObject);
    }

    protected virtual void MoveToRoad(Map.Mass mass, Vector2Int movedPos)
    {
        StartCoroutine(MoveCoroutine(movedPos));
    }

    protected virtual void MoveToNotMoving(Map.Mass mass, Vector2Int movedPos)
    {
        StartCoroutine(NotMoveCoroutine(movedPos));
    }

    private IEnumerator MoveCoroutine(Vector2Int target)
    {
        // マップのマス情報を更新する
        var startMass = Map[Pos.x, Pos.y];
        startMass.ExistObject = null;
        var movedPos = Map.CalcMapPos(target);
        PrevPos = Pos;
        Pos = target;
        var movedMass = Map[Pos.x, Pos.y];
        movedMass.ExistObject = gameObject;

        // モデルの移動処理
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
        IsNowMoving = false;
    }

    private IEnumerator NotMoveCoroutine(Vector2Int target)
    {
        var movedPos = Map.CalcMapPos(target);

        IsNowMoving = true;
        var start = transform.position;
        var timer = 0f;
        movedPos = Vector3.Lerp(start, movedPos, 0.5f);
        while (timer < MoveSecond)
        {
            yield return null;
            timer += Time.deltaTime;
            var t = 1f - Mathf.Abs(timer / MoveSecond * 2 - 1f);
            transform.position = Vector3.Lerp(start, movedPos, t);
        }
        transform.position = start;
        IsNowMoving = false;

    }

    [SerializeField] Weapon _weapon;
    // 装備する際に設定処理が必要なのでプロパティ経由で設定できるようにする
    public Weapon CurrentWeapon
    {
        get => _weapon;
        set
        {
            if (_weapon != null)
            {
                // 古い武器を外す
                _weapon.Detach(this);
            }
            _weapon = value;
            if (_weapon != null)
            {
                // 新しい武器を装備する
                _weapon.Attach(this);
            }
        }
    }

    private void Awake()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Attach(this);
        }
    }



}
