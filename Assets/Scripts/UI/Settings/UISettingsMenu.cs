using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public enum SettingsTab{
        Audio,
        Graphics,
        Controls,
        Language,
        Account
    }
    
    public class UISettingsMenu : UIBaseItem
    {
        [Serializable]
        public struct Tab
        {
            public SettingsTab TabType;
            public UIBaseItem  Item;
        }
        
        [SerializeField] private UITabsController _settingsTabs;
        [SerializeField] private Tab[] _tabs;

        [Header("buttons")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _revertButton;
        
        [Inject] private GarageManager _garageManager;
        
        private void Awake()
        {
            _settingsTabs.OnChanged += OnTabChanged;
            
            _backButton.onClick.AddListener(OnBackButton);
            _saveButton.onClick.AddListener(OnSaveButton);
            _revertButton.onClick.AddListener(OnRevertButton);
        }

        private void OnEnable()
        {
            SetTab(SettingsTab.Audio);
        }

        private void OnDestroy()
        {
            _settingsTabs.OnChanged -= OnTabChanged;

            _backButton.onClick.RemoveListener(OnBackButton);
            _saveButton.onClick.RemoveListener(OnSaveButton);
            _revertButton.onClick.RemoveListener(OnRevertButton);
        }

        private void SetTab(SettingsTab sTab)
        {
            foreach (var item in _settingsTabs.Items)
            {
                if (item is UISettingsTabItem tabItem && tabItem.Tab == sTab)
                {
                    _settingsTabs.SetSelectedItem(item);
                }
            }
        }

        private void OnTabChanged(UITabBaseItem item)
        {
            var cTab = ((UISettingsTabItem)item).Tab;

            foreach (var tab in _tabs)
            {
                if (tab.TabType == cTab)
                {
                    tab.Item.Show();
                }
                else
                {
                    tab.Item.Hide();
                }
            }
        }

        public void Setup()
        {
            
        }

        private void OnBackButton()
        {
            _garageManager.ShowPage(GaragePages.MainMenu);
        }

        private void OnSaveButton()
        {
            _garageManager.ShowPage(GaragePages.MainMenu);
        }
        
        private void OnRevertButton()
        {
            _garageManager.ShowPage(GaragePages.MainMenu);
        }
    }
}
