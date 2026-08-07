namespace KusDepot.Serialization.Buffers;

/**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/main/*'/>*/
public ref struct BufferSpanReader
{
    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/field[@name="buffer"]/*'/>*/
    private ReadOnlySpan<Byte> buffer;

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/property[@name="Position"]/*'/>*/
    public Int32 Position { get; private set; }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/property[@name="Length"]/*'/>*/
    public readonly Int32 Length => buffer.Length;

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/property[@name="Remaining"]/*'/>*/
    public readonly Int32 Remaining => buffer.Length - Position;

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/constructor[@name="Constructor"]/*'/>*/
    public BufferSpanReader(ReadOnlySpan<Byte> buffer)
    {
        this.buffer = buffer; Position = 0;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadByte"]/*'/>*/
    public Boolean TryReadByte(out Byte value)
    {
        if(Remaining < 1)
        {
            value = default;

            return false;
        }

        value = buffer[Position++];

        return true;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadBytes"]/*'/>*/
    public Boolean TryReadBytes(Int32 count , out ReadOnlySpan<Byte> slice)
    {
        if(Remaining < count)
        {
            slice = ReadOnlySpan<Byte>.Empty;

            return false;
        }

        slice = buffer.Slice(Position,count);

        Position += count;

        return true;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadGuid"]/*'/>*/
    public Boolean TryReadGuid(out Guid value)
    {
        value = Guid.Empty;

        if(Remaining < 16) { return false; }

        value = new(buffer.Slice(Position,16));

        Position += 16;

        return true;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadInt64BigEndian"]/*'/>*/
    public Boolean TryReadInt64BigEndian(out Int64 value)
    {
        if(Remaining < sizeof(Int64))
        {
            value = default;

            return false;
        }

        value = ReadInt64BigEndian(buffer.Slice(Position,sizeof(Int64)));

        Position += sizeof(Int64);

        return true;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadUInt16BigEndian"]/*'/>*/
    public Boolean TryReadUInt16BigEndian(out UInt16 value)
    {
        if(Remaining < sizeof(UInt16))
        {
            value = default;
            return false;
        }

        value = ReadUInt16BigEndian(buffer.Slice(Position,sizeof(UInt16)));

        Position += sizeof(UInt16);

        return true;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadUInt32BigEndian"]/*'/>*/
    public Boolean TryReadUInt32BigEndian(out UInt32 value)
    {
        if(Remaining < sizeof(UInt32))
        {
            value = default;

            return false;
        }

        value = ReadUInt32BigEndian(buffer.Slice(Position,sizeof(UInt32)));

        Position += sizeof(UInt32);

        return true;
    }

    /**<include file='BufferSpanReader.xml' path='BufferSpanReader/struct[@name="BufferSpanReader"]/method[@name="TryReadUInt128BigEndian"]/*'/>*/
    public Boolean TryReadUInt128BigEndian(out UInt128 value)
    {
        if(Remaining < 16)
        {
            value = default;

            return false;
        }

        value = ReadUInt128BigEndian(buffer.Slice(Position,16));

        Position += 16;

        return true;
    }
}