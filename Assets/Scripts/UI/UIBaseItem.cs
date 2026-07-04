using System;
using UnityEngine;

namespace UI
{
    public class UIBaseItem : MonoBehaviour
    {
        public virtual void Show(Action onDone = null) => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
        public virtual void Reset() { }
    }
}
