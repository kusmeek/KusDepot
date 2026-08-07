using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

/**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/main/*'/>*/
internal static class StreamTransferStorageCore
{
    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="CreateSessionStorageKey"]/*'/>*/
    public static String CreateSessionStorageKey(Guid sessionid)
    {
        return SegmentedTransferStorageCore.CreateSessionStorageKey(sessionid);
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="GetStorageDirectoryPath"]/*'/>*/
    public static String GetStorageDirectoryPath(String rootpath , String storagekey)
    {
        return SegmentedTransferStorageCore.GetStorageDirectoryPath(rootpath,storagekey);
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="OpenRangeReadStream"]/*'/>*/
    public static Stream OpenRangeReadStream(String path , DataItemTransferRange range)
    {
        return SegmentedTransferStorageCore.OpenRangeReadStream(path,range);
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="OpenStream"]/*'/>*/
    public static FileStream OpenStream(String path , FileMode mode , FileAccess access , FileShare share)
    {
        return SegmentedTransferStorageCore.OpenStream(path,mode,access,share);
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="PrepareAppendStreamFile"]/*'/>*/
    public static async Task<Boolean> PrepareAppendStreamFile(String path , Int64 streamlength , CancellationToken cancel)
    {
        try
        {
            if(streamlength < 0) { Log.Error(StreamInvalidArgument); return false; }

            String? directory = Path.GetDirectoryName(path);

            if(String.IsNullOrWhiteSpace(directory) is false) { Directory.CreateDirectory(directory); }

            cancel.ThrowIfCancellationRequested();

            FileStream stream = OpenStream(path,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read);

            await using (stream.ConfigureAwait(false))
            {
                if(stream.Length < streamlength) { stream.SetLength(streamlength); }

                await stream.FlushAsync(cancel).ConfigureAwait(false);

                return true;
            }
        }
        catch ( Exception _ ) { Log.Error(_,StreamPrepareFileFail,path); return false; }
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="ReadAllBytesSafeAsync"]/*'/>*/
    public static Task<Byte[]?> ReadAllBytesSafeAsync(String path , Guid sessionid , String message , CancellationToken cancel)
    {
        return SegmentedTransferStorageCore.ReadAllBytesSafeAsync(path,sessionid,message,cancel);
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="SaveManifestAsync"]/*'/>*/
    public static async Task SaveManifestAsync(String path , DataStreamTransferManifest manifest , CancellationToken cancel)
    {
        Byte[] payload = manifest.Serialize();

        String? directory = Path.GetDirectoryName(path);

        if(String.IsNullOrWhiteSpace(directory) is false) { Directory.CreateDirectory(directory); }

        await File.WriteAllBytesAsync(path,payload,cancel).ConfigureAwait(false);
    }

    /**<include file='StreamTransferStorageCore.xml' path='StreamTransferStorageCore/class[@name="StreamTransferStorageCore"]/method[@name="WritePayloadAsync"]/*'/>*/
    public static Task<Boolean> WritePayloadAsync(FileStream destination , Stream payload , Int64 expectedlength , CancellationToken cancel)
    {
        return SegmentedTransferStorageCore.WritePayloadAsync(destination,payload,expectedlength,cancel);
    }
}
