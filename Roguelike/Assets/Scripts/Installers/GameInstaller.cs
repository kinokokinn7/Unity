using Zenject;
using Roguelike.Window;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Bind Player Components
        Container.Bind<PlayerInputProvider>().AsSingle();
        Container.Bind<PlayerFoodHandler>().AsSingle();

        // Bind existing Monobehaviours if needed, or assume they are retrieved via FindObjectOfType for now if not converted.
        // But for PlayerFoodHandler to get MessageWindow, we might need to bind MessageWindow.
        // Since MessageWindow.Instance exists, we can bind it from instance if it's already in the scene, 
        // or just let PlayerFoodHandler use MessageWindow.Instance inside if we didn't want to pass it.
        // However, better to bind it.
        
        // Assuming MessageWindow is a singleton in scene.
        // If MessageWindow is a MonoBehaviour in the scene context, we can bind it like this:
        Container.Bind<MessageWindow>().FromComponentInHierarchy().AsSingle();
    }
}
