namespace KusDepot.Reflection;

/**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/main/*'/>*/
public sealed class ObjectInvoker
{
    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/field[@name="Instance"]/*'/>*/
    private Object? Instance;

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/field[@name="TargetType"]/*'/>*/
    private Type? TargetType;

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/field[@name="AllowStaticMembers"]/*'/>*/
    private Boolean AllowStaticMembers;

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/field[@name="Sync"]/*'/>*/
    private readonly Object Sync = new();

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/property[@name="Type"]/*'/>*/
    public Type? Type
    {
        get
        {
            try
            {
                if(!TryEnter(Sync,SyncTime)) { return null; }

                return Instance?.GetType() ?? TargetType;
            }
            catch ( Exception _ ) { KusDepotLog.Error(_,GetTypeFail,nameof(ObjectInvoker),null); if(NoExceptions) { return null; } throw; }

            finally { if(IsEntered(Sync)) { Exit(Sync); } }
        }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/property[@name="Value"]/*'/>*/
    public Object? Value
    {
        get
        {
            try
            {
                if(!TryEnter(Sync,SyncTime)) { return null; }

                return Instance;
            }
            catch ( Exception _ ) { KusDepotLog.Error(_,GetValueFail,nameof(ObjectInvoker),null); if(NoExceptions) { return null; } throw; }

            finally { if(IsEntered(Sync)) { Exit(Sync); } }
        }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ObjectInvoker() {}

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="Create"]/*'/>*/
    public static ObjectInvoker Create(Object? instance)
    {
        try
        {
            ObjectInvoker i = new(); _ = i.SetInstance(instance); return i;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(ObjectInvoker),null); if(NoExceptions) { return new(); } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="CreateType"]/*'/>*/
    public static ObjectInvoker Create(Type? type)
    {
        try
        {
            ObjectInvoker i = new(); _ = i.SetType(type); return i;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(ObjectInvoker),null); if(NoExceptions) { return new(); } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="CreateGeneric"]/*'/>*/
    public static ObjectInvoker Create<T>()
    {
        try
        {
            ObjectInvoker i = new(); _ = i.SetType(typeof(T)); return i;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,nameof(ObjectInvoker),null); if(NoExceptions) { return new(); } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="WithAllowStaticMembers"]/*'/>*/
    public ObjectInvoker WithAllowStaticMembers(Boolean enable = true)
    {
        try { _ = SetAllowStaticMembers(enable); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithAllowStaticMembersFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="SetAllowStaticMembers"]/*'/>*/
    public Boolean SetAllowStaticMembers(Boolean enable)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            AllowStaticMembers = enable; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetAllowStaticMembersFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="WithInstance"]/*'/>*/
    public ObjectInvoker WithInstance(Object? instance)
    {
        try { _ = SetInstance(instance); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithInstanceFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="SetInstance"]/*'/>*/
    public Boolean SetInstance(Object? instance)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Instance is not null) { return false; }

            if(instance is null) { return false; }

            Instance = instance; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetInstanceFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="WithType"]/*'/>*/
    public ObjectInvoker WithType(Type? type)
    {
        try { _ = SetType(type); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithTypeFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="WithTypeGeneric"]/*'/>*/
    public ObjectInvoker WithType<T>()
    {
        try { _ = SetType(typeof(T)); return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,WithTypeFail); if(NoExceptions) { return this; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="SetType"]/*'/>*/
    public Boolean SetType(Type? type)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(Instance is not null) { return false; }

            if(type is null) { return false; }

            TargetType = type; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTypeFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="GetField"]/*'/>*/
    public Object? GetField(String? name)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return null; }

            if(TryFindField(t,name,writable:false,AllowStaticMembers,out FieldInfo? f) is false || f is null) { return null; }

            return f.GetValue(f.IsStatic ? null : Instance);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetFieldFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="SetField"]/*'/>*/
    public Boolean SetField(String? name , Object? value)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(String.IsNullOrWhiteSpace(name)) { return false; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return false; }

            if(TryFindField(t,name,writable:true,AllowStaticMembers,out FieldInfo? f) is false || f is null) { return false; }

            if(ReflectionBinding.IsCompatible(f.FieldType,value,out _) is false) { return false; }

            f.SetValue(f.IsStatic ? null : Instance,value); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetFieldFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="GetProperty"]/*'/>*/
    public Object? GetProperty(String? name)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return null; }

            if(TryFindProperty(t,name,readable:true,writable:false,AllowStaticMembers,out PropertyInfo? p) is false || p is null) { return null; }

            return p.GetValue(p.GetMethod?.IsStatic is true ? null : Instance);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetPropertyFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="SetProperty"]/*'/>*/
    public Boolean SetProperty(String? name , Object? value)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return false; }

            if(String.IsNullOrWhiteSpace(name)) { return false; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return false; }

            if(TryFindProperty(t,name,readable:false,writable:true,AllowStaticMembers,out PropertyInfo? p) is false || p is null) { return false; }

            if(ReflectionBinding.IsCompatible(p.PropertyType,value,out _) is false) { return false; }

            p.SetValue(p.SetMethod?.IsStatic is true ? null : Instance,value); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetPropertyFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="Invoke"]/*'/>*/
    public Object? Invoke(String? name , params Object?[]? args)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return null; }

            if(TrySelectMethod(t,name,args,AllowStaticMembers,out MethodInfo? m,out Object?[]? invoke) is false || m is null || invoke is null)
            {
                return null;
            }

            Object? target = m.IsStatic ? null : Instance;

            if(!m.IsStatic && target is null) { return null; }

            return m.Invoke(target,invoke);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="InvokeDetailed"]/*'/>*/
    public InvocationResult? InvokeDetailed(String? name , params Object?[]? args)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return null; }

            if(TrySelectMethod(t,name,args,AllowStaticMembers,out MethodInfo? m,out Object?[]? invoke) is false || m is null || invoke is null)
            {
                return null;
            }

            Object? target = m.IsStatic ? null : Instance;

            if(!m.IsStatic && target is null) { return null; }

            Object? r = m.Invoke(target,invoke);

            return new InvocationResult(r,invoke,m,GetByRefIndices(m.GetParameters()));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeDetailedFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="InvokeAsync"]/*'/>*/
    public async ValueTask<Object?> InvokeAsync(String? name , Object?[]? args = null , CancellationToken cancel = default)
    {
        MethodInfo? m = null; Object?[]? invoke = null; Object? target = null;

        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return null; }

            if(TrySelectMethod(t,name,args,AllowStaticMembers,out m,out invoke) is false || m is null || invoke is null)
            {
                return null;
            }

            target = m.IsStatic ? null : Instance;

            if(!m.IsStatic && target is null) { return null; }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeAsyncFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }

        try
        {
            Object? r = m.Invoke(target,invoke);

            if(r is null) { return null; }

            if(r is Task t)
            {
                await t.WaitAsync(cancel).ConfigureAwait(false);

                PropertyInfo? rp = t.GetType().GetProperty("Result",BindingFlags.Public | BindingFlags.Instance);

                if(rp is null) { return null; }

                return NormalizeAsyncResult(rp.GetValue(t));
            }

            if(r is ValueTask vt)
            {
                await vt.AsTask().WaitAsync(cancel).ConfigureAwait(false); return null;
            }

            Type rv = r.GetType();

            if(rv.IsGenericType && rv.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                MethodInfo? asTask = rv.GetMethod("AsTask",BindingFlags.Public | BindingFlags.Instance,[]);

                if(asTask is null) { return null; }

                Object? taskObject = asTask.Invoke(r,null);

                if(taskObject is not Task task) { return null; }

                await task.WaitAsync(cancel).ConfigureAwait(false);

                PropertyInfo? rp = taskObject.GetType().GetProperty("Result",BindingFlags.Public | BindingFlags.Instance);

                if(rp is null) { return null; }

                return NormalizeAsyncResult(rp.GetValue(taskObject));
            }

            return r;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeAsyncFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="InvokeDetailedAsync"]/*'/>*/
    public async ValueTask<InvocationResult?> InvokeDetailedAsync(String? name , Object?[]? args = null , CancellationToken cancel = default)
    {
        MethodInfo? m = null; Object?[]? invoke = null; Object? target = null;

        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            Type? t = GetTargetType_NoSync(); if(t is null) { return null; }

            if(TrySelectMethod(t,name,args,AllowStaticMembers,out m,out invoke) is false || m is null || invoke is null)
            {
                return null;
            }

            target = m.IsStatic ? null : Instance;

            if(!m.IsStatic && target is null) { return null; }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeDetailedAsyncFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }

        try
        {
            Object? r = m.Invoke(target,invoke);

            Int32[] byref = GetByRefIndices(m.GetParameters());

            if(r is null) { return new InvocationResult(null,invoke,m,byref); }

            if(r is Task t)
            {
                await t.WaitAsync(cancel).ConfigureAwait(false);

                PropertyInfo? rp = t.GetType().GetProperty("Result",BindingFlags.Public | BindingFlags.Instance);

                if(rp is null) { return new InvocationResult(null,invoke,m,byref); }

                return new InvocationResult(NormalizeAsyncResult(rp.GetValue(t)),invoke,m,byref);
            }

            if(r is ValueTask vt)
            {
                await vt.AsTask().WaitAsync(cancel).ConfigureAwait(false);

                return new InvocationResult(null,invoke,m,byref);
            }

            Type rv = r.GetType();

            if(rv.IsGenericType && rv.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                MethodInfo? asTask = rv.GetMethod("AsTask",BindingFlags.Public | BindingFlags.Instance,[]);

                if(asTask is null) { return new InvocationResult(null,invoke,m,byref); }

                Object? taskObject = asTask.Invoke(r,null);

                if(taskObject is not Task task) { return new InvocationResult(null,invoke,m,byref); }

                await task.WaitAsync(cancel).ConfigureAwait(false);

                PropertyInfo? rp = taskObject.GetType().GetProperty("Result",BindingFlags.Public | BindingFlags.Instance);

                if(rp is null) { return new InvocationResult(null,invoke,m,byref); }

                return new InvocationResult(NormalizeAsyncResult(rp.GetValue(taskObject)),invoke,m,byref);
            }

            return new InvocationResult(r,invoke,m,byref);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeDetailedAsyncFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="InvokeStatic"]/*'/>*/
    public Object? InvokeStatic(String? name , params Object?[]? args)
    {
        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            if(TargetType is null) { return null; }

            if(TrySelectStaticMethod(TargetType,name,args,out MethodInfo? m,out Object?[]? invoke) is false || m is null || invoke is null)
            {
                return null;
            }

            return m.Invoke(null,invoke);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeStaticFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="InvokeStaticAsync"]/*'/>*/
    public async ValueTask<Object?> InvokeStaticAsync(String? name , Object?[]? args = null , CancellationToken cancel = default)
    {
        MethodInfo? m = null; Object?[]? invoke = null;

        try
        {
            if(!TryEnter(Sync,SyncTime)) { return null; }

            if(String.IsNullOrWhiteSpace(name)) { return null; }

            if(TargetType is null) { return null; }

            if(TrySelectStaticMethod(TargetType,name,args,out m,out invoke) is false || m is null || invoke is null)
            {
                return null;
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeStaticAsyncFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Sync)) { Exit(Sync); } }

        try
        {
            Object? r = m.Invoke(null,invoke);

            if(r is null) { return null; }

            if(r is Task t)
            {
                await t.WaitAsync(cancel).ConfigureAwait(false);

                PropertyInfo? rp = t.GetType().GetProperty("Result",BindingFlags.Public | BindingFlags.Instance);

                if(rp is null) { return null; }

                return NormalizeAsyncResult(rp.GetValue(t));
            }

            if(r is ValueTask vt)
            {
                await vt.AsTask().WaitAsync(cancel).ConfigureAwait(false); return null;
            }

            Type rv = r.GetType();

            if(rv.IsGenericType && rv.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                MethodInfo? asTask = rv.GetMethod("AsTask",BindingFlags.Public | BindingFlags.Instance,[]);

                if(asTask is null) { return null; }

                Object? taskObject = asTask.Invoke(r,null);

                if(taskObject is not Task task) { return null; }

                await task.WaitAsync(cancel).ConfigureAwait(false);

                PropertyInfo? rp = taskObject.GetType().GetProperty("Result",BindingFlags.Public | BindingFlags.Instance);

                if(rp is null) { return null; }

                return NormalizeAsyncResult(rp.GetValue(taskObject));
            }

            return r;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InvokeStaticAsyncFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="NormalizeAsyncResult"]/*'/>*/
    private static Object? NormalizeAsyncResult(Object? value)
    {
        try
        {
            if(value is null) { return null; }

            Type t = value.GetType();

            if(String.Equals(t.FullName,"System.Threading.Tasks.VoidTaskResult",Ordinal)) { return null; }

            return value;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,NormalizeAsyncResultFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="GetTargetTypeNoSync"]/*'/>*/
    private Type? GetTargetType_NoSync()
    {
        try
        {
            if(Instance is not null) { return Instance.GetType(); }

            if(AllowStaticMembers && TargetType is not null) { return TargetType; }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTargetTypeFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TryFindField"]/*'/>*/
    private static Boolean TryFindField(Type type , String name , Boolean writable , Boolean allowstatic , out FieldInfo? field)
    {
        field = null;

        try
        {
            FieldInfo? instance = null; FieldInfo? stc = null;

            foreach(FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if(String.Equals(f.Name,name,Ordinal) is false) { continue; }

                if(writable && (f.IsInitOnly || f.IsLiteral)) { continue; }

                if(f.IsStatic)
                {
                    if(!allowstatic) { continue; }

                    stc ??= f;
                }
                else
                {
                    instance ??= f;
                }
            }

            field = instance ?? stc;

            return field is not null;
        }
        catch ( Exception _ ) { field = null; KusDepotLog.Error(_,TryFindFieldFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TryFindProperty"]/*'/>*/
    private static Boolean TryFindProperty(Type type , String name , Boolean readable , Boolean writable , Boolean allowstatic , out PropertyInfo? property)
    {
        property = null;

        try
        {
            PropertyInfo? instance = null; PropertyInfo? stc = null;

            foreach(PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if(String.Equals(p.Name,name,Ordinal) is false) { continue; }

                if(readable && p.CanRead is false) { continue; }

                if(writable && p.CanWrite is false) { continue; }

                MethodInfo? accessor = readable ? p.GetMethod : p.SetMethod;

                if(accessor is null) { continue; }

                if(accessor.IsStatic)
                {
                    if(!allowstatic) { continue; }

                    stc ??= p;
                }
                else
                {
                    instance ??= p;
                }
            }

            property = instance ?? stc;

            return property is not null;
        }
        catch ( Exception _ ) { property = null; KusDepotLog.Error(_,TryFindPropertyFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TrySelectMethod"]/*'/>*/
    private static Boolean TrySelectMethod(Type type , String name , Object?[]? args , Boolean allowstatic , out MethodInfo? method , out Object?[]? invoke)
    {
        method = null; invoke = null;

        try
        {
            Object?[] a = args ?? [];

            Boolean? bestStatic = null; Boolean ambiguous = false;

            Int32 bestLength = Int32.MaxValue; Int32 bestExact = Int32.MinValue; Int32 bestAssignable = Int32.MinValue;

            foreach(MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if(String.Equals(m.Name,name,Ordinal) is false) { continue; }

                if(m.IsStatic && !allowstatic) { continue; }

                ParameterInfo[] p = m.GetParameters();

                if(TryCreateInvokeArgs(p,a,out Object?[]? candidate,out Int32 exact,out Int32 assignable) is false || candidate is null)
                {
                    continue;
                }

                Int32 cmp = ReflectionBindingScore.Compare(
                    p.Length,exact,assignable,m.IsStatic,
                    bestLength,bestExact,bestAssignable,bestStatic,
                    preferInstanceOnTie:true);

                if(cmp > 0)
                {
                    bestLength = p.Length; bestExact = exact; bestAssignable = assignable; bestStatic = m.IsStatic;

                    method = m; invoke = candidate; ambiguous = false; continue;
                }

                if(cmp < 0) { continue; }

                ambiguous = true;
            }

            if(ambiguous) { method = null; invoke = null; return false; }

            return method is not null && invoke is not null;
        }
        catch ( Exception _ ) { method = null; invoke = null; KusDepotLog.Error(_,TrySelectMethodFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TrySelectStaticMethod"]/*'/>*/
    private static Boolean TrySelectStaticMethod(Type type , String name , Object?[]? args , out MethodInfo? method , out Object?[]? invoke)
    {
        method = null; invoke = null;

        try
        {
            Object?[] a = args ?? [];

            Boolean ambiguous = false;

            Int32 bestLength = Int32.MaxValue; Int32 bestExact = Int32.MinValue; Int32 bestAssignable = Int32.MinValue;

            foreach(MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if(String.Equals(m.Name,name,Ordinal) is false) { continue; }

                ParameterInfo[] p = m.GetParameters();

                if(TryCreateInvokeArgs(p,a,out Object?[]? candidate,out Int32 exact,out Int32 assignable) is false || candidate is null)
                {
                    continue;
                }

                Int32 cmp = ReflectionBindingScore.Compare(
                    p.Length,exact,assignable,true,
                    bestLength,bestExact,bestAssignable,true,
                    preferInstanceOnTie:false);

                if(cmp > 0)
                {
                    bestLength = p.Length; bestExact = exact; bestAssignable = assignable;

                    method = m; invoke = candidate; ambiguous = false; continue;
                }

                if(cmp < 0) { continue; }

                ambiguous = true;
            }

            if(ambiguous) { method = null; invoke = null; return false; }

            return method is not null && invoke is not null;
        }
        catch ( Exception _ ) { method = null; invoke = null; KusDepotLog.Error(_,TrySelectStaticMethodFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TryCreateInvokeArgs"]/*'/>*/
    private static Boolean TryCreateInvokeArgs(ParameterInfo[] parameters , Object?[] args , out Object?[]? invoke , out Int32 exact , out Int32 assignable)
    {
        invoke = null; exact = 0; assignable = 0;

        try
        {
            Boolean hasparams = parameters.Length > 0 &&
                Attribute.IsDefined(parameters[^1],typeof(ParamArrayAttribute),inherit:false) &&
                parameters[^1].ParameterType.IsArray;

            if(!hasparams)
            {
                if(args.Length > parameters.Length) { return false; }

                return ReflectionArgumentBinding.TryCreateInvokeArgs(
                    parameters,args,allowdefaultvalues:true,
                    out invoke,out exact,out assignable);
            }

            Int32 last = parameters.Length - 1; Object?[] v = new Object?[parameters.Length];

            if(TryBindLeadingParameters(parameters,args,last,v,ref exact,ref assignable) is false) { return false; }

            if(TryBindParamsTail(parameters,args,last,v,ref exact,ref assignable) is false) { return false; }

            invoke = v; return true;
        }
        catch ( Exception _ ) { invoke = null; exact = 0; assignable = 0; KusDepotLog.Error(_,TryCreateInvokeArgsFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TryBindLeadingParameters"]/*'/>*/
    private static Boolean TryBindLeadingParameters(ParameterInfo[] parameters , Object?[] args , Int32 last , Object?[] target , ref Int32 exact , ref Int32 assignable)
    {
        try
        {
            for(Int32 i = 0; i < last; i++)
            {
                ParameterInfo pi = parameters[i];

                Boolean isout = ReflectionBinding.IsOutParameter(pi);

                Boolean isref = ReflectionBinding.IsRefParameter(pi);

                Type pt = ReflectionBinding.GetBindableType(pi.ParameterType);

                if(i < args.Length)
                {
                    Object? a = args[i];

                    if(ReflectionBinding.IsCompatible(pt,a,out Boolean e) is false) { return false; }

                    if(e) { exact++; } else { assignable++; }

                    target[i] = a;
                }
                else
                {
                    if(isout)
                    {
                        if(ReflectionBinding.TryAutoFillOutParameter(pt,out Object? f) is false) { return false; }

                        target[i] = f; continue;
                    }

                    if(parameters[i].HasDefaultValue)
                    {
                        target[i] = parameters[i].DefaultValue; continue;
                    }

                    if(isref) { return false; }

                    if(ReflectionBinding.CanAssignNull(pt) is false) { return false; }

                    target[i] = null;
                }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryBindLeadingParametersFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="TryBindParamsTail"]/*'/>*/
    private static Boolean TryBindParamsTail(ParameterInfo[] parameters , Object?[] args , Int32 last , Object?[] target , ref Int32 exact , ref Int32 assignable)
    {
        try
        {
            ParameterInfo pi = parameters[last];

            Type arrayType = pi.ParameterType; Type elementType = arrayType.GetElementType()!;

            if(args.Length == parameters.Length && ReflectionBinding.IsCompatible(arrayType,args[last],out Boolean exactArray))
            {
                if(exactArray) { exact++; } else { assignable++; }

                target[last] = args[last]; return true;
            }

            Int32 rem = Math.Max(0,args.Length - last);

            Array packed = Array.CreateInstance(elementType,rem);

            for(Int32 m = 0; m < rem; m++)
            {
                Object? a = args[last + m];

                if(ReflectionBinding.IsCompatible(elementType,a,out Boolean e) is false) { return false; }

                if(e) { exact++; } else { assignable++; }

                packed.SetValue(a,m);
            }

            target[last] = packed; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryBindParamsTailFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ObjectInvoker.xml' path='ObjectInvoker/class[@name="ObjectInvoker"]/method[@name="GetByRefIndices"]/*'/>*/
    private static Int32[] GetByRefIndices(ParameterInfo[] parameters)
    {
        try
        {
            Int32 count = 0;

            for(Int32 i = 0; i < parameters.Length; i++)
            {
                if(parameters[i].ParameterType.IsByRef) { count++; }
            }

            if(Equals(count,0)) { return []; }

            Int32[] indices = new Int32[count]; Int32 w = 0;

            for(Int32 i = 0; i < parameters.Length; i++)
            {
                if(parameters[i].ParameterType.IsByRef) { indices[w++] = i; }
            }

            return indices;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetByRefIndicesFail); if(NoExceptions) { return []; } throw; }
    }
}