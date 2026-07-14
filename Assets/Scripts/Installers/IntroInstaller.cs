using Managers;
using UnityEngine;
using Zenject;

public class IntroInstaller : MonoInstaller
{
    [SerializeField] private IntroManager _introManager;

    public override void InstallBindings()
    {
        Container.Bind<IntroManager>().FromInstance(_introManager).AsSingle();
    }
}
