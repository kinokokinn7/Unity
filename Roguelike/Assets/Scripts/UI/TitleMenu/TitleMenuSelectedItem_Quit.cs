public class TitleMenuSelectedItem_Quit : ISelectedItem
{
    private TitleManager titleManager;

    public TitleMenuSelectedItem_Quit(TitleManager titleManager)
    {
        this.titleManager = titleManager;
    }

    /// <summary>
    /// 「さようなら」が選択された時のアクションを定義します。
    /// </summary>
    public void OnItemSelected()
    {
        this.titleManager.QuitGame();
    }
}
