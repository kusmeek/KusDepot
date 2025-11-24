namespace KusDepot;

public static partial class AccessKeySecret
{
    /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/main/*'/>*/
    internal ref struct BufferCursor
    {
        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/field[@name="buffer"]/*'/>*/
        private Span<Byte> buffer;

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/property[@name="Position"]/*'/>*/
        public Int32 Position { get; private set; }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/property[@name="Length"]/*'/>*/
        public readonly Int32 Length => buffer.Length;

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/property[@name="Remaining"]/*'/>*/
        public readonly Int32 Remaining => buffer.Length - Position;

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/property[@name="WrittenSpan"]/*'/>*/
        public readonly ReadOnlySpan<Byte> WrittenSpan => buffer[..Position];

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/constructor[@name="Constructor"]/*'/>*/
        public BufferCursor(Span<Byte> buffer) { this.buffer = buffer; Position = 0; }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteByte"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteByte(Byte value)
        {
            if(Remaining < 1) { return false; } buffer[Position++] = value; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteBytes"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteBytes(ReadOnlySpan<Byte> source)
        {
            if(Remaining < source.Length) { return false; } source.CopyTo(buffer.Slice(Position,source.Length)); Position += source.Length; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteRandom"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteRandom(Int32 count)
        {
            if(Remaining < count) { return false; } RandomNumberGenerator.Fill(buffer.Slice(Position,count)); Position += count; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteGuid"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteGuid(Guid id)
        {
            if(Remaining < 16) { return false; } return id.TryWriteBytes(buffer.Slice(Position,16)) ? (Position += 16) == Position : false;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteInt64BigEndian"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteInt64BigEndian(Int64 value)
        {
            if(Remaining < 8) { return false; } WriteInt64BigEndian(buffer.Slice(Position,8),value); Position += 8; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteUInt16BigEndian"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteUInt16BigEndian(UInt16 value)
        {
            if(Remaining < 2) { return false; } WriteUInt16BigEndian(buffer.Slice(Position,2), value); Position += 2; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferCursor"]/method[@name="TryWriteUInt128BigEndian"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryWriteUInt128BigEndian(UInt128 value)
        {
            if(Remaining < 16) { return false; } WriteUInt128BigEndian(buffer.Slice(Position,16), value); Position += 16; return true;
        }
    }

    /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/main/*'/>*/
    internal ref struct BufferReader
    {
        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/field[@name="buffer"]/*'/>*/
        private ReadOnlySpan<Byte> buffer;

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/property[@name="Position"]/*'/>*/
        public Int32 Position { get; private set; }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/property[@name="Remaining"]/*'/>*/
        public readonly Int32 Remaining => buffer.Length - Position;

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/constructor[@name="Constructor"]/*'/>*/
        public BufferReader(ReadOnlySpan<Byte> buffer) { this.buffer = buffer; Position = 0; }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/method[@name="TryReadByte"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryReadByte(out Byte value)
        {
            if(Remaining < 1) { value = default; return false; } value = buffer[Position++]; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/method[@name="TryReadBytes"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryReadBytes(Int32 count , out ReadOnlySpan<Byte> slice)
        {
            if(Remaining < count) { slice = ReadOnlySpan<Byte>.Empty; return false; } slice = buffer.Slice(Position,count); Position += count; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/method[@name="TryReadGuid"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryReadGuid(out Guid id)
        {
            if(Remaining < 16) { id = default; return false; } id = new Guid(buffer.Slice(Position,16)); Position += 16; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/method[@name="TryReadInt64BigEndian"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryReadInt64BigEndian(out Int64 value)
        {
            if(Remaining < 8) { value = default; return false; } value = ReadInt64BigEndian(buffer.Slice(Position,8)); Position += 8; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/method[@name="TryReadUInt16BigEndian"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryReadUInt16BigEndian(out UInt16 value)
        {
            if(Remaining < 2) { value = default; return false; } value = ReadUInt16BigEndian(buffer.Slice(Position,2)); Position += 2; return true;
        }

        /**<include file='BufferHandlers.xml' path='BufferHandlers/class[@name="AccessKeySecret"]/struct[@name="BufferReader"]/method[@name="TryReadUInt128BigEndian"]/*'/>*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryReadUInt128BigEndian(out UInt128 value)
        {
            if(Remaining < 16) { value = default; return false; } value = ReadUInt128BigEndian(buffer.Slice(Position,16)); Position += 16; return true;
        }
    }
}