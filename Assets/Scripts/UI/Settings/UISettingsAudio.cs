using UnityEngine;

namespace UI
{
    public class UISettingsAudio : MonoBehaviour
    {
        [SerializeField] private UISlider _masterVolume;
        [SerializeField] private UISlider _musicVolume;
        [SerializeField] private UISlider _soundsVolume;
        [SerializeField] private UISwitch _muteOnUnfocus;
        
        private void Awake()
        {
            _masterVolume.RealValue = 25;
            _musicVolume.RealValue = 50;
            _soundsVolume.RealValue = 75;
            _muteOnUnfocus.SetOn(false);
        }
        
    }
}