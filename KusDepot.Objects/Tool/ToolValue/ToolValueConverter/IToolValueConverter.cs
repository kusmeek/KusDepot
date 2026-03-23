namespace KusDepot;

/**<include file='IToolValueConverter.xml' path='IToolValueConverter/interface[@name="IToolValueConverter"]/main/*'/>*/
public interface IToolValueConverter
{
    /**<include file='IToolValueConverter.xml' path='IToolValueConverter/interface[@name="IToolValueConverter"]/method[@name="TryRead"]/*'/>*/
    Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result);

    /**<include file='IToolValueConverter.xml' path='IToolValueConverter/interface[@name="IToolValueConverter"]/method[@name="TryWrite"]/*'/>*/
    Boolean TryWrite(Object? value , [MaybeNullWhen(false)] out ToolValue? result);
}