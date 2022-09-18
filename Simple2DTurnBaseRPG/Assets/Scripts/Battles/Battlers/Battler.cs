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

    public List<Move> Moves { get; set; }

    // 初期化
    public void Init()
    {
        // 覚えるワザから使えるワザを生成
        Moves = new List<Move>();
        foreach (var learnableMove in Base.LearnableMove)
        {
            if (learnableMove.Level <= level)
            {
                Moves.Add(new Move(learnableMove.MoveBase));
            }
        }
        Debug.Log(Moves.Count);

        MaxHP = _base.MaxHP;
        HP = MaxHP;
        AT = _base.AT;

    }

    public int TakeDamage(Move move, Battler attacker)
    {
        int damage = attacker.AT + move.Base.Power;
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        return damage;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

