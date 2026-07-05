using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIToggleBaseTab : UITabBaseItem
    {
        [SerializeField] private Image _toggleImage; 
        [SerializeField] private Sprite _selectedIcon; 
        [SerializeField] private Sprite _unselectedIcon; 
        
        public override void Select()
        {
            base.Select();
            _toggleImage.sprite = _selectedIcon;
        }
        
        public override void Unselect()
        {
            base.Unselect();
            _toggleImage.sprite = _unselectedIcon;
        }
    }
}