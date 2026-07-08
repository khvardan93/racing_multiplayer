using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI
{
    // Drag-based on-screen joystick for steering/throttle, feeding InputsManager's UI override.
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;

        [Inject] private InputsManager _inputsManager;

        private Vector2 _input;

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background, eventData.position, eventData.pressEventCamera, out var localPoint);

            var radius = _background.sizeDelta.x * 0.5f;
            _input = Vector2.ClampMagnitude(localPoint / radius, 1f);

            _handle.anchoredPosition = _input * radius;
            _inputsManager.SetUIHorizontal(_input.x);
            _inputsManager.SetUIVertical(_input.y);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _input = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;
            _inputsManager.SetUIHorizontal(0f);
            _inputsManager.SetUIVertical(0f);
        }
    }
}
