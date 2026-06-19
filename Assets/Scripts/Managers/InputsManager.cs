using Fusion;
using UnityEngine;

public class InputsManager : MonoBehaviour
{
    private bool _resetPressedLocalDetected;

    private void Update()
    {
        // Capture discrete button downs here so they aren't missed between ticks
        if (Input.GetKeyDown(KeyCode.R)) // Change to your actual Reset button/axis
        {
            _resetPressedLocalDetected = true;
        }
    }
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var myInput = new NetworkCarInputData();

        // 1. Gather your vertical inputs
        var accelButton = Input.GetAxis("Accelerate");
        var vertical = accelButton <= 0 ? Input.GetAxis("Vertical") : accelButton;

        var brakeTrigger = Input.GetAxis("BrakeTriggerXBox");
        var accelTrigger = Input.GetAxis("AccelTriggerXBox");
        if (accelTrigger != 0) vertical = accelTrigger;
        if (brakeTrigger != 0) vertical = -brakeTrigger;

        if (Input.GetAxis("Break") > 0)
        {
            vertical = -1;
        }

        // 2. Assign values to the struct
        myInput.Vertical = vertical;
        myInput.Horizontal = Input.GetAxis("Horizontal");
        myInput.HandBrake = Input.GetAxis("HandBreak") > 0;

        // For single-press buttons like Reset, read them in Update and pass them here
        myInput.ResetPressed = _resetPressedLocalDetected;
        _resetPressedLocalDetected = false; // Clear it locally

        // 3. Hand it over to Fusion
        input.Set(myInput);
    }
}