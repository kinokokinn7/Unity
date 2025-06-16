using System;
using System.Collections;
using UnityEngine;
using Roguelike.Window;

public class FinalGoal : MapObjectBase
{
    /// <summary>
    /// 最終ゴール（クリスタル）を取得したときの処理です。
    /// </summary>
    internal IEnumerator Execute()
    {
        // タイトルBGMを再生
        SoundEffectManager.Instance.PlayTitleBGM();

        // クリア時の効果音を再生
        SoundEffectManager.Instance.PlayLevelUpSound();

        // メッセージウィンドウをクリア
        var messageWindow = MessageWindow.Instance;
        messageWindow.Clear();

        // キャラクターのアニメーションを再生する
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        yield return player.PlayAnimationForFinalGoal();

        // 一定時間待機（5秒）
        yield return new WaitForSeconds(5f);

        // タイトル画面に戻る
        StartCoroutine(TitleManager.Instance.GoToTitle());
    }
}
