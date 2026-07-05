using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UIItemAppearAnimator : UIBaseItem
    {
        [Header("Appear animation")] [SerializeField]
        private float _duration = 0.3f;

        [SerializeField] private float _delay;
        [SerializeField] private Ease _ease = Ease.OutBack;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _animateHide;

        private Transform _target;
        private Vector3 _targetScale;
        private Tween _tween;

        private void Awake()
        {
            _target = transform;
            _targetScale = _target.localScale;
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

            _tween?.Kill();
            _target.localScale = Vector3.zero;

            _tween = _target.DOScale(_targetScale, _duration)
                .SetDelay(_delay)
                .SetEase(_ease)
                .OnComplete(() => { onDone?.Invoke(); })
                .SetTarget(this);
        }

        public override void Hide()
        {
            if(!_target || !_animateHide)
            {
                base.Hide();
                return;
            }
            
            _tween?.Kill();
            _target.localScale = Vector3.one;

            _tween = _target.DOScale(Vector3.zero, _duration)
                .SetDelay(_delay)
                .SetEase(_ease)
                .OnComplete(() => { base.Hide(); })
                .SetTarget(this);
        }

        public override void Reset()
        {
            _tween?.Kill();
            _target.localScale = Vector3.one;
        }
    }
}