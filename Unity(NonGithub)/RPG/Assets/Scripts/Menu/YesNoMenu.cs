using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YesNoMenu : Menu
{
    UnityAction _yesAction;
    UnityAction _noAction;

    /// <summary>
    /// 「はい」を選択した時に実行されるアクション。
    /// </summary>
    public UnityAction YesAction
    {
        private get => _yesAction;
        set => _yesAction = value;
    }

    /// <summary>
    /// 「いいえ」を選択した時に実行されるアクション。
    /// </summary>
    public UnityAction NoAction
    {
        private get => _noAction;
        set => _noAction = value;
    }

    /// <summary>
    /// 「はい」を選択した時の処理。
    /// アクションを実行後、ウィンドウを閉じます。
    /// </summary>
    public void Yes()
    {
        YesAction?.Invoke();
        YesAction = null;
        Close();
    }

    /// <summary>
    /// 「いいえ」を選択した時の処理。
    /// アクションを実行後、ウィンドウを閉じます。
    /// </summary>
    public void No()
    {
        NoAction?.Invoke();
        NoAction = null;
        Close();
    }

    /// <summary>
    /// キャンセルした時の処理。
    /// </summary>
    /// <param name="current"></param>
    protected override void Cancel(MenuRoot current)
    {
    }
}
