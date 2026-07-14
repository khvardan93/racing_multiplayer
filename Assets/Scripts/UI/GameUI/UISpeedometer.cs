using Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UISpeedometer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _speedometerText;
        [SerializeField] private float _smoothTime = 0.15f;

        [Inject] private GameManager _gameManager;

        private float _displayedSpeed;
        private float _speedVelocity;

        private void Update()
        {
            var carControl = _gameManager.CarControl;
            var targetSpeed = carControl != null ? Mathf.Abs(carControl.Speed) : 0f;

            _displayedSpeed = Mathf.SmoothDamp(_displayedSpeed, targetSpeed, ref _speedVelocity, _smoothTime);
            _speedometerText.text = Mathf.RoundToInt(_displayedSpeed).ToString();
        }
    }
}