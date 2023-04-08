using UnityEngine;
using System.Collections;

/// <summary>
/// 回復用のアイテムクラス
/// </summary>
[CreateAssetMenu(menuName = "Item/Life Recovery Item")]
public class LifeRecoveryItem : Item
{
    public int RecoveryPower;
}
