using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public MenuRoot FirstMenuRoot;
    Stack<MenuRoot> _menuRootStack;

    /// <summary>
    /// メニューを開いているか否かを表すフラグ。
    /// true: メニューが開かれている状態
    /// </summary>
    public bool DoOpen { get => gameObject.activeSelf; }

    /// <summary>
    /// メニューウィンドウを開きます。
    /// </summary>
    public virtual void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(UpdateWhenOpen());
    }

    /// <summary>
    /// 現在フォーカスされているメニューウィンドウを閉じます。
    /// </summary>
    public virtual void Close()
    {
        while (_menuRootStack.Count > 0)
        {
            _menuRootStack.Peek().IsActive = false;
            _menuRootStack.Pop();
        }
    }

    /// <summary>
    /// 現在フォーカスされているメニューウィンドウ。
    /// </summary>
    public MenuRoot CurrentMenuObj
    {
        get => _menuRootStack.Peek();
    }

    /// <summary>
    /// 現在フォーカスされているメニュー項目。
    /// </summary>
    public MenuItem CurrentItem
    {
        get
        {
            var top = _menuRootStack.Peek();
            return top.CurrentItem;
        }
    }

    /// <summary>
    /// 初期設定。メニューを閉じた状態にします。
    /// </summary>
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// メニューウィンドウが開かれた時の処理。
    /// 他のメニューを非アクティブにして、
    /// 開いたメニューをアクティブにします。
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateWhenOpen()
    {
        // 他のメニューを非アクティブにして、
        // 開いたメニューをアクティブにする
        var menuRoots = GetComponentsInChildren<MenuRoot>();
        foreach (var root in menuRoots)
        {
            root.IsActive = false;
        }
        _menuRootStack = new Stack<MenuRoot>();
        _menuRootStack.Push(FirstMenuRoot);
        FirstMenuRoot.IsActive = true;

        yield return null;

        // アクティブなメニューウィンドウの入力を受け付ける
        while (_menuRootStack.Count > 0)
        {
            var current = _menuRootStack.Peek();

            // 「↑」または「←」キー押下の場合：カーソル移動（すすむ）
            if (Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow))
            {
                current.Index--;
                ChangeMenuItem(current);
            }
            // 「↓」または「→」キー押下の場合：カーソル移動（もどる）
            else if (Input.GetKeyDown(KeyCode.DownArrow) ||
              Input.GetKeyDown(KeyCode.RightArrow))
            {
                current.Index++;
                ChangeMenuItem(current);
            }
            // スペースキー押下の場合：決定
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Decide(current);
            }
            // Xキー押下の場合：キャンセル
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Cancel(current);
            }
            yield return null;
        }

        // メニューを全て閉じたら処理終了
        gameObject.SetActive(false);
    }

    /// <summary>
    /// メニューウィンドウが変更された時にメニューの項目を変更します。
    /// </summary>
    /// <param name="menuRoot"></param>
    protected virtual void ChangeMenuItem(MenuRoot menuRoot)
    {
    }

    /// <summary>
    /// メニューの項目選択を決定します。
    /// </summary>
    /// <param name="current"></param>
    protected virtual void Decide(MenuRoot current)
    {
        var item = current.CurrentItem;
        switch (item.CurrentKind)
        {
            // 次のメニューに移動する場合
            case MenuItem.Kind.NextMenu:
                if (item.MoveTargetObj.MenuItems.Length > 0)
                {
                    item.MoveTargetObj.IsActive = true;
                    _menuRootStack.Push(item.MoveTargetObj);
                    ChangeMenuItem(_menuRootStack.Peek());
                }
                break;
            // 前のメニューに戻る場合
            case MenuItem.Kind.BackMenu:
                current.IsActive = false;
                _menuRootStack.Pop();
                if (_menuRootStack.Count > 0)
                {
                    ChangeMenuItem(_menuRootStack.Peek());
                }
                break;
            // イベントを発動する場合
            case MenuItem.Kind.Event:
                item.Callbacks.Invoke();
                break;
        }
    }

    /// <summary>
    /// メニューでキャンセルした時の処理。
    /// 1つ前のメニューに戻ります。
    /// </summary>
    /// <param name="current"></param>
    protected virtual void Cancel(MenuRoot current)
    {
        current.IsActive = false;
        _menuRootStack.Pop();
        if (_menuRootStack.Count > 0)
        {
            ChangeMenuItem(_menuRootStack.Peek());
        }
    }
}
