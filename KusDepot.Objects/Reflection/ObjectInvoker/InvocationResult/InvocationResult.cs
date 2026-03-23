namespace KusDepot.Reflection;

/**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/main/*'/>*/
public sealed class InvocationResult
{
    /**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/constructor[@name="Constructor"]/*'/>*/
    internal InvocationResult(Object? returnvalue , Object?[] arguments , MethodInfo method , Int32[] byrefindices)
    {
        ReturnValue = returnvalue; Arguments = arguments; Method = method; ByRefIndices = byrefindices;
    }

    /**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/property[@name="ReturnValue"]/*'/>*/
    public Object? ReturnValue { get; }

    /**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/property[@name="Arguments"]/*'/>*/
    public Object?[] Arguments { get; }

    /**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/property[@name="Method"]/*'/>*/
    public MethodInfo Method { get; }

    /**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/property[@name="ByRefIndices"]/*'/>*/
    public Int32[] ByRefIndices { get; }

    /**<include file='InvocationResult.xml' path='InvocationResult/class[@name="InvocationResult"]/property[@name="HasByRefArguments"]/*'/>*/
    public Boolean HasByRefArguments => !Equals(ByRefIndices.Length,0);
}