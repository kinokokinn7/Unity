using System;
using System.Collections;
using System.Collections.Generic;
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

    }

    public void Defense()
    {

    }

    public void UseItem()
    {
        UpdateItem(RPGSceneManager.Player.BattleParameter);
    }

    public void Escape()
    {

    }
}
