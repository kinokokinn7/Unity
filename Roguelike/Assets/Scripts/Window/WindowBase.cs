using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ベースとなるウィンドウのクラスです。
/// 新しいメッセージの追加や、メッセージの表示数を制限する機能を提供します。
/// </summary>
public class WindowBase : MonoBehaviour
{
    private static WindowBase instance;  // シングルトンインスタンス

    [SerializeField] protected Text MessagePrefab; // メッセージを表示するためのプレハブ

    [Range(1, 15)]
    [SerializeField] protected int MessageLimit = 5; // 一度に表示可能なメッセージの最大数

    protected Transform Root; // メッセージを格納する親トランスフォーム

    protected Coroutine hideCoroutine;    // 進行中の非表示コルーチン

    /// <summary>
    /// シングルトンインスタンスにアクセスするためのプロパティ。
    /// </summary>
    public static WindowBase Instance
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
    /// 新しいメッセージをウィンドウに追加します。
    /// </summary>
    /// <param name="message">ウィンドウに表示する新しいメッセージ。</param>
    public virtual void AppendMessage(string message)
    {
    }

    public virtual void Hide(int delayTime)
    {
        // ウィンドウがすでに非アクティブの場合は何もしないで処理終了
        if (!this.isActiveAndEnabled) return;
    }

    /// <summary>
    /// 既存のメッセージを全てクリアします。
    /// </summary>
    public virtual void Clear()
    {
        Root = transform.Find("Root");
        foreach (Transform child in Root)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
