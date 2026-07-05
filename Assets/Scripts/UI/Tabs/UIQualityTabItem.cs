using UnityEngine;

namespace UI
{
    public class UIQualityTabItem : UITabTextBaseItem
    {
        [SerializeField] private QualityTab _tab;
        
        public QualityTab Tab => _tab;
    }
}