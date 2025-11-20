namespace KusDepot.Data;

/**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/main/*'/>*/
public static class DataControlUtility
{
    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="MakeDataControlUpload"]/*'/>*/
    public static DataControlUpload? MakeDataControlUpload(this DataItem? it)
    {
        try
        {
            Descriptor? d = it?.GetDescriptor(); if(d is null) { return null; }

            String? o = it?.ToString(); if(String.IsNullOrEmpty(o)) { return null; }

            String h = SHA512.HashData(o.ToByteArrayFromBase64()).ToBase64FromByteArray();

            String sh = String.Empty;

            if(d.LiveStream is not true)
            {
                using var s = it?.GetContentStream();

                if(s is not null) { sh = SHA512.HashData(s).ToBase64FromByteArray(); }
            }

            return new() { Descriptor = d , Object = o , ObjectSHA512 = h , StreamSHA512 = sh };
        }
        catch { return null; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="MakeDataControlDownload"]/*'/>*/
    public static DataControlDownload? MakeDataControlDownload(String? it , Stream? fs = null)
    {
        try
        {
            if(String.IsNullOrEmpty(it)) { return null; } fs?.Seek(0,SeekOrigin.Begin);

            String oh = SHA512.HashData(it.ToByteArrayFromBase64()).ToBase64FromByteArray(); String sh = String.Empty;

            if(fs is not null) { sh = SHA512.HashData(fs).ToBase64FromByteArray(); fs.Seek(0,SeekOrigin.Begin); }

            return new() { Object = it , ObjectSHA512 = oh , StreamSHA512 = sh };
        }
        catch { return null; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyUpload"]/*'/>*/
    public static Boolean Verify(this DataControlUpload? dcu)
    {
        try
        {
            if(dcu is null || dcu.Descriptor is null || dcu.Descriptor.ID is null || Equals(dcu.Descriptor.ID,Guid.Empty) || String.IsNullOrEmpty(dcu.Object)) { return false; }

            String h = SHA512.HashData(dcu.Object.ToByteArrayFromBase64()).ToBase64FromByteArray();

            return String.Equals(dcu.ObjectSHA512,h,StringComparison.Ordinal);
        }
        catch { return false; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyDownload"]/*'/>*/
    public static Boolean Verify(this DataControlDownload? dcd)
    {
        try
        {
            if(dcd is null) { return false; }

            String h = SHA512.HashData(dcd.Object.ToByteArrayFromBase64()).ToBase64FromByteArray();

            return String.Equals(dcd.ObjectSHA512,h,StringComparison.Ordinal);
        }
        catch { return false; }
    }
}