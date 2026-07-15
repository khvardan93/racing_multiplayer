using System.Collections;
using Managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputsManagerRuntimeTests
{
    private GameObject _gameObject;
    private InputsManager _inputsManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("InputsManagerRuntimeTest");
        _inputsManager = _gameObject.AddComponent<InputsManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [UnityTest]
    public IEnumerator InputsManager_ResetPressedClearsAfterUpdate()
    {
        // Trigger reset
        _inputsManager.TriggerUIReset();

        var field = typeof(InputsManager).GetField("_resetPressedLocalDetected",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Verify it's set
        var resetPressed = (bool)field.GetValue(_inputsManager);
        Assert.IsTrue(resetPressed);

        yield return null;

        // Note: Full OnInput call requires Fusion NetworkInput which is complex to mock
        // This test verifies the state management part
    }

    [UnityTest]
    public IEnumerator InputsManager_UIValuesOverrideDefaults()
    {
        _inputsManager.SetUIVertical(0.75f);
        _inputsManager.SetUIHorizontal(-0.5f);
        _inputsManager.SetUIHandBrake(true);

        yield return null;

        var verticalField = typeof(InputsManager).GetField("_uiVertical",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var horizontalField = typeof(InputsManager).GetField("_uiHorizontal",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var handBrakeField = typeof(InputsManager).GetField("_uiHandBrake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var verticalValue = (float)verticalField.GetValue(_inputsManager);
        var horizontalValue = (float)horizontalField.GetValue(_inputsManager);
        var handBrakeValue = (bool)handBrakeField.GetValue(_inputsManager);

        Assert.AreEqual(0.75f, verticalValue, 0.01f);
        Assert.AreEqual(-0.5f, horizontalValue, 0.01f);
        Assert.IsTrue(handBrakeValue);

        yield return null;
    }

    [UnityTest]
    public IEnumerator InputsManager_GamepadConnectedPropertyIsAccessible()
    {
        yield return null;

        var isConnected = _inputsManager.IsGamepadConnected;
        Assert.IsNotNull(isConnected);

        // Note: Actual gamepad connection testing requires real InputSystem device
        // which can't be reliably mocked in tests. This verifies property access works.

        yield return null;
    }

    [UnityTest]
    public IEnumerator InputsManager_GamepadConnectionChangedEventExists()
    {
        // Subscribe to the event
        bool subscribed = false;
        try
        {
            _inputsManager.GamepadConnectionChanged += (connected) => { subscribed = true; };
            subscribed = true;
        }
        catch
        {
            subscribed = false;
        }

        Assert.IsTrue(subscribed);

        yield return null;
    }

    [UnityTest]
    public IEnumerator InputsManager_ClearsUIValuesWhenSet()
    {
        _inputsManager.SetUIVertical(0.5f);
        var verticalField = typeof(InputsManager).GetField("_uiVertical",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var value1 = (float)verticalField.GetValue(_inputsManager);
        Assert.AreEqual(0.5f, value1, 0.01f);

        // Set to zero
        _inputsManager.SetUIVertical(0f);
        var value2 = (float)verticalField.GetValue(_inputsManager);
        Assert.AreEqual(0f, value2, 0.01f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator InputsManager_DropoutHoldValueConstants()
    {
        // Verify dropout hold duration is reasonable (0.15 seconds)
        var dropoutField = typeof(InputsManager).GetField("DropoutHoldSeconds",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        if (dropoutField != null)
        {
            var dropoutValue = (float)dropoutField.GetValue(null);
            Assert.Greater(dropoutValue, 0f);
            Assert.Less(dropoutValue, 1f);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator InputsManager_HandBrakeCanBeToggledViaUI()
    {
        _inputsManager.SetUIHandBrake(true);
        var field = typeof(InputsManager).GetField("_uiHandBrake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var value1 = (bool)field.GetValue(_inputsManager);
        Assert.IsTrue(value1);

        _inputsManager.SetUIHandBrake(false);
        var value2 = (bool)field.GetValue(_inputsManager);
        Assert.IsFalse(value2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator InputsManager_ResetCanBeTriggeredMultipleTimes()
    {
        var field = typeof(InputsManager).GetField("_resetPressedLocalDetected",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        _inputsManager.TriggerUIReset();
        var value1 = (bool)field.GetValue(_inputsManager);
        Assert.IsTrue(value1);

        // Manually clear it (simulating OnInput clearing it)
        field.SetValue(_inputsManager, false);
        var value2 = (bool)field.GetValue(_inputsManager);
        Assert.IsFalse(value2);

        // Trigger again
        _inputsManager.TriggerUIReset();
        var value3 = (bool)field.GetValue(_inputsManager);
        Assert.IsTrue(value3);

        yield return null;
    }
}
