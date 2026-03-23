namespace KusDepot.Data;

/**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/main/*'/>*/
public static class DataControlUtility
{
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

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="MakeDataControlDownloadAsync"]/*'/>*/
    public static async Task<DataControlDownload?> MakeDataControlDownloadAsync(String? it , Stream? fs = null , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(it)) { return null; }

            Byte[] ob = it.ToByteArrayFromBase64(); using MemoryStream m = new(ob,writable:false);

            Task<Byte[]> ht = SHA512.HashDataAsync(m,cancel).AsTask();

            Task<Byte[]>? st = null; Boolean hs = false;

            if(fs is not null)
            {
                if(fs.CanSeek) { fs.Seek(0,SeekOrigin.Begin); }

                st = SHA512.HashDataAsync(fs,cancel).AsTask(); hs = true;
            }

            await Task.WhenAll(hs ? new Task[]{ht,st!} : new Task[]{ht}).ConfigureAwait(false);

            String oh = ht.Result.ToBase64FromByteArray(); String sh = String.Empty;

            if(hs && st is not null)
            {
                if(fs is not null && fs.CanSeek) { fs.Seek(0,SeekOrigin.Begin); }

                sh = st.Result.ToBase64FromByteArray();
            }

            return new() { Object = it , ObjectSHA512 = oh , StreamSHA512 = sh };
        }
        catch { return null; }
    }

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

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="MakeDataControlUploadAsync"]/*'/>*/
    public static async Task<DataControlUpload?> MakeDataControlUploadAsync(this DataItem? it , CancellationToken cancel = default)
    {
        try
        {
            Descriptor? d = it?.GetDescriptor(); if(d is null) { return null; }

            String? o = it?.ToString(); if(String.IsNullOrEmpty(o)) { return null; }

            Byte[] ob = o.ToByteArrayFromBase64(); using MemoryStream m = new(ob,writable:false);

            Task<Byte[]> ht = SHA512.HashDataAsync(m,cancel).AsTask();

            Task<Byte[]>? st = null;

            if(d.LiveStream is not true)
            {
                using Stream? s = it?.GetContentStream();

                if(s is not null)
                {
                    st = SHA512.HashDataAsync(s,cancel).AsTask();

                    await Task.WhenAll(ht,st).ConfigureAwait(false);

                    String h = ht.Result.ToBase64FromByteArray(); String sh = st.Result.ToBase64FromByteArray();

                    return new() { Descriptor = d , Object = o , ObjectSHA512 = h , StreamSHA512 = sh };
                }
            }

            Byte[] hb = await ht.ConfigureAwait(false);

            String ho = hb.ToBase64FromByteArray();

            return new() { Descriptor = d , Object = o , ObjectSHA512 = ho , StreamSHA512 = String.Empty };
        }
        catch { return null; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyDownload"]/*'/>*/
    public static Boolean Verify(this DataControlDownload? dcd)
    {
        try
        {
            if(dcd is null) { return false; }

            String h = SHA512.HashData(dcd.Object.ToByteArrayFromBase64()).ToBase64FromByteArray();

            return dcd.ObjectSHA512.AsSpan().SequenceEqual(h.AsSpan());
        }
        catch { return false; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyDownloadAsync"]/*'/>*/
    public static async Task<Boolean> VerifyAsync(this DataControlDownload? dcd , CancellationToken cancel = default)
    {
        try
        {
            if(dcd is null) { return false; }

            Byte[] ob = dcd.Object.ToByteArrayFromBase64(); using MemoryStream m = new(ob,writable:false);

            Byte[] hb = await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false);

            if(cancel.IsCancellationRequested) { return false; }

            String h = hb.ToBase64FromByteArray();

            return dcd.ObjectSHA512.AsSpan().SequenceEqual(h.AsSpan());
        }
        catch { return false; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyUpload"]/*'/>*/
    public static Boolean Verify(this DataControlUpload? dcu)
    {
        try
        {
            if(dcu is null || dcu.Descriptor is null || dcu.Descriptor.ID is null || Equals(dcu.Descriptor.ID,Guid.Empty) || String.IsNullOrEmpty(dcu.Object)) { return false; }

            String h = SHA512.HashData(dcu.Object.ToByteArrayFromBase64()).ToBase64FromByteArray();

            return dcu.ObjectSHA512.AsSpan().SequenceEqual(h.AsSpan());
        }
        catch { return false; }
    }

    /**<include file='DataControlUtility.xml' path='DataControlUtility/class[@name="DataControlUtility"]/method[@name="VerifyUploadAsync"]/*'/>*/
    public static async Task<Boolean> VerifyAsync(this DataControlUpload? dcu , CancellationToken cancel = default)
    {
        try
        {
            if(dcu is null || dcu.Descriptor is null || dcu.Descriptor.ID is null || Equals(dcu.Descriptor.ID,Guid.Empty) || String.IsNullOrEmpty(dcu.Object)) { return false; }

            Byte[] ob = dcu.Object.ToByteArrayFromBase64(); using MemoryStream m = new(ob,writable:false);

            Byte[] hb = await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false);

            if(cancel.IsCancellationRequested) { return false; }

            String h = hb.ToBase64FromByteArray();

            return dcu.ObjectSHA512.AsSpan().SequenceEqual(h.AsSpan());
        }
        catch { return false; }
    }
}