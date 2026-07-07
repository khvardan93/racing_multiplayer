using Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIPausedPopup : UIBaseItem
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _leaveButton;

        [Inject] GameUIManager _gameUIManager;
        
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

        private void OnResume()
        {
            _gameUIManager.ShowGameHud();
        }

        private void OnSettings()
        {
            _gameUIManager.ShowSettingsPopup();
        }

        private void OnLeave()
        {
            _gameUIManager.ShowLoadingScreen();
        }
    }
}
