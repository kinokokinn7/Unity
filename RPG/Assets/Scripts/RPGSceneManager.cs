using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGSceneManager : MonoBehaviour
{
    public Player Player;
    public Map ActiveMap;

    Coroutine _currentCoroutine;

    void Start()
    {
        _currentCoroutine = StartCoroutine(MovePlayer());
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    /// <returns></returns>
    IEnumerator MovePlayer()
    {
        while (true)
        {
            if (GetArrowInput(out var move))
            {
                var movedPos = Player.Pos + move;
                var massData = ActiveMap.GetMassData(movedPos);

                // プレイヤーの移動方向を設定
                Player.SetDir(move);

                // 移動先のマスが通行可能の場合
                if (massData.isMovable)
                {
                    // プレイヤーの位置を移動先に更新
                    Player.Pos = movedPos;
                    // 移動が完了するまで待機
                    yield return new WaitWhile(() => Player.IsMoving);

                    if (massData.massEvent != null)
                    {
                        massData.massEvent.Exec(this);
                    }
                }
            }
            yield return null;
        }

    }

    /// <summary>
    /// 入力された矢印キーから移動方向を取得
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    private bool GetArrowInput(out Vector3Int move)
    {
        var doMove = false;
        move = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move.x -= 1;
            doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move.x += 1;
            doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            move.y += 1;
            doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            move.y -= 1;
            doMove = true;
        }
        return doMove;

    }
}
