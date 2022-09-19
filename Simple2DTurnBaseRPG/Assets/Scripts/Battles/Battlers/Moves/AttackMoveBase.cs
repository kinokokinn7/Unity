using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AttackMoveBase : MoveBase
{
    [SerializeField] int power;
    public int Power { get => power; }

}
