using UnityEngine;

namespace UI
{
    public enum QualityTab{
        Low,
        Medium,
        High,
        Ultra
    }
    
    public enum FpsTab{
        F30,
        F60,
        F120
    }
    
    public class UISettingsGraphics : MonoBehaviour
    {
        [SerializeField] private UITabsController _qualitySelector;
        [SerializeField] private UISlider _renderSacle;
        [SerializeField] private UITabsController _fpsSelector;
        [SerializeField] private UISwitch _postProcessing;
        [SerializeField] private UISwitch _shadows;

        private void Awake()
        {
            SetQuality(QualityTab.Medium);
            _renderSacle.RealValue = 75;
            SetFps(FpsTab.F120);
            _postProcessing.SetOn(false);
            _shadows.SetOn(true);
        }

        private void SetQuality(QualityTab quality)
        {
            foreach (var item in _qualitySelector.Items)
            {
                if (item is UIQualityTabItem qItem && qItem.Tab == quality)
                {
                    _qualitySelector.SetSelectedItem(item);
                }
            }
        }
        
        private void SetFps(FpsTab fps)
        {
            foreach (var item in _fpsSelector.Items)
            {
                if (item is UIFpsLimitTab fItem && fItem.Tab == fps)
                {
                    _fpsSelector.SetSelectedItem(item);
                }
            }
        }
    }
}