using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    /// <summary>
    /// メニューを開いているか否かを表すフラグ。
    /// true: メニューが開かれている状態
    /// </summary>
    public bool DoOpen { get => gameObject.activeSelf; }

    /// <summary>
    /// メニューを開く。
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 初期状態ではメニューを閉じる。
    /// </summary>
    private void Awake()
    {
        gameObject.SetActive(false);
    }

}
