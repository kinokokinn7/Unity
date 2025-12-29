using System.Collections;

public interface IPlayerInteractionManager
{
    void Initialize(Player player);
    bool DoWaitEvent { get; }
    void CheckEvent();
    void SetDoWaitEvent(bool wait);
}
