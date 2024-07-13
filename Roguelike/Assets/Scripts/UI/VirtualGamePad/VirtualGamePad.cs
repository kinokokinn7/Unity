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
        directionInput = Vector2.up;
        Debug.Log("OnUpButtonDown");
    }
    public void OnDownButtonDown()
    {
        directionInput = Vector2.down;
        Debug.Log("OnDownButtonDown");
    }
    public void OnLeftButtonDown()
    {
        directionInput = Vector2.left;
        Debug.Log("OnLeftButtonDown");
    }
    public void OnRightButtonDown()
    {
        directionInput = Vector2.right;
        Debug.Log("OnRightButtonDown");
    }
    public void OnDirectionButtonUp()
    {
        directionInput = Vector2.zero;
        Debug.Log("OnDirectionButtonUp");
    }

    public void OnAButtonDown()
    {
        isPressingA = true;
        Debug.Log("OnAButtonDown");
    }
    public void OnAButtonUp()
    {
        isPressingA = false;
        Debug.Log("OnAButtonDown");
    }
    public void OnBButtonDown()
    {
        isPressingB = true;
        Debug.Log("OnBButtonDown");
    }
    public void OnBButtonUp()
    {
        isPressingB = false;
        Debug.Log("OnBButtonUp");
    }
    public void OnXButtonDown()
    {
        isPressingX = true;
        Debug.Log("OnXButtonDown");
    }
    public void OnXButtonUp()
    {
        isPressingX = false;
        Debug.Log("OnXButtonUp");
    }
    public void OnYButtonDown()
    {
        isPressingY = true;
        Debug.Log("OnYButtonDown");
    }
    public void OnYButtonUp()
    {
        isPressingY = false;
        Debug.Log("OnYButtonUp");
    }


}