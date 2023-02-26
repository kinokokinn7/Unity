using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class TurretPointAtMouse : MonoBehaviour
{
    public Transform playerTransform { get; private set; }

    void Start()
    {
        playerTransform = transform.root.GetComponent<Transform>();
    }

    void Update()
    {
        // プレイヤーの位置を取得する
        Vector3 playerPosition = playerTransform.position;
        // マウスポインタの位置を取得する
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(CrossPlatformInputManager.mousePosition);
        // プレイヤーの向きをマウスの位置に向ける
        Vector3 direction = new Vector3(
                mousePosition.x - playerTransform.position.x,
                mousePosition.y - transform.position.y,
                0
            );
        playerTransform.up = direction;

    }
}
