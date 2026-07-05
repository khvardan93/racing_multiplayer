using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace UI
{
    public class UISlider : Slider
    {
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _format = "{0}";

        private Tween _tween;
        private float _cachedValue;
        private float _realValue;
        
        public event Action<float> OnRealValueChanged;
        
        public float RealValue
        {
            set
            {
                if (_realValue == value || value > maxValue || value < minValue) return;
                
                _realValue = value;
                OnRealValueChanged?.Invoke(value);
            }
            get => _realValue;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            value = 0;

            SetValueAnimated();
            onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            if(!_text) return;
            _text.text = string.Format(_format, (int)value);
        }

        private void SetValueAnimated(TweenCallback onComplete = null)
        {
            _tween?.Kill();
            _tween = this.DOValue(_realValue, _duration)
                .SetEase(_ease)
                .OnComplete(onComplete)
                .SetTarget(this);
        }

        protected override void OnDisable()
        {
            onValueChanged.RemoveListener(OnValueChanged);
            _tween?.Kill();
            base.OnDisable();
        }
    }
}
