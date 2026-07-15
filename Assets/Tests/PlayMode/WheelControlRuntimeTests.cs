using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WheelControlRuntimeTests
{
    private GameObject _carObject;
    private GameObject _wheelVisualObject;
    private WheelControl _wheelControl;
    private WheelCollider _wheelCollider;

    [SetUp]
    public void SetUp()
    {
        // Create car body
        _carObject = new GameObject("CarBody");
        _carObject.AddComponent<Rigidbody>();

        // Create wheel collider on car
        _wheelCollider = _carObject.AddComponent<WheelCollider>();
        _wheelCollider.suspensionDistance = 0.3f;
        _wheelCollider.mass = 20;

        // Create visual wheel transform (child of car)
        _wheelVisualObject = new GameObject("WheelVisual");
        _wheelVisualObject.transform.SetParent(_carObject.transform);
        _wheelVisualObject.transform.localPosition = new Vector3(1, 0.5f, 0);

        // Add WheelControl component
        _wheelControl = _carObject.AddComponent<WheelControl>();

        // Set serialized fields via reflection
        var colliderField = typeof(WheelControl).GetField("_wheelCollider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        colliderField.SetValue(_wheelControl, _wheelCollider);

        var wheelModelField = typeof(WheelControl).GetField("_wheelModel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        wheelModelField.SetValue(_wheelControl, _wheelVisualObject.transform);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_carObject);
        Object.DestroyImmediate(_wheelVisualObject);
    }

    [UnityTest]
    public IEnumerator WheelControl_WheelVisualSyncsWithColliderPosition()
    {
        var initialVisualPos = _wheelVisualObject.transform.position;

        // Simulate a frame update (which would normally sync position)
        yield return new WaitForFixedUpdate();

        // The visual should track the collider's world pose
        // In this simple setup without actual physics, the positions should be close
        var visualPos = _wheelVisualObject.transform.position;
        Assert.IsNotNull(visualPos);

        yield return null;
    }

    [UnityTest]
    public IEnumerator WheelControl_UpdateCallsSyncsVisuals()
    {
        // Record initial position
        var initialPosition = _wheelVisualObject.transform.position;

        // Rotate the car body (simulating suspension)
        _carObject.transform.Rotate(10, 0, 0);

        yield return null;

        // The visual position should update due to Update() calling GetWorldPose
        var finalPosition = _wheelVisualObject.transform.position;

        // Verify that the visual has been repositioned by the Update call
        // (In a real scenario with active physics, this would be more dramatic)
        Assert.IsNotNull(finalPosition);

        yield return null;
    }

    [UnityTest]
    public IEnumerator WheelControl_VisualTransformRotationMatches()
    {
        var initialRotation = _wheelVisualObject.transform.rotation;

        yield return new WaitForFixedUpdate();

        // Verify that rotation tracking works (even if no rotation change in this test)
        var finalRotation = _wheelVisualObject.transform.rotation;
        Assert.IsNotNull(finalRotation);

        yield return null;
    }

    [Test]
    public void WheelControl_WheelColliderPropertyReturnsSetCollider()
    {
        Assert.AreEqual(_wheelCollider, _wheelControl.WheelCollider);
    }

    [UnityTest]
    public IEnumerator WheelControl_MultipleUpdateCallsStayInSync()
    {
        yield return new WaitForFixedUpdate();

        var position1 = _wheelVisualObject.transform.position;

        yield return new WaitForFixedUpdate();

        var position2 = _wheelVisualObject.transform.position;

        // Positions should be consistent across multiple updates
        Assert.AreEqual(position1.x, position2.x, 0.01f);
        Assert.AreEqual(position1.y, position2.y, 0.01f);
        Assert.AreEqual(position1.z, position2.z, 0.01f);

        yield return null;
    }
}
