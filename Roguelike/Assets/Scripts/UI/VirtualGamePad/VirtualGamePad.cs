using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VirtualGamePad : MonoBehaviour
{
    public static VirtualGamePad Instance { get; private set; }

    public UIDocument uiDocument;

    private VisualElement root;
    private VisualElement DPad;
    private VisualElement ABXY;
    private Button upButton, downButton, leftButton, rightButton;
    private Button aButton, bButton, xButton, yButton;
    private Vector2 moveInput;
    private bool upPressed, downPressed, leftPressed, rightPressed;
    private bool aPressed, bPressed, xPressed, yPressed;

    void OnEnable()
    {
        
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned.");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("rootVisualElement is null.");
            return;
        }

        DPad = root.Q<VisualElement>("DPad");
        ABXY = root.Q<VisualElement>("ABXY");

        if (DPad == null || ABXY == null)
        {
            Debug.LogError("DPad or ABXY VisualElement is null.");
            return;
        }

        upButton = DPad.Q<Button>("Up");
        downButton = DPad.Q<Button>("Down");
        leftButton = DPad.Q<Button>("Left");
        rightButton = DPad.Q<Button>("Right");

        aButton = ABXY.Q<Button>("A");
        bButton = ABXY.Q<Button>("B");
        xButton = ABXY.Q<Button>("X");
        yButton = ABXY.Q<Button>("Y");

        if (upButton == null || downButton == null || leftButton == null || rightButton == null || aButton == null || bButton == null || xButton == null || yButton == null)
        {
            Debug.LogError("One or more buttons are null.");
            return;
        }

        RegisterButtonCallbacks();
    }

    void RegisterButtonCallbacks()
    {
        upButton.RegisterCallback<MouseDownEvent>(evt => { moveInput.y = 1; upPressed = true; Debug.Log("Up Button Pressed"); });
        upButton.RegisterCallback<MouseUpEvent>(evt => { moveInput.y = 0; upPressed = false; Debug.Log("Up Button Released"); });
        downButton.RegisterCallback<MouseDownEvent>(evt => { moveInput.y = -1; downPressed = true; Debug.Log("Down Button Pressed"); });
        downButton.RegisterCallback<MouseUpEvent>(evt => { moveInput.y = 0; downPressed = false; Debug.Log("Down Button Released"); });
        leftButton.RegisterCallback<MouseDownEvent>(evt => { moveInput.x = -1; leftPressed = true; Debug.Log("Left Button Pressed"); });
        leftButton.RegisterCallback<MouseUpEvent>(evt => { moveInput.x = 0; leftPressed = false; Debug.Log("Left Button Released"); });
        rightButton.RegisterCallback<MouseDownEvent>(evt => { moveInput.x = 1; rightPressed = true; Debug.Log("Right Button Pressed"); });
        rightButton.RegisterCallback<MouseUpEvent>(evt => { moveInput.x = 0; rightPressed = false; Debug.Log("Right Button Released"); });

        aButton.RegisterCallback<MouseDownEvent>(evt => { aPressed = true; Debug.Log("A Button Pressed"); });
        aButton.RegisterCallback<MouseUpEvent>(evt => { aPressed = false; Debug.Log("A Button Released"); });
        bButton.RegisterCallback<MouseDownEvent>(evt => { bPressed = true; Debug.Log("B Button Pressed"); });
        bButton.RegisterCallback<MouseUpEvent>(evt => { bPressed = false; Debug.Log("B Button Released"); });
        xButton.RegisterCallback<MouseDownEvent>(evt => { xPressed = true; Debug.Log("X Button Pressed"); });
        xButton.RegisterCallback<MouseUpEvent>(evt => { xPressed = false; Debug.Log("X Button Released"); });
        yButton.RegisterCallback<MouseDownEvent>(evt => { yPressed = true; Debug.Log("Y Button Pressed"); });
        yButton.RegisterCallback<MouseUpEvent>(evt => { yPressed = false; Debug.Log("Y Button Released"); });
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