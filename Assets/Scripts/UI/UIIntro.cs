using UnityEngine;
using Zenject;

namespace UI
{
    public class UIIntro : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [Inject] private IntroManager _introManager;

        private void OnEnable()
        {
            Invoke(nameof(LoadGarage), _delay);
        }

        private void LoadGarage()
        {
            _introManager.LoadGarage();
        }
    }
}