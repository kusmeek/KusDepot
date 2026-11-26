namespace DataPodServices.DataControl;

/**<include file='ObjectPayloadReader.xml' path='ObjectPayloadReader/class[@name="ObjectPayloadReader"]/main/*'/>*/
internal static class ObjectPayloadReader
{
    /**<include file='ObjectPayloadReader.xml' path='ObjectPayloadReader/class[@name="ObjectPayloadReader"]/method[@name="ReadAsync"]/*'/>*/
    public static async Task<Byte[]> ReadAsync(Stream input , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        Int64 length = input.Length - (input.CanSeek ? input.Position : 0);

        if(length < 0 || length > Int32.MaxValue) { throw new InvalidDataException(); }

        Byte[] payload = new Byte[(Int32)length]; Int32 offset = 0;

        while(offset < payload.Length)
        {
            Int32 read = await input.ReadAsync(payload.AsMemory(offset),cancel).ConfigureAwait(false);

            if(read == 0) { throw new EndOfStreamException(); }

            offset += read;
        }

        return payload;
    }
}
