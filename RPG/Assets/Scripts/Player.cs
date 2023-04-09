using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    /// <summary>
    /// 初期戦闘パラメータ。
    /// </summary>
    public BattleParameter InitialBattleParameter;

    /// <summary>
    /// 現在の戦闘パラメータ。
    /// </summary>
    public BattleParameterBase BattleParameter;

    /// <summary>
    /// ゲーム起動直後に呼ばれ、下記処理を実行します。
    /// ・戦闘パラメータの初期化
    /// </summary>
    protected override void Start()
    {
        DoMoveCamera = true;
        base.Start();
        InitialBattleParameter.Data.CopyTo(BattleParameter);
    }
}