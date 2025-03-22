using System;
using UnityEngine;

public class FinalGoal : MapObjectBase
{
    /// <summary>
    /// 最終ゴール（クリスタル）を取得したときの処理です。
    /// </summary>
    internal void Execute()
    {
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        player.PlayAnimationForFinalGoal();

        // タイトル画面に戻る
        var titleManager = Resources.FindObjectsOfTypeAll<TitleManager>();
        if (titleManager.Length > 0)
        {
            titleManager[0].StartTitle();
        }
    }
}
