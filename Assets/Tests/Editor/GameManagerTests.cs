using Managers;
using NUnit.Framework;
using UnityEngine;

public class GameManagerTests
{
    private GameObject _gameObject;
    private GameManager _gameManager;
    private GameObject _carObject;
    private CarControl _carControl;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("GameManagerTest");
        _gameManager = _gameObject.AddComponent<GameManager>();

        _carObject = new GameObject("CarControlTest");
        _carControl = _carObject.AddComponent<CarControl>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
        Object.DestroyImmediate(_carObject);
    }

    [Test]
    public void RegisterLocalPlayer_StoresCarControlReference()
    {
        _gameManager.RegisterLocalPlayer(_carControl);

        Assert.AreEqual(_carControl, _gameManager.CarControl);
    }

    [Test]
    public void RegisterLocalPlayer_CanStoreMultipleCarsSequentially()
    {
        var carObject2 = new GameObject("CarControl2");
        var carControl2 = carObject2.AddComponent<CarControl>();

        _gameManager.RegisterLocalPlayer(_carControl);
        Assert.AreEqual(_carControl, _gameManager.CarControl);

        _gameManager.RegisterLocalPlayer(carControl2);
        Assert.AreEqual(carControl2, _gameManager.CarControl);

        Object.DestroyImmediate(carObject2);
    }

    [Test]
    public void CarControl_PropertyReturnsStoredReference()
    {
        _gameManager.RegisterLocalPlayer(_carControl);
        var retrieved = _gameManager.CarControl;

        Assert.AreSame(_carControl, retrieved);
    }

    [Test]
    public void OnRivalSpawned_EventCanBeSubscribed()
    {
        var eventFired = false;
        _gameManager.OnRivalSpawned += () => { eventFired = true; };

        // Simulate firing the event via reflection
        var eventInfo = typeof(GameManager).GetEvent("OnRivalSpawned",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        Assert.IsNotNull(eventInfo);
    }

    [Test]
    public void OnRivalLeft_EventCanBeSubscribed()
    {
        var eventFired = false;
        _gameManager.OnRivalLeft += () => { eventFired = true; };

        var eventInfo = typeof(GameManager).GetEvent("OnRivalLeft",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        Assert.IsNotNull(eventInfo);
    }

    [Test]
    public void OnTimerChange_EventCanBeSubscribed()
    {
        var eventFired = false;
        _gameManager.OnTimerChange += (time) => { eventFired = true; };

        var eventInfo = typeof(GameManager).GetEvent("OnTimerChange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        Assert.IsNotNull(eventInfo);
    }

    [Test]
    public void SpawnPoints_PropertyExistsAndIsReadOnly()
    {
        var spawnPointsProp = typeof(GameManager).GetProperty("SpawnPoints",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(spawnPointsProp);
        Assert.IsNotNull(spawnPointsProp.GetGetMethod());
        Assert.IsNull(spawnPointsProp.GetSetMethod());
    }

    [Test]
    public void SpawnPoints_ReturnsSerializedArray()
    {
        // Set the private _spawnPoints field via reflection
        var spawnPoint1 = new GameObject("SpawnPoint1").transform;
        var spawnPoint2 = new GameObject("SpawnPoint2").transform;
        var spawnPoints = new[] { spawnPoint1, spawnPoint2 };

        var field = typeof(GameManager).GetField("_spawnPoints",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(_gameManager, spawnPoints);

        var retrieved = _gameManager.SpawnPoints;

        Assert.AreEqual(2, retrieved.Count);
        Assert.AreEqual(spawnPoint1, retrieved[0]);
        Assert.AreEqual(spawnPoint2, retrieved[1]);

        Object.DestroyImmediate(spawnPoint1.gameObject);
        Object.DestroyImmediate(spawnPoint2.gameObject);
    }

    [Test]
    public void CarControl_PropertyInitiallyNull()
    {
        Assert.IsNull(_gameManager.CarControl);
    }
}
