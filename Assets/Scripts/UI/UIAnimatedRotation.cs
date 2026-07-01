using DG.Tweening;
using UnityEngine;

public class UIAnimatedRotation : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
    [SerializeField] private bool _clockwise = true;

    private Tween _rotationTween;

    private void OnEnable()
    {
        var angles = _rotationAxis * (_clockwise ? -360f : 360f);
        _rotationTween = transform
            .DOLocalRotate(angles, _duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        _rotationTween?.Kill();
    }
}
