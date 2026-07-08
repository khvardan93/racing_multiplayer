using Managers;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private NetworkSceneManager _networkSceneManager;
    [SerializeField] private InputsManager _inputsManager;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private FusionConnectionManager _fusionConnectionManager;

    public override void InstallBindings()
    {
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<ISceneService>().To<GameManager>().FromResolve();
        Container.Bind<NetworkSceneManager>().FromInstance(_networkSceneManager).AsSingle();
        Container.Bind<InputsManager>().FromInstance(_inputsManager).AsSingle();
        Container.Bind<GameUIManager>().FromInstance(_gameUIManager).AsSingle();
        Container.Bind<FusionConnectionManager>().FromInstance(_fusionConnectionManager).AsSingle();
    }
}
