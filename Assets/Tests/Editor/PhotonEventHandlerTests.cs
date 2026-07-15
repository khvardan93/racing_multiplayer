using Managers;
using NUnit.Framework;
using UnityEngine;

public class PhotonEventHandlerTests
{
    private GameObject _handlerObject;
    private PhotonEventHandler _handler;
    private GameObject _managerObject;
    private GameManager _gameManager;

    [SetUp]
    public void SetUp()
    {
        // Create GameManager mock
        _managerObject = new GameObject("GameManager");
        _gameManager = _managerObject.AddComponent<GameManager>();

        // Setup GameManager spawn points
        var spawnPoint1 = new GameObject("SpawnPoint1").transform;
        var spawnPoint2 = new GameObject("SpawnPoint2").transform;
        spawnPoint1.position = Vector3.zero;
        spawnPoint2.position = new Vector3(5, 0, 0);

        var spawnPointsField = typeof(GameManager).GetField("_spawnPoints",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        spawnPointsField.SetValue(_gameManager, new[] { spawnPoint1, spawnPoint2 });

        // Create PhotonEventHandler
        _handlerObject = new GameObject("PhotonEventHandler");
        _handler = _handlerObject.AddComponent<PhotonEventHandler>();

        // Set injected fields via reflection
        var gameManagerField = typeof(PhotonEventHandler).GetField("_gameManager",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameManagerField.SetValue(_handler, _gameManager);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_handlerObject);
        Object.DestroyImmediate(_managerObject);
    }

    [Test]
    public void PhotonEventHandler_ImplementsPlayerJoinedInterface()
    {
        var implementsInterface = typeof(Fusion.IPlayerJoined).IsAssignableFrom(typeof(PhotonEventHandler));
        Assert.IsTrue(implementsInterface);
    }

    [Test]
    public void PhotonEventHandler_ImplementsPlayerLeftInterface()
    {
        var implementsInterface = typeof(Fusion.IPlayerLeft).IsAssignableFrom(typeof(PhotonEventHandler));
        Assert.IsTrue(implementsInterface);
    }

    [Test]
    public void PhotonEventHandler_HasPlayerPrefabField()
    {
        var field = typeof(PhotonEventHandler).GetField("_playerPrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void PhotonEventHandler_HasPendingJoinsQueue()
    {
        var field = typeof(PhotonEventHandler).GetField("_pendingJoins",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void PhotonEventHandler_HasPendingLeavesQueue()
    {
        var field = typeof(PhotonEventHandler).GetField("_pendingLeaves",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void PhotonEventHandler_SpawnCarMethodExists()
    {
        var method = typeof(PhotonEventHandler).GetMethod("SpawnCar",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void PhotonEventHandler_DespawnCarMethodExists()
    {
        var method = typeof(PhotonEventHandler).GetMethod("DespawnCar",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void PhotonEventHandler_OnInputMethodExists()
    {
        var method = typeof(PhotonEventHandler).GetMethod("OnInput",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }
}
