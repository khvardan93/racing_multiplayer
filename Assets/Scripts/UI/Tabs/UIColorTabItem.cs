using UnityEngine;

namespace UI
{
    public class UIColorTabItem : UITabBaseItem
    {
        [SerializeField] private UIBaseItem _selector;
        
        public override void Select()
        {
            if(!Selected)
            {
                _selector.Show();
                base.Select();
            }
        }
        
        public override void Unselect()
        {
            if (Selected)
            {
                _selector.Hide();
                base.Unselect();
            }
        }
    }
}