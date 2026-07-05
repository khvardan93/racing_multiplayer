using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UIItemsShowAnimator : UIBaseItem
    {
        private enum QueueType
        {
            Interval,
            WaitForPrev
        }
        
        [Header("Sequence")]
        [SerializeField] private QueueType _queueType;
        [SerializeField] private float _delay;
        [SerializeField] private float _interval = 0.15f;
        [SerializeField] private bool _playOnEnable = true;

        [SerializeField] private UIBaseItem[] _items;
        
        [SerializeField] private UnityEvent _onDone;
        
        private Coroutine _routine;
        private int _currentItem;
        private bool _locked;

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
        }

        public void Play()
        {
            HideAll();

            switch (_queueType)
            {
                case QueueType.Interval:
                    PlayByInterval();
                    break;
                case QueueType.WaitForPrev:
                    _currentItem = 0;

                    if (_locked) ResetAll();
                    _locked = true;
                    
                    PlayByPrev();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void PlayByPrev()
        {
            _items[_currentItem].Show(() =>
            {
                _currentItem++;
                if (_currentItem < _items.Length)
                    PlayByPrev();
                else
                {
                    _locked = false;
                    _onDone?.Invoke();
                }
            });
        }
        
        private void ResetAll()
        {
            foreach (var item in _items)
            {
                item.Reset();
            }
        }

        private void HideAll()
        {
            foreach (var item in _items)
            {
                item?.Hide();
            }
        }

        private void PlayByInterval()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            if(!gameObject.activeInHierarchy)
                Debug.LogWarning($"'{name}' gameobject is disabled in hierarchy, but you tried to start a coroutine!");
            
            _routine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            if(_delay > 0)
                yield return new WaitForSeconds(_delay);
            
            var waiter = new WaitForSeconds(_interval);

            foreach (var item in _items)
            {
                item.Show();

                yield return waiter;
            }
            
            _onDone?.Invoke();

            _routine = null;
        }
    }
}
