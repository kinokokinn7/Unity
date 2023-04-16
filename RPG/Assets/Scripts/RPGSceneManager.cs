using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGSceneManager : MonoBehaviour
{
    public Player Player;
    public Map ActiveMap;
    public Menu Menu;

    public void OpenMenu()
    {
        Menu.Open();
    }

    public bool IsPauseScene
    {
        get
        {
            return !MessageWindow.IsEndMessage || Menu.DoOpen;
        }
    }

    Coroutine _currentCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _currentCoroutine = StartCoroutine(MovePlayer());
    }


    IEnumerator MovePlayer()
    {
        while (true)
        {
            if (GetArrowInput(out var move))
            {
                var movedPos = Player.Pos + move;
                var massData = ActiveMap.GetMassData(movedPos);
                Player.SetDir(move);
                if (massData.isMovable)
                {
                    Player.Pos = movedPos;
                    yield return new WaitWhile(() => Player.IsMoving);

                    if (massData.massEvent != null)
                    {
                        massData.massEvent.Exec(this);
                    }
                }
                else if (massData.character != null && massData.character.Event != null)
                {
                    massData.character.Event.Exec(this);
                }
            }
            yield return new WaitWhile(() => IsPauseScene);

            // スペースキー押下時
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // メニュー画面を開く
                OpenMenu();
            }
        }
    }

    bool GetArrowInput(out Vector3Int move)
    {
        var doMove = false;
        move = Vector3Int.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move.x -= 1; doMove = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            move.x += 1; doMove = true;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            move.y += 1; doMove = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            move.y -= 1; doMove = true;
        }
        return doMove;
    }

    public MessageWindow MessageWindow;

    /// <summary>
    /// メッセージウィンドウを表示します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    public void ShowMessageWindow(string message)
    {
        MessageWindow.StartMessage(message);
    }
}