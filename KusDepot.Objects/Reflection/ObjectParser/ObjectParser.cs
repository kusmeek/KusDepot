namespace KusDepot.Reflection;

/**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/main/*'/>*/
public sealed class ObjectParser
{
    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/field[@name="Data"]/*'/>*/
    private String? Data;

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/field[@name="Formatter"]/*'/>*/
    private IFormatProvider? Formatter;

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/field[@name="Parsed"]/*'/>*/
    private Boolean Parsed;

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/field[@name="Sync"]/*'/>*/
    private readonly Object Sync = new();

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/property[@name="Provider"]/*'/>*/
    public TypeProvider Provider { get; }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/property[@name="Value"]/*'/>*/
    public Object? Value { get; private set; }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/constructor[@name="Constructor"]/*'/>*/
    public ObjectParser(TypeResolutionMode mode = Hybrid) { Provider = new(mode); }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="CreateString"]/*'/>*/
    public static ObjectParser Create(String? name , TypeResolutionMode mode = Hybrid)
    {
        try
        {
            ObjectParser p = new(mode); _ = p.Provider.SetType(name); return p;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(ObjectParser),null); if(NoExceptions) { return new(mode); } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="WithData"]/*'/>*/
    public ObjectParser WithData(String? data)
    {
        try { _ = SetData(data); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithDataFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="SetData"]/*'/>*/
    public Boolean SetData(String? data)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Parsed) { return false; }

            Data = data; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetDataFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="WithFormatter"]/*'/>*/
    public ObjectParser WithFormatter(IFormatProvider? formatter)
    {
        try { _ = SetFormatter(formatter); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithFormatterFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="SetFormatter"]/*'/>*/
    public Boolean SetFormatter(IFormatProvider? formatter)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Parsed) { return false; }

            Formatter = formatter; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetFormatterFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="WithContext"]/*'/>*/
    public ObjectParser WithContext(AssemblyLoadContext? context)
    {
        try { _ = SetContext(context); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithContextFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="SetContext"]/*'/>*/
    public Boolean SetContext(AssemblyLoadContext? context)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Parsed) { return false; }

            return Provider.SetContext(context);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContextFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="TryParse"]/*'/>*/
    public Boolean TryParse()
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Parsed) { return true; }

            Type? t = Provider.Resolve(); if(t is null) { return false; }

            if(t == typeof(String))
            {
                Value = Data; Parsed = true; return true;
            }

            MethodInfo? m = FindTryParseMethod(t);

            if(m is not null)
            {
                if(TryInvokeTryParseMethod(m,out Object? parsed) is false || parsed is null) { return false; }

                Value = parsed; Parsed = true; return true;
            }

            MethodInfo? pm = FindParseMethod(t); if(pm is null) { return false; }

            if(TryInvokeParseMethod(pm,out Object? value) is false || value is null) { return false; }

            Value = value; Parsed = true; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="Parse"]/*'/>*/
    public Object? Parse()
    {
        try { return TryParse() ? Value : null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="FindTryParseMethod"]/*'/>*/
    private static MethodInfo? FindTryParseMethod(Type type)
    {
        try
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach(MethodInfo m in methods)
            {
                if(IsTryParseMethod(m,type,formatteraware:true)) { return m; }
            }

            foreach(MethodInfo m in methods)
            {
                if(IsTryParseMethod(m,type,formatteraware:false)) { return m; }
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FindTryParseMethodFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="FindParseMethod"]/*'/>*/
    private static MethodInfo? FindParseMethod(Type type)
    {
        try
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach(MethodInfo m in methods)
            {
                if(IsParseMethod(m,type,formatteraware:true)) { return m; }
            }

            foreach(MethodInfo m in methods)
            {
                if(IsParseMethod(m,type,formatteraware:false)) { return m; }
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FindParseMethodFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="TryInvokeTryParseMethod"]/*'/>*/
    private Boolean TryInvokeTryParseMethod(MethodInfo method , [MaybeNullWhen(false)] out Object? value)
    {
        value = null;

        try
        {
            ParameterInfo[] p = method.GetParameters(); Object?[] a;

            if(Equals(p.Length,3)) { a = [ Data , Formatter , null ]; }

            else if(Equals(p.Length,2)) { a = [ Data , null ]; } else { return false; }

            Boolean ok;

            try { ok = (Boolean)(method.Invoke(null,a) ?? false); } catch { return false; }

            if(!ok) { return false; }

            value = a[^1]; return value is not null;
        }
        catch ( Exception _ ) { value = null; KusDepotLog.Error(_,TryInvokeTryParseFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="TryInvokeParseMethod"]/*'/>*/
    private Boolean TryInvokeParseMethod(MethodInfo method , [MaybeNullWhen(false)] out Object? value)
    {
        value = null;

        try
        {
            ParameterInfo[] p = method.GetParameters(); Object?[] a;

            if(Equals(p.Length,2)) { a = [ Data , Formatter ]; }

            else if(Equals(p.Length,1)) { a = [ Data ]; } else { return false; }

            try { value = method.Invoke(null,a); } catch { return false; }

            return value is not null;
        }
        catch ( Exception _ ) { value = null; KusDepotLog.Error(_,TryInvokeParseFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="IsTryParseMethod"]/*'/>*/
    private static Boolean IsTryParseMethod(MethodInfo method , Type type , Boolean formatteraware)
    {
        try
        {
            if(!String.Equals(method.Name,"TryParse",Ordinal)) { return false; }

            if(method.ReturnType != typeof(Boolean)) { return false; }

            ParameterInfo[] p = method.GetParameters();

            if(formatteraware)
            {
                if(!Equals(p.Length,3)) { return false; }

                if(p[0].ParameterType != typeof(String)) { return false; }

                if(p[1].ParameterType != typeof(IFormatProvider)) { return false; }

                if(!p[2].IsOut) { return false; }

                if(p[2].ParameterType.GetElementType() != type) { return false; }

                return true;
            }

            if(!Equals(p.Length,2)) { return false; }

            if(p[0].ParameterType != typeof(String)) { return false; }

            if(!p[1].IsOut) { return false; }

            if(p[1].ParameterType.GetElementType() != type) { return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsTryParseMethodFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="IsParseMethod"]/*'/>*/
    private static Boolean IsParseMethod(MethodInfo method , Type type , Boolean formatteraware)
    {
        try
        {
            if(!String.Equals(method.Name,"Parse",Ordinal)) { return false; }

            if(method.ReturnType != type) { return false; }

            ParameterInfo[] p = method.GetParameters();

            if(formatteraware)
            {
                if(!Equals(p.Length,2)) { return false; }

                if(p[0].ParameterType != typeof(String)) { return false; }

                if(p[1].ParameterType != typeof(IFormatProvider)) { return false; }

                return true;
            }

            if(!Equals(p.Length,1)) { return false; }

            return p[0].ParameterType == typeof(String);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsParseMethodFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="CanParseType"]/*'/>*/
    public static Boolean CanParse(Type? type)
    {
        try
        {
            if(type is null) { return false; }

            return type == typeof(String) || FindTryParseMethod(type) is not null || FindParseMethod(type) is not null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CanParseTypeFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectParser.xml' path='ObjectParser/class[@name="ObjectParser"]/method[@name="CanParseString"]/*'/>*/
    public static Boolean CanParse(String? name , TypeResolutionMode mode = Hybrid)
    {
        try
        {
            ObjectParser p = new(mode); if(p.Provider.SetType(name) is false) { return false; }

            return CanParse(p.Provider.Resolve());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CanParseNameFail); if(NoExceptions) { return false; } throw; }
    }
}