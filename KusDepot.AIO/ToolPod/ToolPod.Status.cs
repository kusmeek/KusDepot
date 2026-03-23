namespace KusDepot.AI;

public sealed partial class ToolPod
{
    private static readonly Queue<String> DiagnosticLogs = new();

    internal static void LogDiagnostic(String message)
    {
        try
        {
            lock(Sync)
            {
                DiagnosticLogs.Enqueue($"[{DateTimeOffset.UtcNow:O}] {message}");

                if(DiagnosticLogs.Count > 100) { DiagnosticLogs.Dequeue(); }
            }
        }
        catch { }
    }

    [McpServerTool(Name = "ToolPodStatus")]
    [Description(Descriptions.Status)]
    public static ToolPodResult Status()
    {
        try
        {
            List<String> logs; List<String> injectedPaths;

            lock(Sync)
            {
                logs = DiagnosticLogs.ToList();

                injectedPaths = AssemblyPaths.ToList();
            }

            var loadedAssemblies = Context.Assemblies
                .Select(a => new
                {
                    Name = a.GetName().FullName,
                    IsDynamic = a.IsDynamic,
                    Location = a.IsDynamic ? null : a.Location
                })
                .OrderBy(a => a.Name,StringComparer.Ordinal)
                .ToArray();

            var payload = new
            {
                Logs = logs,
                Assemblies = loadedAssemblies,
                InjectedPaths = injectedPaths,
                Objects = ListReferences()
            };

            return ValueResult(JsonSerializer.Serialize(payload),"ToolPodStatusPayload");
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.StatusFailed); return ErrorResult(_.Message); }
    }
}