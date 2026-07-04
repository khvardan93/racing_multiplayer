using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class UITabBaseItem : MonoBehaviour
    {
        [SerializeField] protected Button _button;

        public bool Selected { get; protected set; }
        public event Action<UITabBaseItem> OnSelected;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public virtual void OnClick()
        {
            OnSelected?.Invoke(this);
        }
        
        public virtual void Select()
        {
            Selected = true;
        }
        
        public abstract void Unselect();
    }
}