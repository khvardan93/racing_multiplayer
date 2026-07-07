using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIGameHud : UIBaseItem
    {
        [SerializeField] private Button _pauseButton;

        [Inject] GameUIManager _gameUIManager;
        
        private void Awake()
        {
            _pauseButton.onClick.AddListener(OnPause);
        }

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPause);
        }

        private void OnPause()
        {
            _gameUIManager.ShowPausePopup();
        }
    }
}

