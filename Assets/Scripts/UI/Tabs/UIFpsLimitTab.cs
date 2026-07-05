using UnityEngine;

namespace UI
{
    public class UIFpsLimitTab : UITabTextBaseItem
    {
        [SerializeField] private FpsTab _tab;
        
        public FpsTab Tab => _tab;
    }
}