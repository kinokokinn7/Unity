using System;
using System.Collections;
using UnityEngine;
using Roguelike.Window;

public class FinalGoal : MapObjectBase
{
    [SerializeField] private ParticleSystem clearEffect;

    // ゴール演出中フラグ
    public static bool IsGoalSequenceRunning { get; private set; } = false;

    /// <summary>
    /// 最終ゴール（クリスタル）を取得したときの処理です。
    /// </summary>
    internal IEnumerator Execute()
    {
        IsGoalSequenceRunning = true;

        // タイトルBGMを再生
        SoundEffectManager.Instance.PlayTitleBGM();

        // クリア時の効果音を再生
        SoundEffectManager.Instance.PlayLevelUpSound();

        // メッセージウィンドウをクリア
        var messageWindow = MessageWindow.Instance;
        messageWindow.Clear();

        // クリアエフェクトを再生
        if (clearEffect != null)
        {
            var effect = Instantiate(clearEffect, transform.position, Quaternion.identity);
            effect.Play();
        }

        // キャラクターのアニメーションを再生する
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        yield return player.PlayAnimationForFinalGoal();

        // 一定時間待機（5秒）
        yield return new WaitForSeconds(5f);

        // タイトル画面に戻る
        yield return TitleManager.Instance.GoToTitle(false);

        IsGoalSequenceRunning = false;
    }
}
