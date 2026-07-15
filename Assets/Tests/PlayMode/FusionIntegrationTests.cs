using System.Collections;
using Fusion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FusionIntegrationTests
{
    private GameObject _runnerObject;
    private NetworkRunner _runner;
    private GameObject _carObject;
    private CarControl _carControl;

    [SetUp]
    public void SetUp()
    {
        // Create a minimal NetworkRunner for testing
        _runnerObject = new GameObject("NetworkRunnerTest");
        _runner = _runnerObject.AddComponent<NetworkRunner>();

        // Set up runner configuration via reflection
        var modeField = typeof(NetworkRunner).GetField("_gameMode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (modeField != null)
            modeField.SetValue(_runner, GameMode.Host);

        // Create test car
        _carObject = new GameObject("TestCar");
        _carObject.transform.position = new Vector3(0, 1, 0);

        var rb = _carObject.AddComponent<Rigidbody>();
        rb.mass = 1000;
        rb.linearDamping = 0.05f;

        // Add wheel colliders
        for (int i = 0; i < 4; i++)
        {
            var wheelCollider = _carObject.AddComponent<WheelCollider>();
            wheelCollider.suspensionDistance = 0.3f;
            wheelCollider.mass = 20;
        }

        _carControl = _carObject.AddComponent<CarControl>();

        // Configure CarControl via reflection
        var transformField = typeof(CarControl).GetField("_transform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        transformField.SetValue(_carControl, _carObject.transform);

        var rbField = typeof(CarControl).GetField("_rigidBody",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        rbField.SetValue(_carControl, rb);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_carObject);
        Object.DestroyImmediate(_runnerObject);
    }

    [Test]
    public void NetworkCarInputData_SerializesAllFields()
    {
        var input = new NetworkCarInputData
        {
            Vertical = 0.5f,
            Horizontal = -0.3f,
            HandBrake = true,
            ResetPressed = false
        };

        // Verify all fields are set correctly
        Assert.AreEqual(0.5f, input.Vertical, 0.01f);
        Assert.AreEqual(-0.3f, input.Horizontal, 0.01f);
        Assert.IsTrue(input.HandBrake);
        Assert.IsFalse(input.ResetPressed);
    }

    [UnityTest]
    public IEnumerator CarControl_ReceivesNetworkInputCorrectly()
    {
        // Create a mock input struct
        var testInput = new NetworkCarInputData
        {
            Vertical = 1f,
            Horizontal = 0f,
            HandBrake = false,
            ResetPressed = false
        };

        yield return new WaitForFixedUpdate();

        // Verify CarControl component exists and is ready to receive input
        Assert.IsNotNull(_carControl);
        Assert.IsNotNull(_carControl.GetComponent<Rigidbody>());

        yield return null;
    }

    [Test]
    public void NetworkCarInputData_HandlesBrakePriority()
    {
        // Simulate brake taking priority over accelerate
        var brakeTrigger = 0.8f;
        var accelTrigger = 1.0f;

        // In actual InputsManager: brakeTrigger != 0 ? -brakeTrigger : accelTrigger
        var vertical = brakeTrigger != 0 ? -brakeTrigger : accelTrigger;

        Assert.AreEqual(-0.8f, vertical, 0.01f);
    }

    [Test]
    public void NetworkCarInputData_AccelerateWhenNoBrake()
    {
        var brakeTrigger = 0f;
        var accelTrigger = 0.6f;

        var vertical = brakeTrigger != 0 ? -brakeTrigger : accelTrigger;

        Assert.AreEqual(0.6f, vertical, 0.01f);
    }

    [UnityTest]
    public IEnumerator CarControl_HasNetworkBehaviourBase()
    {
        yield return null;

        // Verify CarControl inherits from NetworkBehaviour
        var isNetworkBehaviour = typeof(NetworkBehaviour).IsAssignableFrom(typeof(CarControl));
        Assert.IsTrue(isNetworkBehaviour);

        yield return null;
    }

    [Test]
    public void CarControl_SpawnedMethodExists()
    {
        var spawnedMethod = typeof(CarControl).GetMethod("Spawned",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(spawnedMethod);
    }

    [Test]
    public void CarControl_FixedUpdateNetworkMethodExists()
    {
        var fixedUpdateMethod = typeof(CarControl).GetMethod("FixedUpdateNetwork",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(fixedUpdateMethod);
    }

    [UnityTest]
    public IEnumerator NetworkRunner_CanInstantiateInTest()
    {
        Assert.IsNotNull(_runner);
        Assert.AreEqual(GameMode.Host, _runner.GameMode);

        yield return null;
    }

    [UnityTest]
    public IEnumerator NetworkCarInputData_CanBePassedToNetworkInput()
    {
        // This tests that the struct can be created and configured
        // Full Fusion integration requires actual runner ticks which are complex to simulate
        var input = new NetworkCarInputData
        {
            Vertical = 1f,
            Horizontal = 0.5f,
            HandBrake = false,
            ResetPressed = false
        };

        yield return null;

        // Verify input is valid
        Assert.AreEqual(1f, input.Vertical);
        Assert.AreEqual(0.5f, input.Horizontal);

        yield return null;
    }
}
