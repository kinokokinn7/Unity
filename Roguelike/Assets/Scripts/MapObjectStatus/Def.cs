using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// DEF(防御力)を表すクラス。
/// </summary>
[System.Serializable]
public class Def
{
    /// <summary>
    /// 現在の防御力。
    /// </summary>
    [SerializeField]
    [JsonProperty("currentValue")]
    private int currentValue;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public Def()
    {
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="initDef">防御力の初期値。</param>
    public Def(int initDef)
    {
        this.currentValue = initDef;
    }

    /// <summary>
    /// 現在の防御力を取得します。
    /// </summary>
    /// <returns>現在の防御力。</returns>
    public int GetCurrentValue()
    {
        return this.currentValue;
    }

    /// <summary>
    /// 現在の防御力を設定します。
    /// </summary>
    /// <param name="value">現在の防御力。</param>
    public void SetCurrentValue(int value)
    {
        this.currentValue = value;
    }

    /// <summary>
    /// 防御力の値を指定分だけ増加します。
    /// </summary>
    /// <param name="increasedValue">防御力の増加量。</param>/
    public void IncreaseCurrentValue(int increasedValue)
    {
        this.currentValue += increasedValue;
    }

    /// <summary>
    /// 防御力の値を指定分だけ減らします。
    /// </summary>
    /// <param name="decreasedValue">防御力の減少量。</param>/
    public void DecreaseCurrentValue(int decreasedValue)
    {
        this.currentValue = Mathf.Max(this.currentValue - decreasedValue, 0);
    }

}
