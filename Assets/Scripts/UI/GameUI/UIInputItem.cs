using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIInputItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent<bool> _onPointed;

        public event Action<bool> OnPointed;
        
        private bool _state;
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            SetState(true);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            SetState(false);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if(!_state) return;
            
            SetState(false);
        }

        private void SetState(bool state)
        {
            _state = state;
            _onPointed?.Invoke(state);
            OnPointed?.Invoke(state);
        }
    }
}