public class TitleMenuSelectedItem_Start : ISelectedItem
{
    private MapSceneManager mapSceneManager;
    private TitleManager titleManager;

    public TitleMenuSelectedItem_Start(MapSceneManager mapSceneManager, TitleManager titleManager)
    {
        this.mapSceneManager = mapSceneManager;
        this.titleManager = titleManager;
    }

    /// <summary>
    /// 「はじめから」が選択された時のアクションを定義します。
    /// </summary>
    public void OnItemSelected()
    {
        mapSceneManager.InitializeMapScene();
        mapSceneManager.SetupMapSceneCommon();
        titleManager.StartGame();
    }
}
