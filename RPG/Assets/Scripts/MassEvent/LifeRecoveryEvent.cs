using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/Life Recovery Event")]
public class LifeRecoveryEvent : MassEvent
{
    [TextArea(3, 15)] public string Message;
    [Min(0)] public int Money = 50;

    [TextArea(3, 15)] public string RecoveryMessage;
    [TextArea(3, 15)] public string NotEnoughMoneyMessage;
    [TextArea(3, 15)] public string NoMessage;

    /// <summary>
    /// イベント処理内容です。
    /// </summary>
    /// <param name="manager"></param>
    public override void Exec(RPGSceneManager manager)
    {
        var messageWindow = manager.MessageWindow;
        var yesNoMenu = messageWindow.YesNoMenu;
        yesNoMenu.YesAction = () =>
        {
            var param = manager.Player.BattleParameter;

        }
    }
}
