using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike.Window
{
    /// <summary>
    /// コマンドカーソルの移動やスクロールを行うウィンドウです。
    /// </summary>
    public class Window_Selectable : Window_Base
    {

        /// <summary>
        /// カーソル位置。
        /// </summary>
        protected int _index;

        /// <summary>
        /// カーソル固定フラグ。
        /// </summary>
        protected bool _cursorFix;

        /// <summary>
        /// カーソル全選択フラグ。
        /// </summary>
        protected bool _cursorAll;

        /// <summary>
        /// 入力ハンドラ。
        /// </summary>
        protected Dictionary<string, Action> _handlers;

        /// <summary>
        /// ヘルプウィンドウ。
        /// </summary>
        private Window_Help _helpWindow;

        public void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void Initialize()
        {
            _index = -1;
            _handlers = new Dictionary<string, Action>();
            _helpWindow = null;
            _handlers = new Dictionary<string, Action>();
            // Deactive();
        }




    }
}


