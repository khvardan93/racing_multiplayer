using System.Collections.Generic;
using System.Linq;
using Fusion;
using Managers;
using UnityEngine;
using Zenject;

public class PhotonEventHandler : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private GameObject _playerPrefab;

    [Inject] private GameManager _gameManager;
    [Inject] private DiContainer _container;
    [Inject] private InputsManager _inputsManager;
    [Inject] private GameUIManager _gameUIManager;

    // Players pending a car spawn/despawn, drained on the next Update. Fusion's
    // IL2CPP-compiled callback invoker crashes natively (SIGSEGV) if Runner.Spawn
    // is called synchronously from inside PlayerJoined/PlayerLeft - spawning
    // reenters the same simulation callback machinery that is still mid-iteration.
    private readonly Queue<PlayerRef> _pendingJoins = new();
    private readonly Queue<PlayerRef> _pendingLeaves = new();

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        _inputsManager.OnInput(runner, input);
    }

    private void Update()
    {
        if (Runner == null || !Runner.IsServer)
            return;

        while (_pendingJoins.Count > 0)
            SpawnCar(_pendingJoins.Dequeue());

        while (_pendingLeaves.Count > 0)
            DespawnCar(_pendingLeaves.Dequeue());
    }

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        _pendingJoins.Enqueue(player);
    }

    void IPlayerLeft.PlayerLeft(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        _gameUIManager.ShowWinPopup();
        _pendingLeaves.Enqueue(player);
    }

    private void SpawnCar(PlayerRef player)
    {
        var playerCount = Runner.ActivePlayers.Count();
        var spawnPoints = _gameManager.SpawnPoints;

        if (playerCount - 1 >= spawnPoints.Count)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        var spawnPoint = spawnPoints[playerCount - 1];
        var car = Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation, player);

        Runner.SetPlayerObject(player, car);
    }

    private void DespawnCar(PlayerRef player)
    {
        var car = Runner.GetPlayerObject(player);
        if (!car) return;

        Runner.Despawn(car);
        _gameManager.NotifyRivalLeft();
    }
}
