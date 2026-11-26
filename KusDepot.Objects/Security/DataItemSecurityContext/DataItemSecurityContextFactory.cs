namespace KusDepot.Security.Data;

/**<include file='DataItemSecurityContextFactory.xml' path='DataItemSecurityContextFactory/class[@name="DataItemSecurityContextFactory"]/main/*'/>*/
public static class DataItemSecurityContextFactory
{
    /**<include file='DataItemSecurityContextFactory.xml' path='DataItemSecurityContextFactory/class[@name="DataItemSecurityContextFactory"]/method[@name="ForObject"]/*'/>*/
    public static DataItemSecurityContextBuilder ForObject(Guid objectid , X509Certificate2 certificate , String? displayname = null)
    {
        return new DataItemSecurityContextBuilder(new DataSecurityObject(objectid,certificate,displayname));
    }

    /**<include file='DataItemSecurityContextFactory.xml' path='DataItemSecurityContextFactory/class[@name="DataItemSecurityContextFactory"]/method[@name="ForDataSecurityObject"]/*'/>*/
    public static DataItemSecurityContextBuilder ForObject(DataSecurityObject localobject)
    {
        ArgumentNullException.ThrowIfNull(localobject);

        return new DataItemSecurityContextBuilder(localobject);
    }
}
