using System.Linq;
using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class CarSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    

    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;
        
        var playerCount = Runner.ActivePlayers.Count();
        var spawnPoints = SceneManager.Instance.SpawnPoints;

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