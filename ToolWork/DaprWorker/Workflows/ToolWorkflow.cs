namespace ToolWork;

public sealed class ToolWorkflow : Workflow<KusDepotCab?,KusDepotCab?>
{
    public override async Task<KusDepotCab?> RunAsync(WorkflowContext context , KusDepotCab? cab)
    {
        return await context.CallActivityAsync<KusDepotCab?>(nameof(ToolActivity),cab);
    }
}