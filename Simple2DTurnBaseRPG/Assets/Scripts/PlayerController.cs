using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // 目標：ランダムエンカウント
    // ・歩き終わったときに必ず敵に会う（ログを出す）
    // ・...一定確率で
    // ・...特定の領域で一定確率で敵にあう:（Layerを使う）
    // ・敵にあったらバトルシーンに行く（Panelを出す）

    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask encountLayer;

    Animator animator;
    bool isMoving;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (x != 0)
            {
                y = 0;
            }

            if (x != 0 || y != 0)
            {
                animator.SetFloat("InputX", x);
                animator.SetFloat("InputY", y);
                StartCoroutine(Move(new Vector2(x, y)));
            }
        }
        animator.SetBool("IsMoving", isMoving);
    }

    // 1マス徐々に近づける
    IEnumerator Move(Vector3 direction)
    {
        isMoving = true;
        Vector3 targetPos = transform.position + direction;
        if (isWalkable(targetPos) == false)
        {
            isMoving = false;
            yield break;
        }

        // 現在とターゲットの位置が違う場合、近づけ続ける
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // 近づける
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime); // (現在地、目標地、速度)：目標地に近づける
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;

        // 敵にあうか調べる
        CheckForEncounts();
    }

    void CheckForEncounts()
    {
        // 移動した地点に、敵がいるか判断する
        if (Physics2D.OverlapCircle(transform.position, 0.2f, encountLayer))
        {
            if (Random.Range(0, 100) < 10)   // 0-99までの数字がランダムに選ばれて、その数字が50より小さかったら
            {
                Debug.Log("敵に遭遇");
            }
        }
    }


    // 今から移動するところに移動できるか判定する関数
    bool isWalkable(Vector3 targetPos)
    {
        // targetPosを中心に円形のRayを作る：SolidObjectsLayerにぶつかったらtrueが返ってくる
        // そのためfalseか否かで歩行可能か判定する
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == false;
    }
}