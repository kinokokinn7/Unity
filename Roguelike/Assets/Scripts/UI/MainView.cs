using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _listEntryTemplate;

    void OnEnable()
    {
        // UXMLはすでにUIDocumentコンポーネントによってインスタンス化されている
        var uiDocument = GetComponent<UIDocument>();

        // メインメニューリストコントローラを初期化する
        var mainMenuListController = new MainMenuListController();
        mainMenuListController.InitializeCharacterList(uiDocument.rootVisualElement, _listEntryTemplate);
    }
}
