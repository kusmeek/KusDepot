namespace KusDepot;

/**<include file='ToolValueBuildConverter.xml' path='ToolValueBuildConverter/class[@name="ToolValueBuildConverter"]/main/*'/>*/
public sealed class ToolValueBuildConverter : IToolValueConverter
{
    ///<inheritdoc/>
    public Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result)
    {
        result = null;

        try
        {
            if(value is null || value.Mode != ToolValueMode.Build || String.IsNullOrWhiteSpace(value.Type) || value.Arguments is null || value.Arguments.Length is 0) { return false; }

            var builder = ObjectBuilder.Create(value.Type);

            foreach(var argument in value.Arguments.OrderBy(_ => _.Index))
            {
                if(TryResolveArgument(argument,out Object? argumentValue) is false) { return false; }

                if(builder.SetArgument(argument.Index,argumentValue) is false) { return false; }
            }

            if(builder.Build() is false) { return false; }

            result = builder.Value;

            return result is not null;
        }
        catch { result = null; return false; }
    }

    ///<inheritdoc/>
    public Boolean TryWrite(Object? value , [MaybeNullWhen(false)] out ToolValue? result)
    {
        result = null; return false;
    }

    /**<include file='ToolValueBuildConverter.xml' path='ToolValueBuildConverter/class[@name="ToolValueBuildConverter"]/method[@name="TryResolveArgument"]/*'/>*/
    private static Boolean TryResolveArgument(ToolValueArgument? argument , [MaybeNullWhen(false)] out Object? value)
    {
        value = null;

        try
        {
            if(argument is null || String.IsNullOrWhiteSpace(argument.Type)) { return false; }

            return argument.Mode switch
            {
                ToolValueMode.Parse => TryResolveParsable(argument.Type,argument.Data,out value),

                ToolValueMode.Build => TryResolveNestedBuild(argument,out value),

                _ => false
            };
        }
        catch { value = null; return false; }
    }

    /**<include file='ToolValueBuildConverter.xml' path='ToolValueBuildConverter/class[@name="ToolValueBuildConverter"]/method[@name="TryResolveNestedBuild"]/*'/>*/
    private static Boolean TryResolveNestedBuild(ToolValueArgument argument , [MaybeNullWhen(false)] out Object? value)
    {
        value = null;

        try
        {
            if(argument.Parameters is null || argument.Parameters.Length is 0 || String.IsNullOrWhiteSpace(argument.Type)) { return false; }

            var builder = ObjectBuilder.Create(argument.Type);

            foreach(var parameter in argument.Parameters.OrderBy(_ => _.Index))
            {
                if(TryResolveParameter(parameter,out Object? parameterValue) is false) { return false; }

                if(builder.SetArgument(parameter.Index,parameterValue) is false) { return false; }
            }

            if(builder.Build() is false) { return false; }

            value = builder.Value;

            return value is not null;
        }
        catch { value = null; return false; }
    }

    /**<include file='ToolValueBuildConverter.xml' path='ToolValueBuildConverter/class[@name="ToolValueBuildConverter"]/method[@name="TryResolveParameter"]/*'/>*/
    private static Boolean TryResolveParameter(ToolValueArgumentParameter? parameter , [MaybeNullWhen(false)] out Object? value)
    {
        value = null;

        try
        {
            if(parameter is null || String.IsNullOrWhiteSpace(parameter.Type)) { return false; }

            return TryResolveParsable(parameter.Type,parameter.Data,out value);
        }
        catch { value = null; return false; }
    }

    /**<include file='ToolValueBuildConverter.xml' path='ToolValueBuildConverter/class[@name="ToolValueBuildConverter"]/method[@name="TryResolveParsable"]/*'/>*/
    private static Boolean TryResolveParsable(String? type , String? data , [MaybeNullWhen(false)] out Object? value)
    {
        value = null;

        try
        {
            if(String.IsNullOrWhiteSpace(type)) { return false; }

            if(String.Equals(type,typeof(String).FullName,Ordinal))
            {
                value = data ?? String.Empty; return true;
            }

            if(data is null) { return true; }

            var parser = ObjectParser.Create(type).WithData(data);

            value = parser.Parse();

            return value is not null;
        }
        catch { value = null; return false; }
    }
}