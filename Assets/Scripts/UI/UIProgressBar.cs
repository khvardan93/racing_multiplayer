using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
   [Header("Bar References")]
    [Tooltip("Use this if your bar is an Image with Fill type set to 'Filled'.")]
    [SerializeField] private Image fillImage;
 
    [Tooltip("Use this if your bar is a UI Slider instead of an Image.")]
    [SerializeField] private Slider slider;
 
    [Tooltip("Optional: a second image behind the main fill that catches up slower (delayed/chase effect).")]
    [SerializeField] private Image delayedFillImage;
 
    [Header("Text (optional)")]
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private string textFormat = "{0}%";
 
    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutQuad;
    [SerializeField] private float delayedFillExtraDuration = 0.4f;
 
    private float currentValue; // 0..1
    private Tween fillTween;
    private Tween delayedTween;
    private Tween textTween;
 
    private void Awake()
    {
        currentValue = GetCurrentFill();
    }
 
    /// <summary>
    /// Set progress instantly, no animation. value should be 0..1.
    /// </summary>
    public void SetProgressInstant(float value)
    {
        value = Mathf.Clamp01(value);
        KillTweens();
 
        currentValue = value;
 
        if (fillImage != null) fillImage.fillAmount = value;
        if (slider != null) slider.value = value;
        if (delayedFillImage != null) delayedFillImage.fillAmount = value;
 
        UpdatePercentageText(value);
    }
 
    /// <summary>
    /// Animate progress bar to target value (0..1) using DOTween.
    /// </summary>
    public void SetProgress(float targetValue, System.Action onComplete = null)
    {
        targetValue = Mathf.Clamp01(targetValue);
        KillTweens();
 
        float startValue = currentValue;
 
        // Animate main fill
        if (fillImage != null)
        {
            fillTween = fillImage.DOFillAmount(targetValue, duration)
                .SetEase(ease)
                .OnUpdate(() => UpdatePercentageText(fillImage.fillAmount))
                .OnComplete(() => onComplete?.Invoke());
        }
        else if (slider != null)
        {
            fillTween = slider.DOValue(targetValue, duration)
                .SetEase(ease)
                .OnUpdate(() => UpdatePercentageText(slider.value))
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            // No visual bar assigned, just animate the text/value
            float tempValue = startValue;
            textTween = DOTween.To(() => tempValue, x =>
            {
                tempValue = x;
                UpdatePercentageText(x);
            }, targetValue, duration).SetEase(ease).OnComplete(() => onComplete?.Invoke());
        }
 
        // Animate delayed "chase" bar (slower, catches up after)
        if (delayedFillImage != null)
        {
            delayedTween = delayedFillImage.DOFillAmount(targetValue, duration + delayedFillExtraDuration)
                .SetEase(Ease.OutQuad);
        }
 
        currentValue = targetValue;
    }
 
    /// <summary>
    /// Convenience overload using a 0..100 percentage instead of 0..1.
    /// </summary>
    public void SetProgressPercent(float percent, System.Action onComplete = null)
    {
        SetProgress(percent / 100f, onComplete);
    }
 
    private void UpdatePercentageText(float value)
    {
        if (percentageText != null)
        {
            int percent = Mathf.RoundToInt(value * 100f);
            percentageText.text = string.Format(textFormat, percent);
        }
    }
 
    private float GetCurrentFill()
    {
        if (fillImage != null) return fillImage.fillAmount;
        if (slider != null) return slider.value;
        return 0f;
    }
 
    private void KillTweens()
    {
        fillTween?.Kill();
        delayedTween?.Kill();
        textTween?.Kill();
    }
 
    private void OnDestroy()
    {
        KillTweens();
    }
 
#if UNITY_EDITOR
    [Header("Editor Test")]
    [Range(0f, 1f)] public float testValue = 1f;
 
    [ContextMenu("Test Animate To Value")]
    private void TestAnimate()
    {
        SetProgress(testValue);
    }
#endif 
}
