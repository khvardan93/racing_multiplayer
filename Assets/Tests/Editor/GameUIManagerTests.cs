using Managers;
using NUnit.Framework;
using UnityEngine;
using UI;

public class GameUIManagerTests
{
    private GameObject _gameObject;
    private GameUIManager _gameUIManager;

    private GameObject _loadingScreenObj;
    private GameObject _pausedPopupObj;
    private GameObject _settingsPopupObj;
    private GameObject _losePopupObj;
    private GameObject _winPopupObj;
    private GameObject _gameHudObj;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("GameUIManager");
        _gameUIManager = _gameObject.AddComponent<GameUIManager>();

        // Create mock UI components
        _loadingScreenObj = new GameObject("LoadingScreen");
        var loadingScreen = _loadingScreenObj.AddComponent<UILoadingScreen>();

        _pausedPopupObj = new GameObject("PausedPopup");
        var pausedPopup = _pausedPopupObj.AddComponent<UIPausedPopup>();

        _settingsPopupObj = new GameObject("SettingsPopup");
        var settingsPopup = _settingsPopupObj.AddComponent<UISettingsPopup>();

        _losePopupObj = new GameObject("LosePopup");
        var losePopup = _losePopupObj.AddComponent<UILosePopup>();

        _winPopupObj = new GameObject("WinPopup");
        var winPopup = _winPopupObj.AddComponent<UIWinPopup>();

        _gameHudObj = new GameObject("GameHud");
        var gameHud = _gameHudObj.AddComponent<UIGameHud>();

        // Set serialized fields via reflection
        var loadingField = typeof(GameUIManager).GetField("_loadingScreen",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        loadingField.SetValue(_gameUIManager, loadingScreen);

        var pausedField = typeof(GameUIManager).GetField("_pausedPopup",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pausedField.SetValue(_gameUIManager, pausedPopup);

        var settingsField = typeof(GameUIManager).GetField("_settingsPopup",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        settingsField.SetValue(_gameUIManager, settingsPopup);

        var loseField = typeof(GameUIManager).GetField("_losePopup",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        loseField.SetValue(_gameUIManager, losePopup);

        var winField = typeof(GameUIManager).GetField("_winPopup",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        winField.SetValue(_gameUIManager, winPopup);

        var hudField = typeof(GameUIManager).GetField("_gameHud",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        hudField.SetValue(_gameUIManager, gameHud);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
        Object.DestroyImmediate(_loadingScreenObj);
        Object.DestroyImmediate(_pausedPopupObj);
        Object.DestroyImmediate(_settingsPopupObj);
        Object.DestroyImmediate(_losePopupObj);
        Object.DestroyImmediate(_winPopupObj);
        Object.DestroyImmediate(_gameHudObj);
    }

    [Test]
    public void ShowLoadingScreen_MethodExists()
    {
        var method = typeof(GameUIManager).GetMethod("ShowLoadingScreen");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ShowGameHud_MethodExists()
    {
        var method = typeof(GameUIManager).GetMethod("ShowGameHud");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ShowPausePopup_MethodExists()
    {
        var method = typeof(GameUIManager).GetMethod("ShowPausePopup");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ShowSettingsPopup_MethodExists()
    {
        var method = typeof(GameUIManager).GetMethod("ShowSettingsPopup");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ShowWinPopup_MethodExists()
    {
        var method = typeof(GameUIManager).GetMethod("ShowWinPopup");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ShowLosePopup_MethodExists()
    {
        var method = typeof(GameUIManager).GetMethod("ShowLosePopup");
        Assert.IsNotNull(method);
    }

    [Test]
    public void GameUIManager_HasCurrentUIField()
    {
        var field = typeof(GameUIManager).GetField("_currentUI",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void GameUIManager_AllUIComponentsHaveShowMethod()
    {
        var uiComponentTypes = new[] { typeof(UILoadingScreen), typeof(UIPausedPopup),
                                       typeof(UISettingsPopup), typeof(UILosePopup),
                                       typeof(UIWinPopup), typeof(UIGameHud) };

        foreach (var uiType in uiComponentTypes)
        {
            var method = uiType.GetMethod("Show",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, $"{uiType.Name} should have Show method");
        }
    }

    [Test]
    public void GameUIManager_AllUIComponentsHaveHideMethod()
    {
        var uiComponentTypes = new[] { typeof(UILoadingScreen), typeof(UIPausedPopup),
                                       typeof(UISettingsPopup), typeof(UILosePopup),
                                       typeof(UIWinPopup), typeof(UIGameHud) };

        foreach (var uiType in uiComponentTypes)
        {
            var method = uiType.GetMethod("Hide",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, $"{uiType.Name} should have Hide method");
        }
    }
}
