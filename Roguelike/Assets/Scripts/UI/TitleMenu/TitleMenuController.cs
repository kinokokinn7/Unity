using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class TitleMenuController : MonoBehaviour, IMenuController
{
    [SerializeField]
    private TitleManager titleManager;
    private MenuControllerCommon _menuControllerCommon;
    public UIDocument _document;
    private VisualElement _titleMenu;
    private ListView _listView;
    private int _selectedIndex = 0;
    public bool Focused { get; private set; } = false;

    /// <summary>
    /// リストアイテム。
    /// </summary>
    private readonly string[] _items = new string[] { "はじめから", "つづきから", "さようなら" };
    private readonly Dictionary<string, ISelectedItem> _selectedItems = new Dictionary<string, ISelectedItem>();

    void Start()
    {
        _menuControllerCommon = UnityEngine.Object.FindAnyObjectByType<MenuControllerCommon>();

        _selectedItems.Add("はじめから", new TitleMenuSelectedItem_Start(this.titleManager));
        _selectedItems.Add("つづきから", new TitleMenuSelectedItem_Continue());
        _selectedItems.Add("さようなら", new TitleMenuSelectedItem_Quit(this.titleManager));

        ShowMenu();
    }

    void OnEnable()
    {
        var root = _document.rootVisualElement;

        // メインメニューとリストビューの取得
        _titleMenu = root.Q<VisualElement>("TitleMenuWindow");
        _listView = root.Q<ListView>("TitleMenuList");

        if (_titleMenu != null)
        {
            // 初期状態で非表示
            _titleMenu.style.display = DisplayStyle.None;
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
            };
        }
    }

    void Update()
    {
        // メニューウィンドウが表示されていてかつフォーカスされていない場合は何も処理をしない
        if (_titleMenu.style.display == DisplayStyle.Flex && !Focused)
        {
            return;
        }

        // 現在のキーボード情報
        var current = Keyboard.current;
        // キーボード接続チェック
        if (current == null)
        {
            Debug.LogWarning("キーボードが接続されていません。");
            return;
        }

        if (_titleMenu.style.display == DisplayStyle.None)
        {
            return;
        }

        if (current.zKey.wasPressedThisFrame && _titleMenu.style.display == DisplayStyle.Flex)
        {
            ExecuteSelection();
        }
        if (current.upArrowKey.wasPressedThisFrame)
        {
            MoveSelectionUp();
        }
        if (current.downArrowKey.wasPressedThisFrame)
        {
            MoveSelectionDown();
        }
    }

    public void ShowMenu()
    {
        _menuControllerCommon?.ShowMenu();

        _titleMenu.style.display = DisplayStyle.Flex;
        // 最初のアイテムを選択
        _listView.selectedIndex = 0;
        // リストビューにフォーカスを当てる
        _listView.Focus();
        Focused = true;
    }

    public void HideMenu()
    {
        _menuControllerCommon?.HideMenu();

        _titleMenu.style.display = DisplayStyle.None;
        Focused = false;
    }

    /// <summary>
    /// 選択した項目を実行します。
    /// </summary>
    public void ExecuteSelection()
    {
        _menuControllerCommon?.ExecuteSelection();

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

    public void MoveSelectionUp()
    {
        _menuControllerCommon?.MoveSelectionUp();

        if (_listView.itemsSource != null && _listView.itemsSource.Count > 0)
        {
            _selectedIndex = (_selectedIndex - 1 + _listView.itemsSource.Count) % _listView.itemsSource.Count;
            _listView.selectedIndex = _selectedIndex;
        }
    }

    public void MoveSelectionDown()
    {
        _menuControllerCommon?.MoveSelectionDown();

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
