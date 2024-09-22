using UnityEngine;

public class TitleMenuSelectedItem_Continue : ISelectedItem
{
    private MapSceneManager mapSceneManager;
    private TitleManager titleManager;
    private SaveLoadController saveLoadController;

    public TitleMenuSelectedItem_Continue(MapSceneManager mapSceneManager, TitleManager titleManager, SaveLoadController saveLoadController)
    {
        this.mapSceneManager = mapSceneManager;
        this.titleManager = titleManager;
        this.saveLoadController = saveLoadController;
    }

    /// <summary>
    /// 「つづきから」が選択された時のアクションを定義します。
    /// </summary>
    public void OnItemSelected()
    {
        var saveData = this.saveLoadController?.Load();
        if (saveData != null)
        {
            this.mapSceneManager.LoadSavedMapScene(saveData); // セーブデータを使ってゲームを再開
            this.mapSceneManager.SetupMapSceneCommon();
        }
        else
        {
            Debug.LogWarning("セーブデータが見つかりませんでした。");
        }

        this.titleManager.StartGame();
    }
}