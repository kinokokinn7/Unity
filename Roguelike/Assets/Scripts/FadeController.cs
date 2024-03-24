using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    /// <summary>
    /// エディタから設定するフェード用のImage。
    /// </summary>
    [SerializeField] private Image fadeImage;

    /// <summary>
    /// フェードにかかる時間。
    /// </summary>
    [SerializeField] private float fadeDuration = 1f;

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
        float elapsedTime = 0f;
        Color color = this.fadeImage.color;

        while (elapsedTime < this.fadeDuration)
        {
            Debug.Log($"[FadeIn] elapsedTime: {elapsedTime}, fadeDuration: {fadeDuration}, color.a: {color.a}");
            elapsedTime += Time.deltaTime;
            color.a = 1f - (elapsedTime / this.fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;   // 完全に透明にする
        fadeImage.color = color;
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = this.fadeImage.color;

        while (elapsedTime < this.fadeDuration)
        {
            Debug.Log($"[FadeOut] elapsedTime: {elapsedTime}, fadeDuration: {fadeDuration}, color.a: {color.a}");
            elapsedTime += Time.deltaTime;
            color.a = elapsedTime / this.fadeDuration;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;   // 完全に不透明にする
        fadeImage.color = color;
    }

}
