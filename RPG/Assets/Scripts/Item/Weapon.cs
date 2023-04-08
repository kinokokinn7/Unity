using UnityEngine;
using System.Collections;

public enum WeaponKind
{
    Attack,
    Defense
}

[CreateAssetMenu(menuName = "Item/Weapon")]
public class Weapon : Item
{
    public WeaponKind Kind;
    public int Power;
}
