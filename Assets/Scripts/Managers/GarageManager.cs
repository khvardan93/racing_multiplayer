using System;
using Managers;
using UI;
using UnityEngine;
using Zenject;

public enum GaragePages
{
    MainMenu,
    SettingsMenu,
    LoadingScreen,
}

public class GarageManager : MonoBehaviour
{
    [Serializable]
    public struct Page
    {
        public GaragePages Type;
        public UIBaseItem Item;
    }
        
    [SerializeField] private Page[] _pages;
    [Inject] private AssetsManager _assetsManager;
    [Inject] private GameConfigs _configs;

    public void ShowPage(GaragePages type)
    {
        foreach (var page in _pages)
        {
            if (page.Type == type)
                page.Item.Show();
            else
                page.Item.Hide();
        }
    }

    public void LoadGame()
    {
        if (_configs.TryGetScene(GameScenes.Game, out var scene))
            _assetsManager.LoadScene(scene);
    }
}