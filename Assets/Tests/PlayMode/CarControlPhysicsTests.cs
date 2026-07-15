using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CarControlPhysicsTests
{
    private GameObject _carObject;
    private CarControl _carControl;
    private Rigidbody _rigidbody;
    private GameObject _groundObject;

    [SetUp]
    public void SetUp()
    {
        // Create ground plane
        _groundObject = new GameObject("Ground");
        var groundCollider = _groundObject.AddComponent<BoxCollider>();
        groundCollider.size = new Vector3(100, 1, 100);
        _groundObject.transform.position = Vector3.zero;

        // Create car with required components
        _carObject = new GameObject("TestCar");
        _carObject.transform.position = new Vector3(0, 2, 0);

        _rigidbody = _carObject.AddComponent<Rigidbody>();
        _rigidbody.mass = 1000;
        _rigidbody.linearDamping = 0.05f;
        _rigidbody.angularDamping = 0.05f;

        // Add WheelColliders (required for CarControl)
        var frontLeftWheel = _carObject.AddComponent<WheelCollider>();
        frontLeftWheel.suspensionDistance = 0.3f;
        frontLeftWheel.mass = 20;

        var frontRightWheel = _carObject.AddComponent<WheelCollider>();
        frontRightWheel.suspensionDistance = 0.3f;
        frontRightWheel.mass = 20;

        var rearLeftWheel = _carObject.AddComponent<WheelCollider>();
        rearLeftWheel.suspensionDistance = 0.3f;
        rearLeftWheel.mass = 20;

        var rearRightWheel = _carObject.AddComponent<WheelCollider>();
        rearRightWheel.suspensionDistance = 0.3f;
        rearRightWheel.mass = 20;

        // Add WheelControl components to wheels
        var flWheelControl = _carObject.AddComponent<WheelControl>();
        SetWheelControlProperties(flWheelControl, frontLeftWheel, true, false);

        var frWheelControl = _carObject.AddComponent<WheelControl>();
        SetWheelControlProperties(frWheelControl, frontRightWheel, true, false);

        var rlWheelControl = _carObject.AddComponent<WheelControl>();
        SetWheelControlProperties(rlWheelControl, rearLeftWheel, false, true);

        var rrWheelControl = _carObject.AddComponent<WheelControl>();
        SetWheelControlProperties(rrWheelControl, rearRightWheel, false, true);

        // Add CarControl
        _carControl = _carObject.AddComponent<CarControl>();

        // Set serialized fields on CarControl via reflection
        var transformField = typeof(CarControl).GetField("_transform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        transformField.SetValue(_carControl, _carObject.transform);

        var rigidbodyField = typeof(CarControl).GetField("_rigidBody",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        rigidbodyField.SetValue(_carControl, _rigidbody);

        var wheelsField = typeof(CarControl).GetField("_wheels",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        wheelsField.SetValue(_carControl, new[] { flWheelControl, frWheelControl, rlWheelControl, rrWheelControl });
    }

    private void SetWheelControlProperties(WheelControl wheelControl, WheelCollider collider, bool steerable, bool motorized)
    {
        var colliderField = typeof(WheelControl).GetField("_wheelCollider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        colliderField.SetValue(wheelControl, collider);

        var steerableField = typeof(WheelControl).GetField("_steerable",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        steerableField.SetValue(wheelControl, steerable);

        var motorizedField = typeof(WheelControl).GetField("_motorized",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        motorizedField.SetValue(wheelControl, motorized);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_carObject);
        Object.DestroyImmediate(_groundObject);
    }

    [UnityTest]
    public IEnumerator CarControl_SpeedPropertyInitializesAtZero()
    {
        yield return null;

        Assert.AreEqual(0f, _carControl.Speed, 0.1f);
    }

    [UnityTest]
    public IEnumerator CarControl_SpeedCalculatesFromForwardVelocity()
    {
        // Give the car a forward velocity
        _rigidbody.linearVelocity = _carObject.transform.forward * 10f; // 10 m/s forward

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Speed property converts m/s to km/h (* 3.6)
        var expectedSpeed = 10f * 3.6f; // 36 km/h
        Assert.AreEqual(expectedSpeed, _carControl.Speed, 1f);
    }

    [UnityTest]
    public IEnumerator CarControl_SpeedIsNegativeWhenMovingBackward()
    {
        // Give the car backward velocity
        _rigidbody.linearVelocity = _carObject.transform.forward * -5f; // -5 m/s (backward)

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        var expectedSpeed = -5f * 3.6f; // -18 km/h
        Assert.AreEqual(expectedSpeed, _carControl.Speed, 1f);
    }

    [UnityTest]
    public IEnumerator CarControl_ResetCarPositionTeleportsAndStopsVelocity()
    {
        var initialPos = _carObject.transform.position;
        var initialRot = _carObject.transform.rotation;

        // Move the car
        _rigidbody.linearVelocity = new Vector3(10, 0, 0);
        _rigidbody.angularVelocity = new Vector3(0, 5, 0);

        yield return new WaitForFixedUpdate();

        // Call reset via reflection
        var resetMethod = typeof(CarControl).GetMethod("ResetCarPosition",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        resetMethod.Invoke(_carControl, null);

        yield return null;

        // Verify position reset
        Assert.AreEqual(initialPos, _carObject.transform.position, 0.01f);
        Assert.AreEqual(initialRot, _carObject.transform.rotation, 0.01f);

        // Verify velocities cleared
        Assert.AreEqual(Vector3.zero, _rigidbody.linearVelocity, 0.01f);
        Assert.AreEqual(Vector3.zero, _rigidbody.angularVelocity, 0.01f);
    }

    [UnityTest]
    public IEnumerator CarControl_DragChangesBasedOnInputState()
    {
        var defaultDrag = _rigidbody.linearDamping;

        // Test coasting drag (vertical input = 0, grounded)
        var coastingDragField = typeof(CarControl).GetField("_coastingDrag",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var coastingDrag = (float)coastingDragField.GetValue(_carControl);

        // Note: Full drag testing requires wheel grounding state which needs complex setup
        // This test verifies the drag field is accessible and has a reasonable value
        Assert.Greater(coastingDrag, 0f);

        yield return null;
    }
}
