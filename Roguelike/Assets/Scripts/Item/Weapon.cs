using System;
using System.Collections;
using System.Collections.Generic;
using Roguelike.Window;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ゲーム内の武器を表すクラスです。マップオブジェクトに装備させることができ、武器によって攻撃力が変化します。
/// </summary>
[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : Item
{
    public Atk Attack = new Atk();

    /// <summary>
    /// 武器を装備します。
    /// </summary>
    /// <param name="target">装備する対象のキャラ。</param>
    public override void Use(MapObjectBase target)
    {
        if (target.CurrentWeapon?.GetInstanceID() != this.GetInstanceID())
        {
            MessageWindow.Instance?.AppendMessage($"{this.Name}を装備した！");
            target.CurrentWeapon = this;
        }
        else
        {
            MessageWindow.Instance?.AppendMessage($"武器を取り外した！");
            target.CurrentWeapon = null;
        }
    }

    /// <summary>
    /// 武器をマップオブジェクトに装備します。攻撃力が増加します。
    /// </summary>
    /// <param name="obj">装備するマップオブジェクト。</param>
    /// <param name="soundOff">効果音をオフにするか否か。</param>
    public void Attach(MapObjectBase obj, bool soundOff = false)
    {
        if (!soundOff)
        {
            SoundEffectManager.Instance.PlayAttachWeaponSound();
        }
        obj.Attack.IncreaseCurrentValue(Attack.GetCurrentValue());
    }

    /// <summary>
    /// 武器をマップオブジェクトから外します。攻撃力が減少します。
    /// </summary>
    /// <param name="obj">装備を外すマップオブジェクト。</param>
    /// <param name="soundOff">効果音をオフにするか否か。</param>
    public void Detach(MapObjectBase obj)
    {
        obj.Attack.DecreaseCurrentValue(Attack.GetCurrentValue());
    }

    /// <summary>
    /// 他の武器と合成して新しい武器を作成します。攻撃力は両方の武器の攻撃力の合計となります。
    /// </summary>
    /// <param name="other">合成する他の武器。</param>
    /// <returns>合成された新しい武器。</returns>
    public Weapon Merge(Weapon other)
    {
        if (other == null) return other;

        var newWeapon = ScriptableObject.CreateInstance<Weapon>();
        newWeapon.Name = Name; // 新しい武器の名前は元の武器の名前を引き継ぎます

        newWeapon.Attack.SetCurrentValue(this.Attack.GetCurrentValue()
                                         + (other != null ? other.Attack.GetCurrentValue() : 0)); // 両方の武器の攻撃力を合算
        return newWeapon;
    }

    /// <summary>
    /// 武器の情報を文字列で返します。主にUIでの表示用です。
    /// </summary>
    /// <returns>武器の名前と攻撃力を含む文字列。</returns>
    public override string ToString()
    {
        return $"{Name} Atk+{Attack.GetCurrentValue()}";
    }

    public override ItemData ToItemData()
    {
        return new WeaponData(this);
    }

    public override void FromItemData(ItemData data)
    {
        base.FromItemData(data);
        if (data is WeaponData weaponData)
        {
            Attack.SetCurrentValue(weaponData.Attack);
        }
    }

}
