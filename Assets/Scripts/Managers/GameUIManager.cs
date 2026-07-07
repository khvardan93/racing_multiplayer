using UI;
using UnityEngine;

namespace Managers
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private UILoadingScreen _loadingScreen;
        [SerializeField] private UIPausedPopup _pausedPopup;
        [SerializeField] private UISettingsPopup _settingsPopup;
        [SerializeField] private UILosePopup _losePopup;
        [SerializeField] private UIWinPopup _winPopup;
    }
}