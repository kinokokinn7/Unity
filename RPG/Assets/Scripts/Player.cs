using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(0, 2)] public float MoveSecond = 0.1f;
    [SerializeField] RPGSceneManager RPGSceneManager;

    Coroutine _moveCoroutine;
    [SerializeField] Vector3Int _pos;
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

}
