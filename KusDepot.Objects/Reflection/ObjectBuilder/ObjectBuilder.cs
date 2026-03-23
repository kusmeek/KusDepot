namespace KusDepot.Reflection;

/**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/main/*'/>*/
public sealed class ObjectBuilder
{
    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/field[@name="Arguments"]/*'/>*/
    private readonly Dictionary<Int32,Object?> Arguments;

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/field[@name="BuildType"]/*'/>*/
    private Type? BuildType;

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/field[@name="Sync"]/*'/>*/
    private readonly Object Sync = new();

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/field[@name="TypeLocked"]/*'/>*/
    private Boolean TypeLocked;

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/property[@name="Provider"]/*'/>*/
    public TypeProvider Provider { get; }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/property[@name="Value"]/*'/>*/
    public Object? Value { get; private set; }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/constructor[@name="Constructor"]/*'/>*/
    public ObjectBuilder(TypeResolutionMode mode = Hybrid) { Arguments = new(); Provider = new(mode); }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="Build"]/*'/>*/
    public Boolean Build()
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null) { return true; }

            Type? t = ResolveBuildType(); if(t is null) { return false; }

            ConstructorInfo? c = SelectConstructor(t); if(c is null) { return false; }

            if(TryCreateInvokeArguments(c.GetParameters(),out Object?[]? p,out _,out _) is false || p is null) { return false; }

            Object? v = c.Invoke(p); if(v is null) { return false; }

            Value = v; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuildFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="CreateType"]/*'/>*/
    public static ObjectBuilder Create(Type? type , TypeResolutionMode mode = Hybrid)
    {
        try
        {
            ObjectBuilder b = new(mode); _ = b.SetType(type); return b;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(ObjectBuilder),null); if(NoExceptions) { return new(mode); } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="CreateString"]/*'/>*/
    public static ObjectBuilder Create(String? name , TypeResolutionMode mode = Hybrid)
    {
        try
        {
            ObjectBuilder b = new(mode); _ = b.SetType(name); return b;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(ObjectBuilder),null); if(NoExceptions) { return new(mode); } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="WithArgument"]/*'/>*/
    public ObjectBuilder WithArgument(Int32 index , Object? value)
    {
        try { _ = SetArgument(index,value); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithArgumentFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="SetArgument"]/*'/>*/
    public Boolean SetArgument(Int32 index , Object? value)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null || index < 0) { return false; }

            Arguments[index] = value; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetArgumentFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="WithTypeType"]/*'/>*/
    public ObjectBuilder WithType(Type? type)
    {
        try { _ = SetType(type); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithTypeFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="WithTypeString"]/*'/>*/
    public ObjectBuilder WithType(String? name)
    {
        try { _ = SetType(name); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithTypeFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="SetTypeType"]/*'/>*/
    public Boolean SetType(Type? type)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null || type is null || TypeLocked) { return false; }

            BuildType = type; TypeLocked = true; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTypeFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="SetTypeString"]/*'/>*/
    public Boolean SetType(String? name)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null || TypeLocked) { return false; }

            if(Provider.SetType(name) is false) { return false; }

            Type? t = Provider.Resolve(); if(t is null) { return false; }

            BuildType = t; TypeLocked = true; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTypeFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="WithContext"]/*'/>*/
    public ObjectBuilder WithContext(AssemblyLoadContext? context)
    {
        try { _ = SetContext(context); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithContextFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="SetContext"]/*'/>*/
    public Boolean SetContext(AssemblyLoadContext? context)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null || TypeLocked) { return false; }

            return Provider.SetContext(context);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContextFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="TryResolveType"]/*'/>*/
    public Boolean TryResolveType(String? name)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Value is not null || TypeLocked) { return false; }

            if(String.IsNullOrWhiteSpace(name)) { return false; }

            Boolean ok = false;

            try
            {
                if(Provider.Clear() is false) { return false; }

                if(Provider.SetType(name) is false) { return false; }

                ok = Provider.TryResolve(); return ok;
            }
            catch { return false; }

            finally { _ = Provider.Clear(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryResolveTypeFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="ResolveBuildType"]/*'/>*/
    private Type? ResolveBuildType()
    {
        try
        {
            if(BuildType is not null) { return BuildType; }

            Type? t = Provider.Resolve(); if(t is null) { return null; }

            BuildType = t; TypeLocked = true; return t;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ResolveBuildTypeFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="SelectConstructor"]/*'/>*/
    private ConstructorInfo? SelectConstructor(Type type)
    {
        try
        {
            ConstructorInfo[] c = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if(Equals(c.Length,0)) { return null; }

            Int32 width = GetRequiredWidth();

            ConstructorInfo? best = null; Boolean ambiguous = false;

            Int32 bestLength = Int32.MaxValue; Int32 bestExact = Int32.MinValue; Int32 bestAssignable = Int32.MinValue;

            foreach(ConstructorInfo ctor in c)
            {
                ParameterInfo[] p = ctor.GetParameters(); if(p.Length < width) { continue; }

                if(TryCreateInvokeArguments(p,out _,out Int32 exact,out Int32 assignable) is false) { continue; }

                Int32 cmp = ReflectionBindingScore.Compare(
                    p.Length,exact,assignable,isstatic:false,
                    bestLength,bestExact,bestAssignable,bestStatic:null,
                    preferInstanceOnTie:false);

                if(cmp > 0)
                {
                    best = ctor; bestLength = p.Length; bestExact = exact; bestAssignable = assignable; ambiguous = false; continue;
                }

                if(cmp < 0) { continue; }

                ambiguous = true;
            }

            if(ambiguous) { return null; }

            return best;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SelectConstructorFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="GetRequiredWidth"]/*'/>*/
    private Int32 GetRequiredWidth()
    {
        try
        {
            if(Equals(Arguments.Count,0)) { return 0; } Int32 m = -1;

            foreach(Int32 _ in Arguments.Keys) { if(_ > m) { m = _; } }

            return m + 1;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetRequiredWidthFail); if(NoExceptions) { return 0; } throw; }
    }

    /**<include file='ObjectBuilder.xml' path='ObjectBuilder/class[@name="ObjectBuilder"]/method[@name="TryCreateInvokeArguments"]/*'/>*/
    private Boolean TryCreateInvokeArguments(ParameterInfo[] parameters , out Object?[]? invoke , out Int32 exact , out Int32 assignable)
    {
        try
        {
            return ReflectionArgumentBinding.TryCreateInvokeArgs(
                parameters,Arguments,allowdefaultvalues:false,
                out invoke,out exact,out assignable);
        }
        catch ( Exception _ ) { invoke = null; exact = 0; assignable = 0; KusDepotLog.Error(_,TryCreateInvokeArgumentsFail); if(NoExceptions) { return false; } throw; }
    }
}