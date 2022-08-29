using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    Battler Battler { get; set; }

    public void Setup(Battler battler)
    {
        Battler = battler;
    }
}
