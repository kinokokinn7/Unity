using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike.Window
{
    /// <summary>
    /// 項目が選択された際の解説などを表示するウィンドウです。
    /// </summary>
    public class Window_Help : Window_Base
    {
        /// <summary>
        /// todo: 実装
        /// 表示される文。
        /// </summary>
        private string _text;
        [SerializeField] protected TextMeshProUGUI textField;

        protected override void Initialize()
        {

        }
    }
}


