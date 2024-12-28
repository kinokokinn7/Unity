using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    public Boolean IsFading { get; private set; } = false;

    /// <summary>
    /// エディタから設定するフェード用のImage。
    /// </summary>
    [SerializeField] private Image fadeImage;

    /// <summary>
    /// フェードにかかる時間。
    /// </summary>
    [SerializeField] private float fadeDuration = 1f;

    // フェード完了時に通知するイベント
    public event Action OnFadeInComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // オプション: シーン遷移しても破棄されないように
        }
    }

    void Start()
    {
        // ゲーム開始時にフェードイン
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// フェードインを行うコルーチンです。
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeIn()
    {
        IsFading = true;

        float elapsedTime = 0f;
        Color color = this.fadeImage.color;

        while (elapsedTime < this.fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - (elapsedTime / this.fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;   // 完全に透明にする
        fadeImage.color = color;

        IsFading = false;

        // フェードイン完了を通知
        OnFadeInComplete?.Invoke();
    }

    public IEnumerator FadeOut()
    {
        IsFading = true;

        float elapsedTime = 0f;
        Color color = this.fadeImage.color;

        while (elapsedTime < this.fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = elapsedTime / this.fadeDuration;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;   // 完全に不透明にする
        fadeImage.color = color;

        IsFading = false;
    }

}
