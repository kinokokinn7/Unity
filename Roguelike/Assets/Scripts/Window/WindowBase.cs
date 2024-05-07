using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike.Window
{
    public class WindowBase : MonoBehaviour
    {
        /// <summary>
        /// ウィンドウのアクティブ状態。
        /// </summary>
        public virtual bool isActive
        {
            get => gameObject.activeSelf;
        }

        /// <summary>
        /// ウィンドウをアクティブにして表示します。
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// ウィンドウを非アクティブにして非表示にします。
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

