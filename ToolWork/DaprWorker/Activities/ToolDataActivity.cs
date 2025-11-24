namespace ToolWork;

public sealed class ToolDataActivity : WorkflowActivity<KusDepotCab?,KusDepotCab?>
{
    private readonly ITool? Tool;

    public ToolDataActivity(IToolConnect connect) { this.Tool = connect.Tool; }

    public override async Task<KusDepotCab?> RunAsync(WorkflowActivityContext context , KusDepotCab? cab)
    {
        try
        {
            return new ToolOutput() { ID = cab?.GetCommandDetails()?.ID , Data = DataItemGenerator.CreateDataSet(5) }.ToKusDepotCab();
        }
        catch ( Exception _ ) { await Task.CompletedTask; return WorkflowExceptionData.Create(_).ToKusDepotCab(); }
    }
}