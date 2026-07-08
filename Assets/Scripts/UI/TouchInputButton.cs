using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI
{
    // On-screen button driving one of InputsManager's UI overrides (HandBrake hold, Reset tap).
    public class TouchInputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private enum Action
        {
            HandBrake,
            Reset
        }

        [SerializeField] private Action _action;

        [Inject] private InputsManager _inputsManager;

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (_action)
            {
                case Action.HandBrake:
                    _inputsManager.SetUIHandBrake(true);
                    break;
                case Action.Reset:
                    _inputsManager.TriggerUIReset();
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_action == Action.HandBrake)
            {
                _inputsManager.SetUIHandBrake(false);
            }
        }
    }
}
