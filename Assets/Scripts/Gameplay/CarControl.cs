using Fusion;
using Managers;
using UnityEngine;
using Zenject;

public class CarControl : NetworkBehaviour
{
    [Inject] private GameManager _gameManager;

    [Header("Motor Settings")]
    [SerializeField] private float _motorTorque = 2000;
    [SerializeField] private float _brakeTorque = 2000;

    [Header("Speed & Steering")]
    [SerializeField] private float _maxSpeed = 20;
    [SerializeField] private float _steeringRange = 30;
    [SerializeField] private float _steeringRangeAtMaxSpeed = 10;

    [Header("Physics")] 
    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _centreOfGravityOffset = -1f;
    [SerializeField] private float _handBreakDrag = 2.5f;
    [SerializeField] private float _breakingDrag = 2.5f;
    [SerializeField] private float _coastingDrag = 1f;

    [Header("Wheels")]
    [SerializeField] private WheelControl[] _wheels;

    public float Speed { get; private set; }

    FloatCompressed VerticalInput { get; set; }
    FloatCompressed HorizontalInput { get; set; }
    NetworkBool HandBreakInput { get; set; }
    
    private float _defaultDrag;
    private Vector3 _initialSpawnPosition;
    private Quaternion _initialSpawnRotation;
    private bool _spawned;

    public override void Spawned()
    {
        var carTransform = _transform;
        
        if(_gameManager)
        {
            if (Object.HasInputAuthority)
            {
                _gameManager.SetCameraTarget(carTransform);
                _gameManager.RegisterLocalPlayer(this);
            }
            else
            {
                _gameManager.SetRivalCameraTarget(carTransform);
                _gameManager.StartTimer();
            }
        }

        // EVERYONE needs to know where this car spawned so 
        // predictions match during a reset execution.
        _initialSpawnPosition = carTransform.position;
        _initialSpawnRotation = carTransform.rotation;
        
        _rigidBody.centerOfMass += Vector3.up * _centreOfGravityOffset;
        _defaultDrag = _rigidBody.linearDamping;
        
        _spawned = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!_spawned || !_transform || !_rigidBody) return;
        
        // 1. Process inputs for BOTH Host and Client loops
        FetchNetworkInputs();

        // 2. Physics & driving calculations run smoothly on both sides now
        var forwardSpeed = Vector3.Dot(_transform.forward, _rigidBody.linearVelocity);
        var speedFactor = Mathf.InverseLerp(0, _maxSpeed, forwardSpeed);
        Speed = forwardSpeed * 3.6f; // m/s to km/h
        var currentMotorTorque = Mathf.Lerp(_motorTorque, 0, speedFactor);
        var currentSteerRange = Mathf.Lerp(_steeringRange, _steeringRangeAtMaxSpeed, speedFactor);

        var isAccelerating = Mathf.Sign(VerticalInput) == Mathf.Sign(forwardSpeed);
        var isGrounded = false;

        Wheels(ref isGrounded, isAccelerating, currentSteerRange, currentMotorTorque);
        Drag(isGrounded, isAccelerating);
    }

    private void Wheels(ref bool isGrounded, bool isAccelerating, float currentSteerRange, float currentMotorTorque)
    {
        foreach (var wheel in _wheels)
        {
            isGrounded = isGrounded || wheel.WheelCollider.isGrounded;

            if (wheel.Steerable)
            {
                wheel.WheelCollider.steerAngle = HorizontalInput * currentSteerRange;
            }

            if (HandBreakInput)
            {
                wheel.WheelCollider.brakeTorque = 100000;
            }
            else if (isAccelerating || VerticalInput > 0f)
            {
                if (wheel.Motorized)
                {
                    wheel.WheelCollider.motorTorque = VerticalInput * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                wheel.WheelCollider.brakeTorque = Mathf.Abs(VerticalInput) * _brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }

    private void Drag(bool isGrounded, bool isAccelerating)
    {
        if (HandBreakInput && isGrounded)
        {
            _rigidBody.linearDamping = _handBreakDrag;
        }
        else if (VerticalInput == 0f && isGrounded)
        {
            _rigidBody.linearDamping = _coastingDrag;
        }
        else if (isAccelerating)
        {
            _rigidBody.linearDamping = _defaultDrag;
        }
        else if (isGrounded)
        {
            _rigidBody.linearDamping = _breakingDrag;
        }
        else
        {
            _rigidBody.linearDamping = _defaultDrag;
        }
    }

    private void FetchNetworkInputs()
    {
        // GetInput extracts the struct for the current client predicting 
        // OR extracts historical ticks if the server is verifying/rolling back.
        if (GetInput(out NetworkCarInputData inputs))
        {
            VerticalInput = Mathf.Clamp(inputs.Vertical, -1f, 1f);
            HorizontalInput = Mathf.Clamp(inputs.Horizontal, -1f, 1f);
            HandBreakInput = inputs.HandBrake;

            // Handle the reset immediately inside the networked loop if true
            if (inputs.ResetPressed)
            {
                ResetCarPosition();
            }
        }
    }

    private void ResetCarPosition()
    {
        if (TryGetComponent(out NetworkTransform nt))
        {
            nt.Teleport(_initialSpawnPosition, _initialSpawnRotation);
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
    }
}