using System;
using UnityEngine;

namespace UI
{
    public class UIIntro : MonoBehaviour
    {
        [SerializeField] private float _delay;

        private void OnEnable()
        {
            Invoke(nameof(LoadGarage), _delay);
        }

        //temporary, will be moved to a manager
        private void LoadGarage()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Garage");
        }
    }
}