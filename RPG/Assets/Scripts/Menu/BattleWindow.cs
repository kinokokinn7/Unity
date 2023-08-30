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

    public override void Open()
    {
        base.Open();
        MainCommands.Index = 0;
        Items.gameObject.SetActive(false);
        Description.transform.parent.gameObject.SetActive(false);
        UpdateUI();
    }

    /// <summary>
    /// メニューウィンドウが変更された時にメニューの項目を変更します。
    /// </summary>
    /// <param name="menuRoot"></param>
    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        base.ChangeMenuItem(menuRoot);

        Enemies.gameObject.SetActive(true);

    }

}
