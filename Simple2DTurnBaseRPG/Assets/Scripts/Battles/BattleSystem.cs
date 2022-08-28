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

    public UnityAction OnBattleOver;

    public void BattleStart()
    {
        state = State.Start;
        Debug.Log("バトル開始");
        actionSelectionUI.Init();
        moveSelectionUI.Init();
        actionSelectionUI.Close();
        StartCoroutine(SetupBattle());

    }

    IEnumerator SetupBattle()
    {
        yield return battleDialog.TypeDialog("XXがあらわれた！\nどうする？");
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
        Debug.Log("両者の攻撃処理");
        yield return battleDialog.TypeDialog("YYの攻撃！", auto: false);
        yield return battleDialog.TypeDialog("XXの攻撃！", auto: false);
        yield return battleDialog.TypeDialog("どうする？");
        ActionSelection();
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
