using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

public class GenreSelectToolkit : MonoBehaviour
{
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.styleSheets.Add(Resources.Load<StyleSheet>("Common"));

        var list = root.Q<ListView>("list");
        var tags = QuizManager.I.GetAvailableExamTags().ToList();

        list.itemsSource = tags;
        list.makeItem = () => new Label() { style = { unityTextAlign = TextAnchor.MiddleCenter, fontSize = 24 } };
        list.bindItem = (e, i) => ((Label)e).text = tags[i];
        list.onSelectionChange += sel =>
        {
            var tag = sel.Cast<string>().FirstOrDefault();
            if (tag == null) return;
            QuizManager.I.PendingExamTag = tag;
            SceneManager.LoadScene("LV_Game");
        };

        root.Q<Button>("btnBack").clicked += () => SceneManager.LoadScene("LV_Title");
    }
}