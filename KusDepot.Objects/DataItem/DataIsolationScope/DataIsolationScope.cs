namespace KusDepot;

/**<include file='DataIsolationScope.xml' path='DataIsolationScope/class[@name="DataIsolationScope"]/main/*'/>*/
public sealed class DataIsolationScope : IDisposable
{
    /**<include file='DataIsolationScope.xml' path='DataIsolationScope/class[@name="DataIsolationScope"]/field[@name="previous"]/*'/>*/
    private readonly DataIsolationMode? previous;

    /**<include file='DataIsolationScope.xml' path='DataIsolationScope/class[@name="DataIsolationScope"]/constructor[@name="Constructor"]/*'/>*/
    public DataIsolationScope(DataIsolationMode mode)
    {
        previous = DataIsolation.GetScopedMode(); DataIsolation.SetScopedMode(mode);
    }

    /**<include file='DataIsolationScope.xml' path='DataIsolationScope/class[@name="DataIsolationScope"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() => DataIsolation.SetScopedMode(previous);
}