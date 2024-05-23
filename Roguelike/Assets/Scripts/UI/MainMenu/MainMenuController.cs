using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour, IMenuController
{
    public UIDocument _document;
    private VisualElement _mainMenu;
    private ListView _listView;
    private int _selectedIndex = 0;
    public bool Focused { get; private set; } = false;

    /// <summary>
    /// リストアイテム。
    /// </summary>
    private readonly string[] _items = new string[] { "アイテム", "設定" };
    private readonly Dictionary<string, ISelectedItem> _selectedItems = new Dictionary<string, ISelectedItem>();

    void Start()
    {
        _selectedItems.Add("アイテム", new MainMenuSelectedItem_GameItem());
        _selectedItems.Add("設定", new MainMenuSelectedItem_Settings());
    }

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
        // メニューウィンドウが表示されていてかつフォーカスされていない場合は何も処理をしない
        if (_mainMenu.style.display == DisplayStyle.Flex && !Focused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _mainMenu.style.display == DisplayStyle.Flex)
        {
            HideMenu();
        }

        if (_mainMenu.style.display == DisplayStyle.None)
        {
            return;
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

    public void ToggleMenu()
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

    public void ShowMenu()
    {
        _mainMenu.style.display = DisplayStyle.Flex;
        // 最初のアイテムを選択
        _listView.selectedIndex = 0;
        // リストビューにフォーカスを当てる
        _listView.Focus();
        Focused = true;

        // メニュー表示時はプレイヤーの歩行を不可にする
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.CanMove = false;
    }

    public void HideMenu()
    {
        _mainMenu.style.display = DisplayStyle.None;
        Focused = false;

        // メニュー非表示後プレイヤーの歩行を可能にする
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.CanMove = true;
    }

    /// <summary>
    /// 選択した項目を実行します。
    /// </summary>
    public void ExecuteSelection()
    {
        var selectedItem = _listView.selectedItem;
        if (selectedItem != null)
        {
            Debug.Log($"{selectedItem}が実行されました。");
            _selectedItems[selectedItem as string].OnItemSelected();
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

    public void Focus()
    {
        // リストビューにフォーカスを当てる
        _listView.Focus();
        Focused = true;
    }

    public void Blur()
    {
        // リストビューにのフォーカスを外す
        _listView.Blur();
        Focused = false;
    }
}
