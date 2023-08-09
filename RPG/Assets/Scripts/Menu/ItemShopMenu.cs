using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopMenu : Menu
{
    [SerializeField] protected RPGSceneManager RPGSceneManager;
    [SerializeField] Text Description;
    public ItemShopEvent ItemShop { get; private set; }

    public void Select()
    {
        var index = CurrentMenuObj.Index;
        if (index < 0 || ItemShop.Items.Count <= index) return;

        var item = ItemShop.Items[index];
        if (item == null) return;

        MessageWindow messageWindow = RPGSceneManager.MessageWindow;
        Player player = RPGSceneManager.Player;

        YesNoMenu yesNoMenu = messageWindow.YesNoMenu;
        if (yesNoMenu.DoOpen) return;

        // 「はい」を選択した場合
        yesNoMenu.YesAction = () =>
        {
            // アイテムをこれ以上持てない場合
            if (player.BattleParameter.IsLimitItemCount && !(item is Weapon))
            {
                messageWindow.Params = null;
                messageWindow.StartMessage(ItemShop.ItemCountOverMessage);
            }
            // アイテムを購入した場合
            else if (player.BattleParameter.Money - item.Money >= 0)
            {
                player.BattleParameter.Money -= item.Money;

                // 武器防具の場合は購入したものを装備する
                if (item is Weapon)
                {
                    var weapon = item as Weapon;
                    switch (weapon.Kind)
                    {
                        case WeaponKind.Attack:
                            player.BattleParameter.AttackWeapon = weapon;
                            break;
                        case WeaponKind.Defense:
                            player.BattleParameter.DefenseWeapon = weapon;
                            break;
                    }
                }
                else
                {
                    player.BattleParameter.Items.Add(item);
                }

                messageWindow.Params = new string[]
                {
                    player.BattleParameter.Money.ToString(),
                    item.Money.ToString()
                };
                messageWindow.StartMessage(ItemShop.BuyMessage);
            }
            // お金が足りない場合
            else
            {
                messageWindow.Params = new string[]
                {
                    player.BattleParameter.Money.ToString()
                };
                messageWindow.StartMessage(ItemShop.NotEnoughMoneyMessage);
            }
            StartCoroutine(WaitInput());
        };

        // 「いいえ」を選択した場合
        yesNoMenu.NoAction = () =>
        {
            EnableInput = true;
            messageWindow.Close();
        };

        EnableInput = false;
        messageWindow.Params = new string[]
        {
            item.Name,
            item.Money.ToString()
        };
        messageWindow.StartMessage(ItemShop.AskBuyMessage);
    }

    /// <summary>
    /// MessageWindowが閉じた時の操作がMenuコンポーネントの操作とかぶってしまうため、
    /// 回避策として用意したコルーチンです。
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitInput()
    {
        EnableInput = false;
        MessageWindow messageWindow = RPGSceneManager.MessageWindow;
        yield return new WaitWhile(() => messageWindow.gameObject.activeSelf);
        yield return null;
        EnableInput = true;
    }

    /// <summary>
    /// アイテムショップ（武器防具屋、道具屋）のメニューを開きます。
    /// </summary>
    /// <param name="itemShop"></param>
    public void Open(ItemShopEvent itemShop)
    {
        base.Open();
        ItemShop = itemShop;

        // メニュー項目（商品）を一覧表示
        MenuItem[] menuItems = FirstMenuRoot.MenuItems;
        for (var i = 0; i < menuItems.Length; i++)
        {
            var menuItem = menuItems[i];
            if (i < itemShop.Items.Count)
            {
                menuItem.gameObject.SetActive(true);
                menuItem.Text = ItemShop.Items[i].Name;
            }
            else
            {
                menuItem.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// メニューウィンドウが変更された時に
    /// 説明欄を更新します。
    /// </summary>
    /// <param name="menuRoot"></param>
    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        base.ChangeMenuItem(menuRoot);
        UpdateDescription();
    }

    /// <summary>
    /// 説明欄を更新します。
    /// </summary>
    private void UpdateDescription()
    {
        Item item = ItemShop.Items[CurrentMenuObj.Index];
        Description.text = item.Description;
    }
}
