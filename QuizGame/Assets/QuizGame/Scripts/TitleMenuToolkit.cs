using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleMenuToolkit : MonoBehaviour
{
    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;
        root.styleSheets.Add(Resources.Load<StyleSheet>("Common")); // Resources/に置いた場合

        root.Q<Button>("btnStart").clicked += () => SceneManager.LoadScene("LV_Genre");
        root.Q<Button>("btnQuit").clicked += () => Application.Quit();
    }
}