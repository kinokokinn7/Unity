using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPを表すクラス。
/// </summary>
public class Hp
{
    /// <summary>
    /// 現在のHP。
    /// </summary>
    private int currentValue;

    /// <summary>
    /// 最大HP。
    /// </summary>
    private int maxValue;

    /// <summary>
    /// 現在のHPを取得します。
    /// </summary>
    /// <returns>現在のHP。</returns>
    int current()
    {
        return this.currentValue;
    }

    /// <summary>
    /// 現在の最大HPを返します。
    /// </summary>
    /// <returns>現在の最大HP。</returns>
    int max()
    {
        return this.maxValue;
    }

    /// <summary>
    /// HPを回復します。
    /// </summary>
    /// <param name="recoveryAmount">回復量。</param>
    void recover(int recoveryAmount)
    {
        this.currentValue = Mathf.Min(this.currentValue + recoveryAmount, this.maxValue);
    }
}
