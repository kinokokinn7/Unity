using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TitleManager : MonoBehaviour
{
    // タイトル用のCanvas
    public GameObject titleCanvas;

    // タイトルメニュー
    [SerializeField]
    private TitleMenuController titleMenuController;

    // タイトル後に有効化したいオブジェクトの配列
    public GameObject[] gameObjectsToEnable;

    private Player player;

    public static TitleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartTitle();
    }

    /// <summary>
    /// タイトルを開始するメソッドです。
    /// </summary>
    public bool IsInGame { get; private set; } = false;

    internal void StartTitle()
    {
        titleMenuController.HideMenu();

        FadeController.Instance.OnFadeInComplete += ShowTitleMenu;

        foreach (GameObject obj in gameObjectsToEnable)
        {
            obj.SetActive(false);
        }

        SoundEffectManager.Instance.PlayTitleBGM();

        // タイトル画面中
        IsInGame = false;
    }

    /// <summary>
    /// タイトルマネージャーが破棄されるときの処理です。
    /// </summary>
    private void OnDestroy()
    {
        // イベント登録解除
        if (FadeController.Instance != null)
        {
            FadeController.Instance.OnFadeInComplete -= ShowTitleMenu;
        }
    }

    /// <summary>
    /// タイトルメニューを表示するメソッドです。
    /// </summary>
    private void ShowTitleMenu()
    {
        titleMenuController.ShowMenu();
    }

    /// <summary>
    /// ゲームを開始するメソッドです。
    /// </summary>
    public void StartGame()
    {
        // フェードアウト中は操作を無効化する
        if (FadeController.Instance.IsFading)
        {
            return;
        }

        // タイトル用Canvasを無効化
        titleCanvas.SetActive(false);

        // 他のオブジェクトを有効化
        foreach (GameObject obj in gameObjectsToEnable)
        {
            obj.SetActive(true);
        }

        // ゲーム中
        IsInGame = true;
    }

    /// <summary>
    /// ゲームを終了するメソッドです。
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// タイトルに戻るメソッドです。
    /// </summary>
    public IEnumerator GoToTitle(bool fadeOutBGM = true)
    {
        if (fadeOutBGM)
        {
            // BGMをフェードアウト（1秒でフェードアウトする例）
            SoundEffectManager.Instance.FadeOutBGM(1.0f);
        }

        // フェードアウトを待つ
        yield return FadeController.Instance.FadeOut();

        // タイトル用Canvasを有効化
        titleCanvas.SetActive(true);

        if (fadeOutBGM)
        {
            // タイトルBGMを再生
            SoundEffectManager.Instance.PlayTitleBGM();
        }
        // フェードインを待つ
        yield return FadeController.Instance.FadeIn();

        ShowTitleMenu();

        // タイトル画面中
        IsInGame = false;
    }
}