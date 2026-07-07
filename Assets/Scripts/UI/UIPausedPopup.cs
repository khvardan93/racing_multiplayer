using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPausedPopup : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _leaveButton;

        private void Awake()
        {
            _resumeButton.onClick.AddListener(OnResume);
            _settingsButton.onClick.AddListener(OnSettings);
            _leaveButton.onClick.AddListener(OnLeave);
        }

        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _leaveButton.onClick.RemoveAllListeners();
        }
        
        private void OnResume(){}
        
        private void OnSettings(){}
        
        private void OnLeave(){}
    }
}
