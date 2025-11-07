extern alias AsyncLinq;

namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="AllAsyncRx"]/*'/>*/
    public static ValueTask<Boolean> AllAsyncRx<TSource>(this IAsyncEnumerable<TSource> source , Func<TSource,Boolean> predicate , CancellationToken cancel = default) { return AsyncLinq.System.Linq.AsyncEnumerable.AllAsync(source,predicate,cancel); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="SelectAwaitRx"]/*'/>*/
    public static IAsyncEnumerable<TResult> SelectAwaitRx<TSource,TResult>(this IAsyncEnumerable<TSource> source , Func<TSource,ValueTask<TResult>> selector) { return AsyncLinq.System.Linq.AsyncEnumerable.SelectAwait(source,selector); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToAsyncEnumerableRx"]/*'/>*/
    public static IAsyncEnumerable<TSource> ToAsyncEnumerableRx<TSource>(this IEnumerable<TSource> source) { return AsyncLinq.System.Linq.AsyncEnumerable.ToAsyncEnumerable(source); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabWorkflowExceptionData"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this WorkflowExceptionData? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabDataItem"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this IDataItem? input) { return input is null ? null : new() { Data = input.ToString() , Type = ((Object)input).GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabKeySet"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this KeySet? input) { return input is null ? null : new() { Data = input.ToString() , Type = ((Object)input).GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabAccessRequestWeb"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this AccessRequestWeb? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabSecurityKeyWeb"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this SecurityKeyWeb? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabCommandDetails"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this CommandDetails? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabAccessRequest"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this AccessRequest? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabSecurityKey"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this SecurityKey? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabDataContent"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this DataContent? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabDescriptor"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this Descriptor? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabToolData"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this ToolData? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToBase64FromByteArray"]/*'/>*/
    public static String ToBase64FromByteArray(this Byte[]? input) { return input is null ? String.Empty : Convert.ToBase64String(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToByteArrayFromBase64"]/*'/>*/
    public static Byte[] ToByteArrayFromBase64(this String? input) { return input is null ? Array.Empty<Byte>() : Convert.FromBase64String(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToByteArrayFromUTF16String"]/*'/>*/
    public static Byte[] ToByteArrayFromUTF16String(this String? input) { return input is null ? Array.Empty<Byte>() : Encoding.Unicode.GetBytes(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToUTF16StringFromByteArray"]/*'/>*/
    public static String ToUTF16StringFromByteArray(this Byte[]? input) { return input is null ? String.Empty : Encoding.Unicode.GetString(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToStringInvariant"]/*'/>*/
    public static String? ToStringInvariant(this Object? input) { return input is null ? null : Convert.ToString(input,CultureInfo.InvariantCulture); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArray"]/*'/>*/
    public static String TryGetStringFromByteArray(this Byte[]? input)
    {
        if(input is null) { return String.Empty; }

        try { return new UTF32Encoding(false,false,true).GetString(input!); } catch { }

        try { return new UnicodeEncoding(false,false,true).GetString(input!); } catch { }

        try { return new UTF8Encoding(false,true).GetString(input!); } catch { }

        throw new FormatException();
    }
}