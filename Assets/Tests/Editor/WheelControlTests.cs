using NUnit.Framework;
using UnityEngine;

public class WheelControlTests
{
    private GameObject _gameObject;
    private WheelControl _wheelControl;
    private WheelCollider _wheelCollider;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("WheelTest");
        _wheelControl = _gameObject.AddComponent<WheelControl>();
        _wheelCollider = _gameObject.AddComponent<WheelCollider>();

        // Set the serialized fields via reflection
        var wheelColliderField = typeof(WheelControl).GetField("_wheelCollider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        wheelColliderField.SetValue(_wheelControl, _wheelCollider);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void WheelCollider_PropertyReturnsAssignedCollider()
    {
        Assert.AreEqual(_wheelCollider, _wheelControl.WheelCollider);
    }

    [Test]
    public void Steerable_ReturnsFalseByDefault()
    {
        Assert.IsFalse(_wheelControl.Steerable);
    }

    [Test]
    public void Steerable_ReturnsConfiguredValue()
    {
        var steerableField = typeof(WheelControl).GetField("_steerable",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        steerableField.SetValue(_wheelControl, true);

        Assert.IsTrue(_wheelControl.Steerable);
    }

    [Test]
    public void Motorized_ReturnsFalseByDefault()
    {
        Assert.IsFalse(_wheelControl.Motorized);
    }

    [Test]
    public void Motorized_ReturnsConfiguredValue()
    {
        var motorizedField = typeof(WheelControl).GetField("_motorized",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        motorizedField.SetValue(_wheelControl, true);

        Assert.IsTrue(_wheelControl.Motorized);
    }

    [Test]
    public void WheelControl_AllPropertiesAreReadOnly()
    {
        // Verify properties are read-only by checking they only have getters
        var wheelColliderProp = typeof(WheelControl).GetProperty("WheelCollider",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(wheelColliderProp.GetGetMethod());
        Assert.IsNull(wheelColliderProp.GetSetMethod());

        var steerableProp = typeof(WheelControl).GetProperty("Steerable",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(steerableProp.GetGetMethod());
        Assert.IsNull(steerableProp.GetSetMethod());

        var motorizedProp = typeof(WheelControl).GetProperty("Motorized",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(motorizedProp.GetGetMethod());
        Assert.IsNull(motorizedProp.GetSetMethod());
    }
}
