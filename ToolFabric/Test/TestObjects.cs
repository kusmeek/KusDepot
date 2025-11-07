namespace KusDepot.ToolFabric;

public class TestAccessManager : AccessManager
{
    public TestAccessManager(ITool? tool = null , ILogger? logger = null , X509Certificate2? certificate = null) : base(tool,logger,certificate){}

    public TestAccessManager() : base(){}

    public override AccessKey? RequestAccess(AccessRequest? request = null)
    {
        try
        {
            if(request is null || Tool is null) { return null; }

            if(request is ManagementRequest)
            {
                ManagementKey? k = ManagementKey.Parse((request as ManagementRequest)!.Data,null);

                if(Tool.CheckManager(k) is true || Tool.CheckOwner(k) is true) { return GenerateExecutiveKey(); }

                k = new OwnerKey(DeserializeCertificate((request as ManagementRequest)!.Data.ToByteArrayFromBase64())!);

                if(Tool.CheckOwner(k) is true) { return GenerateExecutiveKey(); }

                k = new ManagerKey(DeserializeCertificate((request as ManagementRequest)!.Data.ToByteArrayFromBase64())!);

                if(Tool.CheckManager(k) is true) { return GenerateExecutiveKey(); }
            }

            //if(Tool.GetLocked() is not false) { return null; }

            switch(request)
            {
                case HostRequest:
                {
                    if((request as HostRequest)!.External) { return GenerateHostKey(); }

                    if((request as HostRequest)!.Host!.IsHosting(Tool,Tool) ) { return GenerateHostKey(); }

                    return null;
                }

                case ServiceRequest:
                {
                    if(ReferenceEquals(Tool,(request as ServiceRequest)!.Tool)) { return GenerateExecutiveKey(); }

                    return GenerateServiceKey((request as ServiceRequest)!.Tool);
                }

                case StandardRequest: { return GenerateClientKey(); }

                default: { return null; }
            }
        }
        catch { return null; }
    }
}

public sealed class CommandTest : Command
{
    public CommandTest() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        try
        {
            AddOutput(a?.ID,DataItemGenerator.CreateDataSet(12));
        }
        catch ( Exception ) { return null; }

        return a!.ID;
    }


    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddOutput(a?.ID,DataItemGenerator.CreateDataSet(12)); await Task.CompletedTask; return true;
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception ) { AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception ) { AddOutput(a?.ID); CleanUp(a); return null; }
    }
}