using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : BattleUnit
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;


    public override void Setup(Battler battler)
    {
        base.Setup(battler);
        // Player:ステータスの設定
        nameText.text = battler.Base.Name;
        levelText.text = $"Lv:{battler.Level}";
        hpText.text = $"HP:{battler.HP}";
        atText.text = $"AT:{battler.AT}";

    }

    public override void UpdateUI()
    {
        levelText.text = $"Lv:{Battler.Level}";
        hpText.text = $"HP:{Battler.HP}";
        atText.text = $"AT:{Battler.AT}";
    }
}
