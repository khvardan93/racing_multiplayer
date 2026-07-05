using System;
using UnityEngine;

namespace UI
{
    public enum SettingsTab{
        Audio,
        Graphics,
        Controls,
        Language,
        Account
    }
    
    public class UISettingsMenu : MonoBehaviour
    {
        [Serializable]
        public struct Tab
        {
            public SettingsTab TabType;
            public UIBaseItem  Item;
        }
        
        [SerializeField] private UITabsController _settingsTabs;
        [SerializeField] private Tab[] _tabs;

        private void Awake()
        {
            _settingsTabs.OnChanged += OnTabChanged;
        }

        private void OnEnable()
        {
            SetTab(SettingsTab.Audio);
        }

        private void OnDestroy()
        {
            _settingsTabs.OnChanged -= OnTabChanged;
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
    }
}
