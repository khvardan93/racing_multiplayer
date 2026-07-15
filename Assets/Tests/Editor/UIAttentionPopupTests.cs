using Managers;
using NUnit.Framework;
using UnityEngine;
using UI;
using UnityEngine.UI;

public class UIAttentionPopupTests
{
    private GameObject _gameObject;
    private UIAttentionPopup _attentionPopup;
    private GameObject _gameUIManagerObject;
    private GameUIManager _gameUIManager;
    private Text _titleText;
    private Text _messageText;
    private Button _acknowledgeButton;

    [SetUp]
    public void SetUp()
    {
        // Create GameUIManager mock
        _gameUIManagerObject = new GameObject("GameUIManager");
        _gameUIManager = _gameUIManagerObject.AddComponent<GameUIManager>();

        // Create AttentionPopup
        _gameObject = new GameObject("UIAttentionPopup");
        _attentionPopup = _gameObject.AddComponent<UIAttentionPopup>();

        // Create title text
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(_gameObject.transform);
        _titleText = titleObj.AddComponent<Text>();
        _titleText.text = "";

        // Create message text
        var messageObj = new GameObject("Message");
        messageObj.transform.SetParent(_gameObject.transform);
        _messageText = messageObj.AddComponent<Text>();
        _messageText.text = "";

        // Create acknowledge button
        var buttonObj = new GameObject("AcknowledgeButton");
        buttonObj.transform.SetParent(_gameObject.transform);
        _acknowledgeButton = buttonObj.AddComponent<Button>();

        // Set serialized fields via reflection
        var titleField = typeof(UIAttentionPopup).GetField("_titleText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        titleField.SetValue(_attentionPopup, _titleText);

        var messageField = typeof(UIAttentionPopup).GetField("_messageText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        messageField.SetValue(_attentionPopup, _messageText);

        var buttonField = typeof(UIAttentionPopup).GetField("_acknowledgeButton",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        buttonField.SetValue(_attentionPopup, _acknowledgeButton);

        var gameUIManagerField = typeof(UIAttentionPopup).GetField("_gameUIManager",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameUIManagerField.SetValue(_attentionPopup, _gameUIManager);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
        Object.DestroyImmediate(_gameUIManagerObject);
    }

    [Test]
    public void UIAttentionPopup_InheritsFromUIBaseItem()
    {
        Assert.IsTrue(typeof(UIBaseItem).IsAssignableFrom(typeof(UIAttentionPopup)));
    }

    [Test]
    public void UIAttentionPopup_HasTitleTextField()
    {
        var field = typeof(UIAttentionPopup).GetField("_titleText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void UIAttentionPopup_HasMessageTextField()
    {
        var field = typeof(UIAttentionPopup).GetField("_messageText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void UIAttentionPopup_HasAcknowledgeButton()
    {
        var field = typeof(UIAttentionPopup).GetField("_acknowledgeButton",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void UIAttentionPopup_TitleSaysExperimental()
    {
        Assert.That(_titleText.text, Does.Contain("Experimental"));
    }

    [Test]
    public void UIAttentionPopup_TitleHasWarningIcon()
    {
        Assert.That(_titleText.text, Does.Contain("⚠️"));
    }

    [Test]
    public void UIAttentionPopup_MessageMentionsDemonstration()
    {
        Assert.That(_messageText.text, Does.Contain("demonstration"));
    }

    [Test]
    public void UIAttentionPopup_MessageMentionsAnimations()
    {
        Assert.That(_messageText.text, Does.Contain("animations"));
    }

    [Test]
    public void UIAttentionPopup_MessageMentionsNotFinal()
    {
        Assert.That(_messageText.text, Does.Contain("final"));
    }

    [Test]
    public void UIAttentionPopup_HasShowMethod()
    {
        var method = typeof(UIAttentionPopup).GetMethod("Show",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void UIAttentionPopup_HasHideMethod()
    {
        var method = typeof(UIAttentionPopup).GetMethod("Hide",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void UIAttentionPopup_OnAcknowledgeMethodExists()
    {
        var method = typeof(UIAttentionPopup).GetMethod("OnAcknowledge",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void GameUIManager_HasShowAttentionPopupMethod()
    {
        var method = typeof(GameUIManager).GetMethod("ShowAttentionPopup",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }
}
