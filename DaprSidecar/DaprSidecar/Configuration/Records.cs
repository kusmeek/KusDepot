namespace KusDepot.Dapr;

public record ManagerConfig(String BinPath , String ProcessName , String ListenAddress , Int32 GrpcPort , Int32 HealthzPort , Int32 MetricsPort , Int32 Interval , Int32 Timeout)
{
    public override String ToString() => JsonSerializer.Serialize(this);

    public static ManagerConfig? Parse(String input) => String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<ManagerConfig>(input);

    public Boolean Validate()
    {
        return
            !String.IsNullOrWhiteSpace(BinPath) &&
            !String.IsNullOrWhiteSpace(ProcessName) &&
            !String.IsNullOrWhiteSpace(ListenAddress) &&

            GrpcPort > 0 && HealthzPort > 0 && MetricsPort > 0;
    }
}

public record SidecarConfig(String BinPath , String ConfigPath , String ResourcesPath , String ProcessName , String ListenAddress , Int32 GrpcPort , Int32 HttpPort , Int32 MetricsPort , Int32 Interval , Int32 Timeout , String AppID , Int32 AppPort)
{
    public override String ToString() => JsonSerializer.Serialize(this);

    public static SidecarConfig? Parse(String input) => String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<SidecarConfig>(input);

    public Boolean Validate()
    {
        return
            !String.IsNullOrWhiteSpace(AppID) &&
            !String.IsNullOrWhiteSpace(BinPath) &&
            !String.IsNullOrWhiteSpace(ConfigPath) &&
            !String.IsNullOrWhiteSpace(ProcessName) &&
            !String.IsNullOrWhiteSpace(ResourcesPath) &&
            !String.IsNullOrWhiteSpace(ListenAddress) &&

            GrpcPort > 0 && HttpPort > 0 && MetricsPort > 0 && AppPort > 0;
    }
}

public record DaprSupervisorConfig(ManagerConfig PlacementManager , ManagerConfig SchedulerManager , SidecarConfig SidecarManager)
{
    public override String ToString() => JsonSerializer.Serialize(this);

    public static DaprSupervisorConfig? Parse(String input) => String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<DaprSupervisorConfig>(input);

    public Boolean Validate() { return SidecarManager.Validate() && PlacementManager.Validate() && SchedulerManager.Validate(); }
}