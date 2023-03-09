using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20.0f;
    public float lifeTime = 2f;
    private float time;

    void Start()
    {
        time = lifeTime;
        // 2秒後に消す
        Destroy(gameObject, 2f);
    }

    void Update()
    {
    }

}
