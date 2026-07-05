using UnityEngine;

namespace UI
{
    public class UILanguageToggleTab : UIToggleBaseTab
    {
        [SerializeField] private Language _language;
        
        public Language Language => _language;
    }
}