using System.Linq;
using Fusion;
using UnityEngine;
using Zenject;

public class PhotonEventHandler : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;

    [Inject] private ISceneService _sceneService;
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
        var spawnPoints = _sceneService.SpawnPoints;

        if (playerCount - 1 >= spawnPoints.Count)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        var spawnPoint = spawnPoints[playerCount - 1];
        var car = Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation, player);

        // Fusion spawns outside Zenject's instantiation path, so inject manually.
        //_container.InjectGameObject(car.gameObject);
        
       // if(player == Runner.LocalPlayer) _sceneService.SetCameraTarget(car.transform);

        car.gameObject.SetActive(true);
        Runner.SetPlayerObject(player, car);
    }
}
