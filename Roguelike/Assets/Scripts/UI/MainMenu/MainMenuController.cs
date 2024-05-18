using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public UIDocument _document;
    private VisualElement _mainMenu;
    private ListView _listView;
    private int _selectedIndex = 0;

    /// <summary>
    /// リストアイテム。
    /// </summary>
    private string[] _items = new string[] { "アイテム", "設定" };

    void OnEnable()
    {
        var root = _document.rootVisualElement;

        // メインメニューとリストビューの取得
        _mainMenu = root.Q<VisualElement>("MainMenuWindow");
        _listView = root.Q<ListView>("MainMenuList");

        if (_mainMenu != null)
        {
            // 初期状態で非表示
            _mainMenu.style.display = DisplayStyle.None;
        }

        if (_listView != null)
        {
            // リストビューの設定
            _listView.makeItem = () => new Label();
            _listView.bindItem = (element, i) => (element as Label).text = _items[i];
            _listView.itemsSource = _items;
            _listView.selectionType = SelectionType.Single;

            // 選択イベントの処理
            _listView.onSelectionChange += items =>
            {
                foreach (var item in items)
                {
                    Debug.Log($"{item}が選択されました。");
                }
            };
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TogguleMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _mainMenu.style.display == DisplayStyle.Flex)
        {
            HideMenu();
        }
        if (Input.GetKeyDown(KeyCode.Z) && _mainMenu.style.display == DisplayStyle.Flex)
        {
            ExecuteSelection();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelectionUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelectionDown();
        }
    }

    private void TogguleMenu()
    {
        if (_mainMenu.style.display == DisplayStyle.None)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }

    private void ShowMenu()
    {
        _mainMenu.style.display = DisplayStyle.Flex;
        // 最初のアイテムを選択
        _listView.selectedIndex = 0;
        // リストビューにフォーカスを当てる
        _listView.Focus();

        // メニュー表示時はプレイヤーの歩行を不可にする
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.CanMove = false;
    }

    private void HideMenu()
    {
        _mainMenu.style.display = DisplayStyle.None;

        // メニュー非表示後プレイヤーの歩行を可能にする
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.CanMove = true;
    }

    private void ExecuteSelection()
    {
        var selectedItem = _listView.selectedItem;
        if (selectedItem != null)
        {
            Debug.Log($"{selectedItem}が選択されました。");
            // Todo: 選択されたアイテムに対する処理をここに追加
        }
    }

    private void OnItemSelected(IEnumerable<object> selectedItems)
    {
        foreach (var item in selectedItems)
        {
            Debug.Log($"{item}が選択されました。");
        }
    }

    private void MoveSelectionUp()
    {
        if (_listView.itemsSource != null && _listView.itemsSource.Count > 0)
        {
            _selectedIndex = (_selectedIndex - 1 + _listView.itemsSource.Count) % _listView.itemsSource.Count;
            _listView.selectedIndex = _selectedIndex;
        }
    }

    private void MoveSelectionDown()
    {
        if (_listView.itemsSource != null && _listView.itemsSource.Count > 0)
        {
            _selectedIndex = (_selectedIndex + 1) % _listView.itemsSource.Count;
            _listView.selectedIndex = _selectedIndex;
        }
    }


}
