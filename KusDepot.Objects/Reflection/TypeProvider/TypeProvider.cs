namespace KusDepot.Reflection;

/**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/main/*'/>*/
public sealed partial class TypeProvider
{
    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="AssemblyPaths"]/*'/>*/
    private List<String>? AssemblyPaths;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="AssemblyDirectories"]/*'/>*/
    private List<String>? AssemblyDirectories;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="AssemblyImages"]/*'/>*/
    private List<Byte[]>? AssemblyImages;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="AssemblyNames"]/*'/>*/
    private List<AssemblyName>? AssemblyNames;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="Context"]/*'/>*/
    private AssemblyLoadContext? Context;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="ContextResolver"]/*'/>*/
    private Func<AssemblyLoadContext,AssemblyName,Assembly?>? ContextResolver;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="OwnsContext"]/*'/>*/
    private Boolean OwnsContext;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="Mode"]/*'/>*/
    private readonly TypeResolutionMode Mode;

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/field[@name="Sync"]/*'/>*/
    private readonly Object Sync = new();

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/property[@name="Name"]/*'/>*/
    public String? Name { get; private set; }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/property[@name="Value"]/*'/>*/
    public Type? Value { get; private set; }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/constructor[@name="Constructor"]/*'/>*/
    public TypeProvider(TypeResolutionMode mode = Hybrid) { Mode = Enum.IsDefined(mode) ? mode : Hybrid; }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="Create"]/*'/>*/
    public static TypeProvider Create(String? name , TypeResolutionMode mode = Hybrid)
    {
        try { TypeProvider p = new(mode); _ = p.SetType(name); return p; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(TypeProvider),null); if(NoExceptions) { return new(mode); } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="WithType"]/*'/>*/
    public TypeProvider WithType(String? name)
    {
        try { _ = SetType(name); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithTypeFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="SetType"]/*'/>*/
    public Boolean SetType(String? name)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            if(String.IsNullOrWhiteSpace(name)) { return false; }

            Name = name; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTypeFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="WithAssemblyPaths"]/*'/>*/
    public TypeProvider WithAssemblies(IEnumerable<String>? paths)
    {
        try { _ = SetAssemblies(paths); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithAssembliesFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="SetAssemblyPaths"]/*'/>*/
    public Boolean SetAssemblies(IEnumerable<String>? paths)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            if(paths is null) { return false; }

            List<String> p = new();

            foreach(String? _ in paths)
            {
                if(String.IsNullOrWhiteSpace(_)) { continue; }

                String f = Path.GetFullPath(_); if(File.Exists(f) is false) { continue; }

                if(p.Contains(f,StringComparer.OrdinalIgnoreCase)) { continue; }

                p.Add(f);
            }

            if(Equals(p.Count,0)) { return false; }

            AssemblyPaths = p; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetAssembliesFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="WithAssemblyImages"]/*'/>*/
    public TypeProvider WithAssemblies(IEnumerable<MemoryStream?>? streams)
    {
        try { _ = SetAssemblies(streams); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithAssembliesFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="SetAssemblyImages"]/*'/>*/
    public Boolean SetAssemblies(IEnumerable<MemoryStream?>? streams)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            if(streams is null) { return false; }

            List<Byte[]> i = new();

            foreach(MemoryStream? _ in streams)
            {
                if(TrySnapshotAssemblyImage(_,out Byte[]? b) is false || b is null) { continue; }

                if(i.Any(__ => __.AsSpan().SequenceEqual(b))) { continue; }

                i.Add(b);
            }

            if(Equals(i.Count,0)) { return false; }

            AssemblyImages = i; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetAssembliesFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="WithDirectories"]/*'/>*/
    public TypeProvider WithDirectories(IEnumerable<String>? paths)
    {
        try { _ = SetDirectories(paths); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithDirectoriesFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="SetDirectories"]/*'/>*/
    public Boolean SetDirectories(IEnumerable<String>? paths)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            if(paths is null) { return false; }

            List<String> p = new();

            foreach(String? _ in paths)
            {
                if(String.IsNullOrWhiteSpace(_)) { continue; }

                String d = Path.GetFullPath(_); if(Directory.Exists(d) is false) { continue; }

                if(p.Contains(d,StringComparer.OrdinalIgnoreCase)) { continue; }

                p.Add(d);
            }

            if(Equals(p.Count,0)) { return false; }

            AssemblyDirectories = p; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetDirectoriesFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="WithAssemblyNames"]/*'/>*/
    public TypeProvider WithAssemblyNames(IEnumerable<AssemblyName>? names)
    {
        try { _ = SetAssemblyNames(names); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithAssemblyNamesFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="SetAssemblyNames"]/*'/>*/
    public Boolean SetAssemblyNames(IEnumerable<AssemblyName>? names)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            if(names is null) { return false; }

            List<AssemblyName> n = new();

            foreach(AssemblyName? _ in names)
            {
                if(TryNormalizeAssemblyName(_,out AssemblyName? a) is false || a is null) { continue; }

                if(n.Any(__ => AssemblyNamesEqual(__,a))) { continue; }

                n.Add(a);
            }

            if(Equals(n.Count,0)) { return false; }

            AssemblyNames = n; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetAssemblyNamesFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="WithContext"]/*'/>*/
    public TypeProvider WithContext(AssemblyLoadContext? context)
    {
        try { _ = SetContext(context); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithContextFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="SetContext"]/*'/>*/
    public Boolean SetContext(AssemblyLoadContext? context)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            if(context is null) { return false; }

            if(Context is not null && OwnsContext && ReferenceEquals(Context,context) is false)
            {
                if(UnloadOwnedContext_NoSync() is false) { return false; }
            }

            Context = context; ContextResolver = null; OwnsContext = false; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContextFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="Clear"]/*'/>*/
    public Boolean Clear()
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            Name = null; Value = null; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ClearFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="Unload"]/*'/>*/
    public Boolean Unload()
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Context is not null && OwnsContext is false) { return false; }

            Value = null; return UnloadOwnedContext_NoSync();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnloadFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryResolve"]/*'/>*/
    public Boolean TryResolve()
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return false; }

            return TryResolve_NoSync();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryResolveFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="Resolve"]/*'/>*/
    public Type? Resolve()
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(Value is not null) { return Value; }

            return TryResolve_NoSync() ? Value : null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ResolveFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }
}