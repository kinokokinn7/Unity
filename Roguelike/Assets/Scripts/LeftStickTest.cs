using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LeftStickTest : MonoBehaviour
{


    private void Update()
    {
        // 現在のゲームパッド情報
        var current = Gamepad.current;

        // ゲームパッド接続チェック
        if (current == null)
        {
            Debug.LogWarning("ゲームパッドが接続されていません。");
            return;
        }

        // 左スティック入力取得
        var leftStickValue = current.leftStick.ReadValue();
        Debug.Log($"移動量：{leftStickValue}");

        // 方向を決定
        string direction = GetDirection(leftStickValue);

        // デバッグ用に方向を出力
        Debug.Log("Direction: " + direction);

        // キーボード入力をシミュレート
        SimulateKeyboardInput(direction);
    }

    string GetDirection(Vector2 input)
    {
        if (input.magnitude < 0.5f)
        {
            return "None"; // スティックが中心付近にある場合は「非入力」
        }

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360; // 角度を0〜360度に変換

        if (angle >= 45 && angle < 135)
        {
            return "Up";
        }
        else if (angle >= 135 && angle < 225)
        {
            return "Left";
        }
        else if (angle >= 225 && angle < 315)
        {
            return "Down";
        }
        else
        {
            return "Right";
        }
    }

    void SimulateKeyboardInput(string direction)
    {
        // すべてのキーをリリースした状態のキーボードステートを作成
        var keyboardState = new KeyboardState();

        // 対応するキーを押下した状態に変更
        switch (direction)
        {
            case "Up":
                keyboardState.Press(Key.UpArrow);
                break;
            case "Down":
                keyboardState.Press(Key.DownArrow);
                break;
            case "Left":
                keyboardState.Press(Key.LeftArrow);
                break;
            case "Right":
                keyboardState.Press(Key.RightArrow);
                break;
        }

        // キーボードステートをキューに追加
        InputSystem.QueueStateEvent(Keyboard.current, keyboardState);

        // イベントをすぐに処理
        InputSystem.Update();
    }
}