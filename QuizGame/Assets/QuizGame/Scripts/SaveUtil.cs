using System.IO;
using UnityEngine;
public static class SaveUtil
{
    static string Path => System.IO.Path.Combine(
        Application.persistentDataPath,
        "quiz_save.json"
    );

    public static void Save(SaveData d)
    {
        File.WriteAllText(Path, JsonUtility.ToJson(d));
    }

    public static SaveData Load()
    {
        return File.Exists(Path)
            ? JsonUtility.FromJson<SaveData>(File.ReadAllText(Path))
            : new SaveData();
    }
}