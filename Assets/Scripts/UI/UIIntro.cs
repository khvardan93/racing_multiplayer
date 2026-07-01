using TMPro;
using UnityEngine;
using Zenject;

public class UIIntro : MonoBehaviour
{
    [SerializeField] private TMP_Text _logText;
    [SerializeField] private UIProgressBar _progressBar;
    
    [Inject] private FusionConnectionManager _connectionManager;

    //test
    private void Start()
    {
        _connectionManager.Connect();
    }

    private void OnEnable()
    {
        _connectionManager.OnStateChanged += HandleStateChanged;
        _connectionManager.OnConnectionResult += HandleConnectionResult;
    }

    private void OnDisable()
    {
        _connectionManager.OnStateChanged -= HandleStateChanged;
        _connectionManager.OnConnectionResult -= HandleConnectionResult;
    }

    private void HandleStateChanged(FusionConnectionManager.ConnectionState state, int attempt, int maxRetries)
    {
        switch (state)
        {
            case FusionConnectionManager.ConnectionState.Connecting:
                _logText.text = "CONNECTING TO MATCH ...";
                _progressBar.AnimatedSet(0f);
                break;
            case FusionConnectionManager.ConnectionState.Retrying:
                _logText.text = $"RETRYING CONNECTION ({attempt}/{maxRetries}) ...";
                _progressBar.AnimatedSet((float)attempt / (maxRetries + 1));
                break;
            case FusionConnectionManager.ConnectionState.Connected:
                _logText.text = "WAITING FOR PLAYERS TO LOAD IN ...";
                _progressBar.AnimatedSet(1f);
                break;
            case FusionConnectionManager.ConnectionState.Failed:
                _logText.text = "CONNECTION FAILED - PLEASE RESTART";
                break;
        }
    }

    private void HandleConnectionResult(Fusion.NetworkRunner runner)
    {
        if (runner == null)
        {
            _logText.text = "CONNECTION FAILED - PLEASE RESTART";
        }
    }
}
