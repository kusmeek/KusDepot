namespace KusDepot.AI;

public sealed partial class ToolPod
{
    private static readonly Dictionary<String,Object> Objects = new(StringComparer.Ordinal);

    private static readonly Dictionary<String,String> Aliases = new(StringComparer.Ordinal);

    private static readonly Lock Sync = new();

    public static ToolPodRef? GetReference(String? id)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(id)) { return null; }

            lock(Sync)
            {
                if(!Objects.TryGetValue(id,out Object? value) || value is null) { return null; }

                return CreateReference(id,value,Aliases.FirstOrDefault(_ => String.Equals(_.Value,id,StringComparison.Ordinal)).Key);
            }
        }
        catch { return null; }
    }

    public static Object? GetTrackedObject(String? id)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(id)) { return null; }

            lock(Sync)
            {
                return Objects.TryGetValue(id,out Object? value) ? value : null;
            }
        }
        catch { return null; }
    }

    public static IReadOnlyList<ToolPodRef> ListReferences()
    {
        try
        {
            lock(Sync)
            {
                return Objects
                    .Select(_ => CreateReference(_.Key,_.Value,Aliases.FirstOrDefault(__ => String.Equals(__.Value,_.Key,StringComparison.Ordinal)).Key))
                    .Where(_ => _ is not null)
                    .Cast<ToolPodRef>()
                    .ToArray();
            }
        }
        catch { return Array.Empty<ToolPodRef>(); }
    }

    public static String? FindExistingIdByReference(Object instance)
    {
        lock(Sync)
        {
            foreach(var kvp in Objects)
            {
                if(ReferenceEquals(kvp.Value, instance))
                {
                    return kvp.Key;
                }
            }
        }
        return null;
    }

    public static ToolPodRef? RegisterObject(Object? value , String? alias = null)
    {
        try
        {
            if(value is null) { return null; }

            lock(Sync)
            {
                String? id = FindExistingIdByReference(value);

                if(!String.IsNullOrWhiteSpace(alias))
                {
                    if(Aliases.TryGetValue(alias,out String? boundId))
                    {
                        if(id is null || !String.Equals(boundId,id,StringComparison.Ordinal)) { return null; }
                    }
                }

                if(id is null)
                {
                    id = Guid.NewGuid().ToString("N"); Objects[id] = value;

                    LogDiagnostic($"Registered object {id} of type {value.GetType().Name}{(alias is not null ? $" with alias '{alias}'" : "")}");
                }

                if(!String.IsNullOrWhiteSpace(alias))
                {
                    Aliases[alias] = id;
                }

                return CreateReference(id,value,alias);
            }
        }
        catch { return null; }
    }

    public static Boolean RemoveObject(String? id)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(id)) { return false; }

            lock(Sync)
            {
                if(Objects.Remove(id) is false) { return false; }

                foreach(String alias in Aliases.Where(_ => String.Equals(_.Value,id,StringComparison.Ordinal)).Select(_ => _.Key).ToArray())
                {
                    Aliases.Remove(alias);
                }

                LogDiagnostic($"Removed object {id}");

                return true;
            }
        }
        catch { return false; }
    }

    public static Boolean SetObjectAlias(String idoralias, String newalias)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(idoralias) || String.IsNullOrWhiteSpace(newalias)) { return false; }

            String? id = ResolveId(idoralias); if(String.IsNullOrWhiteSpace(id)) { return false; }

            lock(Sync)
            {
                if(!Objects.ContainsKey(id)) { return false; }

                if(Aliases.TryGetValue(newalias,out String? boundId) && !String.Equals(boundId,id,StringComparison.Ordinal))
                {
                    return false;
                }

                foreach(String oldAlias in Aliases.Where(_ => String.Equals(_.Value,id,StringComparison.Ordinal)).Select(_ => _.Key).ToArray())
                {
                    Aliases.Remove(oldAlias);
                }

                Aliases[newalias] = id;

                LogDiagnostic($"Set alias '{newalias}' for object {id}");

                return true;
            }
        }
        catch { return false; }
    }

    public static String? ResolveId(String? idoralias)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(idoralias)) { return null; }

            lock(Sync)
            {
                if(Objects.ContainsKey(idoralias)) { return idoralias; }

                return Aliases.TryGetValue(idoralias,out String? id) ? id : null;
            }
        }
        catch { return null; }
    }

    private static ToolPodRef CreateReference(String id , Object value , String? alias = null)
    {
        Type t = value.GetType();

        return new()
        {
            Id = id,
            Alias = alias,
            Type = t.FullName ?? t.Name,
            Assembly = t.Assembly.GetName().FullName
        };
    }
}