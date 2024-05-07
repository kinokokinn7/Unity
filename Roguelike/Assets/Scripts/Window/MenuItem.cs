using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Roguelike.Window
{
    /// <summary>
    /// メニューの項目を表すクラスです。
    /// </summary>
    public class MenuItem : MonoBehaviour
    {
        public GameObject CursorObj;
        public Text TextObj;
        [SerializeField] bool _isSelecting;

        public enum Type
        {
            NextMenu,
            BackMenu,
            Event,
        }
        public Type CurrentType = Type.NextMenu;

        public MenuRoot MoveTargetObj;
        public UnityEvent Callbacks;

        public bool IsSelecting
        {
            get => _isSelecting;
            set
            {
                _isSelecting = value;
                CursorObj.SetActive(value);
            }
        }

        public string Text
        {
            get => TextObj.text;
            set
            {
                TextObj.text = value;
            }
        }

        private void Awake()
        {
            CursorObj.SetActive(IsSelecting);
        }
    }
}
