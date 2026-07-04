using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UIFallingAnimator : MonoBehaviour
    {
        [Header("Fall animation")]
        [SerializeField] private float _riseHeight = 600f;
        [SerializeField] private float _fallDuration = 0.4f;
        [SerializeField] private float _bounceHeight = 20f;
        [SerializeField] private float _bounceDuration = 0.15f;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private Ease _fallEase = Ease.InQuad;
        [SerializeField] private Ease _bounceEase = Ease.OutQuad;
        [SerializeField] private bool _playOnEnable = true;

        [Header("Content")]
        [SerializeField] private RectTransform _rectTransform;

        private Vector2 _originalPosition;
        private Sequence _sequence;

        private void Awake()
        {
            _originalPosition = _rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }

        public void Play()
        {
            _sequence?.Kill();

            _rectTransform.anchoredPosition = _originalPosition + new Vector2(0, _riseHeight);

            _sequence = DOTween.Sequence()
                .AppendInterval(_delay)
                .Append(_rectTransform.DOAnchorPosY(_originalPosition.y, _fallDuration).SetEase(_fallEase))
                .Append(_rectTransform.DOAnchorPosY(_originalPosition.y + _bounceHeight, _bounceDuration).SetEase(_bounceEase))
                .Append(_rectTransform.DOAnchorPosY(_originalPosition.y, _bounceDuration).SetEase(_fallEase))
                .SetTarget(this);
        }
    }
}
