using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内のメッセージウィンドウを管理するクラスです。新しいメッセージの追加や、メッセージの表示数を制限する機能を提供します。
/// </summary>
public class MessageWindow : MonoBehaviour
{
    private static MessageWindow instance;  // シングルトンインスタンス

    public Text MessagePrefab; // メッセージを表示するためのプレハブ
    [Range(1, 15)]
    public int MessageLimit = 5; // 一度に表示可能なメッセージの最大数
    [Range(1, 10)]
    public int WindowDisplayTime = 5;   // メッセージが追加されてからウィンドウを非表示にするまでの時間(sec)

    Transform Root; // メッセージを格納する親トランスフォーム

    private Coroutine hideCoroutine;    // 進行中の非表示コルーチン

    /// <summary>
    /// シングルトンインスタンスにアクセスするためのプロパティ。
    /// </summary>
    public static MessageWindow Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// オブジェクトが生成された際に呼び出されるメソッドです。既存のメッセージをクリアします。
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // すでにインスタンスが存在する場合は、新たなインスタンスを破棄
            Destroy(gameObject);
        }
        else
        {
            // このインスタンスをシングルトンインスタンスとして設定
            instance = this;
        }

        Clear();
    }

    /// <summary>
    /// 新しいメッセージをウィンドウに追加します。表示可能行数を超えたメッセージは古い順に削除されます。
    /// </summary>
    /// <param name="message">ウィンドウに表示する新しいメッセージ。</param>
    public void AppendMessage(string message)
    {
        // ウィンドウを表示する
        this.gameObject.SetActive(true);

        var obj = Object.Instantiate(MessagePrefab, Root);
        obj.text = message;

        // メッセージが表示可能行数を超過した場合、古いメッセージを削除
        if (Root.childCount > MessageLimit)
        {
            var removeCount = Root.childCount - MessageLimit;
            for (var i = removeCount - 1; i >= 0; i--)
            {
                Object.Destroy(Root.GetChild(i).gameObject);
            }
        }

        // メッセージウィンドウを非表示にする
        Hide(this.WindowDisplayTime);

    }

    public void Hide(int delayTime)
    {
        // ウィンドウがすでに非アクティブの場合は何もしないで処理終了
        if (!this.isActiveAndEnabled) return;

        // 進行中のメッセージ非表示コルーチンがある場合は停止
        if (this.hideCoroutine != null)
        {
            StopCoroutine(this.hideCoroutine);
        }

        // メッセージウィンドウを非表示にするコルーチンを新たに開始
        this.hideCoroutine = StartCoroutine(HideWindowAfterDelay(delayTime));
    }

    /// <summary>
    /// 一定時間待機後、メッセージウィンドウを非表示にします。
    /// メッセージウィンドウのテキストは全てクリアされます。
    /// </summary>
    /// <param name="delayTime">非表示にするまでの時間（秒）</param>
    /// <returns></returns>
    private IEnumerator HideWindowAfterDelay(int delayTime)
    {
        // 指摘された時間だけ待機
        yield return new WaitForSeconds(delayTime);

        // ウィンドウを非表示にする
        this.gameObject.SetActive(false);

        // ウィンドウの全テキストをクリアする
        Clear();
    }

    /// <summary>
    /// 既存のメッセージを全てクリアします。
    /// </summary>
    public void Clear()
    {
        Root = transform.Find("Root");
        foreach (Transform child in Root)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
