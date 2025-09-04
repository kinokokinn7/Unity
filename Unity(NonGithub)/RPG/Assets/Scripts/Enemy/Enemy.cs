using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の情報を表すクラス。
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Enemy")]
public class Enemy : ScriptableObject
{
    /// <summary>
    /// 敵のパラメータ。
    /// </summary>
    public BattleParameterBase Data;

    /// <summary>
    /// 敵の名前。
    /// </summary>
    public string Name;

    /// <summary>
    /// 敵の画像スプライト。
    /// </summary>
    public Sprite Sprite;

    /// <summary>
    /// 使用する敵AI。
    /// </summary>
    public EnemyAI EnemyAI;

    /// <summary>
    /// 敵を複製します。
    /// </summary>
    /// <returns></returns>
    public virtual Enemy Clone()
    {
        Enemy clone = ScriptableObject.CreateInstance<Enemy>();
        clone.Data = new BattleParameterBase();
        Data.CopyTo(clone.Data);
        clone.Name = Name;
        clone.Sprite = Sprite;
        clone.EnemyAI = EnemyAI.Clone();
        return clone;
    }

    public virtual TurnInfo BattleAction(BattleWindow battleWindow)
    {
        return EnemyAI.BattleAction(this, battleWindow);
    }
}
