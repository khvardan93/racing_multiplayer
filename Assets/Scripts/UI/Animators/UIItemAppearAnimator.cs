using System;
using DG.Tweening;
using UI;
using UnityEngine;

public class UIItemAppearAnimator : UIBaseItem
{
    [Header("Appear animation")]
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _delay = 0f;
    [SerializeField] private Ease _ease = Ease.OutBack;
    [SerializeField] private bool _playOnEnable = true;

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
            .OnComplete(() => onDone?.Invoke())
            .SetTarget(this);
    }

    public override void Reset()
    {
        _tween?.Kill();
        _target.localScale = Vector3.one;
    }
}
