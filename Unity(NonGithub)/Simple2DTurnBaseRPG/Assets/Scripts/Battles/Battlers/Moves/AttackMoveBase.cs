using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AttackMoveBase : MoveBase
{
    [SerializeField] int power;
    public int Power { get => power; }

    public override string RunMoveResult(BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        int damage = targetUnit.Battler.TakeDamage(power, sourceUnit.Battler);
        return $"{sourceUnit.Battler.Base.Name}の{Name}！\n" +
               $"{targetUnit.Battler.Base.Name}は{damage}のダメージ！";
    }
}
