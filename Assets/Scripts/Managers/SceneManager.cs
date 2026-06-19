using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform[] _spawnPoints;
    
    public static SceneManager Instance { get; private set; }

    public IReadOnlyList<Transform> SpawnPoints => _spawnPoints;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    public void SetCameraTarget(Transform target)
    {
        _camera.Follow = target;
    }
}
