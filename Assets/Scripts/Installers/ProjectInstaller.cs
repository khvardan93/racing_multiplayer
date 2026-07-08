using Managers;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameConfigs _gameConfigs;

    public override void InstallBindings()
    {
        Container.Bind<AssetsManager>().FromNewComponentOnRoot().AsSingle().NonLazy();
        Container.BindInstance(_gameConfigs).AsSingle();
    }
}