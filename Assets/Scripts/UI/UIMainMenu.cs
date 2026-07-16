using Managers;
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
        [SerializeField] private UIBaseItem _attentionPopup;

        [Inject] private GarageManager _garageManager;
        [Inject] private DataManager _dataManager;
        
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

        private void OnEnable()
        {
            if(_dataManager.TryGet<bool>("attention_popup", out var state) && state)
                return;
            Invoke(nameof(TryShowAttentionPopup), 1f);
        }

        private void OnDisable()
        {
            CancelInvoke();
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
            _dataManager.Set("attention_popup", true);
            _garageManager.ShowPage(GaragePages.SettingsMenu);
        }

        private void TryShowAttentionPopup()
        {
            _attentionPopup.Show();
        }
    }
}