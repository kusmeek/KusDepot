namespace KusDepot.Security;

/**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/main/*'/>*/
public static partial class ToolManifestRegistry
{
    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/field[@name="Sync"]/*'/>*/
    private static readonly Lock Sync = new();

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/field[@name="State"]/*'/>*/
    private static RegistryState State = RegistryState.Empty;

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="CreateUnlockCode"]/*'/>*/
    private static Byte[] CreateUnlockCode(ManagementKey managementkey)
    {
        if(managementkey is null) { return Array.Empty<Byte>(); }

        try { return SHA512.HashData(managementkey.Key); }

        catch { return Array.Empty<Byte>(); }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="GetLocked"]/*'/>*/
    public static Boolean GetLocked()
    {
        try { return State.Locked; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetToolManifestRegistryLockedFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="Lock"]/*'/>*/
    public static Boolean Lock(ManagementKey? managementkey = null)
    {
        try
        {
            Byte[]? unlockcode = managementkey is null ? null : CreateUnlockCode(managementkey);

            lock(Sync)
            {
                RegistryState state = State;

                if(state.Locked) { return false; }

                State = state with { Locked = true , UnlockCode = unlockcode };

                return true;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,LockToolManifestRegistryFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryBindType"]/*'/>*/
    public static Boolean TryBind(Type? tooltype , String? toolschemaid)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(tooltype); if(String.IsNullOrWhiteSpace(toolschemaid)) { return false; }

            lock(Sync)
            {
                RegistryState state = State;

                if(state.Locked) { return false; }

                if(state.BySchemaID.ContainsKey(toolschemaid) is false) { return false; }

                State = state with { SchemaIDByType = state.SchemaIDByType.SetItem(tooltype,toolschemaid) };

                return true;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryBindToolManifestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryBindManifest"]/*'/>*/
    public static Boolean TryBind<TTool>(ToolManifest? manifest)
    {
        try
        {
            return manifest is not null && String.IsNullOrWhiteSpace(manifest.ToolSchemaID) is false && TryBind(typeof(TTool),manifest.ToolSchemaID);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryBindToolManifestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryCreateEntry"]/*'/>*/
    private static Boolean TryCreateEntry(ToolManifest? manifest , out RegistryEntry? entry)
    {
        entry = null;

        try
        {
            if(manifest is null || String.IsNullOrWhiteSpace(manifest.ToolSchemaID)) { return false; }

            var methodindexes = ImmutableDictionary.CreateBuilder<String,Int32>(StringComparer.Ordinal);

            foreach(ToolOperationDescriptor operation in manifest.Operations)
            {
                if(operation is null || operation.Index < 0 || String.IsNullOrWhiteSpace(operation.MethodName)) { return false; }

                if(methodindexes.TryAdd(operation.MethodName,operation.Index) is false) { return false; }
            }

            entry = new RegistryEntry(manifest,methodindexes.ToImmutable()); return true;
        }
        catch { entry = null; return false; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryGetBindings"]/*'/>*/
    public static Boolean TryGetBindings(String? toolschemaid , out ImmutableArray<Type> tooltypes)
    {
        tooltypes = [];

        try
        {
            if(String.IsNullOrWhiteSpace(toolschemaid)) { return false; }

            RegistryState state = State; if(state.BySchemaID.ContainsKey(toolschemaid) is false) { return false; }

            tooltypes = state.SchemaIDByType.Where(_ => StringComparer.Ordinal.Equals(_.Value,toolschemaid)).Select(_ => _.Key).ToImmutableArray();
            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetBindingsFail); if(NoExceptions) { tooltypes = []; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryGetBoundSchemaID"]/*'/>*/
    public static Boolean TryGetBoundSchemaID(Type? tooltype , out String toolschemaid)
    {
        toolschemaid = String.Empty;

        try
        {
            ArgumentNullException.ThrowIfNull(tooltype); RegistryState state = State;

            if(state.SchemaIDByType.TryGetValue(tooltype,out toolschemaid!) is false) { toolschemaid = String.Empty; return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetBoundSchemaIDFail); if(NoExceptions) { toolschemaid = String.Empty; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryGetCurrentManifest"]/*'/>*/
    public static Boolean TryGetCurrentManifest(Type? tooltype , out ToolManifest manifest)
    {
        manifest = null!;

        try
        {
            ArgumentNullException.ThrowIfNull(tooltype);

            if(TryGetBoundSchemaID(tooltype,out String toolschemaid) is false) { return false; }

            if(TryGetManifest(toolschemaid,out manifest) is false) { manifest = null!; return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetToolManifestFail); if(NoExceptions) { manifest = null!; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryGetManifestBySchemaID"]/*'/>*/
    public static Boolean TryGetManifest(String? toolschemaid , out ToolManifest manifest)
    {
        manifest = null!;

        try
        {
            if(String.IsNullOrWhiteSpace(toolschemaid)) { return false; }

            if(State.BySchemaID.TryGetValue(toolschemaid,out RegistryEntry? entry) is false) { return false; }

            manifest = entry.Manifest; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetToolManifestFail); if(NoExceptions) { manifest = null!; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryGetManifestByType"]/*'/>*/
    public static Boolean TryGetManifest(Type? tooltype , out ToolManifest manifest)
    {
        manifest = null!;

        try
        {
            ArgumentNullException.ThrowIfNull(tooltype); RegistryState state = State;

            if(state.SchemaIDByType.TryGetValue(tooltype,out String? schemaid) is false) { return false; }

            if(state.BySchemaID.TryGetValue(schemaid,out RegistryEntry? entry) is false) { return false; }

            manifest = entry.Manifest; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetToolManifestFail); if(NoExceptions) { manifest = null!; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryRegisterManifest"]/*'/>*/
    public static Boolean TryRegister(ToolManifest? manifest)
    {
        try
        {
            if(TryCreateEntry(manifest,out RegistryEntry? entry) is false) { return false; }

            lock(Sync)
            {
                RegistryState state = State; if(state.Locked) { return false; }

                if(state.BySchemaID.ContainsKey(entry!.Manifest.ToolSchemaID!)) { return false; }

                State = state with { BySchemaID = state.BySchemaID.SetItem(entry.Manifest.ToolSchemaID!,entry) };

                return true;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryRegisterToolManifestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryRegisterAndBindManifest"]/*'/>*/
    public static Boolean TryRegisterAndBind<TTool>(ToolManifest? manifest)
    {
        try
        {
            return TryRegister(manifest) && TryBind<TTool>(manifest);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryRegisterToolManifestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryResolveOperationBySchemaID"]/*'/>*/
    public static Boolean TryResolveOperation(String? toolschemaid , String? methodname , out Int32 index)
    {
        index = -1;

        try
        {
            if(String.IsNullOrWhiteSpace(toolschemaid) || String.IsNullOrWhiteSpace(methodname)) { return false; }

            if(State.BySchemaID.TryGetValue(toolschemaid,out RegistryEntry? entry) is false) { return false; }

            if(entry.MethodIndexes.TryGetValue(methodname,out index) is false) { index = -1; return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryResolveToolOperationFail); if(NoExceptions) { index = -1; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryResolveOperationByType"]/*'/>*/
    public static Boolean TryResolveOperation(Type? tooltype , String? methodname , out Int32 index)
    {
        index = -1;

        try
        {
            ArgumentNullException.ThrowIfNull(tooltype); RegistryState state = State;

            if(state.SchemaIDByType.TryGetValue(tooltype,out String? schemaid) is false) { return false; }

            if(state.BySchemaID.TryGetValue(schemaid,out RegistryEntry? entry) is false) { return false; }

            if(entry.MethodIndexes.TryGetValue(methodname ?? String.Empty,out index) is false) { index = -1; return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryResolveToolOperationFail); if(NoExceptions) { index = -1; return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryUnRegisterSchemaID"]/*'/>*/
    public static Boolean TryUnRegister(String? toolschemaid)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(toolschemaid)) { return false; }

            lock(Sync)
            {
                RegistryState state = State;

                if(state.Locked) { return false; }

                if(state.BySchemaID.ContainsKey(toolschemaid) is false) { return false; }

                var bindings = state.SchemaIDByType;

                foreach(var binding in state.SchemaIDByType)
                {
                    if(StringComparer.Ordinal.Equals(binding.Value,toolschemaid)) { bindings = bindings.Remove(binding.Key); }
                }

                State = state with { BySchemaID = state.BySchemaID.Remove(toolschemaid) , SchemaIDByType = bindings };

                return true;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryUnRegisterToolManifestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="TryUnBindType"]/*'/>*/
    public static Boolean TryUnBind(Type? tooltype)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(tooltype);

            lock(Sync)
            {
                RegistryState state = State;

                if(state.Locked) { return false; }

                if(state.SchemaIDByType.ContainsKey(tooltype) is false) { return false; }

                State = state with { SchemaIDByType = state.SchemaIDByType.Remove(tooltype) };

                return true;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryUnBindToolManifestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/class[@name="ToolManifestRegistry"]/method[@name="UnLock"]/*'/>*/
    public static Boolean UnLock(ManagementKey? managementkey)
    {
        try
        {
            RegistryState state = State; if(state.Locked is false || state.UnlockCode is null || managementkey is null) { return false; }

            Byte[] unlockcode = CreateUnlockCode(managementkey);

            try
            {
                if(FixedTimeEquals(state.UnlockCode,unlockcode) is false) { return false; }
            }
            finally { ZeroMemory(unlockcode); }

            lock(Sync)
            {
                state = State;

                if(state.Locked is false || state.UnlockCode is null) { return false; }

                State = state with { Locked = false , UnlockCode = null };

                return true;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnLockToolManifestRegistryFail); if(NoExceptions) { return false; } throw; }
    }
}