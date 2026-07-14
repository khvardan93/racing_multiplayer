using UnityEngine;
using Zenject;

namespace Managers
{
    public class IntroManager : MonoBehaviour
    {
        [Inject] private AssetsManager _assetsManager;
        [Inject] private GameConfigs _configs;

        public void LoadGarage()
        {
            if (_configs.TryGetScene(GameScenes.Menu, out var scene))
                _assetsManager.LoadScene(scene);
        }
    }
}
