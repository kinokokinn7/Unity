using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Battler
{
    [SerializeField] BattlerBase _base;
    [SerializeField] int level;

    public BattlerBase Base { get => _base; }
    public int Level { get => level; }

    // ステータス
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int AT { get; set; }

    // 初期化
    public void Init()
    {
        MaxHP = _base.MaxHP;
        HP = MaxHP;
        AT = _base.AT;

    }

    public int TakeDamage(Battler attacker)
    {
        int damage = attacker.AT;
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        return damage;
    }
}

