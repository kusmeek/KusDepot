namespace KusDepotDataPod.Build;

[TaskName("PulumiUp")]
public sealed class PulumiUpTask : PulumiTask
{
    public override void Run(BuildContext context)
    {
        RunPulumi(context,"up");
    }
}
