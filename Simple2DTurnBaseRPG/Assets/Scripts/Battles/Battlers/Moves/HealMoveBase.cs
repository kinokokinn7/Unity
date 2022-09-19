using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealMoveBase : MoveBase
{
    [SerializeField] int healPoint;
    public int Power { get => healPoint; }

    public override string RunMoveResult(BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        sourceUnit.Battler.Heal(healPoint);
        return $"{sourceUnit.Battler.Base.Name}の{Name}!\n" +
            $"{sourceUnit.Battler.Base.Name}は{healPoint}回復した！";
    }
}

