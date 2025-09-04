using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class TurretPointAtMouse : MonoBehaviour
{
    public Transform playerTransform { get; private set; }

    /// <summary>
    /// 初期化処理を行います。
    /// </summary>
    void Start()
    {
        playerTransform = transform.root.GetComponent<Transform>();
    }

    /// <summary>
    /// 毎フレームの更新処理を行います。
    /// </summary>
    void Update()
    {
        // プレイヤーの位置を取得する
        Vector3 playerPosition = playerTransform.position;
        // マウスポインタの位置を取得する
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(CrossPlatformInputManager.mousePosition
            + Vector3.back * Camera.main.transform.position.z);
        // プレイヤーの向きをマウスの位置に向ける
        Vector3 direction = new Vector3(
                mousePosition.x - playerTransform.position.x,
                mousePosition.y - transform.position.y,
                0
            );
        playerTransform.up = direction;

    }
}
