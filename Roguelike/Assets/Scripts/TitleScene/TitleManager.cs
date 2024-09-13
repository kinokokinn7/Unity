using UnityEngine;

public class TitleManager : MonoBehaviour
{
    // タイトル用のCanvas
    public GameObject titleCanvas;

    // タイトル後に有効化したいオブジェクトの配列
    public GameObject[] gameObjectsToEnable;

    void Start()
    {
        // ゲーム開始時に、タイトル用Canvasだけを有効化
        titleCanvas.SetActive(true);

        // 他のオブジェクトはすべて無効化する
        foreach (GameObject obj in gameObjectsToEnable)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        // フェードイン中は操作を無効化する
        if (FadeController.Instance.IsFading)
        {
            return;
        }

        // タイトル画面がクリックされたら
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z))
        {
            // タイトル用Canvasを無効化
            titleCanvas.SetActive(false);

            // 他のオブジェクトを有効化
            foreach (GameObject obj in gameObjectsToEnable)
            {
                obj.SetActive(true);
            }
        }
    }
}