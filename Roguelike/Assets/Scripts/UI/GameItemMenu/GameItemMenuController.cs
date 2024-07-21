using System;
using System.Collections;
using System.Collections.Generic;
using Roguelike.Window;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class GameItemMenuController : MonoBehaviour, IMenuController
{
    private MenuControllerCommon _menuControllerCommon;

    /// <summary>
    /// UIドキュメント。
    /// </summary>
    public UIDocument _document;

    /// <summary>
    /// アイテムメニューのVisualElement。
    /// </summary>
    private VisualElement _itemMenu;

    /// <summary>
    /// アイテムのリストビュー。
    /// </summary>
    private ListView _listView;

    /// <summary>
    /// アイテムメニューの選択中の行番号。
    /// </summary>
    private int _selectedIndex = 0;

    /// <summary>
    /// 現在のページ番号/最大ページ番号。
    /// </summary>
    private Label _pageNumber;

    /// <summary>
    /// メインメニューのコントローラ。
    /// </summary>
    private MainMenuController _mainMenuController;

    /// <summary>
    /// アイテムメニューウィンドウがフォーカスされているかを表すフラグ。
    /// </summary>
    public bool Focused { get; private set; } = false;

    /// <summary>
    /// アイテム管理クラス。
    /// </summary>
    public ItemInventory _itemInventory;


    /// <summary>
    /// アイテムメニューウィンドウが初めて表示された時のみTrueになるフラグ。
    /// </summary>
    private bool _isMenuJustShown = false;

    /// <summary>
    /// リスト項目。
    /// </summary>
    private List<Item> _itemList = new List<Item>();

    /// <summary>
    /// 現在のページ数。1始まりの値。
    /// </summary>
    private int _currentPage = 1;

    /// <summary>
    /// 1ページあたりのアイテム表示数。
    /// </summary>
    private int _itemsPerPage = 6;

    /// <summary>
    /// 現在表示できる最大ページ数。
    /// アイテム所持数 / 1ページあたりのアイテム表示数 で算出される。
    /// </summary>
    private int _currentMaxPage = 1;

    /// <summary>
    /// クラスインスタンス変数の初期化を行います。
    /// </summary>
    void Start()
    {
        _menuControllerCommon = UnityEngine.Object.FindObjectOfType<MenuControllerCommon>();
        _mainMenuController = UnityEngine.Object.FindObjectOfType<MainMenuController>();
        _itemInventory = UnityEngine.Object.FindObjectOfType<ItemInventory>();
    }

    /// <summary>
    /// アイテムメニューウィンドウの初期設定を行います。
    /// </summary>
    void OnEnable()
    {
        // 所持アイテム数が0の場合は処理終了
        if (_itemInventory.Items.Count == 0) return;

        // 所持アイテム一覧を取得
        int startIndex = (_currentPage - 1) * _itemsPerPage;
        int endIndex = Mathf.Min(startIndex + _itemsPerPage, _itemInventory.Items.Count);
        _itemList.AddRange(_itemInventory.Items.GetRange(startIndex, endIndex));

        // 現在表示できる最大ページ数を設定
        _currentMaxPage = ((_itemInventory.Items.Count - 1) / _itemsPerPage) + 1;

        // アイテムメニューウィンドウのUI Root を取得
        var root = _document.rootVisualElement;

        // メインメニューとリストビューの取得
        _itemMenu = root.Q<VisualElement>("GameItemMenuWindow");
        _listView = root.Q<ListView>("GameItemMenuList");
        _pageNumber = root.Q<Label>("PageNumber");

        if (_itemMenu != null)
        {
            // 初期状態で非表示
            _itemMenu.style.display = DisplayStyle.None;
        }

        if (_listView != null)
        {
            // リストビューの設定
            _listView.makeItem = () => new Label();
            _listView.bindItem = (element, i) =>
            {
                var item = _itemList[i];
                element.Clear();
                element.AddToClassList("item-container");

                if (item is Weapon)
                {
                    var weapon = item as Weapon;
                    if (weapon.IsEquipped)
                    {
                        var equippedLabel = new Label("E");
                        equippedLabel.AddToClassList("equipped-indicator");
                        element.Add(equippedLabel);
                    }

                    var itemNameLabel = new Label(item.Name);
                    itemNameLabel.AddToClassList("item-name");
                    element.Add(itemNameLabel);
                }
                else
                {
                    var itemNameLabel = new Label(item.Name);
                    itemNameLabel.AddToClassList("item-name");
                    element.Add(itemNameLabel);
                }
            };
            _listView.itemsSource = _itemList;
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

        if (_pageNumber != null)
        {
            _pageNumber.text = $"{_currentPage}/{_currentMaxPage}";
        }
    }

    /// <summary>
    ///  キー入力に応じてウィンドウ操作を行います。
    ///  
    ///  Xキー: メニューを表示する
    ///  Xキー or Escapeキー: メニューを閉じる
    ///  Zキー: 決定
    ///  上矢印キー: カーソルを上に移動
    ///  下矢印キー: カーソルを下に移動
    /// </summary>
    void Update()
    {
        // 現在のキーボード情報
        var current = Keyboard.current;
        // キーボード接続チェック
        if (current == null)
        {
            Debug.LogWarning("キーボードが接続されていません。");
            return;
        }

        if (current.xKey.wasPressedThisFrame
            && _itemMenu.style.display == DisplayStyle.Flex)
        {
            HideMenu();
            _mainMenuController.Focus();
        }

        if (_itemMenu.style.display == DisplayStyle.None)
        {
            return;
        }

        // NOTE: ウィンドウを開いた時に自動的に先頭行の項目の選択が決定されるのを防ぐため、
        //        _isMenuJustShownフラグがtrueの場合はフラグをfalseにして以降の処理を中断する
        if (_isMenuJustShown == true)
        {
            // 初めて表示されたタイミングを過ぎたためフラグをリセット
            _isMenuJustShown = false;
            return;
        }

        if (current.zKey.wasPressedThisFrame && _itemMenu.style.display == DisplayStyle.Flex)
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
        if (current.rightArrowKey.wasPressedThisFrame)
        {
            ShowNextPage();
        }
        if (current.leftArrowKey.wasPressedThisFrame)
        {
            ShowPreviousPage();
        }
    }

    /// <summary>
    /// メニューを表示します。
    /// </summary>
    public void ShowMenu()
    {
        _menuControllerCommon?.ShowMenu();

        _itemMenu.style.display = DisplayStyle.Flex;
        _currentPage = 1;
        UpdateItemList();
        UpdatePageNumber();

        // 最初のアイテムを選択
        ResetIndex();

        // リストビューにフォーカスを当てる
        _listView.Focus();
        Focused = true;

        // メニューが初めて表示されたことを表すフラグをONにする
        _isMenuJustShown = true;
    }

    /// <summary>
    /// メニューを非表示にします。
    /// </summary>
    public void HideMenu()
    {
        _menuControllerCommon?.HideMenu();

        _itemMenu.style.display = DisplayStyle.None;
        Focused = false;

        // メニューが隠されたので初回表示フラグをリセット
        _isMenuJustShown = true;
    }

    /// <summary>
    /// 全てのメニューウィンドウを非表示にします。
    /// </summary>
    public void HideAllMenu()
    {
        HideMenu();
        _mainMenuController.HideMenu();
    }

    /// <summary>
    /// 選択されたアイテムを使用します。
    /// 選択されたアイテムが使用不可の場合は何もせず処理を終了します。
    /// </summary>
    public void ExecuteSelection()
    {
        _menuControllerCommon?.ExecuteSelection();

        // 効果音を鳴らす
        SoundEffectManager.Instance.PlayUseItemSound();

        // 選択されたアイテムを使用する
        var selectedItem = _listView.selectedItem as Item;
        if (selectedItem != null)
        {
            Debug.Log($"{selectedItem}が選択されました。");

            // アイテムが使用不可の場合は処理終了
            if (!selectedItem.Usable)
            {
                return;
            }
            var player = UnityEngine.Object.FindObjectOfType<Player>();
            selectedItem.Use(player);

            // アイテムが消耗品の場合はアイテムを一覧から削除する
            if (selectedItem.Consumable)
            {
                // アイテム使用後、一覧から除去する
                _itemList.Remove(selectedItem);
                _listView.itemsSource = _itemList;
                _listView.RefreshItems();

                // アイテム管理クラスのリストから削除する
                _itemInventory.RemoveItem(selectedItem);
            }
        }

        // 全てのメニューウィンドウを閉じる
        HideAllMenu();
    }

    /// <summary>
    /// アイテム選択時に行われる処理です。
    /// デバッグログを出力します。
    /// </summary>
    /// <param name="selectedItems"></param>
    private void OnItemSelected(IEnumerable<object> selectedItems)
    {
        foreach (var item in selectedItems)
        {
            Debug.Log($"{item}が選択されました。");
        }
    }

    /// <summary>
    /// カーソルを1つ上に移動します。
    /// 一番上に位置する場合は、一番下に移動します。
    /// </summary>
    public void MoveSelectionUp()
    {
        _menuControllerCommon?.MoveSelectionUp();

        if (_listView.itemsSource != null && _listView.itemsSource.Count > 0)
        {
            _selectedIndex = (_selectedIndex - 1 + _listView.itemsSource.Count) % _listView.itemsSource.Count;
            _listView.selectedIndex = _selectedIndex;
        }
    }

    /// <summary>
    /// カーソルを1つ下に移動します。
    /// 一番下に位置する場合は、一番上に移動します。
    /// </summary>
    public void MoveSelectionDown()
    {
        _menuControllerCommon?.MoveSelectionDown();

        if (_listView.itemsSource != null && _listView.itemsSource.Count > 0)
        {
            _selectedIndex = (_selectedIndex + 1) % _listView.itemsSource.Count;
            _listView.selectedIndex = _selectedIndex;
        }
    }

    /// <summary>
    /// アイテムメニューウィンドウにフォーカスを当てます。
    /// </summary>
    public void Focus()
    {
        _listView.Focus();
        Focused = true;
    }


    /// <summary>
    /// アイテムメニューウィンドウのフォーカスを解除します。
    /// </summary>
    public void Blur()
    {
        _listView.Blur();
        Focused = false;
    }

    /// <summary>
    /// カーソル位置を1番目の位置に戻します。
    /// </summary>
    public void ResetIndex()
    {
        // 最初のアイテムを選択
        _selectedIndex = 0;
        _listView.ClearSelection();
        _listView.selectedIndex = 0;
    }

    /// <summary>
    /// 前のページを表示します。
    /// </summary>
    void ShowPreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            UpdateItemList();
            UpdatePageNumber();
            _selectedIndex = 0;
            _listView.selectedIndex = _selectedIndex;
        }
    }

    /// <summary>
    /// 次のページを表示します。
    /// </summary>
    void ShowNextPage()
    {
        if (_currentPage * _itemsPerPage < _itemInventory.Items.Count)
        {
            _currentPage++;
            UpdateItemList();
            UpdatePageNumber();
            _selectedIndex = 0;
            _listView.selectedIndex = _selectedIndex;
        }
    }

    /// <summary>
    /// アイテムリストを更新します。
    /// </summary>
    void UpdateItemList()
    {
        _itemList.Clear();

        int startIndex = (_currentPage - 1) * _itemsPerPage;
        int endIndex = Mathf.Min(startIndex + _itemsPerPage, _itemInventory.Items.Count);
        _itemList.AddRange(_itemInventory.Items.GetRange(startIndex, endIndex - startIndex));
        _listView.itemsSource = _itemList;
        _listView.RefreshItems();
    }

    /// <summary>
    /// ページ番号の表示を更新します。
    /// </summary>
    void UpdatePageNumber()
    {
        _currentMaxPage = (_itemInventory.Items.Count - 1) / _itemsPerPage + 1;
        _pageNumber.text = $"{_currentPage}/{_currentMaxPage}";
    }


}
