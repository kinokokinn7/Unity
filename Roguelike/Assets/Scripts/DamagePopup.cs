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

    public void ShowDamage(int damageAmount)
    {
        // ダメージポップアップのインスタンスを生成
        GameObject popup = UnityEngine.Object.Instantiate(DamagePopupPrefab, transform.position, Quaternion.identity, transform);
        TextMeshProUGUI textMesh = popup.GetComponentInChildren<TextMeshProUGUI>();

        // ダメージ値を設定
        textMesh.text = damageAmount.ToString();

        // ポップアップを少し上に移動させる
        // これはCanvas内のオブジェクトの場合、RectTransformを使って調整する必要がある
        RectTransform rectTransform = popup.GetComponent<RectTransform>();
        rectTransform.anchoredPosition += new Vector2(0, 50);

        // アニメーションやフェードアウトのためのコルーチンを開始
        StartCoroutine(AnimatePopup(popup));
    }

    private IEnumerator AnimatePopup(GameObject popup)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // 時間経過とともにポップアップを上に移動させる
            RectTransform rectTransform = popup.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += new Vector2(0, Time.deltaTime);
            yield return null;
        }

        // ポップアップを破棄
        Destroy(popup);



        // 一定時間表示
        yield return new WaitForSeconds(this.duration);
        // 表示を終了
        UnityEngine.Object.Destroy(popup);
    }
}
