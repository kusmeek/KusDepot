namespace KusDepot.Builders;

public partial class ToolBuilder
{
    ///<inheritdoc/>
    public IToolBuilder RegisterService(String name , IHostedService service)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(name) || service is null) { return this; }

            if(this.RegisteredServiceInstances.ContainsKey(name)) { return this; }

            this.RegisteredServiceInstances.Add(new(name),service); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterServiceFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterService(String name , Func<IHostedService?> factory)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(name) || factory is null) { return this; }

            if(this.RegisteredServiceInstances.ContainsKey(name)) { return this; }

            IHostedService? service = factory(); if(service is null) { return this; }

            this.RegisteredServiceInstances.Add(new(name),service); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterServiceFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterService(String name , Type servicetype)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(name) || servicetype is null) { return this; }

            if(servicetype.IsAssignableTo(typeof(IHostedService)) is false) { return this; }

            if(this.RegisteredServiceInstances.ContainsKey(name)) { return this; }

            this.RegisteredServiceInstances.Add(new(name),(Activator.CreateInstance(servicetype) as IHostedService)!); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterServiceFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterService<TService>(String name) where TService : class , IHostedService , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            this.RegisteredServiceInstances.Add(new(name),Activator.CreateInstance<TService>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterServiceFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="AddRegisteredServices"]/*'/>*/
    protected virtual Boolean AddRegisteredServices(ITool tool)
    {
        try
        {
            if(tool is null || Equals(this.RegisteredServiceInstances.Count,0)) { return true; }

            foreach(var _ in this.RegisteredServiceInstances)
            {
                if(tool.AddHostedService(_.Value,name:_.Key,start:false).GetAwaiter().GetResult() is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterServiceFail); return false; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="AddRegisteredServicesAsync"]/*'/>*/
    protected virtual async Task<Boolean> AddRegisteredServicesAsync(ITool tool , CancellationToken cancel = default)
    {
        try
        {
            if(tool is null || Equals(this.RegisteredServiceInstances.Count,0)) { return true; }

            foreach(var _ in this.RegisteredServiceInstances)
            {
                if(await tool.AddHostedService(_.Value,name:_.Key,start:false,cancel:cancel).ConfigureAwait(false) is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterServiceFail); return false; }
    }
}