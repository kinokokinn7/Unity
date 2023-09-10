using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : Menu
{
    [SerializeField] RPGSceneManager RPGSceneManager;
    [SerializeField] MenuRoot MainCommands;
    [SerializeField] MenuRoot Items;
    [SerializeField] MenuRoot Enemies;
    [SerializeField] MenuItem EnemyPrefab;
    [SerializeField] Text Description;
    [SerializeField] GameObject ParameterRoot;
    [SerializeField] EnemyGroup UseEncounter;
    public EnemyGroup Encounter { get; private set; }

    /// <summary>
    /// 戦闘画面を開きます。
    /// </summary>
    public override void Open()
    {
        base.Open();
        MainCommands.Index = 0;
        Items.gameObject.SetActive(false);
        Description.transform.parent.gameObject.SetActive(false);
        UpdateUI();

        Encounter = UseEncounter.Clone();
        SetupEnemies();
    }

    /// <summary>
    /// 敵の初期設定を行います。
    /// </summary>
    void SetupEnemies()
    {
        foreach (var e in Enemies.MenuItems)
        {
            UnityEngine.Object.Destroy(e.gameObject);
        }
        var newEnemies = new List<MenuItem>();
        foreach (var e in Encounter.Enemies)
        {
            var enemy = UnityEngine.Object.Instantiate(EnemyPrefab, Enemies.transform);
            enemy.Text = e.Name;
            var image = enemy.transform.Find("Image").GetComponent<Image>();
            image.sprite = e.Sprite;

            enemy.CurrentKind = MenuItem.Kind.Event;
            enemy.Callbacks.AddListener(this.Attack);
            newEnemies.Add(enemy);
        }
        Enemies.RefreshMenuItems(newEnemies.ToArray());
    }

    /// <summary>
    /// メニューウィンドウが変更された時にメニューの項目を変更します。
    /// </summary>
    /// <param name="menuRoot"></param>
    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        base.ChangeMenuItem(menuRoot);

        Enemies.gameObject.SetActive(true);
        Items.gameObject.SetActive(CurrentMenuObj == Items);

        var player = RPGSceneManager.Player;
        if (CurrentMenuObj == Items &&
            0 <= CurrentMenuObj.Index &&
            CurrentMenuObj.Index < player.BattleParameter.Items.Count)
        {
            Description.transform.parent.gameObject.SetActive(true);
            var item = player.BattleParameter.Items[CurrentMenuObj.Index];
            Description.text = item.Description;
        }
        else
        {
            Description.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 戦闘画面の各ウィンドウを更新します。
    /// </summary>
    void UpdateUI()
    {
        UpdateParameters();
        UpdateItem(RPGSceneManager.Player.BattleParameter);
    }

    /// <summary>
    /// パラメータウィンドウの各パラメータの値を更新します。
    /// </summary>
    private void UpdateParameters()
    {
        var player = RPGSceneManager.Player;
        var param = player.BattleParameter;
        SetParameterText("HP", $"{param.HP}/{param.MaxHP}");
        SetParameterText("ATK", param.AttackPower.ToString());
        SetParameterText("DEF", param.DefensePower.ToString());
    }

    /// <summary>
    /// パラメータウィンドウのパラメータにテキストを設定します。
    /// 設定対象のパラメータはパラメータ名で指定します。
    /// </summary>
    /// <param name="name">パラメータ名</param>
    /// <param name="text">テキスト</param>
    private void SetParameterText(string name, string text)
    {
        var root = ParameterRoot.transform.Find(name);
        var textObj = root.Find("Text").GetComponent<Text>();
        textObj.text = text;
    }

    private void UpdateItem(BattleParameterBase param)
    {
        var items = param.Items;
        var useItems = new List<MenuItem>();
        var menuItems = Items.GetComponentsInChildren<MenuItem>(true);
        for (int i = 0; i < menuItems.Length; i++)
        {
            var menuItem = menuItems[i];
            if (i < items.Count)
            {
                menuItem.gameObject.SetActive(true);
                menuItem.Text = items[i].Name;
                useItems.Add(menuItem);
            }
            else
            {
                menuItem.gameObject.SetActive(false);
            }
        }
        Items.RefreshMenuItems(useItems.ToArray());
    }

    protected override void Cancel(MenuRoot current)
    {
        if (CurrentMenuObj != MainCommands)
        {
            base.Cancel(current);
        }
    }

    public void Attack()
    {
        var enemyIndex = CurrentMenuObj.Index;
        var enemy = Encounter.Enemies[enemyIndex];

        var turnInfo = new TurnInfo();
        turnInfo.Message = $"{enemy.Name}にこうげき！";
        turnInfo.DoneCommand = () =>
        {
            var player = RPGSceneManager.Player.BattleParameter;
            BattleParameterBase.AttackResult result;
            var doKill = player.AttackTo(enemy.Data, out result);

            var messageWindow = RPGSceneManager.MessageWindow;
            var resultMsg = $"{enemy.Name}に{result.Damage}を与えた！";
            if (doKill)
            {
                resultMsg += $"\n{enemy.Name}を倒した！！";
                Encounter.Enemies.RemoveAt(enemyIndex);
                SetupEnemies();
            }
            messageWindow.Params = null;
            messageWindow.StartMessage(resultMsg);
        };
        StartTurn(turnInfo);

    }

    public void Defense()
    {
        var turnInfo = new TurnInfo();
        turnInfo.Message = "身を守っている！";
        turnInfo.DoneCommand = () =>
        {
            RPGSceneManager.Player.BattleParameter.IsNowDefense = true;
        };
        StartTurn(turnInfo);
    }

    public void UseItem()
    {
        Items.gameObject.SetActive(false);

        var player = RPGSceneManager.Player.BattleParameter;
        var itemIndex = CurrentMenuObj.Index;
        var useItem = player.Items[itemIndex];

        var turnInfo = new TurnInfo();
        turnInfo.Message = $"{useItem.Name}を使った！";
        turnInfo.DoneCommand = () =>
        {
            var messageWindow = RPGSceneManager.MessageWindow;
            if (useItem is Weapon)
            {
                messageWindow.StartMessage("しかしなにも起こらなかった！");
            }
            else
            {
                useItem.Use(player);
                messageWindow.StartMessage($"{useItem.Name}はなくなった...");
                player.Items.RemoveAt(itemIndex);
            }
        };
        StartTurn(turnInfo);
    }

    bool DoEscape { get; set; }
    [Min(0)] public float EscapeWaitSecond = 1f;
    public void Escape()
    {
        TurnInfo turnInfo = new TurnInfo();
        turnInfo.Message = "にげだした！";
        turnInfo.DoneCommand = () =>
        {
            var messageWindow = RPGSceneManager.MessageWindow;
            var rnd = new System.Random();
            DoEscape = (float)rnd.NextDouble() < Encounter.EscapeSuccessRate;

            if (DoEscape)
            {
                messageWindow.StartMessage("うまくにげきれた！");
            }
            else
            {
                messageWindow.StartMessage("しかしまわりこまれてしまった！");
            }
        };
        StartTurn(turnInfo);
    }

    private void StartTurn(TurnInfo turnInfo)
    {
        if (_turnCoroutine != null) return;
        while (CurrentMenuObj != MainCommands)
        {
            Cancel(CurrentMenuObj);
        }
        EnableInput = false;

        _turnCoroutine = StartCoroutine(Turn(turnInfo));
    }

    Coroutine _turnCoroutine;
    IEnumerator Turn(TurnInfo turnInfo)
    {
        MessageWindow messageWindow = RPGSceneManager.MessageWindow;
        turnInfo.ShowMessageWindow(messageWindow);
        yield return new WaitWhile(() => !messageWindow.IsEndMessage);
        if (turnInfo.DoneCommand != null)
        {
            turnInfo.DoneCommand();
        }
        yield return new WaitWhile(() => !messageWindow.IsEndMessage);
        UpdateUI();

        if (DoEscape)
        {
            yield return new WaitForSeconds(EscapeWaitSecond);
            Close();
        }
        else
        {
            foreach (var enemy in Encounter.Enemies)
            {
                var info = enemy.BattleAction(this);
                info.ShowMessageWindow(messageWindow);
                yield return new WaitWhile(() => !messageWindow.IsEndMessage);
                if (info.DoneCommand != null)
                {
                    info.DoneCommand();
                }
                yield return new WaitWhile(() => !messageWindow.IsEndMessage);
                UpdateUI();
            }
        }

        var player = RPGSceneManager.Player.BattleParameter;
        if (player.HP <= 0)
        {
            messageWindow.StartMessage($"負けてしまった...");
            yield return new WaitWhile(() => !messageWindow.IsEndMessage);
            Close();
        }
        else if (Encounter.Enemies.Count <= 0)
        {
            var exp = UseEncounter.Enemies.Sum(_e => _e.Data.Exp);
            var money = UseEncounter.Enemies.Sum(_e => _e.Data.Money);
            var msg = $"戦闘に勝った！"
                + $"Exp+{exp}かくとく！"
                + $"お金+${money}かくとく！";
            messageWindow.StartMessage(msg);
            yield return new WaitWhile(() => !messageWindow.IsEndMessage);
            Close();
        }

        TurnEndProcess();
        _turnCoroutine = null;
    }

    private void TurnEndProcess()
    {
        RPGSceneManager.Player.BattleParameter.IsNowDefense = false;
        EnableInput = true;
    }
}