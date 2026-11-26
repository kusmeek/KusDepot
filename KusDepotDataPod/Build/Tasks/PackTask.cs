namespace KusDepotDataPod.Build;

[TaskName("Pack")]
public sealed class PackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var projects = new[]
        {
            "KusDepot.CMD/KusDepot.CMD.csproj",
            "KusDepot.ASPR/KusDepot.ASPR.csproj",
            "KusDepot.BLDRS/KusDepot.BLDRS.csproj",
            "KusDepot.Objects/KusDepot.Objects.csproj",
            "KusDepot.ToolManifests/KusDepot.ToolManifests.csproj",
        };

        foreach(var project in projects)
        {
            context.DotNetPack(context.ProjectPath(project), new DotNetPackSettings
            {
                Configuration = context.BuildConfiguration,

                OutputDirectory = context.ProjectPath("KusDepot.Packages"),
            });
        }
    }
}