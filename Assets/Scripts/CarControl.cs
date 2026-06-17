using Fusion;
using UnityEngine;

// Simple car control script adapted from the Unity tutorial: https://docs.unity3d.com/2021.3/Documentation/Manual/WheelColliderTutorial.html
// Adjustments have been made to add Fusion networking support and to adjust car handling.
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

  // The inputs from the client with state authority over the car are networked so that other clients can use
  // the same inputs in their physics simulation. Errors between the clients are detected and corrected using
  // the forecast physics system.
  [Networked] FloatCompressed VerticalInput { get; set; }
  [Networked] FloatCompressed HorizontalInput { get; set; }
  [Networked] NetworkBool HandBreakInput { get; set; }

  private WheelControl[] _wheels;
  private Rigidbody _rigidBody;
  private float _defaultDrag;
  private bool _resetPressed = false;
  private Vector3 _initialSpawnPosition;
  private Quaternion _initialSpawnRotation;

  // Start is called before the first frame update
  void Start()
  {
    _rigidBody = GetComponent<Rigidbody>();

    // Adjust center of mass vertically, to help prevent the car from rolling
    _rigidBody.centerOfMass += Vector3.up * CentreOfGravityOffset;

    // Find all child GameObjects that have the WheelControl script attached
    _wheels = GetComponentsInChildren<WheelControl>();

    // Record the default drag set on the rigid body so that it can be set back to this after it is adjusted
    _defaultDrag = _rigidBody.linearDamping;
  }

  public override void Spawned()
  {

    // When this car is spawned set up certain things that is only a concern to the client that has state
    // authority over the care.g the player driving the car.
    if (Object.HasStateAuthority)
    {

      // Record the spawn position of the car so that the car can be teleported back to this location when
      // the player presses Reset
      _initialSpawnPosition = transform.position;
      _initialSpawnRotation = transform.rotation;

      // Initialize the camera for the car
      /*if (TryGetComponent<VehicleCamera>(out var camera))
      {
        camera.Init();
      }*/
    }
  }

  private void Update()
  {
    
    Debug.Log(
      $"Input={Object.HasInputAuthority} State={Object.HasStateAuthority}"
    );

    // Check for the button down state here in Update() as this is only true for a frame and it may be missed
    // in FixedUpdate()
    if (Object.HasStateAuthority && Input.GetButtonDown("Reset"))
    {
      _resetPressed = true;
    }
  }

  public override void FixedUpdateNetwork()
  {
    if(!transform || !_rigidBody) return;
    
    Inputs();

    // Calculate current speed in relation to the forward direction of the car
    // (this returns a negative number when traveling backwards)
    float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);

    // Calculate how close the car is to top speed
    // as a number from zero to one
    float speedFactor = Mathf.InverseLerp(0, MaxSpeed, forwardSpeed);

    // Use that to calculate how much torque is available 
    // (zero torque at top speed)
    float currentMotorTorque = Mathf.Lerp(MotorTorque, 0, speedFactor);

    // �and to calculate how much to steer 
    // (the car steers more gently at top speed)
    float currentSteerRange = Mathf.Lerp(SteeringRange, SteeringRangeAtMaxSpeed, speedFactor);

    // Check whether the user input is in the same direction 
    // as the car's velocity
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

      // Apply steering to Wheel colliders that have "Steerable" enabled
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
        // Apply torque to Wheel colliders that have "Motorized" enabled
        if (wheel.Motorized)
        {
          wheel.WheelCollider.motorTorque = VerticalInput * currentMotorTorque;
        }

        wheel.WheelCollider.brakeTorque = 0;
      }
      else
      {
        // If the user is trying to go in the opposite direction
        // apply brakes to all wheels
        wheel.WheelCollider.brakeTorque = Mathf.Abs(VerticalInput) * BrakeTorque;
        wheel.WheelCollider.motorTorque = 0;
      }
    }
  }

  private void Drag(bool isGrounded, bool isAccelerating)
  {
    // Dynamically adjust the car drag to help with handling
    if (HandBreakInput && isGrounded)
    {
      _rigidBody.linearDamping = HandBreakDrag;
    }
    else if (VerticalInput == 0f && isGrounded)
    {
      // Let go of accelerator and just coasting
      _rigidBody.linearDamping = CoastingDrag;
    }
    else if (isAccelerating)
    {
      // Either accelerator or reverse pressed
      _rigidBody.linearDamping = _defaultDrag;
    }
    else if (isGrounded)
    {
      // Use input is in opposite direction of car body, so the player is trying to stop
      _rigidBody.linearDamping = BreakingDrag;
    }
    else
    {
      _rigidBody.linearDamping = _defaultDrag;
    }
  }

  private void Inputs()
  {
    if (!Object.HasStateAuthority) return;
    
    if (_resetPressed)
    {
      Reset();
      _resetPressed = false;
    }

    var accelButton = Input.GetAxis("Accelerate");
    VerticalInput = accelButton <= 0 ? Input.GetAxis("Vertical") : accelButton;
    HorizontalInput = Input.GetAxis("Horizontal");
    HandBreakInput = Input.GetAxis("HandBreak") > 0;

    var brakeTrigger = Input.GetAxis("BrakeTriggerXBox");
    var accelTrigger = Input.GetAxis("AccelTriggerXBox");
    if (accelTrigger != 0) VerticalInput = accelTrigger;
    if (brakeTrigger != 0) VerticalInput = -brakeTrigger;

    var breakButton = Input.GetAxis("Break");

    if (breakButton > 0)
    {
      VerticalInput = -1;
    }
  }

  // Reset method to teleport the car back to its starting position
  private void Reset()
  {

    if (TryGetComponent(out NetworkTransform nt))
    {

      nt.Teleport(_initialSpawnPosition, _initialSpawnRotation);

      // Remove all velocity when resetting
      _rigidBody.linearVelocity = Vector3.zero;
      _rigidBody.angularVelocity = Vector3.zero;
    }

  }

}
