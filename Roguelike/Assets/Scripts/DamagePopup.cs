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

    /// <summary>
    /// 跳ねる高さ。
    /// </summary>
    public float amplitude = 1f;

    /// <summary>
    /// 跳ねる速さ
    /// </summary>
    public float frequency = 2f;

    public void ShowDamage(int damageAmount, Vector3 worldPosition)
    {
        Canvas canvas = GameObject.Find("PopupCanvas").GetComponent<Canvas>();
        if (canvas != null)
        {
            // ダメージポップアップのインスタンスを生成
            GameObject popup = UnityEngine.Object.Instantiate(DamagePopupPrefab, canvas.transform, false);

            // ワールド座標からスクリーン座標への変換
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);

            // スクリーン空間からCanvas内のアンカーポイント基準の位置への変換
            Vector2 anchoredPosition;
            bool isConverted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPoint,
                null,
                out anchoredPosition
            );

            // 変換に成功した時のみ、ポップアップのアンカーポイント基準の位置を設定
            if (!isConverted)
            {
                Debug.LogError("スクリーンポイントをローカルポイントに変換できませんでした。");
                return;
            }

            RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
            popupRectTransform.anchoredPosition = anchoredPosition;

            TextMeshProUGUI textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();

            // ダメージ値を設定
            textMesh.text = damageAmount.ToString();

            // ポップアップを少し上に移動させる
            textMesh.transform.localPosition += new Vector3(0, 100f, 0);

            // アニメーションやフェードアウトのためのコルーチンを開始
            StartCoroutine(AnimatePopup(popup));

        }
        else
        {
            Debug.LogError("PopupCanvasが見つかりませんでした。");
        }

    }

    private IEnumerator AnimatePopup(GameObject popup)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // 時間経過とともにポップアップを跳ねさせる
            // 現在の時間と開始時間の差分
            float timeSinceStart = (Time.time - startTime) / duration;
            // sin関数を使って跳ねる動きを計算
            float sinWave = Mathf.Sin(timeSinceStart * Mathf.PI * frequency) * amplitude;
            // Yの位置をsin波に基づいて調整
            TextMeshProUGUI textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.transform.localPosition = new Vector3(
                textMesh.transform.localPosition.x,
                textMesh.transform.localPosition.y + sinWave,
                textMesh.transform.localPosition.z
            );

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
