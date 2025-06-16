using UnityEngine;

/// <summary>
/// ダメージ計算を行うインタフェース。
/// </summary>
public interface IDamageCalculator
{
    /// <summary>
    /// ダメージの値を計算します。
    /// </summary>
    /// <param name="attacker">攻撃キャラ。</param>
    /// <param name="defender">攻撃対象キャラ。</param>
    /// <param name="isCriticalHit">クリティカルヒットかどうか。</param>
    /// <returns>ダメージの値。</returns>
    public int calculate(MapObjectBase attacker, MapObjectBase defender, bool isCriticalHit);
}
