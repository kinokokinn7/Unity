using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] BattleSystem battleSystem;

    private void Start()
    {
        player.OnEncounts += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    public void StartBattle()
    {
        player.gameObject.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        battleSystem.BattleStart();
    }

    public void EndBattle()
    {
        player.gameObject.SetActive(true);
        battleSystem.gameObject.SetActive(false);
        battleSystem.BattleStart();
    }

    void Update()
    {

    }
}
