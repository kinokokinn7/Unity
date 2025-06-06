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
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] int exp;

    public string Name { get => name; }
    public int MaxHP { get => maxHP; }
    public int AT { get => at; }
    public Sprite Sprite { get => sprite; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; }
    public int Exp { get => exp; }
}
