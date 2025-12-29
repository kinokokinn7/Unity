using System.Threading.Tasks;

public interface IPlayerEffectManager
{
    void Initialize(Player player);
    Task PlayLevelUpEffect();
    Task PlayGoalEffect();
}
