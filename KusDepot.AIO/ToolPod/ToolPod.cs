namespace KusDepot.AI;

[McpServerToolType]
[Description(ToolPodClass)]
public sealed partial class ToolPod
{
    public ToolPod() {}

    private static AssemblyLoadContext Context
    {
        get { lock (Sync) { return context; } }

        set { lock (Sync) { context = value; } }
    }

    private static String ActiveWorkingDirectory
    {
        get { lock (Sync) { return activeworkingdirectory; } }

        set { lock (Sync) { activeworkingdirectory = value; } }
    }

    private static String activeworkingdirectory = Path.GetTempPath();

    private static AssemblyLoadContext context = new AssemblyLoadContext("ToolPodContext",isCollectible:true);
}