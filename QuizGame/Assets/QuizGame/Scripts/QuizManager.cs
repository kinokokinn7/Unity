// QuizManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// クイズの管理を行うシングルトンコンポーネント。
/// ゲームの全体的なクイズ状態（問題一覧、現在のインデックス、スコアなど）を保持し、
/// 問題の開始、取得、回答の提出などの操作を提供します。
/// </summary>
public class QuizManager : MonoBehaviour
{
    /// <summary>
    /// QuizManager の単一インスタンスへのグローバルアクセスポイント。
    /// </summary>
    public static QuizManager I { get; private set; }

    /// <summary>
    /// インスペクターから割り当てるクイズ問題データベースへの参照。
    /// </summary>
    [SerializeField] private QuizQuestionDB db;

    /// <summary>
    /// 現在のクイズセッションで使用される問題の一覧（シャッフル・制限後のもの）。
    /// </summary>
    public List<QuizQuestion> Current = new();

    /// <summary>
    /// 現在表示している問題のインデックス（0ベース）。未開始時は -1。
    /// </summary>
    public int Index = -1;

    /// <summary>
    /// 現在のセッションでの正答数（スコア）。
    /// </summary>
    public int Score = 0;

    /// <summary>
    /// タイトル画面などからジャンル（ExamTag）を遷移時に保持するための一時的な受け渡し文字列。
    /// </summary>
    public string PendingExamTag = null;

    /// <summary>
    /// タイトル画面などからトピック（Topic）を遷移時に保持するための一時的な受け渡し文字列。
    /// </summary>
    public string PendingTopic = null;

    /// <summary>
    /// コンポーネントの初期化処理。シングルトンインスタンスの登録と DontDestroyOnLoad の設定を行います。
    /// </summary>
    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// データベース内に存在する、空でない ExamTag の一覧を重複を除いて取得します。
    /// </summary>
    /// <returns>ソートされたユニークな exam tag の配列。</returns>
    public string[] GetAvailableExamTags() =>
        db.Questions.Where(q => !string.IsNullOrEmpty(q.ExamTag))
                    .Select(q => q.ExamTag).Distinct().OrderBy(x => x).ToArray();

    /// <summary>
    /// 新しいクイズセッションを開始します。内部状態（Score、Index、Current）を初期化します。
    /// </summary>
    /// <param name="examTag">絞り込み対象の ExamTag。null または空文字列で絞り込みを行いません。</param>
    /// <param name="topic">絞り込み対象の Topic。null または空文字列で絞り込みを行いません。</param>
    /// <param name="count">取得する問題数の上限。0 以下で制限なし。</param>
    /// <returns>問題が存在すれば true、存在しなければ false を返します。</returns>
    public bool StartQuiz(string examTag, string topic, int count)
    {
        Score = 0;
        Index = 0;
        Current.Clear();

        var filtered = db.Questions.Where(q =>
            (string.IsNullOrEmpty(examTag) || q.ExamTag == examTag) &&
            (string.IsNullOrEmpty(topic) || q.Topic == topic)).ToList();

        if (filtered.Count == 0) return false;

        // シャッフル
        for (int i = 0; i < filtered.Count; i++)
        {
            int j = Random.Range(i, filtered.Count);
            (filtered[i], filtered[j]) = (filtered[j], filtered[i]);
        }

        if (count > 0 && count < filtered.Count) filtered = filtered.GetRange(0, count);
        Current = filtered;
        return true;
    }

    /// <summary>
    /// 現在のインデックスに対応する QuizQuestion を返します。インデックスが範囲外の場合は null を返します。
    /// </summary>
    /// <returns>現在の問題、存在しなければ null。</returns>
    public QuizQuestion GetCurrent() =>
        (Index >= 0 && Index < Current.Count) ? Current[Index] : null;

    /// <summary>
    /// 現在の問題に対して回答を提出します。正誤判定を行い、スコアとインデックスを更新します。
    /// </summary>
    /// <param name="chosen">ユーザーが選択した選択肢のインデックス。</param>
    /// <param name="isCorrect">呼び出し後に、その回答が正解かどうかを示す値が格納されます。</param>
    /// <returns>現在の問題が存在し回答を処理できた場合は true、存在しない場合は false を返します。</returns>
    public bool SubmitAnswer(int chosen, out bool isCorrect)
    {
        isCorrect = false;
        var q = GetCurrent();
        if (q == null) return false;

        isCorrect = (chosen == q.CorrectIndex);
        if (isCorrect) Score++;
        Index++;
        return true;
    }

    /// <summary>
    /// 現在のセッションでの問題総数を返します。
    /// </summary>
    public int Total => Current.Count;
}
