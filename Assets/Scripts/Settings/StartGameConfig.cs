using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Serializable, designer-friendly config for Fusion's StartGameArgs.
/// Create via Assets > Create > Fusion > Start Game Config.
///
/// Note: Scene and SceneManager are runtime-only objects in Fusion (SceneRef
/// resolves from a build index at runtime, NetworkSceneManagerDefault is a
/// live component), so they can't be serialized directly on the asset.
/// Instead, this stores the *scene name* and builds the SceneRef at runtime,
/// and BuildArgs() accepts the SceneManager as a parameter.
/// </summary>
[CreateAssetMenu(fileName = "StartGameConfig", menuName = "Fusion/Start Game Config")]
public class StartGameConfig : ScriptableObject
{
    [Header("Session")]
    [Tooltip("GameMode to start with (Host, Client, Server, Shared, AutoHostOrClient).")]
    [SerializeField] private GameMode _gameMode = GameMode.Host;

    [Tooltip("Default session/room name. Can be overridden at runtime (e.g. from a UI input field).")]
    [SerializeField] private string _defaultSessionName = "MyRoom";

    [Tooltip("Optional lobby name to group sessions for matchmaking/listing. Leave empty for default lobby.")]
    [SerializeField] private string _customLobbyName = "";

    [Header("Capacity & Visibility")]
    [Tooltip("Maximum number of players allowed in the session.")]
    [Range(1, 64)]
    [SerializeField] private int _playerCount = 8;

    [Tooltip("Whether the session is visible in session list queries (e.g. server browser).")]
    [SerializeField] private bool _isVisible = true;

    [Tooltip("Whether new players can still join the session.")]
    [SerializeField] private bool _isOpen = true;

    [Header("Scene")]
    [Tooltip("Name of the scene to load/sync for this session. Must be added to Build Settings.")]
    [SerializeField] private string _sceneName;

    public GameMode GameMode => _gameMode;
    public string DefaultSessionName => _defaultSessionName;
    public string CustomLobbyName => _customLobbyName;
    public int PlayerCount => _playerCount;
    public bool IsVisible => _isVisible;
    public bool IsOpen => _isOpen;
    public string SceneName => _sceneName;

    /// <summary>
    /// Builds a real StartGameArgs from this config at runtime.
    /// </summary>
    /// <param name="sceneManager">
    /// A NetworkSceneManagerDefault instance (or other INetworkSceneManager)
    /// to handle scene load syncing. Pass null to skip scene management.
    /// </param>
    /// <param name="sessionNameOverride">
    /// Optional override for the session name (e.g. from a UI input field).
    /// Falls back to defaultSessionName if null/empty.
    /// </param>
    public StartGameArgs BuildArgs(INetworkSceneManager sceneManager = null, string sessionNameOverride = null)
    {
        var sessionName = string.IsNullOrEmpty(sessionNameOverride) ? _defaultSessionName : sessionNameOverride;

        var args = new StartGameArgs
        {
            GameMode = _gameMode,
            SessionName = sessionName,
            PlayerCount = _playerCount,
            IsVisible = _isVisible,
            IsOpen = _isOpen,
        };

        if (!string.IsNullOrEmpty(_customLobbyName))
        {
            args.CustomLobbyName = _customLobbyName;
        }

        if (!string.IsNullOrEmpty(_sceneName))
        {
            args.Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(
                GetScenePathByName(_sceneName)));
        }

        if (sceneManager != null)
        {
            args.SceneManager = sceneManager;
        }

        return args;
    }

    private static string GetScenePathByName(string name)
    {
        // Scenes added to Build Settings are referenced by path; this resolves
        // a bare scene name (e.g. "GameScene") to its build path.
        var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (var i = 0; i < sceneCount; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (fileName == name)
            {
                return path;
            }
        }

        Debug.LogError($"[StartGameConfig] Scene '{name}' not found in Build Settings.");
        return string.Empty;
    }
}