using UnityEngine;

public class PhysicsDamageCalculator : IDamageCalculator
{
    /// <summary>
    /// ダメージの値を計算します。
    /// </summary>
    /// <param name="attacker">攻撃キャラ。</param>
    /// <param name="defender">攻撃対象キャラ。</param>
    /// <returns>ダメージの値。</returns>
    public int calculate(MapObjectBase attacker, MapObjectBase defender, bool isCriticalHit)
    {
        // クリティカルヒットの場合
        if (isCriticalHit)
        {
            // クリティカルヒットの場合、ダメージをほぼ攻撃力と等しい値にする
            return Mathf.FloorToInt(attacker.Attack.GetCurrentValue() * Random.Range(99f, 154f) / 128f);
        }
        // クリティカルヒットでない場合
        else
        {
            // 基本ダメージを計算
            // 攻撃力 - 防御力 / 2
            float baseDamage =
                (int)((float)attacker.Attack.GetCurrentValue()
                - (float)defender.Defence.GetCurrentValue() / 2f);

            // ただし、基本ダメージが2未満の場合は、1または0をランダムに返す
            if (baseDamage < 2f)
            {
                return Random.value > 0.5f ? 1 : 0;
            }
            // 基本ダメージが2以上の場合は、ランダムな値を返す
            else
            {
                return Mathf.FloorToInt(baseDamage * Random.Range(99f, 154f) / 256f);
            }
        }
    }
}
