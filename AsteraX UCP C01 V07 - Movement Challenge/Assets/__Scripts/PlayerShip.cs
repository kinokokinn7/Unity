using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShip : MonoBehaviour
{

    public float speed = 20.0f;
    public float rotationSpeed = 100.0f;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 20f;

    void Update()
    {
        Move();
        if (Input.GetMouseButtonDown(0))
        {
            Fire(transform.up);
        }
    }

    /// <summary>
    /// キー入力に合わせてプレイヤーの宇宙船を移動します。
    /// モバイル版の場合は、デバイスの傾きにより宇宙船を移動します。
    /// </summary>
    private void Move()
    {
        float dx = CrossPlatformInputManager.GetAxis("Horizontal");
        float dy = CrossPlatformInputManager.GetAxis("Vertical");
        Vector3 diff = new Vector3(
            dx * Time.deltaTime * speed,
            dy * Time.deltaTime * speed,
            0);
        transform.Translate(diff, Space.World);
    }

    /// <summary>
    /// 砲弾を生成して主塔から発射します。
    /// </summary>
    private void Fire(Vector3 direction)
    {
        GameObject bulletAnchor = GameObject.Find("BulletAnchor");

        // 砲弾をまとめるオブジェクトを生成（存在しない場合のみ）
        if (bulletAnchor == null)
        {
            bulletAnchor = new GameObject("BulletAnchor");
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Bulletの初期設定
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        float bulletSpeed = bulletScript.speed;
        bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

        // BulletAnchorにBulletを追加
        bullet.transform.parent = bulletAnchor.transform;
    }
}
