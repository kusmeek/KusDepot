using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

/**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/main/*'/>*/
internal static class SegmentedTransferStorageCore
{
    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/field[@name="FSCTL_SET_SPARSE"]/*'/>*/
    private const UInt32 FSCTL_SET_SPARSE = 0x900C4;

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/field[@name="FileStreamBufferSize"]/*'/>*/
    private const Int32 FileStreamBufferSize = 524288;

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/field[@name="PayloadCopyBufferSize"]/*'/>*/
    private const Int32 PayloadCopyBufferSize = 1048576;

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="CreateSessionStorageKey"]/*'/>*/
    public static String CreateSessionStorageKey(Guid sessionid)
    {
        return sessionid.ToString("N",InvariantCulture);
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="GetStorageDirectoryPath"]/*'/>*/
    public static String GetStorageDirectoryPath(String rootpath , String storagekey)
    {
        return Path.Combine(Path.GetFullPath(rootpath),storagekey);
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="MarkFileSparse"]/*'/>*/
    private static unsafe void MarkFileSparse(SafeFileHandle handle)
    {
        if(OperatingSystem.IsWindowsVersionAtLeast(5,1,2600))
        {
            HANDLE nativeHandle = new(handle.DangerousGetHandle()); UInt32 bytesReturned = 0;

            if(!PInvoke.DeviceIoControl(nativeHandle,FSCTL_SET_SPARSE,null,0,null,0,&bytesReturned,null))
            {
                Int32 error = Marshal.GetLastWin32Error();

                if(error != 0) { throw new IOException(String.Format(InvariantCulture,"FSCTL_SET_SPARSE failed with Win32 error {0}.",error)); }
            }
        }
        else { throw new PlatformNotSupportedException("Windows sparse file support is not available on this platform."); }
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="OpenRangeReadStream"]/*'/>*/
    public static Stream OpenRangeReadStream(String path , DataItemTransferRange range)
    {
        FileStream stream = OpenStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);

        return new BoundedReadStream(stream,range.Offset,range.Length,leaveopen:false);
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="OpenStream"]/*'/>*/
    public static FileStream OpenStream(String path , FileMode mode , FileAccess access , FileShare share)
    {
        return new(path,mode,access,share,FileStreamBufferSize,FileOptions.RandomAccess);
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="PrepareSparseStreamFile"]/*'/>*/
    public static async Task<Boolean> PrepareSparseStreamFile(String path , Int64 streamlength , CancellationToken cancel)
    {
        try
        {
            if(streamlength < 0) { Log.Error(InvalidArgument); return false; }

            String? directory = Path.GetDirectoryName(path);

            if(String.IsNullOrWhiteSpace(directory) is false) { Directory.CreateDirectory(directory); }

            cancel.ThrowIfCancellationRequested();

            using SafeFileHandle handle = File.OpenHandle(path,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read,FileOptions.RandomAccess);

            if(OperatingSystem.IsWindows()) { MarkFileSparse(handle); }

            FileStream stream = new(handle,FileAccess.ReadWrite,FileStreamBufferSize,false);

            await using (stream.ConfigureAwait(false))
            {
                stream.SetLength(streamlength);

                await stream.FlushAsync(cancel).ConfigureAwait(false);

                return true;
            }
        }
        catch ( Exception _ ) { Log.Error(_,SparseFileFail,path); return false; }
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="ReadAllBytesSafeAsync"]/*'/>*/
    public static async Task<Byte[]?> ReadAllBytesSafeAsync(String path , Guid sessionid , String message , CancellationToken cancel)
    {
        try
        {
            if(File.Exists(path) is false) { return null; }

            return await File.ReadAllBytesAsync(path,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Log.Error(_,message,sessionid); return null; }
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="SaveManifestAsync"]/*'/>*/
    public static async Task SaveManifestAsync(String path , DataItemTransferManifest manifest , CancellationToken cancel)
    {
        Byte[] payload = manifest.Serialize();

        String? directory = Path.GetDirectoryName(path);

        if(String.IsNullOrWhiteSpace(directory) is false) { Directory.CreateDirectory(directory); }

        await File.WriteAllBytesAsync(path,payload,cancel).ConfigureAwait(false);
    }

    /**<include file='SegmentedTransferStorageCore.xml' path='SegmentedTransferStorageCore/class[@name="SegmentedTransferStorageCore"]/method[@name="WritePayloadAsync"]/*'/>*/
    public static async Task<Boolean> WritePayloadAsync(FileStream destination , Stream payload , Int64 expectedlength , CancellationToken cancel)
    {
        Byte[] buffer = ArrayPool<Byte>.Shared.Rent(PayloadCopyBufferSize);

        try
        {
            Int64 remaining = expectedlength;

            while(remaining > 0)
            {
                Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

                Int32 read = await payload.ReadAsync(buffer.AsMemory(0,requested),cancel).ConfigureAwait(false);

                if(read <= 0) { return false; }

                await destination.WriteAsync(buffer.AsMemory(0,read),cancel).ConfigureAwait(false);

                remaining -= read;
            }

            return true;
        }
        finally
        {
            ArrayPool<Byte>.Shared.Return(buffer);
        }
    }
}