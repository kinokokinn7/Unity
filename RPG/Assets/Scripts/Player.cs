using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動方向
/// </summary>
public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public class Player : MonoBehaviour
{
    [Range(0, 2)] public float MoveSecond = 0.1f;
    [SerializeField] RPGSceneManager RPGSceneManager;

    Coroutine _moveCoroutine;
    [SerializeField] Vector3Int _pos;

    /// <summary>
    /// プレイヤー位置
    /// </summary>
    public Vector3Int Pos
    {
        get => _pos;
        set
        {
            if (_pos == value) return;

            if (RPGSceneManager.ActiveMap == null)
            {
                _pos = value;
            }
            else
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                    _moveCoroutine = null;
                }
                _moveCoroutine = StartCoroutine(MoveCoroutine(value));
            }
        }
    }

    /// <summary>
    /// プレイヤー位置をコルーチンなしで設定します。
    /// </summary>
    /// <param name="pos">設定位置</param>
    public void SetPosNoCoroutine(Vector3Int pos)
    {
        _pos = pos;
        transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);
        Camera.main.transform.position = transform.position + Vector3.forward * -10;
    }

    /// <summary>
    /// 移動中か否かを返します。
    /// </summary>
    public bool IsMoving { get => _moveCoroutine != null; }


    /// <summary>
    /// 移動処理のコルーチン
    /// </summary>
    /// <param name="pos">移動先の位置</param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(Vector3Int pos)
    {
        var startPos = transform.position;
        var goalPos = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);
        var t = 0f;
        while (t < MoveSecond)
        {
            yield return null;
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, goalPos, t / MoveSecond);
            Camera.main.transform.position = transform.position + Vector3.forward * -10;
        }
        _pos = pos;
        _moveCoroutine = null;
    }

    private void Start()
    {
        {
            if (RPGSceneManager == null)
                RPGSceneManager = Object.FindObjectOfType<RPGSceneManager>();

            _moveCoroutine = StartCoroutine(MoveCoroutine(Pos));

        }
    }

    /// <summary>
    /// この関数は、エディタからインスペクターの値を変更した際に呼ばれる。
    /// プレイヤーの位置をPosで表すマップ上の位置に自動的に設定する。
    /// </summary>
    private void OnValidate()
    {
        if (RPGSceneManager != null && RPGSceneManager.ActiveMap != null)
        {
            transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(Pos);
        }
    }

    /// <summary>
    /// 移動方向
    /// </summary>
    [SerializeField] Direction _currentDir = Direction.Down;

    public Direction CurrentDir
    {
        get => _currentDir;
        set
        {
            if (_currentDir == value) return;
            _currentDir = value;
            SetDirAnimation(value);
        }
    }

    /// <summary>
    /// 移動方向を設定
    /// </summary>
    /// <param name="move">移動ベクトル</param>
    public void SetDir(Vector3Int move)
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            CurrentDir = move.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            CurrentDir = move.y > 0 ? Direction.Up : Direction.Down;
        }
    }

    Animator Animator { get => GetComponent<Animator>(); }

    /// <summary>
    /// AnimatorControllerのトリガ名
    /// </summary>
    static readonly string TRIGGER_MoveDown = "MoveDownTrigger";
    static readonly string TRIGGER_MoveLeft = "MoveLeftTrigger";
    static readonly string TRIGGER_MoveRight = "MoveRightTrigger";
    static readonly string TRIGGER_MoveUp = "MoveUpTrigger";

    /// <summary>
    /// 移動方向に応じたアニメーショントリガーをセットする
    /// </summary>
    /// <param name="dir">移動方向</param>
    void SetDirAnimation(Direction dir)
    {
        if (Animator == null || Animator.runtimeAnimatorController == null)
            return;

        string triggerName = null;
        switch (dir)
        {
            case Direction.Up: triggerName = TRIGGER_MoveUp; break;
            case Direction.Down: triggerName = TRIGGER_MoveDown; break;
            case Direction.Left: triggerName = TRIGGER_MoveLeft; break;
            case Direction.Right: triggerName = TRIGGER_MoveRight; break;
            default: throw new System.NotImplementedException("");
        }
        Animator.SetTrigger(triggerName);
    }

    /// <summary>
    /// ゲーム起動時に自動実行
    /// </summary>
    private void Awake()
    {
        SetDirAnimation(_currentDir);
    }


}
