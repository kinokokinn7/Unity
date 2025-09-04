using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using static UnityEditor.FilePathAttribute;

public class AsteraX : MonoBehaviour
{
    [Header("Set in Inspactor")]
    public GameObject Asteroid_A_Prefab;
    public GameObject Asteroid_B_Prefab;
    public GameObject Asteroid_C_Prefab;
    public float AsteroidSpawnInitTime_sec;     // 最初の小惑星が生成されるまでの時間（ミリ秒）
    public float AsteroidSpawnRepeatInterval_sec;   // 小惑星が生成される時間間隔（ミリ秒）

    // Use this for initialization
    void Start()
    {
        // 一定時間ごとにAsteroidを生成する
        InvokeRepeating("generateAsteroid", AsteroidSpawnInitTime_sec, AsteroidSpawnRepeatInterval_sec);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Asteroid（小惑星）を生成します。
    /// </summary>
    private void generateAsteroid()
    {
        GameObject asteroidA = Instantiate<GameObject>(
            Asteroid_A_Prefab,
            Vector3.zero,
            Quaternion.identity
        );
        List<GameObject> asteroidBList = new List<GameObject>();
        List<GameObject> asteroidCList = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject asteroidB = Instantiate<GameObject>(
                Asteroid_B_Prefab
            );
            asteroidB.transform.parent = asteroidA.transform;
            asteroidB.transform.position = asteroidB.transform.parent.position;
            asteroidB.transform.rotation = asteroidB.transform.parent.rotation;
            asteroidBList.Add(asteroidB);

            for (int j = 0; j < 2; j++)
            {
                GameObject asteroidC = Instantiate<GameObject>(
                    Asteroid_C_Prefab
                );
                asteroidC.transform.parent = asteroidB.transform;
                asteroidC.transform.position = asteroidC.transform.parent.position;
                asteroidC.transform.rotation = asteroidC.transform.parent.rotation;
                asteroidCList.Add(asteroidC);
            }
        }
    }
}

