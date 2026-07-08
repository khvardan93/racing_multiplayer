using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

// ─── Toolbar element ──────────────────────────────────────────────────────────

[EditorToolbarElement(Id, typeof(SceneView))]
sealed class SceneSwitcherElement : EditorToolbarDropdown
{
    public const string Id = "RacerCar/SceneSwitcher";

    SceneSwitcherElement()
    {
        tooltip = "Switch scene";
        RefreshLabel();

        clicked += ShowMenu;
        EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
    }

    void OnSceneChanged(Scene _, Scene next) => RefreshLabel(next.name);

    void RefreshLabel(string name = null)
    {
        text = name ??SceneManager.GetActiveScene().name;
    }

    void ShowMenu()
    {
        var menu = new GenericMenu();
        var activePath = SceneManager.GetActiveScene().path;

        // Scenes in Build Settings (enabled only)
        var buildScenes = EditorBuildSettings.scenes;
        if (buildScenes.Length > 0)
        {
            foreach (var entry in buildScenes)
            {
                var path = entry.path;
                var label = Path.GetFileNameWithoutExtension(path);

                menu.AddItem(
                    new GUIContent(label),
                    path == activePath,
                    () => TryOpenScene(path));
            }
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Build Scenes/(none — add scenes in Build Settings)"));
        }

        menu.AddSeparator("");

        // All .unity files in the project
        var allGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in allGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var name = Path.GetFileNameWithoutExtension(path);
            // Use directory as submenu to keep list readable
            var dir = Path.GetDirectoryName(path)?.Replace('\\', '/').Replace("Assets/", "");
            var label = string.IsNullOrEmpty(dir) ? name : $"{dir}/{name}";

            menu.AddItem(
                new GUIContent("All Scenes/" + label),
                path == activePath,
                () => TryOpenScene(path));
        }

        menu.ShowAsContext();
    }

    static void TryOpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(path);
    }
}

// ─── Overlay (places the element in the Scene View toolbar) ──────────────────

[Overlay(typeof(SceneView), "racer-car-scene-switcher", "Scene Switcher")]
[Icon("d_UnityLogo")]
sealed class SceneSwitcherOverlay : ToolbarOverlay
{
    SceneSwitcherOverlay() : base(SceneSwitcherElement.Id) { }
}
