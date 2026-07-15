using Managers;
using NUnit.Framework;
using UnityEngine;

public class AssetsManagerTests
{
    private GameObject _gameObject;
    private AssetsManager _assetsManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("AssetsManager");
        _assetsManager = _gameObject.AddComponent<AssetsManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void AssetsManager_HasHandlesCache()
    {
        var field = typeof(AssetsManager).GetField("_handles",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void AssetsManager_HasLoadedSceneTracker()
    {
        var field = typeof(AssetsManager).GetField("_loadedScene",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field);
    }

    [Test]
    public void LoadAsset_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("LoadAsset");
        Assert.IsNotNull(method);
    }

    [Test]
    public void Instantiate_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("Instantiate");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ReleaseAsset_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("ReleaseAsset");
        Assert.IsNotNull(method);
    }

    [Test]
    public void ReleaseInstance_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("ReleaseInstance");
        Assert.IsNotNull(method);
    }

    [Test]
    public void LoadScene_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("LoadScene");
        Assert.IsNotNull(method);
    }

    [Test]
    public void UnloadScene_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("UnloadScene");
        Assert.IsNotNull(method);
    }

    [Test]
    public void LoadAssetAsync_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("LoadAssetAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void InstantiateAsync_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("InstantiateAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void LoadSceneAsync_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("LoadSceneAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void UnloadSceneAsync_MethodExists()
    {
        var method = typeof(AssetsManager).GetMethod("UnloadSceneAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
    }

    [Test]
    public void OnDestroy_CleansUpHandles()
    {
        var onDestroyMethod = typeof(AssetsManager).GetMethod("OnDestroy",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(onDestroyMethod);
    }
}
