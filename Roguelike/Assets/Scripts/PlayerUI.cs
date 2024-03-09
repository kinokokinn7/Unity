using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのUIを管理するクラスです。プレイヤーのレベル、HP、攻撃力、経験値、満腹度、装備している武器、現在のフロアレベルを表示します。
/// </summary>
class PlayerUI : MonoBehaviour
{
    [SerializeField] public Text LevelText; // レベルを表示するテキスト
    [SerializeField] public Text HpText; // HPを表示するテキスト
    [SerializeField] public Text MaxHpText; // 最大HPを表示するテキスト
    [SerializeField] public Text AttackText; // 攻撃力を表示するテキスト
    [SerializeField] public Text ExpText; // 経験値を表示するテキスト
    [SerializeField] public Text FoodText; // 満腹度を表示するテキスト
    [SerializeField] public Text WeaponText; // 装備している武器を表示するテキスト
    [SerializeField] public Text FloorText; // 現在のフロアレベルを表示するテキスト


    public Player Player { get; private set; } // 現在のプレイヤーオブジェクトへの参照

    /// <summary>
    /// プレイヤーオブジェクトをセットします。
    /// </summary>
    /// <param name="player">セットするプレイヤーオブジェクト。</param>
    public void Set(Player player)
    {
        Player = player;
    }

    /// <summary>
    /// 毎フレームプレイヤーのステータスが更新されているかをチェックし、UIを更新します。
    /// </summary>
    private void Update()
    {
        if (Player == null) return; // プレイヤーオブジェクトがセットされていなければ何もしない
        LevelText.text = Player.Level.ToString(); // レベルを更新
        HpText.text = Player.Hp.ToString(); // HPを更新
        MaxHpText.text = Player.MaxHp.ToString(); // MaxHPを更新
        AttackText.text = Player.Attack.ToString(); // 攻撃力を更新
        ExpText.text = Player.Exp.ToString(); // 経験値を更新
        FoodText.text = Player.Food.ToString(); // 満腹度を更新
        WeaponText.text = Player.CurrentWeapon != null ? Player.CurrentWeapon.ToString() : ""; // 装備している武器を更新
        FloorText.text = Player.Floor.ToString(); // 現在のフロアレベルを更新
    }

}
