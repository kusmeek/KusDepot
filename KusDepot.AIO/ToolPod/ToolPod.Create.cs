namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodCreate")]
    [Description(Descriptions.Create)]
    public static ToolPodResult Create(
        [Description(CreateRequest)] ToolPodCreateRequest? request)
    {
        try
        {
            if(request is null) { return ErrorResult(ErrorStrings.CreateRequestRequired); }

            if(String.IsNullOrWhiteSpace(request.Type)) { return ErrorResult(ErrorStrings.CreateRequestTypeRequired); }

            ObjectBuilder b = new ObjectBuilder().WithContext(Context).WithType(request.Type); Object?[] args = BindArguments(request.Arguments);

            for(Int32 i = 0; i < args.Length; i++)
            {
                if(!b.SetArgument(i,args[i])) { return ErrorResult(String.Format(ErrorStrings.BindConstructorArgumentFailedFormat,i)); }
            }

            if(b.Build() is false) { return ErrorResult(String.Format(ErrorStrings.CreateTypeFailedFormat,request.Type)); }

            return NormalizeResult(b.Value,request.Alias);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.CreateFailed); return ErrorResult(_.Message); }
    }
}