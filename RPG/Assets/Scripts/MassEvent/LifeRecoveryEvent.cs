using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/Life Recovery Event")]
public class LifeRecoveryEvent : MassEvent
{
    /// <summary>
    /// メッセージ。
    /// </summary>
    [TextArea(3, 15)] public string Message;

    /// <summary>
    /// 宿屋の金額。
    /// </summary>
    [Min(0)] public int Money = 50;

    /// <summary>
    /// 「はい」を選択して宿屋に泊まった時のメッセージ。
    /// </summary>
    [TextArea(3, 15)] public string RecoveryMessage;

    /// <summary>
    /// 「はい」を選択して金額不足だった場合のメッセージ。
    /// </summary>
    [TextArea(3, 15)] public string NotEnoughMoneyMessage;

    /// <summary>
    /// 「いいえ」を選択した場合のメッセージ。
    /// </summary>
    [TextArea(3, 15)] public string NoMessage;

    /// <summary>
    /// イベント処理内容。
    /// </summary>
    /// <param name="manager"></param>
    public override void Exec(RPGSceneManager manager)
    {
        var messageWindow = manager.MessageWindow;
        var yesNoMenu = messageWindow.YesNoMenu;
        // 「はい」を選択した場合
        yesNoMenu.YesAction = () =>
        {
            var param = manager.Player.BattleParameter;
            if (param.Money - Money >= 0)
            {
                param.HP = param.MaxHP;
                param.Money -= Money;

                messageWindow.Params = new string[]
                {
                    param.HP.ToString(),
                    param.Money.ToString()
                };
                messageWindow.StartMessage(RecoveryMessage);
            }
            else
            {
                messageWindow.Params = new string[]
                {
                    param.Money.ToString()
                };
                messageWindow.StartMessage(NotEnoughMoneyMessage);
            }
        };

        // 「いいえ」を選択した場合
        yesNoMenu.NoAction = () =>
        {
            messageWindow.Params = null;
            messageWindow.StartMessage(NoMessage);
        };

        messageWindow.Params = new string[]
        {
            Money.ToString()
        };
    }
}
