namespace KusDepotDataPod.Build;

[TaskName("PulumiPreview")]
public sealed class PulumiPreviewTask : PulumiTask
{
    public override void Run(BuildContext context)
    {
        RunPulumi(context,"preview");
    }
}
