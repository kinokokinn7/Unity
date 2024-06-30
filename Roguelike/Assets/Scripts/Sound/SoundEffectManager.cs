using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス。
    /// </summary>
    public static SoundEffectManager Instance { get; private set; }

    public AudioSource audioSource;

    /// <summary>
    /// 効果音のリスト。
    /// </summary>
    public AudioClip[] soundEffects;

    /// <summary>
    /// 特定の効果音。
    /// </summary>
    public AudioClip SE_OpenWindow; // ウィンドウを開く
    public AudioClip SE_UseItem; // アイテムを使用する


    void Awake()
    {
        // シングルトンのインスタンスを設定
        if (Instance == null)
        {
            Instance = this;
            // シーンを跨いでインスタンスを保持する
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにインスタンスが存在する場合は、重複するインスタンスを破棄する
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffect(int index)
    {
        if (index < 0 || index >= soundEffects.Length)
        {
            Debug.LogWarning("Sound effect index out of range.");
            return;
        }

        audioSource.PlayOneShot(soundEffects[index]);
    }

    /// <summary>
    /// [ウィンドウを開く]SEを指定して再生します。
    /// </summary>
    public void PlayOpenWindowSound()
    {
        if (SE_OpenWindow == null)
        {
            Debug.LogWarning("This SoundClip is not assigned.");
            return;
        }
        audioSource.PlayOneShot(SE_OpenWindow);
    }

    /// <summary>
    /// [アイテムを使用する]SEを指定して再生します。
    /// </summary>
    public void PlayUseItemSound()
    {
        if (SE_UseItem == null)
        {
            Debug.LogWarning("This SoundClip is not assigned.");
            return;
        }
        audioSource.PlayOneShot(SE_UseItem);
    }

}
