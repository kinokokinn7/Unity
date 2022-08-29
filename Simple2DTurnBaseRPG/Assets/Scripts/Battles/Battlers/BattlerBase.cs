using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattlerBase : ScriptableObject
{
    // Batllerの基礎データ
    [SerializeField] new string name;
    [SerializeField] int maxHP;
    [SerializeField] int at;
    [SerializeField] Sprite sprite;

    public string Name { get => name; }
    public int MaxHP { get => maxHP; }
    public int At { get => at; }
    public Sprite Sprite { get => sprite; }
}
