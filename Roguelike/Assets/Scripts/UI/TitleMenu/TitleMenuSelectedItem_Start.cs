public class TitleMenuSelectedItem_Start : ISelectedItem
{
    private TitleManager titleManager;

    public TitleMenuSelectedItem_Start(TitleManager titleManager)
    {
        this.titleManager = titleManager;
    }

    /// <summary>
    /// 「はじめから」が選択された時のアクションを定義します。
    /// </summary>
    public void OnItemSelected()
    {
        titleManager.StartGame();
    }
}
