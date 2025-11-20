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

    /**<include file='DataControlDownload.xml' path='DataControlDownload/class[@name="DataControlDownload"]/method[@name="ToString"]/*'/>*/
    public override String ToString() { try { return JsonSerializer.Serialize(this); } catch { return String.Empty; } }

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