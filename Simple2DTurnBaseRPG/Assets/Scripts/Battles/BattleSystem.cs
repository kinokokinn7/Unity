using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleSystem : MonoBehaviour
{

    enum State
    {
        Start,
        ActionSelection,
        MoveSelection,
        RunTurns,
        BattleOver,

    }

    State state;

    [SerializeField] ActionSelectionUI actionSelectionUI;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] BattleDialog battleDialog;

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;


    public UnityAction OnBattleOver;

    public void BattleStart(Battler player, Battler enemy)
    {
        state = State.Start;
        Debug.Log("バトル開始");
        actionSelectionUI.Init();
        moveSelectionUI.Init(player.Moves);
        actionSelectionUI.Close();
        StartCoroutine(SetupBattle(player, enemy));

    }

    IEnumerator SetupBattle(Battler player, Battler enemy)
    {
        playerUnit.Setup(player);
        enemyUnit.Setup(enemy);

        yield return battleDialog.TypeDialog($"{enemy.Base.Name}があらわれた！\nどうする？");
        ActionSelection();
    }

    void BattleOver()
    {
        OnBattleOver?.Invoke();
    }

    void ActionSelection()
    {
        state = State.ActionSelection;
        actionSelectionUI.Open();
    }

    void MoveSelection()
    {
        state = State.MoveSelection;
        moveSelectionUI.Open();
    }

    IEnumerator RunTurns()
    {
        state = State.RunTurns;
        Move playerMove = playerUnit.Battler.Moves[moveSelectionUI.SelectedIndex];
        yield return RunMove(playerMove, playerUnit, enemyUnit);
        if (state == State.BattleOver)
        {
            yield return battleDialog.TypeDialog($"{enemyUnit.Battler.Base.Name}を倒した！");
            BattleOver();
            yield break;
        }

        Move enemyMove = enemyUnit.Battler.GetRandomMove();
        yield return RunMove(enemyMove, enemyUnit, playerUnit);
        if (state == State.BattleOver)
        {
            yield return battleDialog.TypeDialog($"{playerUnit.Battler.Base.Name}は倒れてしまった。");
            BattleOver();
            yield break;
        }

        yield return battleDialog.TypeDialog("どうする？");
        ActionSelection();
    }

    IEnumerator RunMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        int damage = targetUnit.Battler.TakeDamage(move, sourceUnit.Battler);
        yield return battleDialog.TypeDialog($"{sourceUnit.Battler.Base.Name}の{move.Base.Name}！\n" +
            $"{targetUnit.Battler.Base.Name}は{damage}のダメージ！", auto: false);
        targetUnit.UpdateUI();

        if (targetUnit.Battler.HP <= 0)
        {
            state = State.BattleOver;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Start:
                break;
            case State.ActionSelection:
                HandleActionSelection();
                break;
            case State.MoveSelection:
                HandleMoveSelection();
                break;
            case State.BattleOver:
                break;
        }
    }

    private void HandleMoveSelection()
    {
        moveSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 技の実行をする
            actionSelectionUI.Close();
            moveSelectionUI.Close();
            StartCoroutine(RunTurns());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            moveSelectionUI.Close();
            ActionSelection();
        }
    }

    private void HandleActionSelection()
    {
        actionSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (actionSelectionUI.SelectedIndex == 0)
            {
                // たたかう
                MoveSelection();
            }
            else if (actionSelectionUI.SelectedIndex == 1)
            {
                // にげる
                BattleOver();
            }
        }
    }
}
