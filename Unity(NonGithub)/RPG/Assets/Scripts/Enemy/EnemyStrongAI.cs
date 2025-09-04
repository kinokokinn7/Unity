using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃だけしてくる敵のAI。
/// </summary>
[CreateAssetMenu(menuName = "Enemy/AI/Strong AI")]
public class EnemyStrongAI : EnemyAI
{
    protected override void TurnAI(Enemy enemy, BattleWindow battleWindow, TurnInfo outTurnInfo)
    {
        if (Turn % 2 == 0)
        {
            outTurnInfo.Message = $"{enemy.Name}はぼーっとしている。";
            outTurnInfo.DoneCommand = () => { };
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
