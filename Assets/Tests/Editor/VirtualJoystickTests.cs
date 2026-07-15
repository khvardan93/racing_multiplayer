using Managers;
using NUnit.Framework;
using UnityEngine;
using UI;
using UnityEngine.UI;

public class VirtualJoystickTests
{
    private GameObject _joystickObject;
    private VirtualJoystick _virtualJoystick;
    private GameObject _inputsManagerObject;
    private InputsManager _inputsManager;
    private RectTransform _background;
    private RectTransform _handle;

    [SetUp]
    public void SetUp()
    {
        // Create InputsManager mock
        _inputsManagerObject = new GameObject("InputsManager");
        _inputsManager = _inputsManagerObject.AddComponent<InputsManager>();

        // Create VirtualJoystick
        _joystickObject = new GameObject("VirtualJoystick");
        var canvas = _joystickObject.AddComponent<Canvas>();
        var graphicRaycaster = _joystickObject.AddComponent<GraphicRaycaster>();

        _virtualJoystick = _joystickObject.AddComponent<VirtualJoystick>();

        // Create background
        var backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(_joystickObject.transform);
        _background = backgroundObj.AddComponent<RectTransform>();
        _background.sizeDelta = new Vector2(200, 200);

        // Create handle
        var handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(_joystickObject.transform);
        _handle = handleObj.AddComponent<RectTransform>();
        _handle.sizeDelta = new Vector2(50, 50);

        // Set serialized fields
        var backgroundField = typeof(VirtualJoystick).GetField("_background",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        backgroundField.SetValue(_virtualJoystick, _background);

        var handleField = typeof(VirtualJoystick).GetField("_handle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        handleField.SetValue(_virtualJoystick, _handle);

        var inputsManagerField = typeof(VirtualJoystick).GetField("_inputsManager",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        inputsManagerField.SetValue(_virtualJoystick, _inputsManager);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_joystickObject);
        Object.DestroyImmediate(_inputsManagerObject);
    }

    [Test]
    public void VirtualJoystick_ImplementsIDragHandler()
    {
        var implementsInterface = typeof(UnityEngine.EventSystems.IDragHandler).IsAssignableFrom(
            typeof(VirtualJoystick));
        Assert.IsTrue(implementsInterface);
    }

    [Test]
    public void VirtualJoystick_ImplementsIPointerDownHandler()
    {
        var implementsInterface = typeof(UnityEngine.EventSystems.IPointerDownHandler).IsAssignableFrom(
            typeof(VirtualJoystick));
        Assert.IsTrue(implementsInterface);
    }

    [Test]
    public void VirtualJoystick_ImplementsIPointerUpHandler()
    {
        var implementsInterface = typeof(UnityEngine.EventSystems.IPointerUpHandler).IsAssignableFrom(
            typeof(VirtualJoystick));
        Assert.IsTrue(implementsInterface);
    }

    [Test]
    public void OnDrag_MethodExists()
    {
        var method = typeof(VirtualJoystick).GetMethod("OnDrag",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void OnPointerDown_MethodExists()
    {
        var method = typeof(VirtualJoystick).GetMethod("OnPointerDown",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void OnPointerUp_MethodExists()
    {
        var method = typeof(VirtualJoystick).GetMethod("OnPointerUp",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void VirtualJoystick_HasInputField()
    {
        var field = typeof(VirtualJoystick).GetField("_input",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void VirtualJoystick_InputStartsAtZero()
    {
        var field = typeof(VirtualJoystick).GetField("_input",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var inputValue = (Vector2)field.GetValue(_virtualJoystick);
        Assert.AreEqual(Vector2.zero, inputValue);
    }

    [Test]
    public void VirtualJoystick_HasBackgroundReference()
    {
        var field = typeof(VirtualJoystick).GetField("_background",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void VirtualJoystick_HasHandleReference()
    {
        var field = typeof(VirtualJoystick).GetField("_handle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void VirtualJoystick_InputsManagerCanBeInjected()
    {
        var field = typeof(VirtualJoystick).GetField("_inputsManager",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);

        var injectedManager = (InputsManager)field.GetValue(_virtualJoystick);
        Assert.AreEqual(_inputsManager, injectedManager);
    }

    [Test]
    public void VirtualJoystick_RequiresCanvasParent()
    {
        // Verify the joystick is a child of a Canvas
        var canvas = _joystickObject.GetComponentInParent<Canvas>();
        Assert.IsNotNull(canvas);
    }
}
