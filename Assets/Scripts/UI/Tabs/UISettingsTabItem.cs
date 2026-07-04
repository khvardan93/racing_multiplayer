using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISettingsTabItem : UITabTextBaseItem
    {
        [SerializeField] private SettingsTab _tab;
        
        public SettingsTab Tab => _tab;
        
        public override void Select()
        {
            base.Select();
        }

        public override void Unselect()
        {
            base.Unselect();
        }
    }
}