using Fusion;
using UnityEngine;

public class CarControl : NetworkBehaviour
{
    public float MotorTorque = 2000;
    public float BrakeTorque = 2000;
    public float MaxSpeed = 20;
    public float SteeringRange = 30;
    public float SteeringRangeAtMaxSpeed = 10;
    public float CentreOfGravityOffset = -1f;
    public float HandBreakDrag = 2.5f;
    public float BreakingDrag = 2.5f;
    public float CoastingDrag = 1f;

    FloatCompressed VerticalInput { get; set; }
    FloatCompressed HorizontalInput { get; set; }
    NetworkBool HandBreakInput { get; set; }

    [SerializeField] private WheelControl[] _wheels;
    private Rigidbody _rigidBody;
    private float _defaultDrag;
    private Vector3 _initialSpawnPosition;
    private Quaternion _initialSpawnRotation;
    private bool _spawned;

    public override void Spawned()
    {
        // EVERYONE needs to know where this car spawned so 
        // predictions match during a reset execution.
        _initialSpawnPosition = transform.position;
        _initialSpawnRotation = transform.rotation;
        
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.centerOfMass += Vector3.up * CentreOfGravityOffset;
        _defaultDrag = _rigidBody.linearDamping;
        
        _spawned = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!_spawned || !transform || !_rigidBody) return;
        
        // 1. Process inputs for BOTH Host and Client loops
        FetchNetworkInputs();

        // 2. Physics & driving calculations run smoothly on both sides now
        float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, MaxSpeed, forwardSpeed);
        float currentMotorTorque = Mathf.Lerp(MotorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(SteeringRange, SteeringRangeAtMaxSpeed, speedFactor);

        bool isAccelerating = Mathf.Sign(VerticalInput) == Mathf.Sign(forwardSpeed);
        bool isGrounded = false;

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
                wheel.WheelCollider.brakeTorque = Mathf.Abs(VerticalInput) * BrakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }

    private void Drag(bool isGrounded, bool isAccelerating)
    {
        if (HandBreakInput && isGrounded)
        {
            _rigidBody.linearDamping = HandBreakDrag;
        }
        else if (VerticalInput == 0f && isGrounded)
        {
            _rigidBody.linearDamping = CoastingDrag;
        }
        else if (isAccelerating)
        {
            _rigidBody.linearDamping = _defaultDrag;
        }
        else if (isGrounded)
        {
            _rigidBody.linearDamping = BreakingDrag;
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
            VerticalInput = inputs.Vertical;
            HorizontalInput = inputs.Horizontal;
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