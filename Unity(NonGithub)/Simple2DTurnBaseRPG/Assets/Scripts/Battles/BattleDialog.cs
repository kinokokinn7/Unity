using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialog : MonoBehaviour
{
    // 1文字ずつメッセージを表示する
    [SerializeField] Text text;
    [SerializeField] float letterPerSecond;

    public IEnumerator TypeDialog(string line, bool auto = true)
    {
        text.text = "";
        foreach (char letter in line)
        {
            text.text += letter;
            yield return new WaitForSeconds(letterPerSecond);
        }

        if (auto)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
    }
}
