using System;
using UI;
using UnityEngine;

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
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Arena");
    }
}