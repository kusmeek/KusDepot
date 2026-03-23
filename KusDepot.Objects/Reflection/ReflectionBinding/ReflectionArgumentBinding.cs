namespace KusDepot.Reflection;

/**<include file='ReflectionArgumentBinding.xml' path='ReflectionArgumentBinding/class[@name="ReflectionArgumentBinding"]/main/*'/>*/
internal static class ReflectionArgumentBinding
{
    /**<include file='ReflectionArgumentBinding.xml' path='ReflectionArgumentBinding/class[@name="ReflectionArgumentBinding"]/method[@name="TryCreateInvokeArgsArray"]/*'/>*/
    internal static Boolean TryCreateInvokeArgs(ParameterInfo[] parameters , Object?[]? arguments , Boolean allowdefaultvalues , out Object?[]? invoke , out Int32 exact , out Int32 assignable)
    {
        invoke = null; exact = 0; assignable = 0;

        try
        {
            Object?[] a = arguments ?? [];

            Object?[] v = new Object?[parameters.Length];

            for(Int32 i = 0; i < parameters.Length; i++)
            {
                ParameterInfo pi = parameters[i];

                Boolean isout = ReflectionBinding.IsOutParameter(pi);

                Boolean isref = ReflectionBinding.IsRefParameter(pi);

                Type pt = ReflectionBinding.GetBindableType(pi.ParameterType);

                if(i < a.Length)
                {
                    Object? x = a[i];

                    if(ReflectionBinding.IsCompatible(pt,x,out Boolean e) is false) { return false; }

                    if(e) { exact++; } else { assignable++; }

                    v[i] = x;
                }
                else
                {
                    if(isout)
                    {
                        if(ReflectionBinding.TryAutoFillOutParameter(pt,out Object? f) is false) { return false; }

                        v[i] = f; continue;
                    }

                    if(allowdefaultvalues && pi.HasDefaultValue)
                    {
                        v[i] = pi.DefaultValue; continue;
                    }

                    if(isref) { return false; }

                    if(ReflectionBinding.CanAssignNull(pt) is false) { return false; }

                    v[i] = null;
                }
            }

            invoke = v; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryCreateInvokeArgsFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ReflectionArgumentBinding.xml' path='ReflectionArgumentBinding/class[@name="ReflectionArgumentBinding"]/method[@name="TryCreateInvokeArgsDictionary"]/*'/>*/
    internal static Boolean TryCreateInvokeArgs(ParameterInfo[] parameters , IReadOnlyDictionary<Int32,Object?>? arguments , Boolean allowdefaultvalues , out Object?[]? invoke , out Int32 exact , out Int32 assignable)
    {
        invoke = null; exact = 0; assignable = 0;

        try
        {
            IReadOnlyDictionary<Int32,Object?> a = arguments ?? new Dictionary<Int32,Object?>();

            Object?[] v = new Object?[parameters.Length];

            for(Int32 i = 0; i < parameters.Length; i++)
            {
                Type pt = parameters[i].ParameterType;

                if(a.TryGetValue(i,out Object? x))
                {
                    if(ReflectionBinding.IsCompatible(pt,x,out Boolean e) is false) { return false; }

                    if(e) { exact++; } else { assignable++; }

                    v[i] = x;
                }
                else
                {
                    if(allowdefaultvalues && parameters[i].HasDefaultValue)
                    {
                        v[i] = parameters[i].DefaultValue; continue;
                    }

                    if(ReflectionBinding.CanAssignNull(pt) is false) { return false; }

                    v[i] = null;
                }
            }

            invoke = v; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryCreateInvokeArgsFail); if(NoExceptions) { return false; } throw; }
    }
}