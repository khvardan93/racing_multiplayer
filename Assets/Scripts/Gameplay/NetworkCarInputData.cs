using Fusion;

public struct NetworkCarInputData : INetworkInput
{
    public float Vertical;
    public float Horizontal;
    public bool HandBrake;
    public NetworkBool ResetPressed;
}