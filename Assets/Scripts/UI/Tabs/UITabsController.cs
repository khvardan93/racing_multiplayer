using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UITabsController : MonoBehaviour
    {
        [SerializeField] private UITabBaseItem[] _items;
        
        public IReadOnlyList<UITabBaseItem> Items => _items;
        public event Action<UITabBaseItem> OnChanged;
        
        private UITabBaseItem _selectedItem;

        private void Awake()
        {
            foreach (var item in _items)
            {
                item.OnSelected += SetSelectedItem;
            }
        }

        private void OnDestroy()
        {
            foreach (var item in _items)
            {
                item.OnSelected -= SetSelectedItem;
            }
        }

        private void OnEnable()
        {
            if (!_selectedItem && _items.Length > 0) _selectedItem = _items[0];
            
            SetSelectedItem(_selectedItem);
        }
        
        public void SetSelectedItem(UITabBaseItem sItem)
        {
            OnChanged?.Invoke(sItem);
            _selectedItem = sItem;

            foreach (var item in _items)
            {
                if( item == sItem)
                    item.Select();
                else
                    item.Unselect();
            }
        }
    }
}