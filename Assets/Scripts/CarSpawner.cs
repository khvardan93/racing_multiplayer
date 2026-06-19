using System.Linq;
using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class CarSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private CinemachineCamera _camera;

    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;
        
        var playerCount = Runner.ActivePlayers.Count();

        if (playerCount - 1 >= _spawnPoints.Length)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        var spawnPoint = _spawnPoints[playerCount - 1];
        var car = Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation, player);

        car.gameObject.SetActive(true);

        if (player == Runner.LocalPlayer)
        {
            _camera.Follow = car.transform;
        }

        Runner.SetPlayerObject(player, car);
    }
}