using UnityEngine;

public class CarWheel : MonoBehaviour
{
    [SerializeField] private WheelCollider _collider;
    [SerializeField] private Transform _visualTransform;
    
    public WheelCollider Collider => _collider;
    public Transform VisualTransform => _visualTransform;
}
