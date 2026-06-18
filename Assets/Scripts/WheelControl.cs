using UnityEngine;

// Simple wheel control script from the Unity tutorial: https://docs.unity3d.com/2021.3/Documentation/Manual/WheelColliderTutorial.html 
public class WheelControl : MonoBehaviour {
  public Transform WheelModel;
  public bool Steerable;
  public bool Motorized;

   public WheelCollider WheelCollider;

  Vector3 _position;
  Quaternion _rotation;


  // Update is called once per frame
  void Update() {
    // Get the Wheel collider's world pose values and
    // use them to set the wheel model's position and rotation
    WheelCollider.GetWorldPose(out _position, out _rotation);
    WheelModel.transform.position = _position;
    WheelModel.transform.rotation = _rotation;
  }
}