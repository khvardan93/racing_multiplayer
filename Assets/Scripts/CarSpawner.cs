using Fusion;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

// SimulationBehaviour found on the Runner object that waits until a player joins
// and spawns a player object if it is the local player.
// To prevent players spawning on top of each other, the player object spawn
// position are offset depending on how many active players there are.
public class CarSpawner : SimulationBehaviour, IPlayerJoined
{

  [SerializeField] private GameObject _playerPrefab;
  [SerializeField] private Transform _spawnPoint;
  [SerializeField] private Vector3 _firstSpawnPosition = new Vector3(0f, 0f, 0f);
  [SerializeField] private Vector3 _spawnOffsetForEachPlayer = new Vector3(3f, 0f, 0f);
  [SerializeField] private CinemachineCamera _camera;

  public void PlayerJoined(PlayerRef player)
  {
    if (player == Runner.LocalPlayer)
    {
      /*int playerCount = Runner.ActivePlayers.Count();
      // To prevent collisions, offset the spawn position depending on how many active players there are.
      var spawnPosition = _firstSpawnPosition + (_spawnOffsetForEachPlayer * (playerCount - 1));*/

      var car = Runner.Spawn(_playerPrefab, _spawnPoint.position, _spawnPoint.rotation, player);

      car.gameObject.SetActive(true);
      _camera.Follow = car.transform;
      
      Runner.SetPlayerObject(player, car);
    }
  }
}