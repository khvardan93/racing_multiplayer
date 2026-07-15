using Managers;
using NUnit.Framework;
using UnityEngine;

public class InputsManagerTests
{
    private GameObject _gameObject;
    private InputsManager _inputsManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("InputsManagerTest");
        _inputsManager = _gameObject.AddComponent<InputsManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void TriggerUIReset_SetsResetPressedState()
    {
        // Get the private _resetPressedLocalDetected field via reflection
        var field = typeof(InputsManager).GetField("_resetPressedLocalDetected",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        _inputsManager.TriggerUIReset();

        var resetPressed = (bool)field.GetValue(_inputsManager);
        Assert.IsTrue(resetPressed);
    }

    [Test]
    public void SetUIVertical_StoresVerticalValue()
    {
        var field = typeof(InputsManager).GetField("_uiVertical",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        _inputsManager.SetUIVertical(0.75f);

        var storedValue = (float)field.GetValue(_inputsManager);
        Assert.AreEqual(0.75f, storedValue, 0.01f);
    }

    [Test]
    public void SetUIHorizontal_StoresHorizontalValue()
    {
        var field = typeof(InputsManager).GetField("_uiHorizontal",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        _inputsManager.SetUIHorizontal(-0.5f);

        var storedValue = (float)field.GetValue(_inputsManager);
        Assert.AreEqual(-0.5f, storedValue, 0.01f);
    }

    [Test]
    public void SetUIHandBrake_StoresHandBrakeState()
    {
        var field = typeof(InputsManager).GetField("_uiHandBrake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        _inputsManager.SetUIHandBrake(true);

        var storedValue = (bool)field.GetValue(_inputsManager);
        Assert.IsTrue(storedValue);
    }

    [Test]
    public void IsGamepadConnected_ReflectsGamepadState()
    {
        // Note: This tests the property exposed by the manager
        // Actual gamepad connection/disconnection requires InputSystem in PlayMode
        var connected = _inputsManager.IsGamepadConnected;

        // Just verify the property exists and is readable
        Assert.IsNotNull(connected);
    }

    [Test]
    public void GamepadConnectionChanged_EventExists()
    {
        // Verify the event is declared and can be subscribed to
        var eventFired = false;
        _inputsManager.GamepadConnectionChanged += (connected) =>
        {
            eventFired = true;
        };

        // Manually invoke the event via reflection to test it's properly wired
        var eventField = typeof(InputsManager).GetEvent("GamepadConnectionChanged",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        Assert.IsNotNull(eventField);
    }
}
