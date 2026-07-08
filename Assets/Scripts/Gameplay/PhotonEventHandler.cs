using System.Linq;
using Fusion;
using UnityEngine;
using Zenject;

public class PhotonEventHandler : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;

    [Inject] private GameManager _gameManager;
    [Inject] private DiContainer _container;
    [Inject] private InputsManager _inputsManager;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        _inputsManager.OnInput(runner, input);
    }
    
    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        var playerCount = Runner.ActivePlayers.Count();
        var spawnPoints = _gameManager.SpawnPoints;

        if(playerCount == 2) _gameManager.StartTimer();
        
        if (playerCount - 1 >= spawnPoints.Count)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        var spawnPoint = spawnPoints[playerCount - 1];
        var car = Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation, player);

        car.gameObject.SetActive(true);
        Runner.SetPlayerObject(player, car);
    }
}
