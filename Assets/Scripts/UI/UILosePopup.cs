using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Managers;

namespace UI
{
    public class UILosePopup : UIBaseItem
    {
        [SerializeField] private Button _rematchButton;
        [SerializeField] private Button _mainMenuButton;

        [Inject] private GameManager _gameManager;
        [Inject] private GameUIManager _gameUIManager;
        
        private void Awake()
        {
            _rematchButton.onClick.AddListener(OnRematch);
            _mainMenuButton.onClick.AddListener(OnMainMenu);
        }

        private void OnDestroy()
        {
            _rematchButton.onClick.RemoveAllListeners();
            _mainMenuButton.onClick.RemoveAllListeners();
        }

        private void OnRematch()
        {
            _gameUIManager.ShowLoadingScreen();
            _gameManager.ReloadGame();
        }

        private void OnMainMenu()
        {
            _gameUIManager.ShowLoadingScreen();
            _gameManager.LeaveGame();
        }
    }
}