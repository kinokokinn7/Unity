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
    internal void StartTitle()
    {
        titleMenuController.HideMenu();

        // フェードイン完了時にタイトルメニューを表示
        FadeController.Instance.OnFadeInComplete += ShowTitleMenu;

        // 他のオブジェクトは無効化
        foreach (GameObject obj in gameObjectsToEnable)
        {
            obj.SetActive(false);
        }

        // タイトルBGMを再生
        SoundEffectManager.Instance.PlayTitleBGM();
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
    public IEnumerator GoToTitle()
    {
        // フェードアウトを待つ
        yield return FadeController.Instance.FadeOut();

        // タイトル用Canvasを有効化
        titleCanvas.SetActive(true);

        // フェードインを待つ
        yield return FadeController.Instance.FadeIn();

        ShowTitleMenu();
    }
}