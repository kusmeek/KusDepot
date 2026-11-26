namespace DataPodServices.DataControl;

internal sealed class OrleansClusterConnectionStatus : IClusterConnectionStatusObserver
{
    private Int32 connected;

    public Boolean IsConnected => Volatile.Read(ref this.connected) != 0;

    public void NotifyClusterConnectionLost() => Volatile.Write(ref this.connected,0);

    public void NotifyGatewayCountChanged(Int32 currentNumberOfGateways , Int32 previousNumberOfGateways , Boolean connectionRecovered) =>
        Volatile.Write(ref this.connected,currentNumberOfGateways > 0 ? 1 : 0);
}
