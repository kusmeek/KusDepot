namespace KusDepot.Serialization.Buffers;

/**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/main/*'/>*/
public ref struct BufferReader
{
    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/field[@name="buffer"]/*'/>*/
    private ReadOnlyMemory<Byte> buffer;

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/property[@name="Position"]/*'/>*/
    public Int32 Position { get; private set; }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/property[@name="Length"]/*'/>*/
    public readonly Int32 Length => buffer.Length;

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/property[@name="Remaining"]/*'/>*/
    public readonly Int32 Remaining => buffer.Length - Position;

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/constructor[@name="Constructor"]/*'/>*/
    public BufferReader(ReadOnlyMemory<Byte> buffer)
    {
        this.buffer = buffer; Position = 0;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadByte"]/*'/>*/
    public Boolean TryReadByte(out Byte value)
    {
        if(Remaining < 1)
        {
            value = default;

            return false;
        }

        value = buffer.Span[Position++];

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadBytes"]/*'/>*/
    public Boolean TryReadBytes(Int32 count , out ReadOnlySpan<Byte> slice)
    {
        if(Remaining < count)
        {
            slice = ReadOnlySpan<Byte>.Empty;

            return false;
        }

        slice = buffer.Span.Slice(Position,count);

        Position += count;

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadMemory"]/*'/>*/
    public Boolean TryReadMemory(Int32 count , out ReadOnlyMemory<Byte> memory)
    {
        if(Remaining < count)
        {
            memory = ReadOnlyMemory<Byte>.Empty;

            return false;
        }

        memory = buffer.Slice(Position,count);

        Position += count;

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadGuid"]/*'/>*/
    public Boolean TryReadGuid(out Guid value)
    {
        value = Guid.Empty;

        if(Remaining < 16) { return false; }

        value = new(buffer.Span.Slice(Position,16));

        Position += 16;

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadInt32BigEndian"]/*'/>*/
    public Boolean TryReadInt32BigEndian(out Int32 value)
    {
        if(Remaining < sizeof(Int32))
        {
            value = default;

            return false;
        }

        value = ReadInt32BigEndian(buffer.Span.Slice(Position,sizeof(Int32)));

        Position += sizeof(Int32);

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadInt64BigEndian"]/*'/>*/
    public Boolean TryReadInt64BigEndian(out Int64 value)
    {
        if(Remaining < sizeof(Int64))
        {
            value = default;

            return false;
        }

        value = ReadInt64BigEndian(buffer.Span.Slice(Position,sizeof(Int64)));

        Position += sizeof(Int64);

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadUInt16BigEndian"]/*'/>*/
    public Boolean TryReadUInt16BigEndian(out UInt16 value)
    {
        if(Remaining < sizeof(UInt16))
        {
            value = default;

            return false;
        }

        value = ReadUInt16BigEndian(buffer.Span.Slice(Position,sizeof(UInt16)));

        Position += sizeof(UInt16);

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadUInt32BigEndian"]/*'/>*/
    public Boolean TryReadUInt32BigEndian(out UInt32 value)
    {
        if(Remaining < sizeof(UInt32))
        {
            value = default;

            return false;
        }

        value = ReadUInt32BigEndian(buffer.Span.Slice(Position,sizeof(UInt32)));

        Position += sizeof(UInt32);

        return true;
    }

    /**<include file='BufferReader.xml' path='BufferReader/struct[@name="BufferReader"]/method[@name="TryReadUInt128BigEndian"]/*'/>*/
    public Boolean TryReadUInt128BigEndian(out UInt128 value)
    {
        if(Remaining < 16)
        {
            value = default;

            return false;
        }

        value = ReadUInt128BigEndian(buffer.Span.Slice(Position,16));

        Position += 16;

        return true;
    }
}