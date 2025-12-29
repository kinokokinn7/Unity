using UnityEngine;
using Zenject;
using Roguelike.Window;

public class PlayerLevelManager : IPlayerLevelManager
{
    private Player _player;
    private MessageWindow _messageWindow;
    private IPlayerEffectManager _effectManager;

    [Inject]
    public void Construct(MessageWindow messageWindow, IPlayerEffectManager effectManager)
    {
        _messageWindow = messageWindow;
        _effectManager = effectManager;
    }

    public void Initialize(Player player)
    {
        _player = player;
    }

    public void AddExperience(int amount)
    {
        if (_player == null) return;

        // 経験値を加算
        _player.Exp.IncreaseCurrentValue(amount);

        // レベルアップ判定
        if (_player.Exp.GetCurrentValue() >= GetNextRequiredExpValue())
        {
            LevelUp();
        }
    }

    private int GetNextRequiredExpValue()
    {
        // レベルに応じた経験値を返す
        return _player.Level * 10;
    }

    private async void LevelUp()
    {
        _player.Level += 1;
        _player.Hp.IncreaseMaxHp(5);
        _player.Attack.IncreaseCurrentValue(1);
        _player.Defence.IncreaseCurrentValue(1);
        _player.Exp.Reset();

        SoundEffectManager.Instance.PlayLevelUpSound();
        _messageWindow.AppendMessage($"{_player.Name}のレベルが{_player.Level}に上がった！");
        _messageWindow.AppendMessage($"  HP +5  Atk + 1");

        // プレイヤーの移動を一時的に停止
        _player.CanMove = false;

        // 回転アニメーションを開始
        await _effectManager.PlayLevelUpEffect();

        // プレイヤーの移動を再開
        _player.CanMove = true;
    }
}
