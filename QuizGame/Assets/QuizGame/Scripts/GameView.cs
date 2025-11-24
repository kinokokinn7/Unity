using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameView : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text textQuestion;
    public TMP_Text textProgress;
    public TMP_Text textExplanation;
    public Button[] choiceButtons;   // 4個
    public TMP_Text[] choiceTexts;   // 4個

    IExplanationProvider explainer;


    void Awake()
    {
        // まずはローカルでOK。生成AIに切替えるなら new GenerativeExplanationProvider()
        explainer = new LocalExplanationProvider();

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            choiceButtons[i].onClick.AddListener(() => OnClickChoice(idx));
        }
    }

    void Start()
    {
        RenderCurrent();
    }

    void RenderCurrent()
    {
        var qm = QuizManager.I;
        var q = qm.GetCurrent();

        if (q == null)
        {
            ShowResult();
            return;
        }

        textQuestion.text = q.Question;

        for (int i = 0; i < 4; i++)
        {
            choiceTexts[i].text = q.Choices[i];
        }

        textExplanation.text = "";
        textProgress.text = $"{qm.Index + 1} / {qm.Total}（正解: {qm.Score}）";
    }

    async void OnClickChoice(int index)
    {
        var qm = QuizManager.I;
        var current = qm.GetCurrent();
        if (current == null) return;

        bool correct;
        qm.SubmitAnswer(index, out correct);

        textExplanation.text = correct ? "✅ 正解！" : "❌ 不正解…";

        // 解説（AI or ローカル）
        var msg = await explainer.GetExplanationAsync(current);
        textExplanation.text = msg;

        // 少し待って次へ
        await System.Threading.Tasks.Task.Delay(350);
        RenderCurrent();
    }

    void ShowResult()
    {
        var qm = QuizManager.I;
        textQuestion.text = $"結果: {qm.Score} / {qm.Total}";

        for (int i = 0; i < 4; i++)
        {
            choiceTexts[i].text = "";
        }

        textExplanation.text = "おつかれさま！";
    }

    public void OnChoice0(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        Answer(0);
    }

    public void OnChoice1(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        Answer(1);
    }

    public void OnChoice2(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        Answer(2);
    }

    public void OnChoice3(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        Answer(3);
    }

    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        ConfirmCurrent();
    }

    public void OnNext(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        GoNext();
    }

    public void OnToggleExplain(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        ToggleExplanation();
    }


    void Answer(int idx)
    {
        Debug.Log($"Answer {idx}");
        /* ここで処理 */
    }

    void ConfirmCurrent()
    {
        Debug.Log("Confirm");
        /* ここで処理 */
    }

    void GoNext()
    {
        Debug.Log("Next");
        /* ここで処理 */
    }

    void ToggleExplanation()
    {
        Debug.Log("ToggleExplain");
        /* ここで処理 */
    }
}