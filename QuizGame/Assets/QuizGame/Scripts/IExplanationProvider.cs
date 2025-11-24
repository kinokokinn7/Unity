using System.Threading.Tasks;

public interface IExplanationProvider
{
    Task<string> GetExplanationAsync(QuizQuestion q);
}
