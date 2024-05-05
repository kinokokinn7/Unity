using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exp(経験値)を表すクラス。
/// </summary>
[System.Serializable]
public class Exp
{
    /// <summary>
    /// 現在の経験値。
    /// </summary>
    [SerializeField]
    private int currentValue;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public Exp()
    {
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="initValue">経験値の初期値。</param>
    public Exp(int initValue)
    {
        this.currentValue = initValue;
    }

    /// <summary>
    /// 現在の経験値を取得します。
    /// </summary>
    /// <returns>現在の経験値。</returns>
    public int GetCurrentValue()
    {
        return this.currentValue;
    }

    /// <summary>
    /// 現在の経験値を設定します。
    /// </summary>
    /// <param name="value">現在の経験値。</param>
    public void SetCurrentValue(int value)
    {
        this.currentValue = value;
    }

    /// <summary>
    /// 経験値の値を指定分だけ増加します。
    /// </summary>
    /// <param name="increasedValue">経験値の増加量。</param>/
    public void IncreaseCurrentValue(int increasedValue)
    {
        this.currentValue += increasedValue;
    }

    /// <summary>
    /// 経験値を０にリセットします。
    /// </summary>
    public void Reset()
    {
        this.currentValue = 0;
    }

}
