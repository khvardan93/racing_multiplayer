using UnityEngine;

namespace UI
{
    public enum ControlType
    {
        Keys,
        Stick,
        Buttons,
        Wheel 
    }
    
    public class UISettingsControls : MonoBehaviour
    {
        [SerializeField] private UITabsController _controlSelector;
        [SerializeField] private UISlider _sensitivitySacle;
        [SerializeField] private UISwitch _invertSteering;
        [SerializeField] private UISwitch _autoAccelerate;
    }
}