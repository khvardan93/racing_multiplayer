using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class AssetsManager : MonoBehaviour
    {
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();
        private SceneInstance? _loadedScene;

        // Loads (or returns the cached) addressable asset for key, invoking onLoaded when ready.
        public void LoadAsset<T>(string key, Action<T> onLoaded) where T : UnityEngine.Object
        {
            LoadAssetAsync(key, onLoaded);
        }

        // Instantiates the addressable prefab for key under parent, invoking onLoaded when ready.
        public void Instantiate(string key, Transform parent, Action<GameObject> onLoaded)
        {
            InstantiateAsync(key, parent, onLoaded);
        }

        // Releases a previously loaded asset handle for key.
        public void ReleaseAsset(string key)
        {
            if (!_handles.TryGetValue(key, out var handle))
                return;

            Addressables.Release(handle);
            _handles.Remove(key);
        }

        // Releases an instance created via Instantiate.
        public void ReleaseInstance(GameObject instance)
        {
            Addressables.ReleaseInstance(instance);
        }

        // Unloads any currently loaded scene, then loads the addressable scene for key, invoking onLoaded when ready.
        public void LoadScene(string key, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null)
        {
            LoadSceneAsync(key, mode, onLoaded);
        }

        // Unloads the currently loaded addressable scene, invoking onUnloaded when done.
        public void UnloadScene(Action onUnloaded)
        {
            UnloadSceneAsync(onUnloaded);
        }

        private async void LoadAssetAsync<T>(string key, Action<T> onLoaded) where T : UnityEngine.Object
        {
            if (_handles.TryGetValue(key, out var existing))
            {
                onLoaded?.Invoke((T)existing.Result);
                return;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            _handles[key] = handle;
            var result = await handle.Task;
            onLoaded?.Invoke(result);
        }

        private async void InstantiateAsync(string key, Transform parent, Action<GameObject> onLoaded)
        {
            var handle = Addressables.InstantiateAsync(key, parent);
            var instance = await handle.Task;
            onLoaded?.Invoke(instance);
        }

        private async void LoadSceneAsync(string key, LoadSceneMode mode, Action onLoaded)
        {
            if (_loadedScene.HasValue)
                await UnloadSceneInternalAsync();

            var handle = Addressables.LoadSceneAsync(key, mode);
            _loadedScene = await handle.Task;
            onLoaded?.Invoke();
        }

        private async void UnloadSceneAsync(Action onUnloaded)
        {
            await UnloadSceneInternalAsync();
            onUnloaded?.Invoke();
        }

        private async Task UnloadSceneInternalAsync()
        {
            if (!_loadedScene.HasValue)
                return;

            await Addressables.UnloadSceneAsync(_loadedScene.Value).Task;
            _loadedScene = null;
        }

        private void OnDestroy()
        {
            foreach (var handle in _handles.Values)
                Addressables.Release(handle);

            _handles.Clear();
        }
    }
}
