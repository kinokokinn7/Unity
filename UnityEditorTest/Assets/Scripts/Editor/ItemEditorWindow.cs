using UnityEngine;
using UnityEditor;

public class ItemEditorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string fileTitle = "File Title";
    private string fileCaption = "File Caption";
    private GUISkin skin;

    private ItemData itemData;

    [MenuItem("Window/Demo/ItemEditorWindow")]
    public static void CreateWindow()
    {
        EditorWindow.GetWindow<ItemEditorWindow>();
    }

    private void OnEnable()
    {
        this.skin = AssetDatabase.LoadAssetAtPath<GUISkin>(
            "Assets/Scripts/Editor/Editor Resources/ItemEditorGUISkin.guiskin"
        );
        this.itemData = Resources.Load<ItemData>("ItemData");
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope(this.skin.GetStyle("Header")))
        {
            EditorGUILayout.LabelField("Assets/Item/Map1Items.json");
            fileTitle = EditorGUILayout.TextField(fileTitle);
            fileCaption = EditorGUILayout.TextArea(
                fileCaption,
                GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)
            );

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Button("アイテムを追加");
                GUILayout.FlexibleSpace();
                GUILayout.Button("元に戻す");
                GUILayout.Button("保存");
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
            {
                this.scrollPosition = scroll.scrollPosition;
                for (int i = 0; i < 100; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("100001", GUILayout.MaxWidth(50f));
                        EditorGUILayout.LabelField("アイテム名", GUILayout.MaxWidth(200f));
                        GUILayout.Button("編集", GUILayout.MaxWidth(50f));
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(this.skin.GetStyle("Inspector")))
            {
                EditorGUILayout.TextField("ID", "100001");
                EditorGUILayout.TextField("アイテム名", "アイテム名");
                EditorGUILayout.Popup("アイテムタイプ", 0,
                    new string[] {
                        "武器", "防具", "回復", "トラップ",
                        "強化素材", "進化素材", "重要"
                    }
                );
                EditorGUILayout.TextArea("説明文",
                    GUILayout.Height(EditorGUIUtility.singleLineHeight * 4f));
                GUILayout.FlexibleSpace();
            }
        }

    }

}
