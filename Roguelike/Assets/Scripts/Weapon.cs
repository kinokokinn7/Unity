using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
class Weapon : ScriptableObject
{
    public string Name = "";
    public int Attack = 1;

    /// <summary>
    /// 装備する
    /// </summary>
    /// <param name="obj"></param>
    public void Attach(MapObjectBase obj)
    {
        obj.Attack += Attack;
    }

    /// <summary>
    /// 装備を外す
    /// </summary>
    /// <param name="obj"></param>
    public void Detach(MapObjectBase obj)
    {
        obj.Attack -= Attack;
    }

    /// <summary>
    /// 合成
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Weapon Merge(Weapon other)
    {
        var newWeapon = ScriptableObject.CreateInstance<Weapon>();
        newWeapon.Name = Name;
        newWeapon.Attack = Attack;
        if (other != null) newWeapon.Attack += other.Attack;
        return newWeapon;

    }

    /// <summary>
    /// 画面上部（ステータスバー）に表示する文字列
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Name} Atk+{Attack}";
    }


}
