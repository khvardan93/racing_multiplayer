using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UINumericTextAnimator : UIBaseItem
    {
        [Header("Count animation")]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _format = "{0}";
        [SerializeField] private float[] _targetValues = { 100f };
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _delay;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField] private bool _wholeNumbers = true;
        [SerializeField] private bool _playOnEnable = true;

        private float[] _currentValues;
        private Tween _tween;
        private object[] _formatArgs;

        private void Awake()
        {
            _currentValues = new float[_targetValues.Length];
            _formatArgs = new object[_targetValues.Length];
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Show();
            }
        }

        private void OnDisable()
        {
            Reset();
        }

        public override void Show(Action onDone = null)
        {
            base.Show(onDone);
            Play(_targetValues, onDone);
        }

        public void Play(float[] targetValues, Action onDone = null)
        {
            _targetValues = targetValues;

            if (_currentValues == null || _currentValues.Length != _targetValues.Length)
            {
                _currentValues = new float[_targetValues.Length];
                _formatArgs = new object[_targetValues.Length];
            }

            _tween?.Kill();
            Array.Clear(_currentValues, 0, _currentValues.Length);
            UpdateText();

            _tween = DOVirtual.Float(0f, 1f, _duration, SetProgress)
                .SetDelay(_delay)
                .SetEase(_ease)
                .OnComplete(() => onDone?.Invoke())
                .SetTarget(this);
        }

        private void SetProgress(float progress)
        {
            for (int i = 0; i < _targetValues.Length; i++)
            {
                _currentValues[i] = _targetValues[i] * progress;
            }

            UpdateText();
        }

        private void UpdateText()
        {
            for (int i = 0; i < _currentValues.Length; i++)
            {
                _formatArgs[i] = _wholeNumbers ? Mathf.RoundToInt(_currentValues[i]) : _currentValues[i];
            }

            _text.text = string.Format(_format, _formatArgs);
        }

        public override void Reset()
        {
            _tween?.Kill();

            for (int i = 0; i < _currentValues.Length; i++)
            {
                _currentValues[i] = _targetValues[i];
            }

            UpdateText();
        }
    }
}
