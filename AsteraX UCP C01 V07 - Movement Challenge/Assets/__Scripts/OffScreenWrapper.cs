using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenWrapper : MonoBehaviour
{
    private GameObject screenBounds;
    private BoxCollider screenBoundsCollider;

    private void Start()
    {
        this.screenBounds = GameObject.FindGameObjectWithTag("OnScreenBounds");
        this.screenBoundsCollider = screenBounds.GetComponent<BoxCollider>();
    }

    /// <summary>
    /// スクリーンエリア外にはみ出した場合は
    /// スクリーンの左右反対側にラップします。
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OnScreenBounds"))
        {
            // ラップ後の位置を設定
            Vector3 screenBoundsSize = this.screenBoundsCollider.size;

            if (Mathf.Abs(transform.position.x) >= screenBoundsSize.x / 2)
            {
                Vector3 wrappedPos = new Vector3(
                    -transform.position.x,
                    transform.position.y,
                    0
                );
                transform.position = wrappedPos;
            }

            if (Mathf.Abs(transform.position.y) >= screenBoundsSize.y / 2)
            {
                Vector3 wrappedPos = new Vector3(
                    transform.position.x,
                    -transform.position.y,
                    0
                );
                transform.position = wrappedPos;
            }
        }
    }
}
