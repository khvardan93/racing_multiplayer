using System.Linq;
using Fusion;
using UnityEngine;
using Zenject;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    [Inject] private ISceneService _sceneService;
    [Inject] private DiContainer _container;

    private void OnEnable()
    {
        FusionEvents.OnPlayerJoined += PlayerJoined;
    }

    private void OnDisable()
    {
        FusionEvents.OnPlayerJoined -= PlayerJoined;
    }

    public void PlayerJoined(PlayerRef player, NetworkRunner runner)
    {
        if (!runner.IsServer)
            return;

        var playerCount = runner.ActivePlayers.Count();
        var spawnPoints = _sceneService.SpawnPoints;

        if (playerCount - 1 >= spawnPoints.Count)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        var spawnPoint = spawnPoints[playerCount - 1];
        var car = runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation, player);

        // Fusion spawns outside Zenject's instantiation path, so inject manually.
        _container.InjectGameObject(car.gameObject);
        _sceneService.SetCameraTarget(car.transform);

        car.gameObject.SetActive(true);
        runner.SetPlayerObject(player, car);
    }
}
