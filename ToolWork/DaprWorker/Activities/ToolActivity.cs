namespace ToolWork;

public sealed class ToolActivity : WorkflowActivity<KusDepotCab?,KusDepotCab?>
{
    private ITool? Tool;

    public ToolActivity(IToolConnect connect) { this.Tool = connect.Tool; }

    public override async Task<KusDepotCab?> RunAsync(WorkflowActivityContext context , KusDepotCab? cab)
    {
        try
        {
            return new ToolOutput() { ID = cab?.GetCommandDetails()?.ID , Data = Tool?.GetID() }.ToKusDepotCab();
        }
        catch ( Exception _ ) { await Task.CompletedTask; return WorkflowExceptionData.Create(_).ToKusDepotCab(); }
    }
}