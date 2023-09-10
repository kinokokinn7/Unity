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

        result.Damage = Mathf.Max(0, AttackPower - target.DefensePower);
        if (target.IsNowDefense)
        {
            result.Damage /= 2;
        }
        target.HP -= result.Damage;
        result.LeaveHP = target.HP;
        return target.HP <= 0;
    }


}

[CreateAssetMenu(menuName = "BattleParameter")]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}
