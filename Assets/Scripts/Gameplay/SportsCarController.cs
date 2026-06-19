using UnityEngine;

public class SportsCarController : MonoBehaviour
{
    [Header("🛞 Wheel Setup")]
    [SerializeField] private CarWheel _frontLeft;
    [SerializeField] private CarWheel _frontRight;
    [SerializeField] private CarWheel _rearLeft;
    [SerializeField] private CarWheel _rearRight;

    [Header("🏎️ Sports Car Physics Settings")]
    [SerializeField] private float _motorTorque = 2500f;   // Explosive acceleration
    [SerializeField] private float _brakeTorque = 5000f;   // High-performance brakes
    [SerializeField] private float _maxSteerAngle = 32f;   // Sharp, responsive steering
    [SerializeField] private Vector3 _centerOfMassOffset = new Vector3(0, -0.5f, 0); // Low center of mass

    private Rigidbody _rb;
    private float _moveInput;
    private float _steerInput;
    private bool _isBraking;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        // Lower the Center of Mass to prevent flipping on high-speed corners
        _rb.centerOfMass += _centerOfMassOffset;
    }

    private void Update()
    {
        GetInputs();
        UpdateWheelVisuals();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
    }

    private void GetInputs()
    {
        // Works out of the box with default Unity Input manager (WASD / Arrow Keys / Space)
        _moveInput = Input.GetAxis("Vertical");
        _steerInput = Input.GetAxis("Horizontal");
        _isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        if (_isBraking)
        {
            ApplyBraking(_brakeTorque);
        }
        else
        {
            ApplyBraking(0f);
            
            // RWD: Send 100% of the motor power to the rear wheels
            _rearLeft.Collider.motorTorque = _moveInput * _motorTorque;
            _rearRight.Collider.motorTorque = _moveInput * _motorTorque;
        }
    }

    private void HandleSteering()
    {
        // Handle steering on the front wheels only
        var currentSteerAngle = _steerInput * _maxSteerAngle;
        _frontLeft.Collider.steerAngle = currentSteerAngle;
        _frontRight.Collider.steerAngle = currentSteerAngle;
    }

    private void ApplyBraking(float torque)
    {
        // Apply brake torque to all four wheels for a high-performance stop
        _frontLeft.Collider.brakeTorque = torque;
        _frontRight.Collider.brakeTorque = torque;
        _rearLeft.Collider.brakeTorque = torque;
        _rearRight.Collider.brakeTorque = torque;

        // Reset motor torque if braking
        if (torque > 0)
        {
            _rearLeft.Collider.motorTorque = 0;
            _rearRight.Collider.motorTorque = 0;
        }
    }

    private void UpdateWheelVisuals()
    {
        // Synchronize the 3D visual wheel meshes with the physics WheelColliders
        UpdateSingleWheel(_frontLeft);
        UpdateSingleWheel(_frontRight);
        UpdateSingleWheel(_rearLeft);
        UpdateSingleWheel(_rearRight);
    }

    private void UpdateSingleWheel(CarWheel wheel)
    {
        if (!wheel.Collider || !wheel.VisualTransform) return;

        wheel.Collider.GetWorldPose(out var pos, out var rot);
        
        wheel.VisualTransform.position = pos;
        wheel.VisualTransform.rotation = rot;
    }
}