using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ui
{
    public class UIMainMenu : UIBaseItem
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;

        [Inject] private GarageManager _garageManager;
        
        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlay);
            _settingsButton.onClick.AddListener(OnSettings);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(OnPlay);
            _settingsButton.onClick.RemoveListener(OnSettings);
        }
        
        private void OnPlay()
        {
            _garageManager.ShowPage(GaragePages.LoadingScreen);
            Invoke(nameof(TestLoad), 5f);
        }
        
        //for test, remove later
        private void TestLoad()
        {
            _garageManager.LoadGame();
        }
        
        private void OnSettings()
        {
            _garageManager.ShowPage(GaragePages.SettingsMenu);
        }
    }
}