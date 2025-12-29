using System.Threading.Tasks;

public interface IPlayerLevelManager
{
    void Initialize(Player player);
    void AddExperience(int amount);
}
