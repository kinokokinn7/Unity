using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableText : MonoBehaviour
{
    // Textの色を変える
    // 選択中なら黄色：そうでないなら白
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // 選択中なら色を変える
    public void SetSelectedColor(bool selected)
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        if (selected)
        {
            text.color = Color.yellow;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
