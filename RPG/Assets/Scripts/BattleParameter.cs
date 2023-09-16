using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーまたは敵キャラの
/// 戦闘パラメータのベースクラス。
/// </summary>
[System.Serializable]
public class BattleParameterBase
{
    [Min(1)] public int HP;
    [Min(1)] public int MaxHP;

    [Min(1)] public int Attack;
    [Min(1)] public int Defense;

    [Min(1)] public int Level;
    [Min(0)] public int Exp;
    [Min(0)] public int Money;

    [Min(1)] public int LimitAttack = 500;
    [Min(1)] public int LimitDefense = 400;
    [Min(1)] public int LimitHP = 900;


    public Weapon AttackWeapon;
    public Weapon DefenseWeapon;

    public List<Item> Items;    // 上限4個までを想定して他のものを作成している

    /// <summary>
    /// 攻撃力。
    /// </summary>
    public int AttackPower
    {
        get => Attack + (AttackWeapon != null ? AttackWeapon.Power : 0);
    }

    /// <summary>
    /// 防御力。
    /// </summary>
    public int DefensePower
    {
        get => Defense + (DefenseWeapon != null ? DefenseWeapon.Power : 0);
    }

    public bool IsNowDefense { get; set; } = false;

    /// <summary>
    /// アイテム上限数。
    /// </summary>
    public bool IsLimitItemCount { get => Items.Count >= 4; }

    /// <summary>
    /// パラメータ値を他のパラメータベースオブジェクトにコピーします。
    /// </summary>
    /// <param name="dest"></param>
    public virtual void CopyTo(BattleParameterBase dest)
    {
        dest.HP = HP;
        dest.MaxHP = HP < MaxHP ? MaxHP : HP;
        dest.Attack = Attack;
        dest.Defense = Defense;
        dest.Level = Level;
        dest.Exp = Exp;
        dest.Money = Money;

        dest.AttackWeapon = AttackWeapon;
        dest.DefenseWeapon = DefenseWeapon;

        dest.Items = new List<Item>(Items.ToArray());

        dest.LimitAttack = LimitAttack;
        dest.LimitDefense = LimitDefense;
        dest.LimitHP = LimitHP;
    }

    /// <summary>
    /// こうげきを実行した時のダメージ値とこうげき後の残HPの情報。
    /// </summary>
    public class AttackResult
    {
        public int LeaveHP;
        public int Damage;
    }

    /// <summary>
    /// こうげきを実行した時のダメージ計算およびHP減算処理。
    /// </summary>
    public virtual bool AttackTo(BattleParameterBase target, out AttackResult result)
    {
        result = new AttackResult();

        float basicBamage = Mathf.Max(0, AttackPower / 2 - target.DefensePower / 4);
        float randomDamageAbs = basicBamage / 16 + 1;
        float randomDamage = Random.Range(-randomDamageAbs, randomDamageAbs);
        result.Damage = (int)Mathf.Max(Mathf.Floor(basicBamage + randomDamage), 0);
        if (target.IsNowDefense)
        {
            result.Damage /= 2;
        }
        target.HP -= result.Damage;
        result.LeaveHP = target.HP;
        return target.HP <= 0;
    }

    /// <summary>
    /// 経験値取得処理。
    /// レベルアップ判定処理。
    /// </summary>
    /// <param name="exp">取得した経験値。</param>
    /// <returns></returns>
    public bool GetExp(int exp)
    {
        Exp += exp;
        if (Exp >= (Level + 1) * 5)
        {
            AdjustParamWithLevel();
            return true;
        }
        return false;
    }

    public void AdjustParamWithLevel()
    {
        Level = Exp / 5;
        Attack = (int)(LimitAttack * Level / 100f);
        Defense = (int)(LimitDefense * Level / 100f);
        MaxHP = (int)(LimitHP * Level / 100f);
    }
}

[CreateAssetMenu(menuName = "BattleParameter")]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}
