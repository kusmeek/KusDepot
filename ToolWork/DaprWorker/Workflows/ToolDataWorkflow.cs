namespace ToolWork;

public sealed class ToolDataWorkflow : Workflow<KusDepotCab?,KusDepotCab?>
{
    public override async Task<KusDepotCab?> RunAsync(WorkflowContext context , KusDepotCab? cab)
    {
        return await context.CallActivityAsync<KusDepotCab?>(nameof(ToolDataActivity),cab);
    }
}