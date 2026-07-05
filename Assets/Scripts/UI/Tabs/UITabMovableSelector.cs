using DG.Tweening;
using UnityEngine;

namespace UI
{
    public enum UITabSelectorAxis
    {
        Horizontal,
        Vertical
    }

    public class UITabMovableSelector : MonoBehaviour
    {
        [SerializeField] private UITabsController _tabsController;
        [SerializeField] private RectTransform _image;
        [SerializeField] private UITabSelectorAxis _axis = UITabSelectorAxis.Horizontal;
        [SerializeField] private float _delay;
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField] private Vector2 _originalSize;

        private Tween _positionTween;
        private Tween _sizeTween;

        private void Awake()
        {
            _tabsController.OnChanged += MoveTo;
        }

        private void OnDestroy()
        {
            _tabsController.OnChanged -= MoveTo;
            _positionTween?.Kill();
            _sizeTween?.Kill();
        }

        private void OnEnable()
        {
            var containerRect = (RectTransform)_tabsController.transform;
            var corners = new Vector3[4];
            containerRect.GetWorldCorners(corners);

            // corners: 0 bottom-left, 1 top-left, 2 top-right, 3 bottom-right
            var edgeWorldPoint = _axis == UITabSelectorAxis.Horizontal ? corners[0] : corners[1];
            var edgeLocalPoint = _image.parent.InverseTransformPoint(edgeWorldPoint);

            var sizeDelta = _image.sizeDelta;
            var anchoredPosition = _image.anchoredPosition;

            if (_axis == UITabSelectorAxis.Horizontal)
            {
                sizeDelta.x = 0f;
                anchoredPosition.x = edgeLocalPoint.x;
            }
            else
            {
                sizeDelta.y = 0f;
                anchoredPosition.y = edgeLocalPoint.y;
            }

            _image.sizeDelta = sizeDelta;
            _image.anchoredPosition = anchoredPosition;

            Resize();
        }

        private void Resize()
        {
            _sizeTween?.Kill();
            _sizeTween = _image.DOSizeDelta(_originalSize, _duration)
                .SetDelay(_delay)
                .SetEase(_ease);
        }

        private void MoveTo(UITabBaseItem item)
        {
            if (!item) return;

            var target = (RectTransform)item.transform;

            _positionTween?.Kill();

            if (_axis == UITabSelectorAxis.Horizontal)
            {
                _positionTween = _image.DOAnchorPosX(target.anchoredPosition.x, _duration).SetEase(_ease);
            }
            else
            {
                _positionTween = _image.DOAnchorPosY(target.anchoredPosition.y, _duration).SetEase(_ease);
            }
        }
    }
}
