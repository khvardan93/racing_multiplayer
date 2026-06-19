using UnityEngine;

public class WheelControl : MonoBehaviour {
  [Header("Wheel Visual")]
  [SerializeField] private Transform _wheelModel;

  [Header("Wheel Configuration")]
  [SerializeField] private bool _steerable;
  [SerializeField] private bool _motorized;
  [SerializeField] private WheelCollider _wheelCollider;

  public WheelCollider WheelCollider => _wheelCollider;
  public bool Steerable => _steerable;
  public bool Motorized => _motorized;

  private void Update() {
    _wheelCollider.GetWorldPose(out var position, out var rotation);
    _wheelModel.transform.position = position;
    _wheelModel.transform.rotation = rotation;
  }
}