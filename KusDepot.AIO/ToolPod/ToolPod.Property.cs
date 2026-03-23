namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodGetProperty")]
    [Description(Descriptions.GetProperty)]
    public static ToolPodResult GetProperty(
        [Description(GetPropertyIdOrAlias)] String? idoralias,
        [Description(GetPropertyName)] String? name,
        [Description(GetPropertyAlias)] String? alias = null)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(idoralias)) { return ErrorResult(ErrorStrings.ObjectIdRequired); }

            if(String.IsNullOrWhiteSpace(name)) { return ErrorResult(ErrorStrings.PropertyNameRequired); }

            String? id = ResolveId(idoralias); if(String.IsNullOrWhiteSpace(id)) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            Object? target = GetTrackedObject(id); if(target is null) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            ObjectInvoker i = ObjectInvoker.Create(target); Object? value = i.GetProperty(name);

            return NormalizeResult(value,alias);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.GetPropertyFailed); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "ToolPodSetProperty")]
    [Description(Descriptions.SetProperty)]
    public static ToolPodResult SetProperty(
        [Description(SetPropertyIdOrAlias)] String? idoralias,
        [Description(SetPropertyName)] String? name,
        [Description(SetPropertyArgument)] ToolPodArgument? argument)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(idoralias)) { return ErrorResult(ErrorStrings.ObjectIdRequired); }

            if(String.IsNullOrWhiteSpace(name)) { return ErrorResult(ErrorStrings.PropertyNameRequired); }

            String? id = ResolveId(idoralias); if(String.IsNullOrWhiteSpace(id)) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            Object? target = GetTrackedObject(id); if(target is null) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            ObjectInvoker i = ObjectInvoker.Create(target); Object? value = BindArgument(argument);

            if(!i.SetProperty(name,value)) { return ErrorResult(String.Format(ErrorStrings.SetPropertyErrorFormat,name)); }

            return VoidResult();
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.SetPropertyFailed); return ErrorResult(_.Message); }
    }
}