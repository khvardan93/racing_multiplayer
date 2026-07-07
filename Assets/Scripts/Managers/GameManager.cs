using System.Collections.Generic;
using Managers;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using System;

public class GameManager : MonoBehaviour, ISceneService
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineCamera _rivalCamera;
    [SerializeField] private Transform[] _spawnPoints;

    public event Action OnRivalSpawned;
    
    public IReadOnlyList<Transform> SpawnPoints => _spawnPoints;
    
    [Inject]  private GameUIManager _gameUIManager;

    public void SetCameraTarget(Transform target)
    {
        _camera.Follow = target;
        _gameUIManager.ShowGameHud();
    }
    
    public void SetRivalCameraTarget(Transform target)
    {
        _rivalCamera.Follow = target;
        _gameUIManager.ShowGameHud();
        OnRivalSpawned?.Invoke();
    }
}
