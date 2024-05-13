using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum MainMenuItem
{
    Item,
    Settings
}

[CreateAssetMenu]
public class MainMenuData : ScriptableObject
{
    public string _menuItemName;

    private static readonly Dictionary<MainMenuItem, string> menuItemLabel =
        new Dictionary<MainMenuItem, string>
        {
            {MainMenuItem.Item, "アイテム"},
            {MainMenuItem.Settings, "設定"}
        };
}
