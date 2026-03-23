namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodDescribe")]
    [Description(Describe)]
    public static ToolPodResult DescribeObject(
        [Description(DescribeIdOrAlias)] String? idoralias)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(idoralias)) { return ErrorResult(ErrorStrings.ObjectIdOrAliasRequired); }

            String? id = ResolveId(idoralias); if(String.IsNullOrWhiteSpace(id)) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            Object? value = GetTrackedObject(id); if(value is null) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            ToolPodRef? reference = GetReference(id); if(reference is null) { return ErrorResult(ErrorStrings.ReferenceCreationFailed); }

            Type t = value.GetType();

            ToolPodObjectInfo info = new()
            {
                Reference = reference,
                Properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(_ => _.Name)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(_ => _,StringComparer.Ordinal)
                    .ToArray(),
                Methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(_ => _.IsSpecialName is false)
                    .Select(_ => _.Name)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(_ => _,StringComparer.Ordinal)
                    .ToArray()
            };

            return ValueResult(JsonSerializer.Serialize(info),typeof(ToolPodObjectInfo).FullName);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.DescribeFailed); return ErrorResult(_.Message); }
    }
}