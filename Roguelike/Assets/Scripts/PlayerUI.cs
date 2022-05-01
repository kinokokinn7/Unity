using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class PlayerUI : MonoBehaviour
{
    [SerializeField] public Text LevelText;
    [SerializeField] public Text HpText;
    [SerializeField] public Text AttackText;
    [SerializeField] public Text ExpText;
    [SerializeField] public Text FoodText;
    [SerializeField] public Text WeaponText;
    [SerializeField] public Text FloorText;

    public Player Player { get; private set; }

    public void Set(Player player)
    {
        Player = player;
    }

    private void Update()
    {
        if (Player == null) return;
        LevelText.text = Player.Level.ToString();
        HpText.text = Player.Hp.ToString();
        AttackText.text = Player.Attack.ToString();
        ExpText.text = Player.Exp.ToString();
        FoodText.text = Player.Food.ToString();
        if (Player.CurrentWeapon != null)
        {
            WeaponText.text = Player.CurrentWeapon.ToString();
        }
        else
        {
            WeaponText.text = "";
        }
        FloorText.text = Player.Floor.ToString();
    }

}
