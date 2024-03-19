namespace KusDepot.Data;

/**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/main/*'/>*/
public static class DataControlUtility
{
    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="MakeDataControlUpload"]/*'/>*/
    public static DataControlUpload? MakeDataControlUpload(this KusDepot.Common? it)
    {
        try
        {
            if(it is null) { return null; }

            MethodInfo? m = it.GetType().GetMethod("GetDescriptor");                        if(m is null) { return null; }

            String? o = it.ToString();                                                      if(o is null) { return null; }

            String? h = SHA512.HashData(o.ToByteArrayFromBase64()).ToBase64FromByteArray(); if(h is null) { return null; }

            Descriptor? d = (Descriptor?)m.Invoke(it,null);                                 if(d is null) { return null; }

            return new DataControlUpload() { Descriptor = d , Object = o , ObjectSHA512 = h };
        }
        catch ( Exception ) { return null; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="MakeDataControlDownload"]/*'/>*/
    public static DataControlDownload? MakeDataControlDownload(String? it)
    {
        try
        {
            if(it is null) { return null; }

            String? h = SHA512.HashData(it.ToByteArrayFromBase64()).ToBase64FromByteArray(); if(h is null) { return null; }

            return new DataControlDownload() { Object = it , ObjectSHA512 = h };
        }
        catch ( Exception ) { return null; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyUpload"]/*'/>*/
    public static Boolean Verify(this DataControlUpload? dcu)
    {
        try
        {
            if(dcu is null) { return false; }

            String? h = SHA512.HashData(dcu.Object.ToByteArrayFromBase64()).ToBase64FromByteArray(); if(h is null) { return false; }

            return String.Equals(dcu.ObjectSHA512,h,StringComparison.Ordinal);
        }
        catch ( Exception ) { return false; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyDownload"]/*'/>*/
    public static Boolean Verify(this DataControlDownload? dcd)
    {
        try
        {
            if(dcd is null) { return false; }

            String? h = SHA512.HashData(dcd.Object.ToByteArrayFromBase64()).ToBase64FromByteArray(); if(h is null) { return false; }

            return String.Equals(dcd.ObjectSHA512,h,StringComparison.Ordinal);
        }
        catch ( Exception ) { return false; }
    }
}