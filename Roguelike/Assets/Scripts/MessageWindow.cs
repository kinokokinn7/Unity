using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    public Text MessagePrefab;
    [Range(1, 15)]
    public int MessageLimit = 5;

    Transform Root;

    public static MessageWindow Find()
    {
        return Object.FindObjectOfType<MessageWindow>();
    }

    public void AppendMessage(string message)
    {
        var obj = Object.Instantiate(MessagePrefab, Root);
        obj.text = message;

        // メッセージが表示可能行数を超過した場合
        if (Root.childCount > MessageLimit)
        {
            var removeCount = Root.childCount - MessageLimit;
            for (var i = removeCount - 1; i >= 0; i--)
            {
                // 削除すると子GameObjectの並び順が変わるので、削除するものの後ろから順に見ている
                Object.Destroy(Root.GetChild(i).gameObject);
            }
        }
    }

    private void Awake()
    {
        Root = transform.Find("Root");
        foreach (Transform child in Root)
        {
            Object.Destroy(child.gameObject);
        }
    }

}
