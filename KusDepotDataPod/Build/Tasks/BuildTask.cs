namespace KusDepotDataPod.Build;

[TaskName("Build")]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    private static readonly String[] Projects =
    [
        "KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj",
        "KusDepotDataPod/DataControl/KusDepotDataPod.DataControl.csproj",
        "KusDepotDataPod/Services/KusDepotDataPod.Services.csproj",
        "KusDepotDataPod/Database/KusDepotDataPod.Database.csproj",
    ];

    public override void Run(BuildContext context)
    {
        foreach(var project in Projects)
        {
            context.DotNetBuild(context.ProjectPath(project),new DotNetBuildSettings
            {
                Configuration = context.BuildConfiguration,
            });
        }
    }
}