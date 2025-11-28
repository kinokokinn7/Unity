using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class GameViewToolkit : MonoBehaviour
{
    Label question, progress, explain;
    Button[] choice = new Button[4];

    IExplanationProvider explainer = new LocalExplanationProvider(); // 生成AIにするなら差し替え

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.styleSheets.Add(Resources.Load<StyleSheet>("Common"));
        root.styleSheets.Add(Resources.Load<StyleSheet>("Game"));

        question = root.Q<Label>("question");
        progress = root.Q<Label>("progress");
        explain = root.Q<Label>("explain");
        choice[0] = root.Q<Button>("choice0");
        choice[1] = root.Q<Button>("choice1");
        choice[2] = root.Q<Button>("choice2");
        choice[3] = root.Q<Button>("choice3");

        for (int i = 0; i < 4; i++) { int idx = i; choice[i].clicked += () => OnClickChoice(idx); }

        // 受け取ったExamTagで開始
        var ex = QuizManager.I.PendingExamTag;
        if (!QuizManager.I.StartQuiz(ex, null, 10))
        {
            question.text = "問題がありません"; return;
        }
        RenderCurrent();
    }

    void RenderCurrent()
    {
        var qm = QuizManager.I;
        var q = qm.GetCurrent();
        if (q == null) { ShowResult(); return; }

        question.text = q.Question;
        for (int i = 0; i < 4; i++) choice[i].text = q.Choices[i];
        explain.text = "";
        progress.text = $"{qm.Index + 1} / {qm.Total}（正解: {qm.Score}）";
    }

    async void OnClickChoice(int idx)
    {
        var qm = QuizManager.I;
        var current = qm.GetCurrent(); if (current == null) return;

        bool correct;
        qm.SubmitAnswer(idx, out correct);
        explain.text = correct ? "✅ 正解！" : "❌ 不正解…";

        var msg = await explainer.GetExplanationAsync(current);
        explain.text = msg;

        await Task.Delay(350);
        RenderCurrent();
    }

    void ShowResult()
    {
        var qm = QuizManager.I;
        question.text = $"結果: {qm.Score} / {qm.Total}";
        for (int i = 0; i < 4; i++) choice[i].text = "";
    }
}