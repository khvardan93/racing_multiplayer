using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    // Plain C# class (not a MonoBehaviour) - bound as a project-scoped singleton
    // in ProjectInstaller so it survives scene loads like AssetsManager/GameConfigs.
    public class DataManager
    {
        [Serializable]
        private class Entry
        {
            public string Key;
            public string Value;
        }

        [Serializable]
        private class SerializableStore
        {
            public List<Entry> Entries = new();
        }

        private const string PlayerPrefsKey = "DataManager.Store";

        private readonly Dictionary<string, string> _values = new();

        public DataManager()
        {
            Load();
            Application.quitting += Save;
        }

        public void Set<T>(string key, T value)
        {
            _values[key] = JsonUtility.ToJson(new Wrapper<T> { Value = value });
            Save();
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_values.TryGetValue(key, out var json))
            {
                value = JsonUtility.FromJson<Wrapper<T>>(json).Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool HasKey(string key) => _values.ContainsKey(key);

        public void Remove(string key) => _values.Remove(key);

        private string ToJson()
        {
            var store = new SerializableStore();
            foreach (var pair in _values)
                store.Entries.Add(new Entry { Key = pair.Key, Value = pair.Value });

            return JsonUtility.ToJson(store);
        }

        private void LoadFromJson(string json)
        {
            _values.Clear();

            if (string.IsNullOrEmpty(json)) return;

            var store = JsonUtility.FromJson<SerializableStore>(json);
            if (store?.Entries == null) return;

            foreach (var entry in store.Entries)
                _values[entry.Key] = entry.Value;
        }

        private void Save()
        {
            PlayerPrefs.SetString(PlayerPrefsKey, ToJson());
            PlayerPrefs.Save();
        }

        private void Load()
        {
            LoadFromJson(PlayerPrefs.GetString(PlayerPrefsKey, string.Empty));
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T Value;
        }
    }
}
