using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MassEvent : ScriptableObject
{
    public TileBase Tile;

    /// <summary>
    /// マスのイベントを実行します。
    /// </summary>
    /// <param name="manager">RPGシーンマネージャー</param>
    public virtual void Exec(RPGSceneManager manager) { }
}