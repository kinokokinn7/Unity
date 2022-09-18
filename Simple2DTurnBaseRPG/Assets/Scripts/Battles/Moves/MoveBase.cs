using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ワザの基礎データ
[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int power;

    public string Name { get => name; }
    public int Power { get => power; }
}
