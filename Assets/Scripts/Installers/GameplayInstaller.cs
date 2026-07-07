using Managers;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private SceneManager _sceneManager;
    [SerializeField] private NetworkSceneManager _networkSceneManager;
    [SerializeField] private InputsManager _inputsManager;
    [SerializeField] private StartGameConfig _startGameConfig;
    [SerializeField] private GameUIManager _gameUIManager;

    public override void InstallBindings()
    {
        Container.Bind<ISceneService>().FromInstance(_sceneManager).AsSingle();
        Container.Bind<NetworkSceneManager>().FromInstance(_networkSceneManager).AsSingle();
        Container.Bind<InputsManager>().FromInstance(_inputsManager).AsSingle();
        Container.Bind<GameUIManager>().FromInstance(_gameUIManager).AsSingle();
        Container.BindInstance(_startGameConfig).AsSingle();
    }
}
