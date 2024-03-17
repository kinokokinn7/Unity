using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Food
{
    /// <summary>
    /// 現在の満腹度。
    /// </summary>
    [SerializeField]
    private int currentValue;
    public int CurrentValue
    {
        get => currentValue;
        set => currentValue = Mathf.Min(value, this.MaxValue);
    }

    /// <summary>
    /// 最大満腹度。
    /// </summary>
    [SerializeField]
    private int maxValue;
    public int MaxValue
    {
        get => this.maxValue;
        set => this.maxValue = value;
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="initValue">満腹度の初期値。</param>
    public Food(int initValue)
    {
        this.MaxValue = initValue;
        this.CurrentValue = initValue;
    }

    /// <summary>
    /// 最大値を指定値だけ増加させます。
    /// </summary>
    /// <param name="value"></param>
    public void IncreaseMaxValue(int value)
    {
        this.MaxValue += value;
    }
}