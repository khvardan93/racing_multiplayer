using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SportsCarControllerTests
{
    private GameObject _carObject;
    private SportsCarController _sportsCarController;
    private Rigidbody _rigidbody;
    private GameObject _groundObject;

    private CarWheel _frontLeft;
    private CarWheel _frontRight;
    private CarWheel _rearLeft;
    private CarWheel _rearRight;

    [SetUp]
    public void SetUp()
    {
        // Create ground
        _groundObject = new GameObject("Ground");
        var groundCollider = _groundObject.AddComponent<BoxCollider>();
        groundCollider.size = new Vector3(100, 1, 100);

        // Create car body
        _carObject = new GameObject("SportsCarTest");
        _carObject.transform.position = new Vector3(0, 2, 0);

        _rigidbody = _carObject.AddComponent<Rigidbody>();
        _rigidbody.mass = 1500;

        _sportsCarController = _carObject.AddComponent<SportsCarController>();

        // Create wheels (CarWheel components with WheelColliders)
        _frontLeft = CreateWheel("FrontLeft", new Vector3(-0.8f, 0.3f, 1f));
        _frontRight = CreateWheel("FrontRight", new Vector3(0.8f, 0.3f, 1f));
        _rearLeft = CreateWheel("RearLeft", new Vector3(-0.8f, 0.3f, -1f));
        _rearRight = CreateWheel("RearRight", new Vector3(0.8f, 0.3f, -1f));

        // Set wheel references via reflection
        SetWheelReference("_frontLeft", _frontLeft);
        SetWheelReference("_frontRight", _frontRight);
        SetWheelReference("_rearLeft", _rearLeft);
        SetWheelReference("_rearRight", _rearRight);
    }

    private CarWheel CreateWheel(string name, Vector3 position)
    {
        var wheelObj = new GameObject(name);
        wheelObj.transform.SetParent(_carObject.transform);
        wheelObj.transform.localPosition = position;

        var wheelCollider = wheelObj.AddComponent<WheelCollider>();
        wheelCollider.suspensionDistance = 0.3f;
        wheelCollider.mass = 25;

        var visualObj = new GameObject(name + "_Visual");
        visualObj.transform.SetParent(wheelObj.transform);
        visualObj.transform.localPosition = Vector3.zero;

        var carWheel = wheelObj.AddComponent<CarWheel>();
        var colliderField = typeof(CarWheel).GetField("_collider",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        colliderField.SetValue(carWheel, wheelCollider);

        var visualField = typeof(CarWheel).GetField("_visualTransform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        visualField.SetValue(carWheel, visualObj.transform);

        return carWheel;
    }

    private void SetWheelReference(string fieldName, CarWheel wheel)
    {
        var field = typeof(SportsCarController).GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(_sportsCarController, wheel);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_carObject);
        Object.DestroyImmediate(_groundObject);
    }

    [UnityTest]
    public IEnumerator SportsCarController_RearWheelsReceiveMotorTorque()
    {
        // Simulate accelerate input
        SimulateInput(1f, 0f, false);

        yield return new WaitForFixedUpdate();

        // Rear wheels should have motor torque
        Assert.Greater(_rearLeft.Collider.motorTorque, 0f);
        Assert.Greater(_rearRight.Collider.motorTorque, 0f);

        // Front wheels should have zero motor torque (FWD setup)
        Assert.AreEqual(0f, _frontLeft.Collider.motorTorque);
        Assert.AreEqual(0f, _frontRight.Collider.motorTorque);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SportsCarController_RearWheelsReduceMotorTorque_WhenBraking()
    {
        // First apply acceleration
        SimulateInput(1f, 0f, false);
        yield return new WaitForFixedUpdate();

        var motorTorqueWhileAccelerating = _rearLeft.Collider.motorTorque;
        Assert.Greater(motorTorqueWhileAccelerating, 0f);

        // Now brake
        SimulateInput(0f, 0f, true);
        yield return new WaitForFixedUpdate();

        // Motor torque should be zero when braking
        Assert.AreEqual(0f, _rearLeft.Collider.motorTorque);
        Assert.AreEqual(0f, _rearRight.Collider.motorTorque);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SportsCarController_AllWheelsBrakeWhenBraking()
    {
        SimulateInput(0f, 0f, true);

        yield return new WaitForFixedUpdate();

        // All wheels should have brake torque
        Assert.Greater(_frontLeft.Collider.brakeTorque, 0f);
        Assert.Greater(_frontRight.Collider.brakeTorque, 0f);
        Assert.Greater(_rearLeft.Collider.brakeTorque, 0f);
        Assert.Greater(_rearRight.Collider.brakeTorque, 0f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SportsCarController_FrontWheelsSteer()
    {
        SimulateInput(0f, 0.5f, false);

        yield return new WaitForFixedUpdate();

        // Front wheels should have steering angle
        var maxSteerField = typeof(SportsCarController).GetField("_maxSteerAngle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxSteer = (float)maxSteerField.GetValue(_sportsCarController);

        var expectedSteer = 0.5f * maxSteer;
        Assert.AreEqual(expectedSteer, _frontLeft.Collider.steerAngle, 0.1f);
        Assert.AreEqual(expectedSteer, _frontRight.Collider.steerAngle, 0.1f);

        // Rear wheels shouldn't steer
        Assert.AreEqual(0f, _rearLeft.Collider.steerAngle, 0.01f);
        Assert.AreEqual(0f, _rearRight.Collider.steerAngle, 0.01f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SportsCarController_SteeringFullRange()
    {
        // Test full left
        SimulateInput(0f, -1f, false);
        yield return new WaitForFixedUpdate();

        var maxSteerField = typeof(SportsCarController).GetField("_maxSteerAngle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxSteer = (float)maxSteerField.GetValue(_sportsCarController);

        Assert.AreEqual(-maxSteer, _frontLeft.Collider.steerAngle, 0.1f);

        // Test full right
        SimulateInput(0f, 1f, false);
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(maxSteer, _frontLeft.Collider.steerAngle, 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SportsCarController_ReverseMotorTorque()
    {
        SimulateInput(-1f, 0f, false);

        yield return new WaitForFixedUpdate();

        // Negative input should produce negative motor torque
        Assert.Less(_rearLeft.Collider.motorTorque, 0f);
        Assert.Less(_rearRight.Collider.motorTorque, 0f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SportsCarController_CenterOfMassOffset()
    {
        // Verify center of mass has been adjusted
        var comOffsetField = typeof(SportsCarController).GetField("_centerOfMassOffset",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var comOffset = (Vector3)comOffsetField.GetValue(_sportsCarController);

        Assert.AreNotEqual(Vector3.zero, comOffset);

        yield return null;
    }

    private void SimulateInput(float moveInput, float steerInput, bool isBraking)
    {
        var moveField = typeof(SportsCarController).GetField("_moveInput",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moveField.SetValue(_sportsCarController, moveInput);

        var steerField = typeof(SportsCarController).GetField("_steerInput",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        steerField.SetValue(_sportsCarController, steerInput);

        var brakeField = typeof(SportsCarController).GetField("_isBraking",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        brakeField.SetValue(_sportsCarController, isBraking);
    }
}
