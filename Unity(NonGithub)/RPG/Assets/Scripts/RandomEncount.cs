using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Random Encount")]
public class RandomEncount : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        [Range(0, 1)] public float EncountRate;
        public EnemyGroup EnemyGroup;
    }

    [Range(0, 1)] public float EncountRate = 0.2f;
    public List<Data> List;

    public EnemyGroup Encount(System.Random rnd)
    {
        if (EncountRate < rnd.NextDouble()) return null;
        foreach (var d in List)
        {
            var t = rnd.NextDouble();
            if (t < d.EncountRate)
            {
                return d.EnemyGroup;
            }
        }
        return null;
    }
}
