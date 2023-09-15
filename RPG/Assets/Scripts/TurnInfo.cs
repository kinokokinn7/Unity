using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// プレイヤーや敵の行動を行うときに
/// 似たような部分をまとめたクラス。
/// </summary>
public class TurnInfo
{
    public string Message;
    public string[] Params;
    public Animator[] Effects;

    public UnityAction DoneCommand;

    public void ShowMessageWindow(MessageWindow messageWindow)
    {
        messageWindow.Params = Params;
        messageWindow.Effects = Effects;
        messageWindow.StartMessage(Message);
    }
}
