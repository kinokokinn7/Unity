using System.Threading.Tasks;
public class LocalExplanationProvider : IExplanationProvider
{
    public Task<string> GetExplanationAsync(QuizQuestion q) =>
        Task.FromResult(string.IsNullOrEmpty(q.FallbackExplanation) ? "解説は準備中だよ。" : q.FallbackExplanation);
}