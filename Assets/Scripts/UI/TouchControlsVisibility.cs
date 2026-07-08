using UnityEngine;
using Zenject;

namespace UI
{
    // Hides the on-screen joystick/buttons whenever a gamepad (incl. Bluetooth
    // controllers) is connected, and shows them again once it disconnects.
    public class TouchControlsVisibility : MonoBehaviour
    {
        [SerializeField] private GameObject _touchControlsRoot;

        [Inject] private InputsManager _inputsManager;

        private void OnEnable()
        {
            _inputsManager.GamepadConnectionChanged += SetVisible;
            SetVisible(_inputsManager.IsGamepadConnected);
        }

        private void OnDisable()
        {
            _inputsManager.GamepadConnectionChanged -= SetVisible;
        }

        private void SetVisible(bool gamepadConnected)
        {
            _touchControlsRoot.SetActive(!gamepadConnected);
        }
    }
}
