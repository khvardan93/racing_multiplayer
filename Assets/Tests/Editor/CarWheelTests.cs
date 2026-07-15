using NUnit.Framework;
using UnityEngine;

public class CarWheelTests
{
    private GameObject _carWheelObject;
    private CarWheel _carWheel;
    private WheelCollider _wheelCollider;
    private GameObject _visualObj;

    [SetUp]
    public void SetUp()
    {
        _carWheelObject = new GameObject("CarWheel");
        _carWheel = _carWheelObject.AddComponent<CarWheel>();

        _wheelCollider = _carWheelObject.AddComponent<WheelCollider>();
        _wheelCollider.mass = 20;
        _wheelCollider.suspensionDistance = 0.3f;

        _visualObj = new GameObject("WheelVisual");
        _visualObj.transform.SetParent(_carWheelObject.transform);

        // Set serialized fields via reflection
        var colliderField = typeof(CarWheel).GetField("_collider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        colliderField.SetValue(_carWheel, _wheelCollider);

        var visualField = typeof(CarWheel).GetField("_visualTransform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        visualField.SetValue(_carWheel, _visualObj.transform);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_carWheelObject);
        Object.DestroyImmediate(_visualObj);
    }

    [Test]
    public void CarWheel_InheritsMonoBehaviour()
    {
        Assert.IsTrue(typeof(MonoBehaviour).IsAssignableFrom(typeof(CarWheel)));
    }

    [Test]
    public void Collider_PropertyReturnsWheelCollider()
    {
        Assert.AreEqual(_wheelCollider, _carWheel.Collider);
    }

    [Test]
    public void VisualTransform_PropertyReturnsVisualTransform()
    {
        Assert.AreEqual(_visualObj.transform, _carWheel.VisualTransform);
    }

    [Test]
    public void Collider_PropertyIsReadOnly()
    {
        var property = typeof(CarWheel).GetProperty("Collider",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(property.GetGetMethod());
        Assert.IsNull(property.GetSetMethod());
    }

    [Test]
    public void VisualTransform_PropertyIsReadOnly()
    {
        var property = typeof(CarWheel).GetProperty("VisualTransform",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(property.GetGetMethod());
        Assert.IsNull(property.GetSetMethod());
    }

    [Test]
    public void CarWheel_HasColliderField()
    {
        var field = typeof(CarWheel).GetField("_collider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void CarWheel_HasVisualTransformField()
    {
        var field = typeof(CarWheel).GetField("_visualTransform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void CarWheel_UpdateMethodExists()
    {
        var method = typeof(CarWheel).GetMethod("Update",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void CarWheel_CanHaveNullCollider()
    {
        var colliderField = typeof(CarWheel).GetField("_collider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        colliderField.SetValue(_carWheel, null);

        Assert.IsNull(_carWheel.Collider);
    }

    [Test]
    public void CarWheel_CanHaveNullVisualTransform()
    {
        var visualField = typeof(CarWheel).GetField("_visualTransform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        visualField.SetValue(_carWheel, null);

        Assert.IsNull(_carWheel.VisualTransform);
    }

    [Test]
    public void WheelCollider_HasExpectedDefaults()
    {
        Assert.Greater(_wheelCollider.mass, 0f);
        Assert.Greater(_wheelCollider.suspensionDistance, 0f);
    }

    [Test]
    public void VisualObject_IsChildOfWheelGameObject()
    {
        Assert.AreEqual(_carWheelObject.transform, _visualObj.transform.parent);
    }

    [Test]
    public void CarWheel_ComponentIsAttachedToGameObject()
    {
        var component = _carWheelObject.GetComponent<CarWheel>();
        Assert.AreEqual(_carWheel, component);
    }
}
