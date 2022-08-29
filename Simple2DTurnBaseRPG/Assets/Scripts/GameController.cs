using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] Battler enemyBattler;

    private void Start()
    {
        player.OnEncounts += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    public void StartBattle()
    {
        enemyBattler.Init();
        player.gameObject.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        battleSystem.BattleStart(player.Battler, enemyBattler);
    }

    public void EndBattle()
    {
        player.gameObject.SetActive(true);
        battleSystem.gameObject.SetActive(false);
    }

    void Update()
    {

    }
}
