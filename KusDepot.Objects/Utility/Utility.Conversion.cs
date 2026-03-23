namespace KusDepot.Utilities;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabWorkflowExceptionData"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this WorkflowExceptionData? input) { return input is null ? null : new() { Data = input.ToString() , Type = input.GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabDataItem"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this IDataItem? input) { return input is null ? null : new() { Data = input.ToString() , Type = ((Object)input).GetType().FullName }; }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToKusDepotCabKeySet"]/*'/>*/
    public static KusDepotCab? ToKusDepotCab(this KeySet? input) { return input is null ? null : new() { Data = input.ToString() , Type = ((Object)input).GetType().FullName }; }

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

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToUTF16StringFromByteArrayAsync"]/*'/>*/
    public static async Task<String> ToUTF16StringFromByteArrayAsync(this Byte[]? input , CancellationToken cancel = default)
    {
        if(input is null || input.Length == 0) { return String.Empty; }

        using MemoryStream s = new(input,writable:false);

        using StreamReader r = new(s,Encoding.Unicode,detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:false);

        return await r.ReadToEndAsync(cancel).ConfigureAwait(false);
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToUTF16StringFromStreamAsync"]/*'/>*/
    public static async Task<String> ToUTF16StringFromStreamAsync(this Stream? input , CancellationToken cancel = default)
    {
        if(input is null || !input.CanSeek) { return String.Empty; }

        var op = input.Position; input.Seek(0,SeekOrigin.Begin);

        using StreamReader r = new(input,Encoding.Unicode,detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:true);

        String _ = await r.ReadToEndAsync(cancel).ConfigureAwait(false);

        input.Seek(op,SeekOrigin.Begin);

        return _;
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToStringInvariant"]/*'/>*/
    public static String? ToStringInvariant(this Object? input) { return input is null ? null : Convert.ToString(input,CultureInfo.InvariantCulture); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArray"]/*'/>*/
    public static String TryGetStringFromByteArray(this Byte[]? input)
    {
        if(input is null) { return String.Empty; }

        try { return new UTF32Encoding(bigEndian:false,byteOrderMark:false,throwOnInvalidCharacters:true).GetString(input!); } catch { }

        try { return new UnicodeEncoding(bigEndian:false,byteOrderMark:false,throwOnInvalidBytes:true).GetString(input!); } catch { }

        try { return new UTF8Encoding(encoderShouldEmitUTF8Identifier:false,throwOnInvalidBytes:true).GetString(input!); } catch { }

        throw new FormatException();
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArrayAsync"]/*'/>*/
    public static async Task<String> TryGetStringFromByteArrayAsync(this Byte[]? input , CancellationToken cancel = default)
    {
        if(input is null) { return String.Empty; }

        try
        {
            using MemoryStream s = new(input,writable:false);

            using StreamReader r = new(s,new UTF32Encoding(bigEndian:false,byteOrderMark:false,throwOnInvalidCharacters:true),detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:false);

            return await r.ReadToEndAsync(cancel).ConfigureAwait(false);
        }
        catch { }

        try
        {
            using MemoryStream s = new(input,writable:false);

            using StreamReader r = new(s,new UnicodeEncoding(bigEndian:false,byteOrderMark:false,throwOnInvalidBytes:true),detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:false);

            return await r.ReadToEndAsync(cancel).ConfigureAwait(false);
        }
        catch { }

        try
        {
            using MemoryStream s = new(input,writable:false);

            using StreamReader r = new(s,new UTF8Encoding(encoderShouldEmitUTF8Identifier:false,throwOnInvalidBytes:true),detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:false);

            return await r.ReadToEndAsync(cancel).ConfigureAwait(false);
        }
        catch { }

        throw new FormatException();
    }
}