using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountArea : MonoBehaviour
{
    // 複数の敵をセット
    [SerializeField] List<Battler> enemies;

    // ランダムに1体渡す
    public Battler GetRandomBattler()
    {
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
    }
}
