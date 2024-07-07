using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VirtualGamePad : MonoBehaviour
{
    public static VirtualGamePad Instance { get; private set; }

    private VisualElement root;
    private Button upButton, downButton, leftButton, rightButton;
    private Button aButton, bButton, xButton, yButton;
    private Vector2 moveInput;
    private bool upPressed, downPressed, leftPressed, rightPressed;
    private bool aPressed, bPressed, xPressed, yPressed;

    private void Awake()
    {
        // シングルトンインスタンスを設定
        if (Instance == null)
        {
            Instance = this;
            // シーンを切り替えてもこのオブジェクトを保持する
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにインスタンスが存在する場合、このオブジェクトを破棄する
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // 十字キーのボタンを取得
        upButton = root.Q<Button>("Up");
        downButton = root.Q<Button>("Down");
        leftButton = root.Q<Button>("Left");
        rightButton = root.Q<Button>("Right");

        // ABXYボタンを取得
        aButton = root.Q<Button>("A");
        bButton = root.Q<Button>("B");
        xButton = root.Q<Button>("X");
        yButton = root.Q<Button>("Y");

        // コールバックの設定
        upButton.RegisterCallback<PointerDownEvent>(evt => { moveInput.y = 1; upPressed = true; });
        upButton.RegisterCallback<PointerUpEvent>(evt => { moveInput.y = 0; upPressed = false; });
        downButton.RegisterCallback<PointerDownEvent>(evt => { moveInput.y = -1; downPressed = true; });
        downButton.RegisterCallback<PointerUpEvent>(evt => { moveInput.y = 0; downPressed = false; });
        leftButton.RegisterCallback<PointerDownEvent>(evt => { moveInput.x = -1; leftPressed = true; });
        leftButton.RegisterCallback<PointerUpEvent>(evt => { moveInput.x = 0; leftPressed = false; });
        rightButton.RegisterCallback<PointerDownEvent>(evt => { moveInput.x = 1; rightPressed = true; });
        rightButton.RegisterCallback<PointerUpEvent>(evt => { moveInput.x = 0; rightPressed = false; });

        aButton.RegisterCallback<PointerDownEvent>(evt => aPressed = true);
        aButton.RegisterCallback<PointerDownEvent>(evt => aPressed = false);
        bButton.RegisterCallback<PointerDownEvent>(evt => bPressed = true);
        bButton.RegisterCallback<PointerDownEvent>(evt => bPressed = false);
        xButton.RegisterCallback<PointerDownEvent>(evt => xPressed = true);
        xButton.RegisterCallback<PointerDownEvent>(evt => xPressed = false);
        yButton.RegisterCallback<PointerDownEvent>(evt => yPressed = true);
        yButton.RegisterCallback<PointerDownEvent>(evt => yPressed = false);
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool IsAPressed()
    {
        return aPressed;
    }

    public bool IsBPressed()
    {
        return bPressed;
    }

    public bool IsXPressed()
    {
        return xPressed;
    }

    public bool IsYPressed()
    {
        return yPressed;
    }

    public bool IsUpPressed()
    {
        return upPressed;
    }

    public bool IsDownPressed()
    {
        return downPressed;
    }

    public bool IsLeftPressed()
    {
        return leftPressed;
    }

    public bool IsRightPressed()
    {
        return rightPressed;
    }

}
