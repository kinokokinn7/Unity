using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputProvider
{
    private Player.Action _nowAction = Player.Action.None;
    public Player.Action NowAction => _nowAction;

    private bool _requestUseItem = false;

    public void PollInput()
    {
        _nowAction = Player.Action.None;

        var current = Keyboard.current;
        if (current == null)
        {
            return;
        }

        // UIからの要求があれば優先して確定
        if (_requestUseItem)
        {
            _requestUseItem = false;
            _nowAction = Player.Action.UseItem;
            return;
        }

        // 現在のキー状態
        bool attack = current.zKey.isPressed;
        bool up = current.upArrowKey.isPressed;
        bool down = current.downArrowKey.isPressed;
        bool right = current.rightArrowKey.isPressed;
        bool left = current.leftArrowKey.isPressed;

        // 連続入力を許可するため、単純に押されているかチェックする
        // 元のPlayerクラスではWaitInput内で毎回prevフラグをリセットしていたため、
        // ターンごとに押されていれば反応していた（押しっぱなしで連続移動できていた）。
        // ここでも同様に、現在のフレームで押されていればアクションを設定する。
        
        if (attack)
        {
            _nowAction = Player.Action.Attack;
        }
        else if (up)
        {
            _nowAction = Player.Action.MoveUp;
        }
        else if (down)
        {
            _nowAction = Player.Action.MoveDown;
        }
        else if (right)
        {
            _nowAction = Player.Action.MoveRight;
        }
        else if (left)
        {
            _nowAction = Player.Action.MoveLeft;
        }
    }

    public void RequestUseItem()
    {
        _requestUseItem = true;
    }

    public void ResetAction()
    {
        _nowAction = Player.Action.None;
    }
}
