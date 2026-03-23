namespace KusDepot.Builders;

public partial class ToolBuilder
{
    ///<inheritdoc/>
    public IToolBuilder RegisterCommand(String handle , ICommand command , ImmutableArray<Int32>? permissions = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(handle) || command is null) { return this; }

            if(this.RegisteredCommands.Any(_ => String.Equals(_.Handle,handle,StringComparison.Ordinal))) { return this; }

            this.RegisteredCommands.Add(new(){ Command = command , Handle = new(handle) , Permissions = permissions }); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand(String handle , Func<ICommand?> factory , ImmutableArray<Int32>? permissions = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(handle) || factory is null) { return this; }

            if(this.RegisteredCommands.Any(_ => String.Equals(_.Handle,handle,StringComparison.Ordinal))) { return this; }

            ICommand? command = factory(); if(command is null) { return this; }

            this.RegisteredCommands.Add(new(){ Command = command , Handle = new(handle) , Permissions = permissions }); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand(String handle , Type commandtype , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(handle) || commandtype is null) { return this; }

            if(commandtype.IsAssignableTo(typeof(ICommand)) is false) { return this; }

            if(this.RegisteredCommands.Any(_ => String.Equals(_.Handle,handle,StringComparison.Ordinal))) { return this; }

            if(arguments is null)
            {
                this.RegisteredCommands.Add(new(){ Command = (ICommand)Activator.CreateInstance(commandtype)! , Handle = new(handle) , Permissions = permissions }); return this;
            }

            var b = ObjectBuilder.Create(commandtype);

            for(Int32 i = 0; i < arguments.Length; i++)
            {
                if(b.SetArgument(i,arguments[i]) is false) { return this; }
            }

            if(b.Build() is false || b.Value is not ICommand cmd) { return this; }

            this.RegisteredCommands.Add(new(){ Command = cmd , Handle = new(handle) , Permissions = permissions }); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand<TCommand>(String handle , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null) where TCommand : notnull , ICommand , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(String.IsNullOrEmpty(handle)) { return this; }

            if(this.RegisteredCommands.Any(_ => String.Equals(_.Handle,handle,StringComparison.Ordinal))) { return this; }

            if(arguments is null)
            {
                this.RegisteredCommands.Add(new(){ Command = Activator.CreateInstance<TCommand>()! , Handle = new(handle) , Permissions = permissions }); return this;
            }

            var b = ObjectBuilder.Create(typeof(TCommand));

            for(Int32 i = 0; i < arguments.Length; i++)
            {
                if(b.SetArgument(i,arguments[i]) is false) { return this; }
            }

            if(b.Build() is false || b.Value is not ICommand cmd) { return this; }

            this.RegisteredCommands.Add(new(){ Command = cmd , Handle = new(handle) , Permissions = permissions }); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="RegisterCommands"]/*'/>*/
    protected virtual Boolean RegisterCommands(ITool tool)
    {
        try
        {
            if(tool is null || Equals(this.RegisteredCommands.Count,0)) { return true; }

            foreach(var _ in this.RegisteredCommands)
            {
                if(tool.RegisterCommand(_.Handle,_.Command,permissions:_.Permissions).GetAwaiter().GetResult() is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return false; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="RegisterCommandsAsync"]/*'/>*/
    protected virtual async Task<Boolean> RegisterCommandsAsync(ITool tool , CancellationToken cancel = default)
    {
        try
        {
            if(tool is null || Equals(this.RegisteredCommands.Count,0)) { return true; }

            foreach(var _ in this.RegisteredCommands)
            {
                if(await tool.RegisterCommand(_.Handle,_.Command,permissions:_.Permissions,cancel:cancel).ConfigureAwait(false) is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return false; }
    }
}