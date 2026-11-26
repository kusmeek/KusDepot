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

        Encoding encoding = ResolveTextEncoding(input,out Int32 offset);

        try { return encoding.GetString(input,offset,input.Length - offset); }

        catch ( DecoderFallbackException _ ) { throw new FormatException(InvalidTextEncoding,_); }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArrayWithEncoding"]/*'/>*/
    public static String TryGetStringFromByteArray(this Byte[]? input , Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding); if(input is null) { return String.Empty; }

        Encoding strict = (Encoding)encoding.Clone(); strict.DecoderFallback = DecoderFallback.ExceptionFallback;

        Int32 offset = input.AsSpan().StartsWith(strict.Preamble) ? strict.Preamble.Length : 0;

        try { return strict.GetString(input,offset,input.Length - offset); }

        catch ( DecoderFallbackException _ ) { throw new FormatException(InvalidTextEncoding,_); }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArrayAsync"]/*'/>*/
    public static async Task<String> TryGetStringFromByteArrayAsync(this Byte[]? input , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested(); if(input is null) { return String.Empty; }

        Encoding encoding = ResolveTextEncoding(input,out Int32 offset);

        return await DecodeTextAsync(input,encoding,offset,cancel).ConfigureAwait(false);
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArrayAsyncWithEncoding"]/*'/>*/
    public static async Task<String> TryGetStringFromByteArrayAsync(this Byte[]? input , Encoding encoding , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(encoding); cancel.ThrowIfCancellationRequested(); if(input is null) { return String.Empty; }

        Encoding strict = (Encoding)encoding.Clone(); strict.DecoderFallback = DecoderFallback.ExceptionFallback;

        Int32 offset = input.AsSpan().StartsWith(strict.Preamble) ? strict.Preamble.Length : 0;

        return await DecodeTextAsync(input,strict,offset,cancel).ConfigureAwait(false);
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecodeTextAsync"]/*'/>*/
    private static async Task<String> DecodeTextAsync(Byte[] input , Encoding encoding , Int32 offset , CancellationToken cancel)
    {
        try
        {
            using MemoryStream s = new(input,offset,input.Length - offset,writable:false);

            using StreamReader r = new(s,encoding,detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:false);

            return await r.ReadToEndAsync(cancel).ConfigureAwait(false);
        }
        catch ( DecoderFallbackException _ ) { throw new FormatException(InvalidTextEncoding,_); }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ResolveTextEncoding"]/*'/>*/
    internal static Encoding ResolveTextEncoding(ReadOnlySpan<Byte> input , out Int32 offset)
    {
        if(input.StartsWith(UTF32LittleEndianPreamble)) { offset = 4; return new UTF32Encoding(false,false,true); }

        if(input.StartsWith(UTF32BigEndianPreamble))    { offset = 4; return new UTF32Encoding(true,false,true); }

        if(input.StartsWith(UTF8Preamble))              { offset = 3; return new UTF8Encoding(false,true); }

        if(input.StartsWith(UTF16LittleEndianPreamble)) { offset = 2; return new UnicodeEncoding(false,false,true); }

        if(input.StartsWith(UTF16BigEndianPreamble))    { offset = 2; return new UnicodeEncoding(true,false,true); }

        offset = 0; return new UTF8Encoding(false,true);
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/field[@name="UTF32LittleEndianPreamble"]/*'/>*/
    private static readonly Byte[] UTF32LittleEndianPreamble = [0xFF,0xFE,0x00,0x00];

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/field[@name="UTF32BigEndianPreamble"]/*'/>*/
    private static readonly Byte[] UTF32BigEndianPreamble = [0x00,0x00,0xFE,0xFF];

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/field[@name="UTF8Preamble"]/*'/>*/
    private static readonly Byte[] UTF8Preamble = [0xEF,0xBB,0xBF];

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/field[@name="UTF16LittleEndianPreamble"]/*'/>*/
    private static readonly Byte[] UTF16LittleEndianPreamble = [0xFF,0xFE];

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/field[@name="UTF16BigEndianPreamble"]/*'/>*/
    private static readonly Byte[] UTF16BigEndianPreamble = [0xFE,0xFF];
}