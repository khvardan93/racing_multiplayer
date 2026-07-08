using UnityEngine;

namespace RacerCar.UI
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private float _updateInterval = 0.5f;
        [SerializeField] private int _fontSize = 28;
        [SerializeField] private Color _textColor = Color.white;

        private GUIStyle _style;
        private string _fpsText = "0 FPS";
        private float _timer;
        private int _frames;

        private void Update()
        {
            _frames++;
            _timer += Time.unscaledDeltaTime;

            if (_timer >= _updateInterval)
            {
                float fps = _frames / _timer;
                _fpsText = $"{fps:F0} FPS";
                _timer = 0f;
                _frames = 0;
            }
        }

        private void OnGUI()
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = _fontSize,
                    alignment = TextAnchor.LowerCenter,
                    normal = { textColor = _textColor }
                };
            }

            const float width = 300f;
            const float height = 50f;
            const float bottomMargin = 20f;

            Rect rect = new Rect((Screen.width - width) * 0.5f, Screen.height - height - bottomMargin, width, height);
            GUI.Label(rect, _fpsText, _style);
        }
    }
}
