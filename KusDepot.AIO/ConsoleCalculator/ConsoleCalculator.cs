namespace KusDepot.AI;

[McpServerToolType()]
public static class ConsoleCalculator
{
    static ConsoleCalculator() { Tool = ToolBuilderFactory.CreateBuilder().RegisterCommand<CalculateCommand>("Calculate").AutoStart().Build(); }

    [McpServerTool(Name = "Calculate") , Description("Perform operations with two double precision numbers. The operation must be one of 'Add' 'Subtract' 'Multiply' or 'Divide'")]
    public static Double Calculate(String operation , Double x , Double y)
    {
        try
        {
            switch(operation)
            {
                case "Add":      return Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Add" ,      ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue;

                case "Subtract": return Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Subtract" , ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue;

                case "Multiply": return Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Multiply" , ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue;

                case "Divide":   return Tool!.GetRemoveOutputAsync(Tool.ExecuteCommand(new() { Handle = "Calculate" , Arguments = new(){ ["Operation"] = "Divide" ,   ["x"] = x , ["y"] = y } })).GetAwaiter().GetResult() as Double? ?? Double.MinValue;

                default: return Double.MinValue;
            }
        }
        catch { return Double.MinValue;; }
    }

    private static readonly ITool? Tool;
}