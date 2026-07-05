using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UISwitch : MonoBehaviour,IPointerClickHandler
    {
        [Serializable]
        private class SwitchState
        {
            public Color Color = Color.white;
            public Color BgColor = Color.white;
            public bool UsePosition;
            public Vector2 AnchoredPosition;
        }

        [SerializeField] private Image _knobImage;
        [SerializeField] private Image _bgImage;
        [SerializeField] private SwitchState _offState;
        [SerializeField] private SwitchState _onState;

        [Header("Animation")]
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        [SerializeField] private bool _isOn;

        private RectTransform _knobRect;
        private Sequence _sequence;

        public bool IsOn => _isOn;
        public event Action<bool> OnValueChanged;

        private void Awake()
        {
            _knobRect = _knobImage.rectTransform;
        }

        private void OnEnable()
        {
            Apply(_isOn, false);
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }

        public void Toggle()
        {
            SetOn(!_isOn);
        }

        public void SetOn(bool isOn, bool animate = true)
        {
            if (_isOn == isOn)
            {
                return;
            }

            _isOn = isOn;
            Apply(_isOn, animate);
            OnValueChanged?.Invoke(_isOn);
        }

        public void Apply(bool isOn, bool animate)
        {
            var state = isOn ? _onState : _offState;

            _sequence?.Kill();

            if (!animate)
            {
                _knobImage.color = state.Color;
                _bgImage.color = state.BgColor;
                
                if (state.UsePosition)
                {
                    _knobRect.anchoredPosition = state.AnchoredPosition;
                }

                return;
            }

            _sequence = DOTween.Sequence()
                .Join(_knobImage.DOColor(state.Color, _duration))
                .Join(_bgImage.DOColor(state.BgColor, _duration))
                .SetEase(_ease)
                .SetTarget(this);

            if (state.UsePosition)
            {
                _sequence.Join(_knobRect.DOAnchorPos(state.AnchoredPosition, _duration));
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }
    }
}
