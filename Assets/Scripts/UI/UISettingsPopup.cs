using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISettingsPopup : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _revertButton;
        [SerializeField] private Button _saveButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(OnClose);
            _revertButton.onClick.AddListener(OnRevert);
            _saveButton.onClick.AddListener(OnSave);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(OnClose);
            _revertButton.onClick.RemoveListener(OnRevert);
            _saveButton.onClick.RemoveListener(OnSave);
        }
        
        private void OnClose(){}
        private void OnRevert(){}
        private void OnSave(){}
    }
} 