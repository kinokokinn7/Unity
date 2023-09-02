using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦闘用の敵グループの情報を表すクラス。
/// </summary>
[CreateAssetMenu(menuName = "EncounterEnemy")]
public class EnemyGroup : ScriptableObject
{
    /// <summary>
    /// 「にげる」の成功率。
    /// </summary>
    [Range(0, 1)] public float EscapeSuccessRate = 0.7f;

    /// <summary>
    /// 敵グループに含まれる敵のリスト。
    /// </summary>
    public List<Enemy> Enemies;

    /// <summary>
    /// 敵グループを複製します。
    /// </summary>
    /// <returns></returns>
    public EnemyGroup Clone()
    {
        EnemyGroup clone = ScriptableObject.CreateInstance<EnemyGroup>();
        clone.EscapeSuccessRate = EscapeSuccessRate;
        clone.Enemies = new List<Enemy>(Enemies.Count);
        foreach (Enemy e in Enemies)
        {
            clone.Enemies.Add(e.Clone());
        }
        return clone;
    }
}
