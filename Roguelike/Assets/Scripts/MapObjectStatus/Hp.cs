using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPを表すクラス。
/// </summary>
[System.Serializable]
public class Hp
{
    /// <summary>
    /// 現在のHP。
    /// </summary>
    [SerializeField]
    private int currentValue;

    /// <summary>
    /// 最大HP。
    /// </summary>
    [SerializeField]
    private int maxValue;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="initHp">最大HP、HPの初期値。</param>
    public Hp(int initHp)
    {
        this.maxValue = initHp;
        this.currentValue = initHp;
    }

    /// <summary>
    /// 現在のHPを取得します。
    /// </summary>
    /// <returns>現在のHP。</returns>
    public int GetCurrentValue()
    {
        return this.currentValue;
    }

    /// <summary>
    /// 最大HPを返します。
    /// </summary>
    /// <returns>最大HP。</returns>
    public int GetMaxValue()
    {
        return this.maxValue;
    }

    /// <summary>
    /// 現在のHPを設定します。
    /// </summary>
    /// <param name="value">現在のHP。</param>
    public void SetCurrentValue(int value)
    {
        this.currentValue = value;
    }

    /// <summary>
    /// 最大HPを設定します。
    /// </summary>
    /// <returns></returns>
    public void SetMaxValue(int value)
    {
        this.maxValue = value;
    }

    /// <summary>
    /// HPを回復します。
    /// </summary>
    /// <param name="recoveryAmount">回復量。</param>
    public void recover(int recoveryAmount)
    {
        this.currentValue = Mathf.Min(this.currentValue + recoveryAmount, this.maxValue);
    }

    /// <summary>
    /// 最大HPの値を指定分だけ増加します。
    /// </summary>
    /// <param name="increasedValue">最大HPの増加量。</param>/
    public void IncreaseMaxHp(int increasedValue)
    {
        this.maxValue += increasedValue;
        this.currentValue += increasedValue;
    }

    /// <summary>
    /// HPの値を指定分だけ減らします。
    /// </summary>
    /// <param name="decreasedValue">HPの減少量。</param>/
    public void decreaseCurrentValue(int decreasedValue)
    {
        this.currentValue = Mathf.Max(this.currentValue - decreasedValue, 0);
    }

    /// <summary>
    /// HPが0か判定します。
    /// </summary>
    public bool isZero()
    {
        return this.currentValue == 0;
    }

}
