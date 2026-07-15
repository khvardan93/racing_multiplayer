using Managers;
using NUnit.Framework;
using UnityEngine;

public class FusionConnectionManagerTests
{
    private GameObject _gameObject;
    private FusionConnectionManager _connectionManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("FusionConnectionManager");
        _connectionManager = _gameObject.AddComponent<FusionConnectionManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void ConnectionState_EnumHasExpectedValues()
    {
        Assert.That((int)FusionConnectionManager.ConnectionState.Idle, Is.EqualTo(0));
        Assert.That((int)FusionConnectionManager.ConnectionState.Connecting, Is.EqualTo(1));
        Assert.That((int)FusionConnectionManager.ConnectionState.Retrying, Is.EqualTo(2));
        Assert.That((int)FusionConnectionManager.ConnectionState.Connected, Is.EqualTo(3));
        Assert.That((int)FusionConnectionManager.ConnectionState.Failed, Is.EqualTo(4));
    }

    [Test]
    public void MaxRetries_PropertyIsReadable()
    {
        var maxRetries = _connectionManager.MaxRetries;
        Assert.GreaterOrEqual(maxRetries, 0);
    }

    [Test]
    public void InitialRetryDelay_PropertyIsReadable()
    {
        var delay = _connectionManager.InitialRetryDelay;
        Assert.Greater(delay, 0f);
    }

    [Test]
    public void BackoffMultiplier_PropertyIsReadable()
    {
        var multiplier = _connectionManager.BackoffMultiplier;
        Assert.Greater(multiplier, 0f);
    }

    [Test]
    public void RunnerPrefab_PropertyIsReadable()
    {
        var prefab = _connectionManager.RunnerPrefab;
        // Just verifies the property is readable without throwing; it can be null if not assigned.
        _ = prefab;
    }

    [Test]
    public void State_StartsAsIdle()
    {
        Assert.AreEqual(FusionConnectionManager.ConnectionState.Idle, _connectionManager.State);
    }

    [Test]
    public void OnStateChanged_EventExists()
    {
        var eventInfo = typeof(FusionConnectionManager).GetEvent("OnStateChanged",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(eventInfo);
    }

    [Test]
    public void OnConnectionResult_EventExists()
    {
        var eventInfo = typeof(FusionConnectionManager).GetEvent("OnConnectionResult",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(eventInfo);
    }

    [Test]
    public void OnSessionLost_EventExists()
    {
        var eventInfo = typeof(FusionConnectionManager).GetEvent("OnSessionLost",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(eventInfo);
    }

    [Test]
    public void ConnectWithRetry_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("ConnectWithRetry");
        Assert.IsNotNull(method);
    }

    [Test]
    public void Disconnect_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("Disconnect");
        Assert.IsNotNull(method);
    }

    [Test]
    public void SetState_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("SetState",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void HandleSessionLost_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("HandleSessionLost",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void BackoffCalculation_IsExponential()
    {
        // Verify backoff multiplier creates exponential delays
        var initialDelay = _connectionManager.InitialRetryDelay;
        var multiplier = _connectionManager.BackoffMultiplier;

        var delay1 = initialDelay;
        var delay2 = initialDelay * multiplier;
        var delay3 = initialDelay * multiplier * multiplier;

        Assert.Less(delay1, delay2);
        Assert.Less(delay2, delay3);
    }

    [Test]
    public void FusionConnectionManager_ImplementsNetworkRunnerCallbacks()
    {
        var implementsInterface = typeof(Fusion.INetworkRunnerCallbacks).IsAssignableFrom(
            typeof(FusionConnectionManager));
        Assert.IsTrue(implementsInterface);
    }

    [Test]
    public void OnDisconnectedFromServer_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("OnDisconnectedFromServer",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void OnConnectFailed_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("OnConnectFailed",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void OnShutdown_MethodExists()
    {
        var method = typeof(FusionConnectionManager).GetMethod("OnShutdown",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void MaxRetries_DefaultValueIsReasonable()
    {
        var maxRetries = _connectionManager.MaxRetries;
        Assert.GreaterOrEqual(maxRetries, 1);
        Assert.LessOrEqual(maxRetries, 10);
    }

    [Test]
    public void InitialRetryDelay_DefaultValueIsReasonable()
    {
        var delay = _connectionManager.InitialRetryDelay;
        Assert.GreaterOrEqual(delay, 0.1f);
        Assert.LessOrEqual(delay, 10f);
    }

    [Test]
    public void BackoffMultiplier_DefaultValueIsReasonable()
    {
        var multiplier = _connectionManager.BackoffMultiplier;
        Assert.GreaterOrEqual(multiplier, 1f);
        Assert.LessOrEqual(multiplier, 3f);
    }
}
