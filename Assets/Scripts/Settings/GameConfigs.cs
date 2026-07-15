using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScenes
{
    Intro,
    Menu,
    Game,
}

[CreateAssetMenu(fileName = "GameConfigs", menuName = "Game Configs")]
public class GameConfigs : ScriptableObject
{
    [Serializable]
    struct SceneItem
    {
        public GameScenes Scene;
        public string Name;
    }
    
    [Header("Session")]
    [SerializeField] private GameMode _gameMode = GameMode.Host;
    [SerializeField] private string _defaultSessionName = "MyRoom";
    [SerializeField] private string _customLobbyName = "";

    [Header("Capacity & Visibility")]
    [Range(1, 64)]
    [SerializeField] private int _playerCount = 8;
    [SerializeField] private bool _isVisible = true;
    [SerializeField] private bool _isOpen = true;

    [Header("Scene")]
    //[SerializeField] private string _sceneName;
    [SerializeField] private  SceneItem[] _sceneItems;
    
    private readonly Dictionary<GameScenes, string> _scenes = new();

    private void OnEnable()
    {
        foreach (var sceneItem in _sceneItems)
        {
            _scenes[sceneItem.Scene] = sceneItem.Name;
        }
    }

    public bool TryGetScene(GameScenes gameScene, out string scene)
    {
        if(!_scenes.TryGetValue(gameScene, out scene) || string.IsNullOrEmpty(scene))
        {
            Debug.LogError($"{gameScene} Scene not found.");
            return false;
        }

        return true;
    }

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

        /*if (!string.IsNullOrEmpty(_sceneName))
        {
            args.Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(
                GetScenePathByName(_sceneName)));
        }*/

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
        var sceneCount = SceneManager.sceneCountInBuildSettings;
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