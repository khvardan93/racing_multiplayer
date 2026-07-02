using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UITextPrintAnimator : MonoBehaviour
    {
        [Header("Print animation")]
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _playOnEnable = true;

        [Header("Loop")]
        [SerializeField] private bool _loop;
        [SerializeField] private int _loopCharCount = 0;
        [SerializeField] private float _loopStepDelay;
        
        [Header("Content")]
        [SerializeField] private TMP_Text _text;

        private string _fullText;
        private Coroutine _printRoutine;
        private Coroutine _cursorRoutine;

        private void Awake()
        {
            _fullText = _text.text;
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            StopPrinting();
        }

        public void Play()
        {
            StopPrinting();
            _printRoutine = StartCoroutine(PrintRoutine());
        }

        private void StopPrinting()
        {
            if (_printRoutine != null)
            {
                StopCoroutine(_printRoutine);
                _printRoutine = null;
            }

            if (_cursorRoutine != null)
            {
                StopCoroutine(_cursorRoutine);
                _cursorRoutine = null;
            }
        }

        private IEnumerator PrintRoutine()
        {
            _text.text = string.Empty;

            var characterCount = _fullText.Length;

            if (_delay > 0f)
            {
                
                yield return new WaitForSeconds(_delay);
            }
            
            var stepDuration =  _duration / characterCount;
            var waiter = new WaitForSeconds(stepDuration);

            for (int i = 0; i < characterCount; i++)
            {
                _text.text = _fullText[..i];
                yield return waiter;
            }
            
            _text.text = _fullText;
            
            if (_loop)
            {
                var step = _loopCharCount + 1;
                waiter = new WaitForSeconds(_loopStepDelay);
                
                while (true)
                {
                    step--;
                    
                    _text.maxVisibleCharacters = characterCount - step;

                    if (step == 0) step = _loopCharCount;
                    
                    yield return waiter;
                }
            }
        }
    }
}
