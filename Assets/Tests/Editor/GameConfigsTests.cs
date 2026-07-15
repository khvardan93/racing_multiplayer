using Fusion;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConfigsTests
{
    private GameConfigs _configs;

    [SetUp]
    public void SetUp()
    {
        _configs = ScriptableObject.CreateInstance<GameConfigs>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_configs);
    }

    private void SetScenes(params (GameScenes scene, string name)[] items)
    {
        var so = new SerializedObject(_configs);
        var sceneItems = so.FindProperty("_sceneItems");
        sceneItems.arraySize = items.Length;
        for (var i = 0; i < items.Length; i++)
        {
            var element = sceneItems.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("Scene").enumValueIndex = (int)items[i].scene;
            element.FindPropertyRelative("Name").stringValue = items[i].name;
        }
        so.ApplyModifiedPropertiesWithoutUndo();

        // OnEnable populates the lookup dictionary; invoke it directly since
        // CreateInstance doesn't fire Unity's normal enable callback in edit mode tests.
        typeof(GameConfigs)
            .GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(_configs, null);
    }

    [Test]
    public void TryGetScene_ReturnsTrueAndName_WhenSceneIsRegistered()
    {
        SetScenes((GameScenes.Game, "Arena"));

        var found = _configs.TryGetScene(GameScenes.Game, out var name);

        Assert.IsTrue(found);
        Assert.AreEqual("Arena", name);
    }

    [Test]
    public void TryGetScene_ReturnsFalse_WhenSceneIsNotRegistered()
    {
        SetScenes((GameScenes.Menu, "Menu"));

        var found = _configs.TryGetScene(GameScenes.Game, out var name);

        Assert.IsFalse(found);
    }

    [Test]
    public void TryGetScene_ReturnsFalse_WhenRegisteredNameIsEmpty()
    {
        SetScenes((GameScenes.Intro, ""));

        var found = _configs.TryGetScene(GameScenes.Intro, out _);

        Assert.IsFalse(found);
    }

    [Test]
    public void BuildArgs_UsesDefaultSessionName_WhenNoOverrideGiven()
    {
        var so = new SerializedObject(_configs);
        so.FindProperty("_defaultSessionName").stringValue = "DefaultRoom";
        so.ApplyModifiedPropertiesWithoutUndo();

        var args = _configs.BuildArgs();

        Assert.AreEqual("DefaultRoom", args.SessionName);
    }

    [Test]
    public void BuildArgs_UsesOverride_WhenSessionNameOverrideGiven()
    {
        var args = _configs.BuildArgs(sessionNameOverride: "CustomRoom");

        Assert.AreEqual("CustomRoom", args.SessionName);
    }

    [Test]
    public void BuildArgs_OmitsCustomLobbyName_WhenNotConfigured()
    {
        var args = _configs.BuildArgs();

        Assert.IsTrue(string.IsNullOrEmpty(args.CustomLobbyName));
    }

    [Test]
    public void BuildArgs_SetsCustomLobbyName_WhenConfigured()
    {
        var so = new SerializedObject(_configs);
        so.FindProperty("_customLobbyName").stringValue = "MyLobby";
        so.ApplyModifiedPropertiesWithoutUndo();

        var args = _configs.BuildArgs();

        Assert.AreEqual("MyLobby", args.CustomLobbyName);
    }

    [Test]
    public void BuildArgs_SetsPlayerCount_FromConfig()
    {
        var so = new SerializedObject(_configs);
        so.FindProperty("_playerCount").intValue = 16;
        so.ApplyModifiedPropertiesWithoutUndo();

        var args = _configs.BuildArgs();

        Assert.AreEqual(16, args.PlayerCount);
    }

    [Test]
    public void BuildArgs_SetsVisibility_FromConfig()
    {
        var so = new SerializedObject(_configs);
        so.FindProperty("_isVisible").boolValue = false;
        so.ApplyModifiedPropertiesWithoutUndo();

        var args = _configs.BuildArgs();

        Assert.IsFalse(args.IsVisible);
    }

    [Test]
    public void BuildArgs_SetsIsOpen_FromConfig()
    {
        var so = new SerializedObject(_configs);
        so.FindProperty("_isOpen").boolValue = false;
        so.ApplyModifiedPropertiesWithoutUndo();

        var args = _configs.BuildArgs();

        Assert.IsFalse(args.IsOpen);
    }

    [Test]
    public void BuildArgs_SetsGameMode_FromConfig()
    {
        var so = new SerializedObject(_configs);
        so.FindProperty("_gameMode").enumValueIndex = (int)GameMode.Client;
        so.ApplyModifiedPropertiesWithoutUndo();

        var args = _configs.BuildArgs();

        Assert.AreEqual(GameMode.Client, args.GameMode);
    }

    [Test]
    public void BuildArgs_AcceptsSceneManager_WhenProvided()
    {
        var mockManager = new MockNetworkSceneManager();

        var args = _configs.BuildArgs(sceneManager: mockManager);

        Assert.AreEqual(mockManager, args.SceneManager);
    }

    private class MockNetworkSceneManager : INetworkSceneManager
    {
        public void Initialize(NetworkRunner runner)
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public bool IsRunnerScene(Scene scene)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPhysicsScene2D(out PhysicsScene2D scene2D)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPhysicsScene3D(out PhysicsScene scene3D)
        {
            throw new System.NotImplementedException();
        }

        public void MakeDontDestroyOnLoad(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        public bool MoveGameObjectToScene(GameObject gameObject, SceneRef sceneRef)
        {
            throw new System.NotImplementedException();
        }

        public NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, NetworkLoadSceneParameters parameters)
        {
            throw new System.NotImplementedException();
        }

        NetworkSceneAsyncOp INetworkSceneManager.UnloadScene(SceneRef sceneRef)
        {
            throw new System.NotImplementedException();
        }

        public SceneRef GetSceneRef(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }

        public SceneRef GetSceneRef(string sceneNameOrPath)
        {
            throw new System.NotImplementedException();
        }

        public bool OnSceneInfoChanged(NetworkSceneInfo sceneInfo, NetworkSceneInfoChangeSource changeSource)
        {
            throw new System.NotImplementedException();
        }

        public bool IsBusy { get; }
        public Scene MainRunnerScene { get; }
    }
}
