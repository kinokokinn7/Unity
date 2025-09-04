using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGSceneManager : MonoBehaviour
{
    public Player Player;
    public Map ActiveMap;
    public Menu Menu;
    public ItemShopMenu ItemShopMenu;
    public Vector3Int MassEventPos { get; private set; }
    [SerializeField] public BattleWindow BattleWindow;
    [SerializeField, TextArea(3, 15)] string GameOverMessage = "体力がなくなった・・・";
    [SerializeField] Map RespawnMapPrefab;
    [SerializeField] Vector3Int RespawnPos;
    [SerializeField, TextArea(3, 15)] string GameClearMessage = "ゲームクリア";
    [SerializeField] GameClear gameClearObj;


    public void OpenMenu()
    {
        Menu.Open();
    }

    /// <summary>
    /// メニュー表示によりシーンがポーズ状態か否かを表すフラグ。
    /// </summary>
    public bool IsPauseScene
    {
        get
        {
            return !MessageWindow.IsEndMessage ||
                Menu.DoOpen ||
                ItemShopMenu.DoOpen ||
                BattleWindow.DoOpen;
        }
    }

    Coroutine _currentCoroutine;
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
                        MassEventPos = movedPos;
                        massData.massEvent.Exec(this);
                    }
                    else if (ActiveMap.RandomEncount != null)
                    {
                        var rnd = new System.Random();
                        var encount = ActiveMap.RandomEncount.Encount(rnd);
                        if (encount != null)
                        {
                            BattleWindow.SetUseEncounter(encount);
                            BattleWindow.Open();
                        }
                    }
                }
                else if (massData.character != null && massData.character.Event != null)
                {
                    MassEventPos = movedPos;
                    massData.character.Event.Exec(this);
                }
            }
            yield return new WaitWhile(() => IsPauseScene);

            if (Player.BattleParameter.HP <= 0)
            {
                StartCoroutine(GameOver());
                yield break;
            }

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

    IEnumerator GameOver()
    {
        MessageWindow.StartMessage(GameOverMessage);
        yield return new WaitUntil(() => MessageWindow.IsEndMessage);

        RespawnMap(true);
    }

    /// <summary>
    /// プレイヤーを復活させて
    /// 指定のマップの指定位置に移動します。
    /// ゲームオーバーの場合は、HPを1に戻し、お金を100Gに戻します。
    /// </summary>
    /// <param name="isGameOver">ゲームオーバーの場合はtrue</param>
    void RespawnMap(bool isGameOver)
    {
        Object.Destroy(ActiveMap.gameObject);
        ActiveMap = Object.Instantiate(RespawnMapPrefab);

        Player.SetPosNoCoroutine(RespawnPos);
        Player.CurrentDir = Direction.Down;
        if (isGameOver)
        {
            Player.BattleParameter.HP = 1;
            Player.BattleParameter.Money = 100;
        }

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(MovePlayer());
    }

    public void GameClear()
    {
        StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(GameClearCoroutine());
    }

    IEnumerator GameClearCoroutine()
    {
        MessageWindow.StartMessage(GameClearMessage);
        yield return new WaitUntil(() => MessageWindow.IsEndMessage);

        gameClearObj.StartMessage(gameClearObj.Message);
        yield return new WaitWhile(() => gameClearObj.DoOpen);

        _currentCoroutine = null;
        RespawnMap(false);
    }
}