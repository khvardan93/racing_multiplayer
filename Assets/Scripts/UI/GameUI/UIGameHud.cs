using Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIGameHud : UIBaseItem
    {
        [SerializeField] private Button _pauseButton;
        
        [Header("UI Input Controls")]
        [SerializeField] private UIInputItem _accelerateInputItem;
        [SerializeField] private UIInputItem _brakeInputItem;
        [SerializeField] private UIInputItem _leftInputItem;
        [SerializeField] private UIInputItem _rightInputItem;

        [Inject] GameUIManager _gameUIManager;
        [Inject] InputsManager _inputsManager;
        
        private void Awake()
        {
            _pauseButton.onClick.AddListener(OnPause);
            
            _accelerateInputItem.OnPointed += OnAccelerate;
            _brakeInputItem.OnPointed += OnBrake;
            _leftInputItem.OnPointed += OnLeft;
            _rightInputItem.OnPointed += OnRight;
        }

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPause);
            
            _accelerateInputItem.OnPointed -= OnAccelerate;
            _brakeInputItem.OnPointed -= OnBrake;
            _leftInputItem.OnPointed -= OnLeft;
            _rightInputItem.OnPointed -= OnRight;
        }

        private void OnPause()
        {
            _gameUIManager.ShowPausePopup();
        }

        private void OnAccelerate(bool state)
        {
            _inputsManager.SetUIVertical(state ? 1f : 0);
        }

        private void OnBrake(bool state)
        {
            _inputsManager.SetUIVertical(state ? -1f : 0);
        }

        private void OnLeft(bool state)
        {
            _inputsManager.SetUIHorizontal(state ? -1f : 0);
        }

        private void OnRight(bool state)
        {
            _inputsManager.SetUIHorizontal(state ? 1f : 0);
        }
    }
}

