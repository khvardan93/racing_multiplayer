using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private SceneManager _sceneManager;
    [SerializeField] private InputsManager _inputsManager;

    public override void InstallBindings()
    {
        Container.Bind<ISceneService>().FromInstance(_sceneManager).AsSingle();
        Container.Bind<InputsManager>().FromInstance(_inputsManager).AsSingle();
    }
}
