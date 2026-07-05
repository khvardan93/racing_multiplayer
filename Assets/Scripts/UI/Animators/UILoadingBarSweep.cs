using UnityEngine;

namespace UI
{
    /// <summary>
    /// Drives the indeterminate loading bar: sweeps a runner segment
    /// across the track on a loop. No dependencies, no allocations.
    ///
    /// Setup:
    ///   Track (Image, dark)  + RectMask2D
    ///     └─ Runner (Image, cyan)  ← assign as "runner"
    /// Runner should be anchored to the LEFT edge of the track
    /// (anchor min/max = (0, 0.5), pivot = (0, 0.5)).
    /// </summary>
    public sealed class UILoadingBarSweep : MonoBehaviour
    {
        [SerializeField] private RectTransform _runner;

        [Tooltip("Full sweep cycle duration in seconds.")]
        [SerializeField, Min(0.1f)] private float _cycleDuration = 1.2f;

        [Tooltip("Easing applied within each sweep. Linear by default.")]
        [SerializeField] private AnimationCurve _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private RectTransform _track;
        private float _elapsed;

        private void Awake()
        {
            _track = (RectTransform)transform;
        }

        private void OnEnable()
        {
            _elapsed = 0f;
            ApplyPosition(0f);
        }

        private void Update()
        {
            // unscaledDeltaTime: keeps animating even if Time.timeScale = 0 during loads
            _elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Repeat(_elapsed / _cycleDuration, 1f);
            ApplyPosition(_ease.Evaluate(t));
        }

        private void ApplyPosition(float t)
        {
            float trackWidth = _track.rect.width;
            float runnerWidth = _runner.rect.width;

            // start fully hidden left of the track, end fully hidden right of it
            float x = Mathf.LerpUnclamped(-runnerWidth, trackWidth, t);

            Vector2 pos = _runner.anchoredPosition;
            pos.x = x;
            _runner.anchoredPosition = pos;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_runner == null)
                Debug.LogWarning($"[{nameof(UILoadingBarSweep)}] Runner is not assigned.", this);
        }
#endif
    }
}
