using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃とアイテム（装備品除く）を盗むを繰り返す敵のAI。
/// </summary>
[CreateAssetMenu(menuName = "Enemy/AI/Steal AI")]
public class EnemyStealAI : EnemyAI
{
    protected override void TurnAI(Enemy enemy, BattleWindow battleWindow, TurnInfo outTurnInfo)
    {
        if (Turn % 2 == 0)
        {
            var player = battleWindow.Player;
            if (player.Items.Count > 0)
            {
                var item = player.Items[0];
                outTurnInfo.Message = $"{enemy.Name}は{item.Name}を盗んだ！";
                outTurnInfo.DoneCommand = () =>
                {
                    player.Items.RemoveAt(0);
                };
            }
            else
            {
                outTurnInfo.Message = $"{enemy.Name}はアイテムを盗もうとしたが\nプレイヤーは何も持っていなかった。";
                outTurnInfo.DoneCommand = () => { };
            }
        }
        else
        {
            outTurnInfo.Message = $"{enemy.Name}の攻撃！";
            outTurnInfo.DoneCommand = () =>
            {
                enemy.Data.AttackTo(battleWindow.Player, out BattleParameterBase.AttackResult result);
                var messageWindow = battleWindow.GetRPGSceneManager.MessageWindow;
                var resultMsg = "";
                if (result.Damage > 0)
                {
                    resultMsg = $"プレイヤーは{result.Damage}のダメージを受けた！";
                }
                else
                {
                    resultMsg = "プレイヤーはダメージを受けなかった！"; ;
                }
                messageWindow.Params = null;
                messageWindow.StartMessage(resultMsg);
            };
        }
    }
}
