using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 攻撃を受けた時のダメージ値をポップアップ表示するクラスです。
/// </summary>
public class DamagePopup : MonoBehaviour
{
    /// <summary>
    /// TextMeshProのプレハブ。（インスペクタからアサインする）
    /// </summary>
    public GameObject DamagePopupPrefab;

    /// <summary>
    /// ダメージ値の表示時間。
    /// </summary>
    public int duration = 1;

    public void ShowDamage(int damageAmount, Vector3 position)
    {
        // ダメージポップアップのインスタンスを生成
        GameObject popup = UnityEngine.Object.Instantiate(DamagePopupPrefab, position, Quaternion.identity, transform);
        TextMeshProUGUI textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();

        // ダメージ値を設定
        textMesh.text = damageAmount.ToString();

        // ポップアップを少し上に移動させる
        textMesh.transform.localPosition += new Vector3(0, 100f, 0);

        // アニメーションやフェードアウトのためのコルーチンを開始
        StartCoroutine(AnimatePopup(popup));
    }

    private IEnumerator AnimatePopup(GameObject popup)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // 時間経過とともにポップアップを跳ねさせる
            const float amplitude = 100f; // 跳ねる高さ
            const float frequency = 1f; // 跳ねる速さ

            // 現在の時間と開始時間の差分
            float timeSinceStart = (Time.time - startTime) / duration;
            // sin関数を使って跳ねる動きを計算
            float sinWave = Mathf.Sin(timeSinceStart * Mathf.PI * frequency) * amplitude;
            // Yの位置をsin波に基づいて調整
            TextMeshProUGUI textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.transform.localPosition = new Vector3(0, sinWave, 0);

            // ポップアップを徐々にフェードアウトさせる
            Color textColor = textMesh.color;
            textColor.a = Mathf.Lerp(1, 0, (Time.time - startTime) / duration);
            textMesh.color = textColor;

            yield return null;
        }

        // ポップアップを破棄
        Destroy(popup);
    }
}
