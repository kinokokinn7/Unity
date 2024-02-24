using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内のメッセージウィンドウを管理するクラスです。新しいメッセージの追加や、メッセージの表示数を制限する機能を提供します。
/// </summary>
public class MessageWindow : MonoBehaviour
{
    public Text MessagePrefab; // メッセージを表示するためのプレハブ
    [Range(1, 15)]
    public int MessageLimit = 5; // 一度に表示可能なメッセージの最大数

    Transform Root; // メッセージを格納する親トランスフォーム

    /// <summary>
    /// シーン内からMessageWindowインスタンスを検索して返します。
    /// </summary>
    /// <returns>見つかったMessageWindowインスタンス。</returns>
    public static MessageWindow Find()
    {
        return Object.FindObjectOfType<MessageWindow>();
    }

    /// <summary>
    /// 新しいメッセージをウィンドウに追加します。表示可能行数を超えたメッセージは古い順に削除されます。
    /// </summary>
    /// <param name="message">ウィンドウに表示する新しいメッセージ。</param>
    public void AppendMessage(string message)
    {
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
    }

    /// <summary>
    /// オブジェクトが生成された際に呼び出されるメソッドです。既存のメッセージをクリアします。
    /// </summary>
    private void Awake()
    {
        Root = transform.Find("Root");
        foreach (Transform child in Root)
        {
            Object.Destroy(child.gameObject);
        }
    }

}
