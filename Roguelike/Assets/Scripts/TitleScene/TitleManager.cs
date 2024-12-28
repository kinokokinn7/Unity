using System;
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

    void Start()
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

    private void OnDestroy()
    {
        // イベント登録解除
        if (FadeController.Instance != null)
        {
            FadeController.Instance.OnFadeInComplete -= ShowTitleMenu;
        }
    }

    private void ShowTitleMenu()
    {
        titleMenuController.ShowMenu();
    }

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

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}