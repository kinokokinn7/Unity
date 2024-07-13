using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 仮想ゲームパッドを制御するクラス
/// </summary>
public class VirtualGamepad : MonoBehaviour
{
    /// <summary>VIrtualGamepadのシングルトンインスタンス</summary>
    public static VirtualGamepad Instance { get; private set; }

    /// <summary>十字キーの上ボタン</summary>
    public Button upButton;
    /// <summary>十字キーの下ボタン</summary>
    public Button downButton;
    /// <summary>十字キーの左ボタン</summary>
    public Button leftButton;
    /// <summary>十字キーの右ボタン</summary>
    public Button rightButton;

    /// <summary>Aボタン</summary>
    public Button aButton;
    /// <summary>Bボタン</summary>
    public Button bButton;
    /// <summary>Xボタン</summary>
    public Button xButton;
    /// <summary>Yボタン</summary>
    public Button yButton;

    /// <summary>十字キーの入力方向</summary>
    [HideInInspector]
    public Vector2 directionInput;

    /// <summary>Aボタンが押されているかどうか</summary>
    [HideInInspector]
    public bool isPressingA;
    /// <summary>Bボタンが押されているかどうか</summary>
    [HideInInspector]
    public bool isPressingB;
    /// <summary>Xボタンが押されているかどうか</summary>
    [HideInInspector]
    public bool isPressingX;
    /// <summary>Yボタンが押されているかどうか</summary>
    [HideInInspector]
    public bool isPressingY;

    private bool isUpPressed, isDownPressed, isLeftPressed, isRightPressed;

    /// <summary>
    /// シングルトンインスタンスの設定とオブジェクトの永続化を行う
    /// </summary>
    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnUpButtonDown()
    {
        if (!isUpPressed)
        {
            isUpPressed = true;
            directionInput = Vector2.up;
            Debug.Log("OnUpButtonDown");
        }
    }
    public void OnUpButtonUp()
    {
        isUpPressed = false;
        if (directionInput == Vector2.up) directionInput = Vector2.zero;
        Debug.Log("OnUpButtonUp");
    }
    public void OnDownButtonDown()
    {
        directionInput = Vector2.down;
        Debug.Log("OnDownButtonDown");
    }
    public void OnDownButtonUp()
    {
        isDownPressed = false;
        if (directionInput == Vector2.down) directionInput = Vector2.zero;
        Debug.Log("OnDownButtonUp");
    }
    public void OnLeftButtonDown()
    {
        directionInput = Vector2.left;
        Debug.Log("OnLeftButtonDown");
    }
    public void OnLeftButtonUp()
    {
        isLeftPressed = false;
        if (directionInput == Vector2.left) directionInput = Vector2.zero;
        Debug.Log("OnLeftButtonUp");
    }
    public void OnRightButtonDown()
    {
        directionInput = Vector2.right;
        Debug.Log("OnRightButtonDown");
    }
    public void OnRightButtonUp()
    {
        isRightPressed = false;
        if (directionInput == Vector2.right) directionInput = Vector2.zero;
        Debug.Log("OnRightButtonUp");
    }
    public void OnAButtonDown()
    {
        if (!isPressingA)
        {
            isPressingA = true;
            Debug.Log("OnAButtonDown");
        }
    }
    public void OnAButtonUp()
    {
        isPressingA = false;
        Debug.Log("OnAButtonDown");
    }
    public void OnBButtonDown()
    {
        if (!isPressingB)
        {

            isPressingB = true;
            Debug.Log("OnBButtonDown");
        }
    }
    public void OnBButtonUp()
    {
        isPressingB = false;
        Debug.Log("OnBButtonUp");
    }
    public void OnXButtonDown()
    {
        if (!isPressingX)
        {
            isPressingX = true;
            Debug.Log("OnXButtonDown");
        }
    }
    public void OnXButtonUp()
    {
        isPressingX = false;
        Debug.Log("OnXButtonUp");
    }
    public void OnYButtonDown()
    {
        if (!isPressingY)
        {
            isPressingY = true;
            Debug.Log("OnYButtonDown");
        }
    }
    public void OnYButtonUp()
    {
        isPressingY = false;
        Debug.Log("OnYButtonUp");
    }


}