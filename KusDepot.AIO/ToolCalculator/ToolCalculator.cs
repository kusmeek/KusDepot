namespace KusDepot.AI;

public sealed class ToolCalculator : MCPTool
{
    public ToolCalculator() { this.Tool = ToolBuilderFactory.CreateBuilder().RegisterCommand<CalculateCommand>("Calculate").AutoStart().Build(); }

    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request , CancellationToken cancel = default)
    {
        try
        {
            String? o = request.Params?.Name; if(String.IsNullOrEmpty(o)) { return new() { Content = [] , IsError = true }; }

            if( Double.TryParse(request.Params?.Arguments?["x"].ToString(),out Double x) is false ||
                Double.TryParse(request.Params?.Arguments?["y"].ToString(),out Double y) is false)
                { return new() { Content = [] , IsError = true }; }

            switch(request.Params?.Arguments?["Operation"].ToString())
            {
                case "Add":      return ToolResult( Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Add" ,      ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue );

                case "Subtract": return ToolResult( Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Subtract" , ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue );

                case "Multiply": return ToolResult( Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Multiply" , ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue );

                case "Divide":   return ToolResult( Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Divide" ,   ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue );

                case null: return new() { Content = [] , IsError = true };

                default: return new() { Content = [] , IsError = true };
            }
        }
        catch ( Exception _ ) { Log.Error(_,MCPToolInvokeFail,this.GetType().Name,this.Tool?.GetID()?.ToString()); await Task.CompletedTask; return new() { Content = [] , IsError = true }; }
    }

    private static CallToolResult ToolResult(Double result)
    {
        return new() { Content = [ new TextContentBlock() { Text = result.ToString() , Type = "text" } ] , IsError = false };
    }

    public override ModelContextProtocol.Protocol.Tool ProtocolTool
    {
        get
        {
            return new()
            { Name = "ToolCalculator" , InputSchema = GetJsonSchema(),
              Description = "Perform operations with two double precision numbers." +
              "The first parameter 'Operation' must be one of 'Add' 'Subtract' 'Multiply' or 'Divide'." +
              "The following two parameters 'x' and 'y' are the numbers."};
        }
    }

    private static JsonElement GetJsonSchema()
    {
        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["Operation"] = new JsonObject { ["type"] = "string" },
                ["x"]         = new JsonObject { ["type"] = "string" },
                ["y"]         = new JsonObject { ["type"] = "string" }
            },
            ["required"] = new JsonArray("Operation","x","y")
        };

        return JsonDocument.Parse(schema.ToJsonString()).RootElement.Clone();
    }
}