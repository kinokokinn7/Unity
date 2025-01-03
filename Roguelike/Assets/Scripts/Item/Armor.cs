using System;
using System.Collections;
using System.Collections.Generic;
using Roguelike.Window;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ゲーム内の防具（鎧）を表すクラスです。
/// マップオブジェクトに装備させることができ、防具（鎧）によって防御力が変化します。
/// </summary>
[CreateAssetMenu(menuName = "Armor")]
public class Armor : Item
{
    public Def Defence = new Def();
    public bool IsEquipped { get; set; } = false;

    /// <summary>
    /// 防具（鎧）を装備します。
    /// </summary>
    /// <param name="target">装備する対象のキャラ。</param>
    public override void Use(MapObjectBase target)
    {
        if (target.CurrentArmor?.GetInstanceID() != this.GetInstanceID())
        {
            MessageWindow.Instance?.AppendMessage($"{this.Name}を装備した！");
            SoundEffectManager.Instance.PlayAttachArmorSound();
            target.CurrentArmor = this;
        }
        else
        {
            MessageWindow.Instance?.AppendMessage($"{this.Name}を取り外した！");
            target.CurrentArmor = null;
        }
    }

    /// <summary>
    /// 防具（鎧）をマップオブジェクトに装備します。防御力が増加します。
    /// </summary>
    /// <param name="obj">装備するマップオブジェクト。</param>
    /// <param name="soundOff">効果音をオフにするか否か。</param>
    public void Attach(MapObjectBase obj)
    {
        obj.Attack.IncreaseCurrentValue(Defence.GetCurrentValue());
        this.IsEquipped = true;

    }

    /// <summary>
    /// 防具（鎧）をマップオブジェクトから外します。防御力が減少します。
    /// </summary>
    /// <param name="obj">装備を外すマップオブジェクト。</param>
    /// <param name="soundOff">効果音をオフにするか否か。</param>
    public void Detach(MapObjectBase obj)
    {
        obj.Attack.DecreaseCurrentValue(Defence.GetCurrentValue());
        this.IsEquipped = false;
    }

    /// <summary>
    /// 他の防具（鎧）と合成して新しい防具（鎧）を作成します。防御力は両方の防具（鎧）の防御力の合計となります。
    /// </summary>
    /// <param name="other">合成する他の防具（鎧）。</param>
    /// <returns>合成された新しい防具（鎧）。</returns>
    public Armor Merge(Armor other)
    {
        if (other == null) return other;

        var newArmor = ScriptableObject.CreateInstance<Armor>();
        newArmor.Name = Name; // 新しい防具（鎧）の名前は元の防具（鎧）の名前を引き継ぎます

        newArmor.Defence.SetCurrentValue(this.Defence.GetCurrentValue()
                                         + (other != null ? other.Defence.GetCurrentValue() : 0)); // 両方の防具（鎧）の防御力を合算
        return newArmor;
    }

    /// <summary>
    /// 防具（鎧）の情報を文字列で返します。主にUIでの表示用です。
    /// </summary>
    /// <returns>防具（鎧）の名前と防御力を含む文字列。</returns>
    public override string ToString()
    {
        return $"{Name} Def+{Defence.GetCurrentValue()}";
    }

    public override ItemData ToItemData()
    {
        return new ArmorData(this);
    }

    public override void FromItemData(ItemData data)
    {
        base.FromItemData(data);
        if (data is not ArmorData ArmorData)
        {
            return;
        }

        Defence.SetCurrentValue(ArmorData.Defence);
        this.IsEquipped = ArmorData.IsEquipped;
    }

}
