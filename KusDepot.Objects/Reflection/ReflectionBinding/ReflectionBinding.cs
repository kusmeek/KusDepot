namespace KusDepot.Reflection;

/**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/main/*'/>*/
internal static class ReflectionBinding
{
    /**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/method[@name="IsCompatible"]/*'/>*/
    internal static Boolean IsCompatible(Type parametertype , Object? argument , out Boolean exact)
    {
        exact = false;

        try
        {
            if(argument is null) { return CanAssignNull(parametertype); }

            Type at = argument.GetType();

            if(parametertype == at) { exact = true; return true; }

            if(parametertype.IsAssignableFrom(at)) { return true; }

            return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsCompatibleFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/method[@name="CanAssignNull"]/*'/>*/
    internal static Boolean CanAssignNull(Type type)
    {
        try
        {
            if(type.IsValueType is false) { return true; }

            return Nullable.GetUnderlyingType(type) is not null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CanAssignNullFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/method[@name="GetBindableType"]/*'/>*/
    internal static Type GetBindableType(Type parametertype)
    {
        try
        {
            return parametertype.IsByRef ? parametertype.GetElementType()! : parametertype;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetBindableTypeFail); if(NoExceptions) { return parametertype; } throw; }
    }

    /**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/method[@name="IsOutParameter"]/*'/>*/
    internal static Boolean IsOutParameter(ParameterInfo parameter)
    {
        try
        {
            return parameter.IsOut && parameter.ParameterType.IsByRef;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsOutParameterFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/method[@name="IsRefParameter"]/*'/>*/
    internal static Boolean IsRefParameter(ParameterInfo parameter)
    {
        try
        {
            return parameter.ParameterType.IsByRef && !parameter.IsOut;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsRefParameterFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ReflectionBinding.xml' path='ReflectionBinding/class[@name="ReflectionBinding"]/method[@name="TryAutoFillOutParameter"]/*'/>*/
    internal static Boolean TryAutoFillOutParameter(Type elementtype , out Object? value)
    {
        value = null;

        try
        {
            if(CanAssignNull(elementtype)) { return true; }

            if(elementtype.IsValueType) { value = Activator.CreateInstance(elementtype); return true; }

            return false;
        }
        catch ( Exception _ ) { value = null; KusDepotLog.Error(_,AutoFillOutParameterFail); if(NoExceptions) { return false; } throw; }
    }
}