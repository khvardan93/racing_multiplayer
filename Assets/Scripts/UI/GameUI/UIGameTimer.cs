using Zenject;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIGameTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timer;

        [Inject] private GameManager _gameManager;
        
        private void Start()
        {
            _gameManager.OnTimerChange += OnTimerChange;
        }

        private void OnDestroy()
        {
            _gameManager.OnTimerChange -= OnTimerChange;
        }

        private void OnTimerChange(int time)
        {
            var minutes = time / 60;
            var seconds = time % 60;
            _timer.text = $"{minutes:00}:{seconds:00}";
        }
    }
}