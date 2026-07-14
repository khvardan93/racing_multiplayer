using Managers;
using UnityEngine;
using Zenject;

public class GarageInstaller : MonoInstaller
{
    [SerializeField] private GarageManager _garageManager;
    
    public override void InstallBindings()
    {
        Container.Bind<GarageManager>().FromInstance(_garageManager).AsSingle();
    }
}
