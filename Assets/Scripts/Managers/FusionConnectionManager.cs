using System;
using System.Threading.Tasks;
using Fusion;
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
public class FusionConnectionManager : MonoBehaviour
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

    [Inject] private StartGameConfig _configs;
    [Inject] private NetworkSceneManager _sceneManager;
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

    private NetworkRunner _activeRunner;

    private void Start()
    {
        
        ConnectWithRetry(_configs.BuildArgs(_sceneManager));
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

            // Clean up the failed runner before retrying.
            if (runner != null)
            {
                Destroy(runner.gameObject);
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
        if (_activeRunner != null)
        {
            _activeRunner.Shutdown();
            _activeRunner = null;
        }

        SetState(ConnectionState.Idle, 0);
    }

    private void SetState(ConnectionState newState, int attempt)
    {
        State = newState;
        OnStateChanged?.Invoke(newState, attempt, _maxRetries);
        Debug.Log($"[FusionConnectionManager] State: {newState} (attempt {attempt}/{_maxRetries})");
    }
}