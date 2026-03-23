namespace KusDepot.Utilities;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CloneByteArray"]/*'/>*/
    public static Byte[] CloneByteArray(this Byte[]? input) { return input?.Clone() as Byte[] ?? Array.Empty<Byte>(); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetFileSize"]/*'/>*/
    public static Int64? GetFileSize(String? path)
    {
        try
        {
            if(String.IsNullOrEmpty(path)) { return null; }

            FileInfo _ = new(path); return _.Exists ? _.Length : null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetFileSizeFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="IsNullOrEmptyGuid"]/*'/>*/
    public static Boolean IsNullOrEmpty(Guid? input) { try { return input is null || Equals(input,Guid.Empty); } catch { return false; } }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ReadAllBytesAsync"]/*'/>*/
    public static async Task<Byte[]> ReadAllBytesAsync(Stream? stream , CancellationToken cancel = default)
    {
        try
        {
            if(stream is null || stream.CanRead is false) { return Array.Empty<Byte>(); }

            using MemoryStream m = new();

            await stream.CopyToAsync(m,cancel).ConfigureAwait(false);

            return m.ToArray();
        }
        catch ( OperationCanceledException ) { return Array.Empty<Byte>(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ReadAllBytesAsyncFail); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

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