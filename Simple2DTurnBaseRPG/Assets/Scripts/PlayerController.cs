using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // 目標：playerのAnimationをコードから制御する
    // InputX
    // InputY
    // IsMoving

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

    IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        Vector3 targetPos = transform.position + direction;
        // 現在とターゲットの位置が違う場合、近づけ続ける
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // 近づける
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime); // (現在地、目標地、速度)：目標地に近づける
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }
}
