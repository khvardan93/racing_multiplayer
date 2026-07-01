using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform _progressBar;
    [SerializeField] private float _maxLength;
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _ease = Ease.OutCubic;
    [SerializeField] private TMP_Text _label;

    private float _displayedValue;
    private Tween _progressTween;
    private Tween _labelTween;

    public void ForceSet(float value)
    {
        _labelTween?.Kill();
        _progressTween?.Kill();
        
        _displayedValue = value;
        _progressBar.sizeDelta = new Vector2(_maxLength * value, _progressBar.sizeDelta.y);
        
        SetLabel(value);
    }

    public void AnimatedSet(float value)
    {
        _labelTween?.Kill();
        _progressTween?.Kill();

        _progressTween = _progressBar
            .DOSizeDelta(new Vector2(_maxLength * value, _progressBar.sizeDelta.y), _animationDuration)
            .SetEase(_ease);

        if (!_label) return;
        
        _labelTween = DOTween.To(() => _displayedValue, x =>
            {
                _displayedValue = x;
                SetLabel(x);
            }, value, _animationDuration)
            .SetEase(_ease);
    }

    private void SetLabel(float value)
    {
        if (!_label) return;
        _label.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
