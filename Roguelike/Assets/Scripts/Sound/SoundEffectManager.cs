using System;
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
    public AudioClip SE_Attack; // 攻撃
    public AudioClip SE_Damaged; // ダメージ
    public AudioClip SE_Recovered; // 回復

    public AudioClip SE_FoodDamaged; // 満腹度ダメージ

    public AudioClip SE_ItemGet; // アイテム入手
    public AudioClip SE_Stair; // 階段

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
    /// 一定時間だけSEを再生します。
    /// </summary>
    /// <param name="clip">再生するSE。</param>
    /// <param name="duration">再生時間。</param>
    /// <param name="pitch">ピッチ。</param>
    public void PlaySound(AudioClip clip, float duration, float pitch)
    {
        StartCoroutine(PlaySoundForDurationCoroutine(clip, duration, pitch));
    }

    private IEnumerator PlaySoundForDurationCoroutine(AudioClip clip, float duration, float pitch)
    {
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
        audioSource.pitch = 1f;
    }

    /// <summary>
    /// [ウィンドウを開く]SEを指定して再生します。
    /// </summary>
    public void PlayOpenWindowSound()
    {
        audioSource?.PlayOneShot(SE_OpenWindow);
    }

    /// <summary>
    /// [アイテムを使用する]SEを指定して再生します。
    /// </summary>
    public void PlayUseItemSound()
    {
        audioSource?.PlayOneShot(SE_UseItem);
    }

    /// <summary>
    /// [攻撃]SEを指定して再生します。
    /// </summary>
    public void PlayAttackSound()
    {

        audioSource?.PlayOneShot(SE_Attack);
    }

    /// <summary>
    /// [ダメージ]SEを指定して再生します。
    /// </summary>
    public void PlayDamagedSound()
    {
        audioSource?.PlayOneShot(SE_Damaged);
    }

    /// <summary>
    /// [回復]SEを指定して再生します。
    /// </summary>
    public void PlayRecoveredSound()
    {
        audioSource?.PlayOneShot(SE_Recovered);
    }

    /// <summary>
    /// [満腹度ダメージ]SEを指定して再生します。
    /// </summary>
    public void PlayFoodDamagedSound()
    {
        audioSource?.PlayOneShot(SE_FoodDamaged);
    }

    /// <summary>
    /// [アイテム入手]SEを指定して再生します。
    /// </summary>
    public void PlayItemGetSound()
    {
        audioSource?.PlayOneShot(SE_ItemGet);
    }

    /// <summary>
    /// [階段]SEを指定して再生します。
    /// </summary>
    public void PlayStairSound()
    {
        if (audioSource == null) return;

        PlaySound(SE_Stair, 0.8f, 3f);
    }
}
