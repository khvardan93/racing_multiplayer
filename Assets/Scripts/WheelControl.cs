using UnityEngine;

public class WheelControl : MonoBehaviour {
  public Transform WheelModel;
  public bool Steerable;
  public bool Motorized;

   public WheelCollider WheelCollider;

  private void Update() {
    WheelCollider.GetWorldPose(out var position, out var rotation);
    WheelModel.transform.position = position;
    WheelModel.transform.rotation = rotation;
  }
}