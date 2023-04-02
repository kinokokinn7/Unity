using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltWithVelocity : MonoBehaviour
{
    [Tooltip("宇宙船が最大速度時の角度。")]
    public int degrees = 30;
    public bool tiltTowards = true;

    private int prevDegrees = int.MaxValue;
    private float tan;

    Rigidbody rigidBody;

    /// <summary>
    /// 初期化処理を行います。
    /// </summary>
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// フレームごとに呼び出される更新処理を行います。
    /// </summary>
    void Update()
    {
        // Mathf.Tan()は少し処理が重いため、
        // 毎フレームのFixedUpdateメソッド内での呼び出しをする代わりに計算結果をキャッシュしておく
        if (degrees != prevDegrees)
        {
            prevDegrees = degrees;
            tan = Mathf.Tan(Mathf.Deg2Rad * degrees);
        }
        Vector3 pitchDir = (tiltTowards) ? -rigidBody.velocity : rigidBody.velocity;
        pitchDir += Vector3.forward / tan * PlayerShip.MAX_SPEED;

    }
}
