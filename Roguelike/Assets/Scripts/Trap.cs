using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内のトラップオブジェクトを表すクラスです。プレイヤーがこのトラップに触れると特定の効果が発生します。
/// </summary>
public class Trap : MapObjectBase
{
    /// <summary>
    /// トラップの種類を表します。LifeDownはHPを減少させ、FoodDownは満腹度を減少させます。
    /// </summary>
    public enum Type
    {
        LifeDown, // HP減少
        FoodDown, // 満腹度減少
    }

    public Type CurrentType = Type.LifeDown; // 現在のトラップの種類
    public int Value = 5; // トラップの効果の強さ
    public GameObject _effectPrefab; // エフェクトのプレハブ

    public bool Hide = true; // トラップがデフォルトで隠されているかどうか

    /// <summary>
    /// トラップが生成された際に呼び出されます。トラップが隠されているかどうかの初期設定を行います。
    /// </summary>
    void Start()
    {
        SetHide(Hide);
    }

    /// <summary>
    /// トラップの隠し状態を設定します。隠されている場合、トラップは見えなくなります。
    /// </summary>
    /// <param name="doHide">トラップを隠す場合はtrue、そうでない場合はfalse。</param>
    public void SetHide(bool doHide)
    {
        Hide = doHide;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(doHide);
        }
    }

    /// <summary>
    /// エフェクトを生成します。
    /// </summary>
    /// <param name="position">エフェクトの生成位置。</param>
    /// <returns>生成されたエフェクトのParticleSystem。</returns>
    internal ParticleSystem SpawnHealingEffect(Vector3 position)
    {
        if (!_effectPrefab)
        {
            return null;
        }
        //プレハブを指定した位置に生成
        var effectInstance = Instantiate(_effectPrefab, position, Quaternion.identity);
        return effectInstance.GetComponent<ParticleSystem>();

    }
}
