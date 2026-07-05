using TMPro;
using UnityEngine;

namespace UI
{
    public class UITabTextBaseItem : UITabBaseItem
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;
        
        public override void Select()
        {
            base.Select();
            _text.color = _selectedColor;
        }

        public override void Unselect()
        {
            base.Unselect();
            _text.color = _unselectedColor;
        }
    }
}