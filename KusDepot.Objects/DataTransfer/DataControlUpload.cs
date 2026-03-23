namespace KusDepot.Data;

/**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/main/*'/>*/
public sealed class DataControlUpload
{
    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="Descriptor"]/*'/>*/
    public Descriptor Descriptor {get;init;} = new();

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="Object"]/*'/>*/
    public String Object         {get;init;} = String.Empty;

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="ObjectSHA512"]/*'/>*/
    public String ObjectSHA512   {get;init;} = String.Empty;

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="StreamSHA512"]/*'/>*/
    public String StreamSHA512   {get;init;} = String.Empty;

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/method[@name="Parse"]/*'/>*/
    public static DataControlUpload? Parse(String input , IFormatProvider? format) { try { return JsonSerializer.Deserialize<DataControlUpload>(input); } catch { return null; } }

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/method[@name="ParseAsync"]/*'/>*/
    public static async Task<DataControlUpload?> ParseAsync(String input , IFormatProvider? format = null , CancellationToken cancel = default)
    {
        try
        {
            if(input is null) { return null; }

            Byte[] b = Encoding.UTF8.GetBytes(input);

            using MemoryStream s = new(b,writable:false);

            return await JsonSerializer.DeserializeAsync<DataControlUpload>(s,cancellationToken:cancel).ConfigureAwait(false);
        }
        catch { return null; }
    }

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/method[@name="ToString"]/*'/>*/
    public override String ToString() { try { return JsonSerializer.Serialize(this); } catch { return String.Empty; } }

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/method[@name="ToStringAsync"]/*'/>*/
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

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataControlUpload dcu)
    {
        dcu = null; if(input is null) { return false; }

        try
        {
            DataControlUpload? _ = JsonSerializer.Deserialize<DataControlUpload>(input); if(_ is not null) { dcu = _; return true; }

            return false;
        }
        catch { return false; }
    }
}