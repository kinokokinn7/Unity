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
    public AudioClip SE_FoodRecovered; // 満腹度回復

    public AudioClip SE_ItemGet; // アイテム入手
    public AudioClip SE_Stair; // 階段
    public AudioClip SE_LevelUp; // レベルアップ
    public AudioClip SE_AttachWeapon; // 武器装備

    public AudioClip BGM_Title; // タイトル画面のBGM
    private AudioClip BGM_Dungeon;   // ダンジョンのBGM

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
    /// [満腹度回復]SEを指定して再生します。
    /// </summary>
    public void PlayFoodRecoveredSound()
    {
        audioSource?.PlayOneShot(SE_FoodRecovered);
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

    /// <summary>
    /// [レベルアップ]SEを指定して再生します。
    /// </summary>
    public void PlayLevelUpSound()
    {
        if (audioSource == null) return;

        PlaySound(SE_LevelUp, 0.5f, 3f);
    }

    /// <summary>
    /// [武器装備]SEを指定して再生します。
    /// </summary>
    public void PlayAttachWeaponSound()
    {
        audioSource?.PlayOneShot(SE_AttachWeapon);
    }

    /// <summary>
    /// [タイトル画面]BGMを指定して再生します。
    /// </summary>
    public void PlayTitleBGM()
    {
        if (audioSource == null) return;

        audioSource.clip = BGM_Title;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetBGM_Dungeon(AudioClip bgm)
    {
        this.BGM_Dungeon = bgm;
    }

    /// <summary>
    /// [ダンジョン]BGMを指定して再生します。
    /// </summary>
    public void PlayDungeonBGM()
    {
        if (audioSource == null) return;

        audioSource.clip = BGM_Dungeon;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// BGMをフェードアウトします。
    /// </summary>
    /// <param name="fadeDuration">フェードアウトにかかる時間。</param>
    public void FadeOutBGM(float fadeDuration)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    /// <summary>
    /// BGMのフェードアウトを行うコルーチン。
    /// </summary>
    /// <param name="fadeDuration"></param>
    /// <returns></returns>
    private IEnumerator FadeOutCoroutine(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        // 指定された時間の間、音量を徐々に減少させる
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        // 音量が0になったら停止
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
