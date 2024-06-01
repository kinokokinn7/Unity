using System;
using System.Collections;
using System.Collections.Generic;
using Roguelike.Window;
using UnityEngine;
using UnityEngine.UIElements;

public class GameItemMenuController : MonoBehaviour, IMenuController
{
    public UIDocument _document;
    private VisualElement _itemMenu;
    private ListView _listView;
    private int _selectedIndex = 0;
    private MainMenuController _mainMenuController;
    public bool Focused { get; private set; } = false;

    /// <summary>
    /// メニューが初めて表示されたことを示すフラグ。
    /// </summary>
    public ItemInventory ItemInventory;


    private bool _isMenuJustShown = false;

    /// <summary>
    /// リストアイテム。
    /// </summary>
    private List<Item> _items = new List<Item>();

    void Start()
    {
        _mainMenuController = UnityEngine.Object.FindObjectOfType<MainMenuController>();
        ItemInventory = GetComponent<ItemInventory>();
    }

    void OnEnable()
    {
        // 所持アイテム一覧を取得
        _items = ItemInventory.Items;

        // アイテムメニューウィンドウのUI Root を取得
        var root = _document.rootVisualElement;

        // メインメニューとリストビューの取得
        _itemMenu = root.Q<VisualElement>("GameItemMenuWindow");
        _listView = root.Q<ListView>("GameItemMenuList");

        if (_itemMenu != null)
        {
            // 初期状態で非表示
            _itemMenu.style.display = DisplayStyle.None;
        }

        if (_listView != null)
        {
            // リストビューの設定
            _listView.makeItem = () => new Label();
            _listView.bindItem = (element, i) => (element as Label).text = _items[i].Name;
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
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape)) && _itemMenu.style.display == DisplayStyle.Flex)
        {
            HideMenu();
            _mainMenuController.Focus();
        }

        if (_itemMenu.style.display == DisplayStyle.None)
        {
            return;
        }

        if (_isMenuJustShown == true)
        {
            // 初めて表示されたタイミングを過ぎたためフラグをリセット
            _isMenuJustShown = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z) && _itemMenu.style.display == DisplayStyle.Flex)
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

    public void ShowMenu()
    {
        _itemMenu.style.display = DisplayStyle.Flex;
        // 最初のアイテムを選択
        ResetIndex();
        // リストビューにフォーカスを当てる
        _listView.Focus();
        Focused = true;
        // メニューが初めて表示されたことを表すフラグをONにする
        _isMenuJustShown = true;
    }

    public void HideMenu()
    {
        _itemMenu.style.display = DisplayStyle.None;
        Focused = false;

        // メニューが隠されたので初回表示フラグをリセット
        _isMenuJustShown = true;
    }

    public void ExecuteSelection()
    {
        var selectedItem = _listView.selectedItem;
        if (selectedItem != null)
        {
            Debug.Log($"{selectedItem}が選択されました。");
            // Todo: 選択されたアイテムに対する処理をここに追加
            MessageWindow.Instance.AppendMessage($"{selectedItem}が選択されました。");
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

    public void ResetIndex()
    {
        // 最初のアイテムを選択
        _selectedIndex = 0;
        _listView.ClearSelection();
        _listView.selectedIndex = 0;
    }

}
