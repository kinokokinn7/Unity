using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

    // 目標：ランダムエンカウント
    // ・敵にあったらバトルシーンに行く（Panelを出す）

    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask encountLayer;

    [SerializeField] Battler battler;
    public Battler Battler { get => battler; }

    public UnityAction OnEncounts;  // エンカウントした時に実行したい関数を登録できる

    Animator animator;
    bool isMoving;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        battler.Init();
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
            if (Random.Range(0, 100) < 100)   // 0-99までの数字がランダムに選ばれて、その数字が50より小さかったら
            {
                OnEncounts?.Invoke();   // もしOnEncountsに関数が登録されていれば実行する
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
