using Fusion;
using Zenject;

public class ZenjectNetworkObjectProvider : NetworkObjectProviderDefault
{
    [Inject] private DiContainer _container;

    protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
    {
        var instance = base.InstantiatePrefab(runner, prefab);
        _container.InjectGameObject(instance.gameObject);
        return instance;
    }
}