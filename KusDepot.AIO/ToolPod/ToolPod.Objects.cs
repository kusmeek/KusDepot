namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodList")]
    [Description(Descriptions.ListObjects)]
    public static ToolPodResult ListObjects()
    {
        try
        {
            return ValueResult(JsonSerializer.Serialize(ListReferences()),typeof(ToolPodRef[]).FullName);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.ListObjectsFailed); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "ToolPodSetAlias")]
    [Description(Descriptions.SetAlias)]
    public static ToolPodResult SetAlias(
        [Description(SetAliasIdOrAlias)] String idoralias,
        [Description(SetAliasNewAlias)] String newalias)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(idoralias)) { return ErrorResult(ErrorStrings.ObjectIdOrAliasRequired); }
            if(String.IsNullOrWhiteSpace(newalias)) { return ErrorResult(ErrorStrings.NewAliasRequired); }

            if(!SetObjectAlias(idoralias, newalias)) 
            {
                return ErrorResult(ErrorStrings.SetAliasFailed); 
            }

            ToolPodRef? reference = GetReference(ResolveId(newalias));
            
            return reference is not null ? ReferenceResult(reference) : ErrorResult(ErrorStrings.AliasSetNoReference);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.SetAliasFailed); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "ToolPodRemove")]
    [Description(Remove)]
    public static ToolPodResult RemoveTrackedObject(
        [Description(RemoveIdOrAlias)] String? idoralias)
    {
        try
        {
            String? id = ResolveId(idoralias); if(String.IsNullOrWhiteSpace(id)) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            if(!RemoveObject(id)) { return ErrorResult(ErrorStrings.RemoveObjectFailed); }

            return VoidResult();
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.RemoveObjectFailed); return ErrorResult(_.Message); }
    }
}