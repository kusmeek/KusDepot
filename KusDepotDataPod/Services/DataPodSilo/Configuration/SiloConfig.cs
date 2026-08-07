namespace DataPodServices;

public record SiloConfig(String ClusterId , String ServiceId , String IPAddress , Int32 SiloPort , Int32 GatewayPort , Boolean ListenOnAnyHostAddress = false)
{
    public override String ToString() => JsonSerializer.Serialize(this);

    public static SiloConfig? Parse(String input) => JsonSerializer.Deserialize<SiloConfig>(input);

    public Boolean Validate()
    {
        return
            !String.IsNullOrWhiteSpace(ClusterId) &&
            !String.IsNullOrWhiteSpace(ServiceId) &&
            !String.IsNullOrWhiteSpace(IPAddress) &&

            SiloPort > 0 && GatewayPort > 0;
    }
}