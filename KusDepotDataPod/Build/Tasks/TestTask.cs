namespace KusDepotDataPod.Build;

[TaskName("Test")]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest(
            context.ProjectPath("KusDepot.Exam/KusDepot.Exam.csproj"),
            new DotNetTestSettings
            {
                Configuration = context.BuildConfiguration,
            });
    }
}