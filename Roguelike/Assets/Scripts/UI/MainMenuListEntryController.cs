using UnityEngine.UIElements;

public class MainMenuListEntryController
{
    private Label _menuItemLabel;

    public void SetVisualElement(VisualElement rootElement)
    {
        _menuItemLabel = rootElement.Q<Label>(("MenuItem"));
    }

    public void SetMainMenuData(MainMenuData mainMenuData)
    {
        _menuItemLabel.text = mainMenuData._menuItemName;
    }
}
