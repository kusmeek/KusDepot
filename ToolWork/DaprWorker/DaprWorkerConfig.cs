namespace ToolWork;

public record DaprWorkerConfig(Int32 GrpcPort)
{
    public override String ToString() => JsonSerializer.Serialize(this);

    public static DaprWorkerConfig? Parse(String input) => JsonSerializer.Deserialize<DaprWorkerConfig>(input);

    public Boolean Validate()
    {
        if( GrpcPort < 0 || GrpcPort > 65535 ) { return false; }

        return true;
    }
}