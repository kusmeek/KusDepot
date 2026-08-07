namespace KusDepot.Serialization.Buffers;

/**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/main/*'/>*/
public ref struct BufferWriter
{
    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/field[@name="buffer"]/*'/>*/
    private Span<Byte> buffer;

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/property[@name="Position"]/*'/>*/
    public Int32 Position { get; private set; }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/property[@name="Length"]/*'/>*/
    public readonly Int32 Length => buffer.Length;

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/property[@name="Remaining"]/*'/>*/
    public readonly Int32 Remaining => buffer.Length - Position;

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/property[@name="WrittenSpan"]/*'/>*/
    public readonly ReadOnlySpan<Byte> WrittenSpan => buffer[..Position];

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/constructor[@name="Constructor"]/*'/>*/
    public BufferWriter(Span<Byte> buffer)
    {
        this.buffer = buffer; Position = 0;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteByte"]/*'/>*/
    public Boolean TryWriteByte(Byte value)
    {
        if(Remaining < 1) { return false; }

        buffer[Position++] = value;

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteBytes"]/*'/>*/
    public Boolean TryWriteBytes(ReadOnlySpan<Byte> source)
    {
        if(Remaining < source.Length) { return false; }

        source.CopyTo(buffer.Slice(Position,source.Length));

        Position += source.Length;

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteGuid"]/*'/>*/
    public Boolean TryWriteGuid(Guid value)
    {
        if(Remaining < 16) { return false; }

        if(!value.TryWriteBytes(buffer.Slice(Position,16))) { return false; }

        Position += 16;

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteInt32BigEndian"]/*'/>*/
    public Boolean TryWriteInt32BigEndian(Int32 value)
    {
        if(Remaining < sizeof(Int32)) { return false; }

        WriteInt32BigEndian(buffer.Slice(Position,sizeof(Int32)),value);

        Position += sizeof(Int32);

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteInt64BigEndian"]/*'/>*/
    public Boolean TryWriteInt64BigEndian(Int64 value)
    {
        if(Remaining < sizeof(Int64)) { return false; }

        WriteInt64BigEndian(buffer.Slice(Position,sizeof(Int64)),value);

        Position += sizeof(Int64);

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteUInt16BigEndian"]/*'/>*/
    public Boolean TryWriteUInt16BigEndian(UInt16 value)
    {
        if(Remaining < sizeof(UInt16)) { return false; }

        WriteUInt16BigEndian(buffer.Slice(Position,sizeof(UInt16)),value);

        Position += sizeof(UInt16);

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteUInt32BigEndian"]/*'/>*/
    public Boolean TryWriteUInt32BigEndian(UInt32 value)
    {
        if(Remaining < sizeof(UInt32)) { return false; }

        WriteUInt32BigEndian(buffer.Slice(Position,sizeof(UInt32)),value);

        Position += sizeof(UInt32);

        return true;
    }

    /**<include file='BufferWriter.xml' path='BufferWriter/struct[@name="BufferWriter"]/method[@name="TryWriteUInt128BigEndian"]/*'/>*/
    public Boolean TryWriteUInt128BigEndian(UInt128 value)
    {
        if(Remaining < 16) { return false; }

        WriteUInt128BigEndian(buffer.Slice(Position,16),value);

        Position += 16;

        return true;
    }
}