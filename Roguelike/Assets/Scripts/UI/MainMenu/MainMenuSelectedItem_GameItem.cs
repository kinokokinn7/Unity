public class MainMenuSelectedItem_GameItem : ISelectedItem
{
    /// <summary>
    /// 「アイテム」項目が選択された時のアクションを定義します。
    /// </summary>
    public void OnItemSelected()
    {
        var gameItemMenuController = UnityEngine.Object.FindObjectOfType<GameItemMenuController>();
        var mainMenuController = UnityEngine.Object.FindObjectOfType<MainMenuController>();

        // 効果音を鳴らす
        SoundEffectManager.Instance.PlayOpenWindowSound();

        // アイテムメニューを表示
        gameItemMenuController.ShowMenu();
        mainMenuController.Blur();
    }
}
