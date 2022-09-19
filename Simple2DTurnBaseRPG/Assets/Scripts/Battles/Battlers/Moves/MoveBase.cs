using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ワザの基礎データ
[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

    public string Name { get => name; }
}
