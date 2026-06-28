using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SceneManager : MonoBehaviour, ISceneService
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform[] _spawnPoints;

    public IReadOnlyList<Transform> SpawnPoints => _spawnPoints;

    public void SetCameraTarget(Transform target)
    {
        _camera.Follow = target;
    }
}
