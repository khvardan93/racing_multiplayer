using UnityEngine;

namespace UI
{
    public class UIControlSchemeTab : UITabTextBaseItem
    {
         [SerializeField] private ControlType _tab;
        
        public ControlType Tab => _tab;
    }
}