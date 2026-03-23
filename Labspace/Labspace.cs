namespace KusDepot.Test;

public sealed class LabHost : Tool
{
    public LabHost() : base() {}

    public new String Status => GetLifeCycleState().ToString();

    public override ToolDescriptor? GetToolDescriptor(AccessKey? key = null)
    {
        if(this.AccessCheck(key) is false) { return null; }

        return new ToolDescriptor
        {
            ID = GetID(),
            Type = GetType().FullName,
            Specifications = "Host tool for dynamic service composition. Supports AddHostedService, RemoveHostedService, RegisterCommand, and full lifecycle management.",
            ExtendedData = ["Role: Host", "Lifecycle: Initialized, Active, InActive", "Capabilities: Hosting, Commands, Lifecycle"]
        };
    }
}