namespace KusDepot;

/**<include file='ToolValueConverterRegistration.xml' path='ToolValueConverterRegistration/record[@name="ToolValueConverterRegistration"]/main/*'/>*/
public sealed class ToolValueConverterRegistration
{
    /**<include file='ToolValueConverterRegistration.xml' path='ToolValueConverterRegistration/record[@name="ToolValueConverterRegistration"]/property[@name="Name"]/*'/>*/
    public String Name { get; init; } = String.Empty;

    /**<include file='ToolValueConverterRegistration.xml' path='ToolValueConverterRegistration/record[@name="ToolValueConverterRegistration"]/property[@name="Order"]/*'/>*/
    public Int32 Order { get; init; }

    /**<include file='ToolValueConverterRegistration.xml' path='ToolValueConverterRegistration/record[@name="ToolValueConverterRegistration"]/property[@name="Converter"]/*'/>*/
    public IToolValueConverter? Converter { get; init; }
}