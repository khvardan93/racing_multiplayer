using System.Collections.Generic;
using System.Collections;
using Managers;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineCamera _rivalCamera;
    [SerializeField] private Transform[] _spawnPoints;

    private int _timer;
    private Coroutine _timerCoroutine;
    private CarControl _carControl;
    
    public event Action OnRivalSpawned;
    public event Action<int> OnTimerChange;
    
    public IReadOnlyList<Transform> SpawnPoints => _spawnPoints;
    public CarControl CarControl => _carControl;
    
    [Inject]  private GameUIManager _gameUIManager;
    [Inject] AssetsManager _assetsManager;
    [Inject] GameConfigs _configs;
    [Inject] private FusionConnectionManager _connectionManager;

    private void OnDisable()
    {
        if(_timerCoroutine != null) StopCoroutine(_timerCoroutine);
        _timerCoroutine = null;
    }

    public void SetCameraTarget(Transform target)
    {
        _camera.Follow = target;
        _gameUIManager.ShowGameHud();
    }

    public void RegisterLocalPlayer(CarControl carControl)
    {
        _carControl = carControl;
    }
    
    public void SetRivalCameraTarget(Transform target)
    {
        _rivalCamera.Follow = target;
        _gameUIManager.ShowGameHud();
        OnRivalSpawned?.Invoke();
    }

    public void LeaveGame()
    {
        if (_configs.TryGetScene(GameScenes.Menu, out var scene))
            _assetsManager.LoadScene(scene);
        _connectionManager.Disconnect();
    }

    public void StartTimer()
    {
        if (_timerCoroutine == null && gameObject.activeInHierarchy)
            _timerCoroutine = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        var waiter = new WaitForSeconds(1f);

        while (gameObject.activeInHierarchy)
        {
            _timer += 1;
            OnTimerChange?.Invoke(_timer);
            
            yield return waiter;
        }

        waiter = null;
    }
}
