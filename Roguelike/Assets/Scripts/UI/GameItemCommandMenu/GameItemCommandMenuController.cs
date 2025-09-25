using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class GameItemCommandMenuController : MonoBehaviour, IMenuController
{
    public UIDocument _document;

    private VisualElement _commandMenu;
    private ListView _commandListView;
    private List<string> _commands = new List<string>();
    private int _selectedIndex = 0;
    private Item _targetItem;
    public bool Focused { get; private set; } = false;

    public Action<string, Item> OnCommandSelected; // コマンド選択時のコールバック

    public GameItemMenuController ItemMenuController; // アイテムメニューコントローラ参照をInspector等でセット

    private bool _skipInputThisFrame = false;

    void OnEnable()
    {
        // UI要素の取得
        var root = _document.rootVisualElement;
        _commandMenu = root.Q<VisualElement>("GameItemCommandWindow");
        _commandListView = root.Q<ListView>("GameItemCommandList");

        if (_commandMenu != null)
        {
            _commandMenu.style.display = DisplayStyle.None;
        }

        if (_commandListView != null)
        {
            _commandListView.makeItem = () => new Label();
            _commandListView.bindItem = (element, i) =>
            {
                (element as Label).text = _commands[i];
            };
            _commandListView.selectionType = SelectionType.Single;
        }
    }

    void Update()
    {
        if (_commandMenu == null || _commandMenu.style.display == DisplayStyle.None) return;

        if (_skipInputThisFrame)
        {
            _skipInputThisFrame = false;
            return;
        }

        var current = Keyboard.current;
        if (current == null) return;

        if (current.zKey.wasPressedThisFrame)
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
        if (current.xKey.wasPressedThisFrame || current.escapeKey.wasPressedThisFrame)
        {
            HideMenu();
        }
    }

    /// <summary>
    /// コマンドメニューを表示し、コマンド内容をセットします。
    /// </summary>
    public void ShowMenu(Item item)
    {
        _targetItem = item;
        _commands.Clear();

        if (item is Weapon || item is Armor)
        {
            _commands.Add("そうび");
            _commands.Add("すてる");
        }
        else if (item.Usable)
        {
            _commands.Add("つかう");
            _commands.Add("すてる");
        }
        else
        {
            _commands.Add("すてる");
        }

        _commandListView.itemsSource = _commands;
        _commandListView.RefreshItems();
        _commandMenu.style.display = DisplayStyle.Flex;
        _selectedIndex = 0;
        _commandListView.selectedIndex = 0;
        _commandListView.Focus();
        Focused = true;

        // このフレームは入力をスキップ
        _skipInputThisFrame = true;
    }

    public void HideMenu()
    {
        if (_commandMenu != null)
        {
            _commandMenu.style.display = DisplayStyle.None;
            Focused = false;

            // アイテムメニューウィンドウにフォーカスを戻す
            if (ItemMenuController != null)
            {
                ItemMenuController.Focus();
            }
        }
    }

    public void ExecuteSelection()
    {
        // コマンド選択時のコールバックを呼び出す
        if (_commands.Count == 0 || _selectedIndex < 0 || _selectedIndex >= _commands.Count) return;

        string command = _commands[_selectedIndex];
        HideMenu();

        // コールバックが設定されていれば呼び出す（GameItemMenuController側で実処理）
        OnCommandSelected?.Invoke(command, _targetItem);
    }

    public void MoveSelectionUp()
    {
        if (_commands.Count == 0) return;
        _selectedIndex = (_selectedIndex - 1 + _commands.Count) % _commands.Count;
        _commandListView.selectedIndex = _selectedIndex;
    }

    public void MoveSelectionDown()
    {
        if (_commands.Count == 0) return;
        _selectedIndex = (_selectedIndex + 1) % _commands.Count;
        _commandListView.selectedIndex = _selectedIndex;
    }

    public void ShowMenu()
    {
        if (_commandMenu != null)
        {
            _commandMenu.style.display = DisplayStyle.Flex;
            _selectedIndex = 0;
            if (_commandListView != null && _commands.Count > 0)
            {
                _commandListView.selectedIndex = 0;
                _commandListView.Focus();
            }
            Focused = true;
        }
    }
}
