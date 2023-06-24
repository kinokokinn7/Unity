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
    /// 説明文。
    /// </summary>
    public Text Description;

    /// <summary>
    /// アイテムを使用します。
    /// </summary>
    public void UseItem()
    {
        var index = CurrentMenuObj.Index;
        var player = RPGSceneManager.Player;
        var item = GetItem(player.BattleParameter, index);

        // アイテムが無いか、またはアイテムが武器防具の場合は使用不可
        if (item == null || item is Weapon) return;

        // アイテムを使用
        item.Use(player.BattleParameter);

        // 使用したアイテムを削除
        // NOTE: 武器は1行目、防具は2行目の想定のため、
        // アイテムは3行目以降と想定して行を数える
        int offset = 0;
        if (player.BattleParameter.AttackWeapon != null) offset++;
        if (player.BattleParameter.DefenseWeapon != null) offset++;
        player.BattleParameter.Items.RemoveAt(index - offset);

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

    Item GetItem(BattleParameterBase param, int index)
    {
        int i = 0;
        if (param.AttackWeapon != null)
        {
            if (index == i) return param.AttackWeapon;
            i++;
        }
        if (param.DefenseWeapon != null)
        {
            if (index == i) return param.DefenseWeapon;
            i++;
        }

        index -= i;
        for (var itemIndex = 0; itemIndex < param.Items.Count; itemIndex++)
        {
            if (index == itemIndex) return param.Items[itemIndex];
        }
        return null;
    }

    /// <summary>
    /// メインメニュー画面に遷移した場合に
    /// メニューの項目を変更します。
    /// </summary>
    /// <param name="menuRoot"></param>
    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        UpdateDescription();
    }

    /// <summary>
    /// アイテム説明欄の描画を更新します。
    /// 
    /// アイテム管理ウィンドウがフォーカス状態の場合は
    /// アイテム説明欄を表示して選択中のアイテムの説明を表示します。
    /// そうでなければアイテム表示欄を非表示にします。
    /// </summary>
    private void UpdateDescription()
    {
        if (CurrentMenuObj == ItemInventory)
        {
            Description.transform.parent.gameObject.SetActive(true);
            Description.text = GetItem(
                    RPGSceneManager.Player.BattleParameter,
                    CurrentMenuObj.Index
                ).Description;
        }
        else
        {
            Description.transform.parent.gameObject.SetActive(false);
        }
    }
}
