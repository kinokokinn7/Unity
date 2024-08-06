using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// ATK(攻撃力)を表すクラス。
/// </summary>
[System.Serializable]
public class Atk
{
    /// <summary>
    /// 現在の攻撃力。
    /// </summary>
    [SerializeField]
    [JsonProperty("currentValue")]
    private int currentValue;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public Atk()
    {
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="initAtk">攻撃力の初期値。</param>
    public Atk(int initAtk)
    {
        this.currentValue = initAtk;
    }

    /// <summary>
    /// 現在の攻撃力を取得します。
    /// </summary>
    /// <returns>現在の攻撃力。</returns>
    public int GetCurrentValue()
    {
        return this.currentValue;
    }

    /// <summary>
    /// 現在の攻撃力を設定します。
    /// </summary>
    /// <param name="value">現在の攻撃力。</param>
    public void SetCurrentValue(int value)
    {
        this.currentValue = value;
    }

    /// <summary>
    /// 攻撃力の値を指定分だけ増加します。
    /// </summary>
    /// <param name="increasedValue">攻撃力の増加量。</param>/
    public void IncreaseCurrentValue(int increasedValue)
    {
        this.currentValue += increasedValue;
    }

    /// <summary>
    /// 攻撃力の値を指定分だけ減らします。
    /// </summary>
    /// <param name="decreasedValue">攻撃力の減少量。</param>/
    public void DecreaseCurrentValue(int decreasedValue)
    {
        this.currentValue = Mathf.Max(this.currentValue - decreasedValue, 0);
    }

}
