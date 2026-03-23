namespace KusDepot.AI;

public sealed partial class ToolPod
{
    private static readonly List<String> AssemblyPaths = new();

    public static IReadOnlyList<String> GetAssemblyPaths()
    {
        try
        {
            lock(Sync) { return AssemblyPaths.ToArray(); }
        }
        catch { return Array.Empty<String>(); }
    }

    [McpServerTool(Name = "ToolPodLoadAssembly")]
    [Description(Descriptions.LoadAssembly)]
    public static ToolPodResult LoadAssembly(
        [Description(LoadAssemblyFileName)] String? fileName)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(fileName)) { return ErrorResult(ErrorStrings.AssemblyFileNameRequired); }

            String path;

            lock (Sync) { path = Path.Combine(activeworkingdirectory,fileName); }

            if(!File.Exists(path)) { return ErrorResult(String.Format(ErrorStrings.AssemblyFileNotFoundFormat,path)); }

            var assembly = Context.LoadFromAssemblyPath(path);

            lock(Sync)
            {
                if(!AssemblyPaths.Contains(path,StringComparer.OrdinalIgnoreCase))
                {
                    AssemblyPaths.Add(path);
                }
            }

            LogDiagnostic($"Loaded assembly {assembly.GetName().Name} from {path}");

            return ValueResult(JsonSerializer.Serialize(
                new
                {
                    FileName = fileName,
                    Path = path,
                    Assembly = assembly.GetName().Name
                }),typeof(String).FullName);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.AssemblyLoadFailed); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "ToolPodListAssemblies")]
    [Description(Descriptions.ListAssemblies)]
    public static ToolPodResult ListAssemblies()
    {
        try
        {
            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => new
                {
                    Name = a.GetName().FullName,
                    IsDynamic = a.IsDynamic,
                    Location = a.IsDynamic ? null : a.Location
                })
                .OrderBy(a => a.Name, StringComparer.Ordinal)
                .ToArray();

            var contextAssemblies = Context.Assemblies
                .Select(a => new
                {
                    Name = a.GetName().FullName,
                    IsDynamic = a.IsDynamic,
                    Location = a.IsDynamic ? null : a.Location
                })
                .OrderBy(a => a.Name,StringComparer.Ordinal)
                .ToArray();

            List<String> injectedPaths;

            lock (Sync) { injectedPaths = AssemblyPaths.ToList(); }

            var payload = new
            {
                AppDomainAssemblies = appDomainAssemblies,
                ContextAssemblies = contextAssemblies,
                InjectedPaths = injectedPaths
            };

            return ValueResult(JsonSerializer.Serialize(payload),"ToolPodAssemblyPayload");
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.ListAssembliesFailed); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "ToolPodListTypes")]
    [Description(Descriptions.ListTypes)]
    public static ToolPodResult ListTypes(
        [Description(ListTypesAssemblyName)] String? assemblyName)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(assemblyName)) { return ErrorResult(ErrorStrings.AssemblyNameRequired); }

            Assembly? match = Context.Assemblies
                .FirstOrDefault(a => String.Equals(a.GetName().Name,assemblyName,StringComparison.OrdinalIgnoreCase));

            if(match is null) { return ErrorResult(String.Format(ErrorStrings.AssemblyNotFoundFormat,assemblyName)); }

            var types = match.GetExportedTypes()
                .OrderBy(t => t.FullName,StringComparer.Ordinal)
                .Select(t => new
                {
                    FullName = t.FullName,
                    BaseType = t.BaseType?.FullName,
                    IsAbstract = t.IsAbstract,
                    IsInterface = t.IsInterface,
                    IsEnum = t.IsEnum
                })
                .ToArray();

            var payload = new
            {
                Assembly = match.GetName().Name,
                TypeCount = types.Length,
                Types = types
            };

            return ValueResult(JsonSerializer.Serialize(payload),"ToolPodTypeListPayload");
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.ListTypesFailed); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "ToolPodUnloadAssemblies")]
    [Description(Descriptions.UnloadAssemblies)]
    public static ToolPodResult UnloadAssemblies()
    {
        try
        {
            AssemblyLoadContext old;

            lock(Sync)
            {
                Objects.Clear(); Aliases.Clear(); AssemblyPaths.Clear(); 

                old = context;

                context = new AssemblyLoadContext("ToolPodContext",isCollectible:true);

                LogDiagnostic("Unloaded ToolPod assemblies and reset context.");
            }

            old.Unload(); GC.Collect(); GC.WaitForPendingFinalizers();

            return ValueResult("{}","ToolPodUnloaded");
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.AssembliesUnloadFailed); return ErrorResult(_.Message); }
    }
}