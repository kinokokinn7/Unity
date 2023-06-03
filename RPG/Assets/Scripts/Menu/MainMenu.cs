using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField]
    protected RPGSceneManager RPGSceneManager;

    public GameObject ParameterRoot;
    public MenuRoot ItemInventory;

    /// <summary>
    /// アイテム使用時の処理です。
    /// </summary>
    public void UseItem()
    {
        // TODO
        UpdateUI();
    }

    /// <summary>
    /// メニューウィンドウを開きます。
    /// </summary>
    public override void Open()
    {
        base.Open();
        UpdateUI();
    }

    /// <summary>
    /// メニュー画面のUIを更新します。
    /// </summary>
    private void UpdateUI()
    {
        UpdateItems();
        UpdateParameters();
    }

    /// <summary>
    /// メニューに表示されるアイテム項目を更新します。
    /// </summary>
    private void UpdateItems()
    {
        // アイテムは6個までを上限と想定して作成（武器＋防具＋アイテム4個）
        var player = RPGSceneManager.Player;
        var items = player.BattleParameter.Items;
        var menuItems = ItemInventory.MenuItems;

        // 前アイテム項目を非アクティブにする
        foreach (var menuItem in menuItems)
        {
            menuItem.gameObject.SetActive(false);
        }

        int i = 0;
        // 武器は1番目に設定
        if (player.BattleParameter.AttackWeapon != null)
        {
            menuItems[i].gameObject.SetActive(true);
            menuItems[i].Text = player.BattleParameter.AttackWeapon.Name;
            i++;
        }
        // 防具は2番目に設定
        if (player.BattleParameter.DefenseWeapon != null)
        {
            menuItems[i].gameObject.SetActive(true);
            menuItems[i].Text = player.BattleParameter.DefenseWeapon.Name;
            i++;
        }

        // その他のアイテムは3番目以降に設定
        for (var itemIndex = 0; i < menuItems.Length && itemIndex < items.Count; i++, itemIndex++)
        {
            var menuItem = menuItems[i];
            menuItem.gameObject.SetActive(true);
            menuItem.Text = items[itemIndex].Name;
        }
    }

    /// <summary>
    /// メニュー画面に表示するプレイヤーのパラメータを最新状態に更新します。
    /// </summary>
    private void UpdateParameters()
    {
        var player = RPGSceneManager.Player;
        var param = player.BattleParameter;
        SetParameterText("LEVEL", param.Level.ToString());

    }

    /// <summary>
    /// 画面に描画するパラメータ値を設定します。
    /// </summary>
    /// <param name="name">パラメータ名</param>
    /// <param name="text">パラメータ値</param>
    private void SetParameterText(string name, string text)
    {
        var root = ParameterRoot.transform.Find(name);
        var textObj = root.Find("Text").GetComponent<Text>();
        textObj.text = text;
    }
}
