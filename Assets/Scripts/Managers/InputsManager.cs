using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
public class InputsManager : MonoBehaviour
{
    const string CAR = "Car";
    const string STEER = "Steer";
    const string ACCELERATE = "Accelerate";
    const string BRAKE = "Brake";
    const string HAND_BRAKE = "HandBrake";
    const string RESET = "Reset";
    
    [SerializeField] private InputActionAsset _inputActions;

    private InputAction _steerAction;
    private InputAction _accelerateAction;
    private InputAction _brakeAction;
    private InputAction _handBrakeAction;
    private InputAction _resetAction;

    private bool _resetPressedLocalDetected;

    private float _uiVertical;
    private float _uiHorizontal;
    private bool _uiHandBrake;

    // Cheap Bluetooth gamepads occasionally drop a few HID reports (radio
    // interference/polling jitter), which reads as a brief all-zero input.
    // Bridge gaps shorter than this by holding the last known stick value
    // instead of snapping the car to a stop and back.
    private const float DropoutHoldSeconds = 0.15f;
    private float _lastSteer;
    private float _lastVertical;
    private float _lastGamepadInputTime;

    // Fired whenever a gamepad (incl. Bluetooth controllers, which show up as a
    // regular Gamepad device once paired) connects or disconnects.
    public event Action<bool> GamepadConnectionChanged;
    public bool IsGamepadConnected { get; private set; }

    private void Awake()
    {
        var carMap = _inputActions.FindActionMap(CAR);
        _steerAction = carMap.FindAction(STEER);
        _accelerateAction = carMap.FindAction(ACCELERATE);
        _brakeAction = carMap.FindAction(BRAKE);
        _handBrakeAction = carMap.FindAction(HAND_BRAKE);
        _resetAction = carMap.FindAction(RESET);

        _resetAction.performed += OnResetPerformed;
        IsGamepadConnected = Gamepad.current != null;
    }

    private void OnEnable()
    {
        _inputActions.FindActionMap(CAR).Enable();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        
        _inputActions.FindActionMap(CAR).Disable();
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDestroy()
    {
        _resetAction.performed -= OnResetPerformed;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad) return;

        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                var connected = Gamepad.current != null;
                if (connected == IsGamepadConnected) return;
                IsGamepadConnected = connected;
                GamepadConnectionChanged?.Invoke(connected);
                break;
        }
    }

    private void OnResetPerformed(InputAction.CallbackContext context)
    {
        // Capture discrete presses here so they aren't missed between ticks
        _resetPressedLocalDetected = true;
    }

    // Called by on-screen UI controls (joystick/buttons). These take priority
    // over keyboard/gamepad whenever actively driven, same as the trigger overrides below.
    public void SetUIVertical(float value) => _uiVertical = value;
    public void SetUIHorizontal(float value) => _uiHorizontal = value;
    public void SetUIHandBrake(bool pressed) => _uiHandBrake = pressed;
    public void TriggerUIReset() => _resetPressedLocalDetected = true;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var myInput = new NetworkCarInputData();

        // 1. Gather vertical input from Accelerate/Brake, brake taking priority
        var accelTrigger = _accelerateAction.ReadValue<float>();
        var brakeTrigger = _brakeAction.ReadValue<float>();
        var vertical = brakeTrigger != 0 ? -brakeTrigger : accelTrigger;
        var horizontal = _steerAction.ReadValue<float>();

        // Bridge brief Bluetooth dropouts: if a connected gamepad suddenly reads
        // all-zero right after reporting real input, hold the last value briefly
        // rather than treating it as the player releasing the stick.
        if (IsGamepadConnected)
        {
            if (vertical != 0f || horizontal != 0f)
            {
                _lastVertical = vertical;
                _lastSteer = horizontal;
                _lastGamepadInputTime = Time.unscaledTime;
            }
            else if (Time.unscaledTime - _lastGamepadInputTime < DropoutHoldSeconds)
            {
                vertical = _lastVertical;
                horizontal = _lastSteer;
            }
        }

        if (_uiVertical != 0) vertical = _uiVertical;
        if (_uiHorizontal != 0) horizontal = _uiHorizontal;

        
        // 2. Assign values to the struct
        myInput.Vertical = vertical;
        myInput.Horizontal = horizontal;
        myInput.HandBrake = _handBrakeAction.IsPressed() || _uiHandBrake;

        // For single-press buttons like Reset, read them via the performed callback and pass them here
        myInput.ResetPressed = _resetPressedLocalDetected;
        _resetPressedLocalDetected = false; // Clear it locally

        // 3. Hand it over to Fusion
        input.Set(myInput);
    }
}
}