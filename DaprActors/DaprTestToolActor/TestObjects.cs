namespace KusDepot.DaprActors;

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
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a);

            AddOutput(a.ID,Guid.NewGuid()); return a.ID;
        }
        catch { AddOutput(a?.ID); CleanUp(a); return null; }

        finally { CleanUp(a); }
    }
}