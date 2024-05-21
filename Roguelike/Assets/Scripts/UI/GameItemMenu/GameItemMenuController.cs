using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameItemMenuController : MonoBehaviour, IMenuController
{
    public UIDocument _document;
    private VisualElement _itemMenu;
    private ListView _listView;
    private int _selectedIndex = 0;
    private MainMenuController _mainMenuController;

    /// <summary>
    /// リストアイテム。
    /// </summary>
    private string[] _items = new string[] { "アイテム1", "アイテム2", "アイテム3", "アイテム4", "アイテム5", "アイテム6" };

    void Start()
    {
        _mainMenuController = UnityEngine.Object.FindObjectOfType<MainMenuController>();
    }

    void OnEnable()
    {
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

        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape)) && _itemMenu.style.display == DisplayStyle.Flex)
        {
            HideMenu();
            _mainMenuController.Focus();
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
        _listView.selectedIndex = 0;
        // リストビューにフォーカスを当てる
        _listView.Focus();
    }

    public void HideMenu()
    {
        _itemMenu.style.display = DisplayStyle.None;
    }

    public void ExecuteSelection()
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
