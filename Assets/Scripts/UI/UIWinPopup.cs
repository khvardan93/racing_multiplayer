using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class UIWinPopup : UIBaseItem
    {
        [SerializeField] private Button _rematchButton;
        [SerializeField] private Button _mainMenuButton;

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
            
        }

        private void OnMainMenu()
        {
            
        }
    }
}