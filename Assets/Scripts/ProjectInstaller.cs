using Fusion;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller<ProjectInstaller>
{
    [SerializeField] private StartGameConfig _startGameConfig;
    [SerializeField] private NetworkRunner _networkRunner;
    [SerializeField] private NetworkSceneManager _networkSceneManager;

    public override void InstallBindings()
    {
        Container.Bind<NetworkRunner>().FromComponentInNewPrefab(_networkRunner).AsSingle().NonLazy();
        Container.Bind<NetworkSceneManager>().FromComponentInNewPrefab(_networkSceneManager).AsSingle().NonLazy();
        Container.Bind<FusionConnectionManager>().AsSingle();
        Container.BindInstance(_startGameConfig).AsSingle();
    }
}