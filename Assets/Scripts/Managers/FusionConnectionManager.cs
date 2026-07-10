using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Zenject;

/// <summary>
/// Wraps NetworkRunner.StartGame with automatic retry logic.
/// Useful because the first connection attempt to Photon's Name Server can
/// time out due to NAT mapping setup (common UDP behavior), even though a
/// retry almost always succeeds instantly.
///
/// Use this instead of (or alongside) FusionBootstrap's default connect call.
/// </summary>
public class FusionConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public enum ConnectionState
    {
        Idle,
        Connecting,
        Retrying,
        Connected,
        Failed
    }

    [Header("Retry Settings")]
    [Tooltip("How many times to retry before giving up and reporting failure.")]
    [SerializeField] private int _maxRetries = 3;

    [Tooltip("Delay before the first retry attempt, in seconds.")]
    [SerializeField] private float _initialRetryDelay = 1f;

    [Tooltip("Multiplier applied to the delay after each failed attempt (backoff).")]
    [SerializeField] private float _backoffMultiplier = 1.5f;

    [Header("References")]
    [SerializeField] private NetworkRunner _runnerPrefab;

    [Inject] private GameConfigs _configs;
    //[Inject] private NetworkSceneManager _sceneManager;
    [Inject] private DiContainer _container;

    public int MaxRetries => _maxRetries;
    public float InitialRetryDelay => _initialRetryDelay;
    public float BackoffMultiplier => _backoffMultiplier;
    public NetworkRunner RunnerPrefab => _runnerPrefab;

    public ConnectionState State { get; private set; } = ConnectionState.Idle;

    // Fired with the current state any time it changes - hook your UI here.
    public event Action<ConnectionState, int /*attempt*/, int /*maxRetries*/> OnStateChanged;

    // Fired once on final success/failure with the runner (or null on failure).
    public event Action<NetworkRunner> OnConnectionResult;

    // Fired when an already-connected session drops (server shutdown or lost connection).
    public event Action OnSessionLost;

    private NetworkRunner _activeRunner;

    private void Start()
    {
        
        ConnectWithRetry(_configs.BuildArgs());
    }

    /// <summary>
    /// Starts the game/session with retry logic. Mirrors the relevant fields
    /// from NetworkRunner.StartGame's StartGameArgs.
    /// </summary>
    public async void ConnectWithRetry(StartGameArgs args)
    {
        var attempt = 0;
        var delay = _initialRetryDelay;

        while (attempt <= _maxRetries)
        {
            attempt++;

            SetState(attempt == 1 ? ConnectionState.Connecting : ConnectionState.Retrying, attempt);

            // Always spin up a fresh runner for each attempt - reusing a runner
            // that failed to start can leave it in a bad internal state.
            var runner = Instantiate(_runnerPrefab);
            runner.name = $"NetworkRunner (attempt {attempt})";
            runner.AddCallbacks(this);
            _container.InjectGameObject(runner.gameObject);

            var result = await runner.StartGame(args);

            if (result.Ok)
            {
                _activeRunner = runner;
                SetState(ConnectionState.Connected, attempt);
                OnConnectionResult?.Invoke(runner);
                return;
            }

            Debug.LogWarning($"[FusionConnectionManager] Attempt {attempt} failed: " +
                              $"{result.ShutdownReason} - {result.ErrorMessage}");

            // Clean up the failed runner before retrying. Shutdown() must be used
            // instead of a bare Destroy() - destroying the GameObject directly can
            // leave Fusion's internal simulation still ticking against components
            // that are mid-destruction, which crashes IL2CPP builds with a native
            // SIGSEGV inside the player-joined/left callback invoker.
            if (runner != null)
            {
                runner.Shutdown(true, ShutdownReason.Error, true);
            }

            if (attempt > _maxRetries)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(delay));
            delay *= _backoffMultiplier;
        }

        SetState(ConnectionState.Failed, attempt);
        OnConnectionResult?.Invoke(null);
    }

    public void Disconnect()
    {
        Debug.Log($"[FusionConnectionManager] Disconnect");

        if (_activeRunner != null)
        {
            var runner = _activeRunner;
            _activeRunner = null;
            runner.Shutdown();
        }

        SetState(ConnectionState.Idle, 0);
    }

    private void SetState(ConnectionState newState, int attempt)
    {
        State = newState;
        OnStateChanged?.Invoke(newState, attempt, _maxRetries);
        Debug.Log($"[FusionConnectionManager] State: {newState} (attempt {attempt}/{_maxRetries})");
    }

    private void HandleSessionLost(NetworkRunner runner)
    {
        if (runner != _activeRunner)
            return;

        _activeRunner = null;
        SetState(ConnectionState.Idle, 0);
        OnSessionLost?.Invoke();
    }

    #region INetworkRunnerCallbacks

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogWarning($"[FusionConnectionManager] Disconnected from server: {reason}");
        HandleSessionLost(runner);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogWarning($"[FusionConnectionManager] Connect failed: {reason}");
        HandleSessionLost(runner);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[FusionConnectionManager] Runner shutdown: {shutdownReason}");
        HandleSessionLost(runner);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    #endregion
}