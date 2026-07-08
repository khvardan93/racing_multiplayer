using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UIInputAnimator : MonoBehaviour
    {
        [SerializeField] private float _pressedScale = 1.1f;
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private Vector3 _originalScale;
        private Tween _tween;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        public void Animate(bool state)
        {
            _tween?.Kill();

            var targetScale = state ? _originalScale * _pressedScale : _originalScale;
            _tween = transform.DOScale(targetScale, _duration).SetEase(_ease).SetTarget(this);
        }
    }
}