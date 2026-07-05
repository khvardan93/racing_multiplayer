using UnityEngine;

namespace UI
{
    public class UISettingsTabItem : UITabTextBaseItem
    {
        [SerializeField] private SettingsTab _tab;
        
        public SettingsTab Tab => _tab;
    }
}