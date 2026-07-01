using System;
using Fusion;

public class FusionEvents : SimulationBehaviour, IPlayerJoined
{
    public static event Action<PlayerRef, NetworkRunner> OnPlayerJoined;
    
    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        OnPlayerJoined?.Invoke(player, Runner);
    }
}
