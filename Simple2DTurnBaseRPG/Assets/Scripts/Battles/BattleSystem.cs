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

    public UnityAction OnBattleOver;

    public void BattleStart()
    {
        state = State.Start;
        Debug.Log("バトル開始");
        actionSelectionUI.Init();
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
        throw new NotImplementedException();
    }

    private void HandleActionSelection()
    {
        actionSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (actionSelectionUI.SelectedIndex == 0)
            {
                Debug.Log("たたかう");
            }
            else if (actionSelectionUI.SelectedIndex == 1)
            {
                // にげる
                BattleOver();
            }
        }
    }
}
