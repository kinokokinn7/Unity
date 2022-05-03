using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SaveData
{
    public int Floor;
    public int Level;
    public int Hp;
    public int Attack;
    public int Exp;
    public string WeaponName;
    public int WeaponAttack;
    public List<string> MapData;

    public void Save()
    {
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("save", json);
    }

    public static SaveData Recover()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            var json = PlayerPrefs.GetString("save");
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return null;
        }
    }

    public static void Destroy()
    {
        PlayerPrefs.DeleteAll();
    }
}
