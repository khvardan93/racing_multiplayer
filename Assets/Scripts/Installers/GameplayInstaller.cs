using Managers;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private InputsManager _inputsManager;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private FusionConnectionManager _fusionConnectionManager;

    public override void InstallBindings()
    {
        Container.Bind<InputsManager>().FromInstance(_inputsManager).AsSingle();
        Container.Bind<GameManager>().FromInstance(_gameManager).AsSingle();
        Container.Bind<GameUIManager>().FromInstance(_gameUIManager).AsSingle();
        Container.Bind<FusionConnectionManager>().FromInstance(_fusionConnectionManager).AsSingle();
    }
}
