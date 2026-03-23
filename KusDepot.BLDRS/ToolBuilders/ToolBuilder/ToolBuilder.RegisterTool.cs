namespace KusDepot.Builders;

public partial class ToolBuilder
{
    ///<inheritdoc/>
    public IToolBuilder RegisterTool(ITool tool , String? name = null , ImmutableArray<Int32>? permissions = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            RegisterTool_NoSync(tool,name,permissions); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterTool(Func<ITool?> factory , String? name = null , ImmutableArray<Int32>? permissions = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(factory is null) { return this; }

            ITool? tool = factory(); if(tool is null) { return this; }

            RegisterTool_NoSync(tool,name,permissions); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterTool(Type tooltype , Object?[]? arguments = null , String? name = null , ImmutableArray<Int32>? permissions = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(tooltype is null) { return this; }

            if(tooltype.IsAssignableTo(typeof(ITool)) is false) { return this; }

            ITool tool;

            if(arguments is null)
            {
                tool = (ITool)Activator.CreateInstance(tooltype)!;
            }
            else
            {
                var b = ObjectBuilder.Create(tooltype);

                for(Int32 i = 0; i < arguments.Length; i++)
                {
                    if(b.SetArgument(i,arguments[i]) is false) { return this; }
                }

                if(b.Build() is false || b.Value is not ITool built) { return this; }

                tool = built;
            }

            RegisterTool_NoSync(tool,name,permissions); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterTool<TTool>(String? name = null , ImmutableArray<Int32>? permissions = null) where TTool : class , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            RegisterTool_NoSync(Activator.CreateInstance<TTool>()!,name,permissions); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterTool<TTool>(Int32 count , Func<Int32,String> namefactory , Func<Int32,ImmutableArray<Int32>?>? permissionsfactory = null) where TTool : class , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(count <= 0 || namefactory is null) { return this; }

            List<String> names = new(count);

            for(Int32 i = 0; i < count; i++)
            {
                String? n = namefactory(i);

                if(String.IsNullOrEmpty(n)) { return this; }

                names.Add(new(n));
            }

            if(names.Distinct(StringComparer.Ordinal).Count() != names.Count) { return this; }

            if(names.Any(n => this.RegisteredHostedTools.Any(_ => String.Equals(_.Name,n,StringComparison.Ordinal)))) { return this; }

            for(Int32 i = 0; i < count; i++)
            {
                RegisterTool_NoSync(Activator.CreateInstance<TTool>()!,names[i],permissionsfactory?.Invoke(i));
            }

            return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="AddRegisteredTools"]/*'/>*/
    protected virtual Boolean AddRegisteredTools(ITool tool)
    {
        try
        {
            if(tool is null || Equals(this.RegisteredHostedTools.Count,0)) { return true; }

            foreach(var _ in this.RegisteredHostedTools)
            {
                if(tool.AddHostedService(_.Tool,name:_.Name,permissions:_.Permissions,start:false).GetAwaiter().GetResult() is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderAddRegisteredToolsFail); return false; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="AddRegisteredToolsAsync"]/*'/>*/
    protected virtual async Task<Boolean> AddRegisteredToolsAsync(ITool tool , CancellationToken cancel = default)
    {
        try
        {
            if(tool is null || Equals(this.RegisteredHostedTools.Count,0)) { return true; }

            foreach(var _ in this.RegisteredHostedTools)
            {
                if(await tool.AddHostedService(_.Tool,name:_.Name,permissions:_.Permissions,start:false,cancel:cancel).ConfigureAwait(false) is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderAddRegisteredToolsFail); return false; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="RegisterTool_NoSync"]/*'/>*/
    protected void RegisterTool_NoSync(ITool tool , String? name = null , ImmutableArray<Int32>? permissions = null)
    {
        if(tool is null) { return; }

        if(this.RegisteredHostedTools.Any(_ => ReferenceEquals(_.Tool,tool))) { return; }

        if(String.IsNullOrEmpty(name) is false && this.RegisteredHostedTools.Any(_ => String.Equals(_.Name,name,StringComparison.Ordinal))) { return; }

        this.RegisteredHostedTools.Add(new(){ Tool = tool , Name = name is null ? null : new(name) , Permissions = permissions });
    }
}