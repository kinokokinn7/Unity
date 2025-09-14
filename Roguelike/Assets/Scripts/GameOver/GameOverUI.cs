using System.Collections;
using UnityEngine;
using Roguelike.Window;

public class GameOverUI : MonoBehaviour
{
    /// <summary>
    /// パネルが表示中かどうか。
    /// </summary>
    private bool _isActive;

    void Awake()
    {
        _isActive = false;
        gameObject.SetActive(false); // 初期状態では非表示
    }

    private void OnEnable()
    {
        _isActive = true;
        // GameOver 表示時に自動でタイトルへ戻す（短い遅延を挟んで UI が表示されるようにする）
        StartCoroutine(AutoReturnToTitle());
    }

    private void OnDisable()
    {
        _isActive = false;
    }

    private IEnumerator AutoReturnToTitle()
    {
        // UI が一瞬描画されるように短い遅延（任意）。即戻したければ 0 に。
        yield return new WaitForSeconds(2.0f);

        yield return DoGiveUp();
    }

    /// <summary>
    /// ゲームオーバー処理（プレイヤー削除・セーブ破棄・タイトルへ遷移）
    /// </summary>
    private IEnumerator DoGiveUp()
    {
        // 2重実行防止
        if (!_isActive) yield return null;
        _isActive = false;

        // プレイヤーを削除
        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.Destroy();
        }

        // セーブデータを破棄
        SaveData.Destroy();

        // メッセージウィンドウをクリア
        MessageWindow.Instance.Clear();

        // タイトル画面へ戻る（IEnumerator を StartCoroutine で実行する）
        if (TitleManager.Instance == null)
        {
            Debug.LogError("TitleManager.Instance が見つかりません。タイトルへ戻れません。");
        }

        // TitleManager に紐づくコルーチンとして開始すると、GameOverUI が無効化されても処理が継続する
        yield return TitleManager.Instance.GoToTitle(true);

        // ゲームオーバーのパネルを閉じる
        gameObject.SetActive(false);
    }
}
