using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleSystem : MonoBehaviour
{
    public UnityAction OnBattleOver;

    public void BattleStart()
    {
        Debug.Log("バトル開始");
    }

    public void BattleOver()
    {
        OnBattleOver?.Invoke();
    }

    private void Update()
    {
        // テスト
        if (Input.GetKeyDown(KeyCode.R))
        {
            BattleOver();
        }
    }
}
