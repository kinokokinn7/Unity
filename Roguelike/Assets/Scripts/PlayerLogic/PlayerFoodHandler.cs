using UnityEngine;
using Zenject;
using Roguelike.Window;

public class PlayerFoodHandler
{
    private Food _foodValue;
    private Player _player;
    
    // Dependencies
    private MessageWindow _messageWindow;

    public PlayerFoodHandler(MessageWindow messageWindow)
    {
        _messageWindow = messageWindow;
    }

    public void Initialize(Player player, Food foodValue)
    {
        _player = player;
        _foodValue = foodValue;
    }

    public void UpdateFood(int stepCount, int stepsToReduce)
    {
        if (_player == null || _foodValue == null) return;

        // 満腹度が1減る歩数を満たしていない場合は処理をスキップする
        if (stepCount % stepsToReduce != 0) return;

        _foodValue.CurrentValue--;
        if (_foodValue.CurrentValue <= 0)
        {
            _foodValue.CurrentValue = 0;
            _messageWindow.AppendMessage($"空腹で1ダメージ！");
            _player.Damaged(1);
        }
    }
}
