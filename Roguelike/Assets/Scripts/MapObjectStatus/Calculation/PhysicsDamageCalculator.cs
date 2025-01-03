using UnityEngine;

public class PhysicsDamageCalculator : IDamageCalculator
{
    /// <summary>
    /// ダメージの値を計算します。
    /// </summary>
    /// <param name="attacker">攻撃キャラ。</param>
    /// <param name="defender">攻撃対象キャラ。</param>
    /// <returns>ダメージの値。</returns>
    public int calculate(MapObjectBase attacker, MapObjectBase defender)
    {
        float baseDamage =
            (int)((float)attacker.Attack.GetCurrentValue()
            - (float)defender.Defence.GetCurrentValue() / 2f);
        if (baseDamage < 2f)
        {
            return Random.value > 0.5f ? 1 : 0;
        }
        else
        {
            return Mathf.FloorToInt(baseDamage * Random.Range(99f, 154f) / 256f);
        }
    }
}
