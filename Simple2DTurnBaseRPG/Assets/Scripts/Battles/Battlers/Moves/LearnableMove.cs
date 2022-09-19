using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// どのレベルでどのワザを覚えるのかを対応づける
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase { get => moveBase; }
    public int Level { get => level; }
}
