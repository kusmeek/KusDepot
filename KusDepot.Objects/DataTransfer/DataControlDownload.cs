namespace KusDepot.Data;

/**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/main/*'/>*/
public sealed class DataControlDownload
{
    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/property[@name="Object"]/*'/>*/
    public String Object       {get;init;} = String.Empty;

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/property[@name="ObjectSHA512"]/*'/>*/
    public String ObjectSHA512 {get;init;} = String.Empty;

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/property[@name="StreamSHA512"]/*'/>*/
    public String StreamSHA512 {get;init;} = String.Empty;

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/method[@name="Parse"]/*'/>*/
    public static DataControlDownload? Parse(String input , IFormatProvider? format = null) { try { return JsonSerializer.Deserialize<DataControlDownload>(input); } catch { return null; } }

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/method[@name="ParseAsync"]/*'/>*/
    public static async Task<DataControlDownload?> ParseAsync(String input , IFormatProvider? format = null , CancellationToken cancel = default)
    {
        try
        {
            if(input is null) { return null; }

            Byte[] b = Encoding.UTF8.GetBytes(input);

            using MemoryStream s = new(b,writable:false);

            return await JsonSerializer.DeserializeAsync<DataControlDownload>(s,cancellationToken:cancel).ConfigureAwait(false);
        }
        catch { return null; }
    }

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/method[@name="ToString"]/*'/>*/
    public override String ToString() { try { return JsonSerializer.Serialize(this); } catch { return String.Empty; } }

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/method[@name="ToStringAsync"]/*'/>*/
    public async Task<String> ToStringAsync(CancellationToken cancel = default)
    {
        try
        {
            using MemoryStream s = new();

            await JsonSerializer.SerializeAsync(s,this,cancellationToken:cancel).ConfigureAwait(false); s.Seek(0,SeekOrigin.Begin);

            using StreamReader r = new(s,Encoding.UTF8,detectEncodingFromByteOrderMarks:false,bufferSize:ConversionBufferSize,leaveOpen:false);

            return await r.ReadToEndAsync(cancel).ConfigureAwait(false);
        }
        catch { return String.Empty; }
    }

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataControlDownload dcd)
    {
        dcd = null; if(input is null) { return false; }

        try
        {
            DataControlDownload? _ = JsonSerializer.Deserialize<DataControlDownload>(input); if(_ is not null) { dcd = _; return true; }

            return false;
        }
        catch { return false; }
    }
}