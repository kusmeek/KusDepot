namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CloneByteArray"]/*'/>*/
    public static Byte[] CloneByteArray(this Byte[]? input) { return input?.Clone() as Byte[] ?? Array.Empty<Byte>(); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="IsNullOrEmptyGuid"]/*'/>*/
    public static Boolean IsNullOrEmpty(Guid? input) { try { return input is null || Equals(input,Guid.Empty); } catch { return false; } }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ReadAllText"]/*'/>*/
    public static String? ReadAllText(String? path)
    {
        try
        {
            return String.IsNullOrEmpty(path) ? null : File.ReadAllText(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReadAllTextFail); if(NoExceptions) { return null; } throw; }
    }
}