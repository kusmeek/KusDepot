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

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/method[@name="ToString"]/*'/>*/
    public override String ToString() { try { return JsonSerializer.Serialize(this); } catch { return String.Empty; } }

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