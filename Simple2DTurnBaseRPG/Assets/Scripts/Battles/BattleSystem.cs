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

    public UnityAction OnBattleOver;

    public void BattleStart()
    {
        state = State.Start;
        Debug.Log("バトル開始");
    }

    public void BattleOver()
    {
        OnBattleOver?.Invoke();
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
        throw new NotImplementedException();
    }
}
