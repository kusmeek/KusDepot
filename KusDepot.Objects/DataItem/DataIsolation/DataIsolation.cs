namespace KusDepot;

/**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/main/*'/>*/
public static class DataIsolation
{
    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/property[@name="Mode"]/*'/>*/
    public static DataIsolationMode Mode { get; private set; } = Enabled;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/field[@name="scopedmode"]/*'/>*/
    private static readonly AsyncLocal<DataIsolationMode?> scopedmode = new();

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="BeginScope"]/*'/>*/
    public static IDisposable BeginScope(DataIsolationMode mode) => new DataIsolationScope(mode);

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="Disable"]/*'/>*/
    public static void Disable() => Mode = Disabled;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="Enable"]/*'/>*/
    public static void Enable() => Mode = Enabled;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="IsDisabled"]/*'/>*/
    public static Boolean IsDisabled() => IsEnabled() is false;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="IsEnabled"]/*'/>*/
    public static Boolean IsEnabled() => (scopedmode.Value ?? Mode) == Enabled;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="IsScopeActive"]/*'/>*/
    public static Boolean IsScopeActive() => scopedmode.Value.HasValue;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="GetScopedMode"]/*'/>*/
    internal static DataIsolationMode? GetScopedMode() => scopedmode.Value;

    /**<include file='DataIsolation.xml' path='DataIsolation/class[@name="DataIsolation"]/method[@name="SetScopedMode"]/*'/>*/
    internal static void SetScopedMode(DataIsolationMode? mode) => scopedmode.Value = mode;
}