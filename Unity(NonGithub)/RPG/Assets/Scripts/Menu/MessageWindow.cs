using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MessageWindow : MonoBehaviour
{
    public const string YES_NO_MENU_LINE_TEXT = "<YESNO>";
    public const string EFFECT_LINE_TEXT = "<ANIMATION>";

    public string Message = "";
    public float TextSpeedPerChar = 1000 / 10f;
    [Min(1)] public float SpeedUpRate = 3f;
    [Min(1)] public int MaxLineCount = 4;
    public Animator[] Effects { get; set; }

    /// <summary>
    /// メッセージ表示が終了しているかを表すフラグ。
    /// </summary>
    public bool IsEndMessage { get; private set; } = true;

    public YesNoMenu YesNoMenu;
    public string[] Params { get; set; }

    Transform TextRoot;
    Text TextTemplate;

    private void Awake()
    {
        TextRoot = transform.Find("Panel");
        TextTemplate = TextRoot.Find("TextTemplate").GetComponent<Text>();
        TextTemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// メッセージ表示を開始します。
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    public void StartMessage(string message)
    {
        Message = message;
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(MessageAnimation());
    }

    /// <summary>
    /// メッセージウィンドウを閉じます。
    /// 「はい/いいえ」選択ウィンドウもあわせて閉じます。
    /// </summary>
    public void Close()
    {
        StopAllCoroutines();
        Params = null;
        IsEndMessage = true;
        YesNoMenu.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// メッセージのアニメーションを表示します。
    /// </summary>
    /// <returns></returns>
    private IEnumerator MessageAnimation()
    {
        IsEndMessage = false;
        DestroyLineText();

        var lines = Message.Split(new[] { "¥n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var lineCount = 0;
        var textObjs = new List<Text>();

        foreach (var line in lines)
        {
            lineCount++;
            if (lineCount >= MaxLineCount)
            {
                UnityEngine.Object.Destroy(textObjs[0].gameObject);
                textObjs.RemoveAt(0);
            }
            var lineText = UnityEngine.Object.Instantiate(TextTemplate, TextRoot);
            lineText.gameObject.SetActive(true);
            lineText.text = "";
            textObjs.Add(lineText);

            // <YESNO>タグの場合
            if (line == YES_NO_MENU_LINE_TEXT)
            {
                // 「はい/いいえ」メニューを開く
                YesNoMenu.gameObject.SetActive(true);
                YesNoMenu.Open();

                yield return new WaitWhile(() => YesNoMenu.DoOpen);
            }
            else if (line.IndexOf(EFFECT_LINE_TEXT) == 0)
            {
                yield return new WaitUntil(() => Input.anyKeyDown);

                var elements = line.Split(' ');
                if (elements.Length < 3)
                {
                    Debug.LogWarning($"Invalid EffectLine... line={line}");
                    continue;
                }
                if (!int.TryParse(elements[1], out int effectIndex))
                {
                    Debug.LogWarning($"Fail to parse EffectIndex... line={line}");
                    continue;
                }

                var effect = Effects[effectIndex];
                effect.SetTrigger(elements[2]);

                var canvas = GetComponent<Canvas>();
                canvas.enabled = false;
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() =>
                {
                    return Input.anyKeyDown;
                });
                canvas.enabled = true;

            }
            else
            {
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == '#' && (i + 1) < line.Length)
                    {
                        // パラメータテキストの場合（「#1」や「#2」など）
                        // NOTE:
                        // テキストに埋め込む際は「#1」や「#2」など#を頭文字にした後に数字を一文字指定してください。
                        // 指定した数字を添え字として「MessageWindow」コンポーネントの
                        // 「Params」フィールドに設定したものへ変換するようにしています。
                        if (char.IsDigit(line[i + 1]))
                        {
                            var index = line[i + 1] - '0';
                            var paramText = (index < Params.Length) ? Params[index] : $"#{line[i + 1]}";

                            foreach (var ch in paramText)
                            {
                                lineText.text += ch;
                                float speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                                yield return new WaitForSeconds(speed);
                            }
                        }
                        // 文字が"#"の場合
                        // NOTE:
                        // テキストの中に「##」と書くと文字列のエスケープシーケンス「\\」と同じ感じで「#」に変換されます。
                        else if (line[i + 1] == '#')
                        {
                            lineText.text += '#';
                            float speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                            yield return new WaitForSeconds(speed);
                        }
                        // それ以外の場合
                        else
                        {
                            lineText.text += line[i + 1];
                            float speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                            yield return new WaitForSeconds(speed);
                        }
                        i++;
                    }
                    else
                    {
                        lineText.text += line[i];
                        float speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                        yield return new WaitForSeconds(speed);
                    }
                }
            }
        }
        // 全行表示後、任意のキーが押されたらウィンドウを閉じる
        yield return new WaitUntil(() => Input.anyKeyDown);
        Params = null;
        IsEndMessage = true;
        gameObject.SetActive(false);
    }

    void DestroyLineText()
    {
        foreach (var text in TextRoot.GetComponentsInChildren<Text>()
            .Where(_t => _t != TextTemplate))
        {
            UnityEngine.Object.Destroy(text.gameObject);
        }
    }

}