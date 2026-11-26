namespace KusDepotDataPod.Build;

[TaskName("PulumiDestroy")]
public sealed class PulumiDestroyTask : PulumiTask
{
    public override void Run(BuildContext context)
    {
        RunPulumi(context,"destroy");
    }
}
