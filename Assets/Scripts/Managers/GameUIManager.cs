using UI;
using UnityEngine;

namespace Managers
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private UILoadingScreen _loadingScreen;
        [SerializeField] private UIPausedPopup _pausedPopup;
        [SerializeField] private UISettingsPopup _settingsPopup;
        [SerializeField] private UILosePopup _losePopup;
        [SerializeField] private UIWinPopup _winPopup;
        [SerializeField] private UIGameHud _gameHud;
        
        private UIBaseItem _currentUI;

        private void Start()
        {
            ShowLoadingScreen();
        }

        public void ShowLoadingScreen()
        {
            _currentUI?.Hide();
            _loadingScreen.Show();
            _currentUI = _loadingScreen;
        }
        
        public void ShowGameHud()
        {
            _currentUI?.Hide();
            _gameHud.Show();
            _currentUI = _gameHud;
        }
        
        public  void ShowPausePopup()
        {
            _currentUI?.Hide();
            _pausedPopup.Show();
            _currentUI = _pausedPopup;
        }

        public void ShowSettingsPopup()
        {
            _currentUI?.Hide();
            _settingsPopup.Show();
            _currentUI = _settingsPopup;
        }
    }
}