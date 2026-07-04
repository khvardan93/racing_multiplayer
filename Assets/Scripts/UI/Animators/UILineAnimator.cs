using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILineAnimator : UIBaseItem
    {
        private enum GrowAnchor
        {
            Middle,
            Left,
            Right,
            Up,
            Down
        }

        [Header("Appear animation")]
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private GrowAnchor _growAnchor = GrowAnchor.Middle;
        [SerializeField] private Ease _ease = Ease.OutBack;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _fade = true;

        [Header("Content")]
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;

        private float _targetOpacity;
        private Sequence _sequence;
        private Vector2 _targetSize;

        private void Awake()
        {
            _targetSize = _rectTransform.sizeDelta;
            _targetOpacity = _image.color.a;
        }

        private static Vector2 GetPivot(GrowAnchor anchor)
        {
            return anchor switch
            {
                GrowAnchor.Left => new Vector2(0f, 0.5f),
                GrowAnchor.Right => new Vector2(1f, 0.5f),
                GrowAnchor.Up => new Vector2(0.5f, 1f),
                GrowAnchor.Down => new Vector2(0.5f, 0f),
                _ => new Vector2(0.5f, 0.5f)
            };
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
            _sequence?.Kill();
        }

        public void Show()
        {
            _sequence?.Kill();

            //_rectTransform.pivot = GetPivot(_growAnchor);

            if(_fade)
            {
                var trColor = _image.color;
                trColor.a = 0f;
                _image.color = trColor;
            }
            _rectTransform.sizeDelta = Vector2.zero;

            _sequence = DOTween.Sequence()
                .AppendInterval(_delay)
                .Append(_image.DOFade(_targetOpacity, _duration))
                .Join(_rectTransform.DOSizeDelta(_targetSize, _duration).SetEase(_ease))
                .SetTarget(this);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
