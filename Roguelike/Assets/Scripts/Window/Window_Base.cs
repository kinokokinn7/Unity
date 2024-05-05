using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike.Window
{

    /// <summary>
    /// ベースとなるウィンドウのクラスです。
    /// 新しいメッセージの追加や、メッセージの表示数を制限する機能を提供します。
    /// </summary>
    public abstract class Window_Base : MonoBehaviour
    {

        /// <summary>
        /// 表示状態。
        /// </summary>
        private bool _visible;

        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
            }
        }

        protected abstract void Initialize();

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public virtual void Show()
        {
            Visible = true;
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public virtual void Hide()
        {
            Visible = false;
        }


    }
}