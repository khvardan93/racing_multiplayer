using System;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class FusionConnectionManager
{
    public enum ConnectionState
    {
        Idle,
        Connecting,
        Retrying,
        Connected,
        Failed
    }

    [Inject] private NetworkRunner _runnerPrefab;

    [Inject] private StartGameConfig _configs;
    [Inject] private NetworkSceneManager _sceneManager;
    [Inject] private DiContainer _container;

    public NetworkRunner RunnerPrefab => _runnerPrefab;

    public ConnectionState State { get; private set; } = ConnectionState.Idle;

    // Fired with the current state any time it changes - hook your UI here.
    public event Action<ConnectionState, int /*attempt*/, int /*maxRetries*/> OnStateChanged;

    // Fired once on final success/failure with the runner (or null on failure).
    public event Action<NetworkRunner> OnConnectionResult;

    private NetworkRunner _activeRunner;

    public void Connect()
    {
        ConnectWithRetry();
    }

    private async void ConnectWithRetry()
    {
        var args = _configs.BuildArgs(_sceneManager);
        
        var attempt = 0;
        var delay = _configs.InitialRetryDelay;
        var maxRetries = _configs.MaxRetries;
        var backoffMultiplier = _configs.BackoffMultiplier;

        while (attempt <= maxRetries)
        {
            attempt++;

            SetState(attempt == 1 ? ConnectionState.Connecting : ConnectionState.Retrying, attempt);

            // Always spin up a fresh runner for each attempt - reusing a runner
            // that failed to start can leave it in a bad internal state.
            var runner = _runnerPrefab;
            runner.name = $"NetworkRunner (attempt {attempt})";
           // _container.InjectGameObject(runner.gameObject);

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
                Object.Destroy(runner.gameObject);
            }

            if (attempt > maxRetries)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(delay));
            delay *= backoffMultiplier;
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
        var maxRetries = _configs.MaxRetries;
        State = newState;
        OnStateChanged?.Invoke(newState, attempt, maxRetries);
        Debug.Log($"[FusionConnectionManager] State: {newState} (attempt {attempt}/{maxRetries})");
    }
}