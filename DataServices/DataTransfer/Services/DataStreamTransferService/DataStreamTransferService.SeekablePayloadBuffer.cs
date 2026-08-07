namespace KusDepot.Data.Services.DataTransfer;

public sealed partial class DataStreamTransferService
{
    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="SeekablePayloadBuffer"]/main/*'/>*/
    private sealed class SeekablePayloadBuffer : IAsyncDisposable
    {
        /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="SeekablePayloadBuffer"]/property[@name="Stream"]/*'/>*/
        public Stream Stream { get; }

        /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="SeekablePayloadBuffer"]/constructor[@name="SeekablePayloadBuffer"]/*'/>*/
        private SeekablePayloadBuffer(Stream stream) => this.Stream = stream;

        /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="SeekablePayloadBuffer"]/method[@name="Create"]/*'/>*/
        public static async Task<SeekablePayloadBuffer> Create(Stream source , Int64 expectedlength , String? temporaryuploadpath , CancellationToken cancel)
        {
            ArgumentNullException.ThrowIfNull(source); ArgumentOutOfRangeException.ThrowIfNegative(expectedlength);

            if(source.CanSeek)
            {
                source.Position = 0;

                return new(source);
            }

            String rootPath = String.IsNullOrWhiteSpace(temporaryuploadpath) ? Path.GetTempPath() : temporaryuploadpath!;

            Directory.CreateDirectory(rootPath);

            String path = Path.Combine(rootPath,$"KusDepot.StreamTransfer.{Guid.NewGuid():N}.tmp");

            FileStream staging = new(path,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None,80*KiB,FileOptions.Asynchronous | FileOptions.DeleteOnClose | FileOptions.SequentialScan);

            Byte[] buffer = ArrayPool<Byte>.Shared.Rent(80*KiB);

            try
            {
                Int64 remaining = expectedlength;

                while(remaining > 0)
                {
                    Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

                    Int32 read = await source.ReadAsync(buffer.AsMemory(0,requested),cancel).ConfigureAwait(false);

                    if(read <= 0) { throw new EndOfStreamException(); }

                    await staging.WriteAsync(buffer.AsMemory(0,read),cancel).ConfigureAwait(false);

                    remaining -= read;
                }

                staging.Position = 0;

                return new(staging);
            }
            catch
            {
                await staging.DisposeAsync().ConfigureAwait(false);

                throw;
            }
            finally
            {
                ArrayPool<Byte>.Shared.Return(buffer);
            }
        }

        /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="SeekablePayloadBuffer"]/method[@name="DisposeAsync"]/*'/>*/
        public async ValueTask DisposeAsync() => await this.Stream.DisposeAsync().ConfigureAwait(false);
    }
}
