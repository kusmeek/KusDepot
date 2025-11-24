namespace KusDepot.Dapr;

[StructLayout(LayoutKind.Sequential)]
public struct MIB_TCPROW_OWNER_PID
{
    public UInt32 State;

    public UInt32 LocalAddress;

    [MarshalAs(UnmanagedType.ByValArray , SizeConst = 4)]
    public Byte[] LocalPort;

    public UInt32 RemoteAddress;

    [MarshalAs(UnmanagedType.ByValArray , SizeConst = 4)]
    public Byte[] RemotePort;

    public UInt32 OwningPID;

    public readonly MIB_TCPROW_PID ToMIB_TCPROW_PID()
    {
        return new MIB_TCPROW_PID
        (
            (Int32)this.State,

            new IPAddress(BitConverter.GetBytes(this.LocalAddress)).ToString(),

            ConvertPort(this.LocalPort),

            new IPAddress(BitConverter.GetBytes(this.RemoteAddress)).ToString(),

            ConvertPort(this.RemotePort),
            
            (Int32)this.OwningPID
        );
    }
    
    private static Int32 ConvertPort(Byte[] nb) { UInt16 _ = BitConverter.ToUInt16(nb,0); return (_ & 0xFF00) >> 8 | (_ & 0x00FF) << 8; }
}

[StructLayout(LayoutKind.Sequential)]
public struct MIB_TCPTABLE_OWNER_PID
{
      public UInt32 EntryCount;

      [MarshalAs(UnmanagedType.ByValArray , ArraySubType = UnmanagedType.Struct , SizeConst = 1)]
      public MIB_TCPROW_OWNER_PID[] Table;
}

public record MIB_TCPROW_PID(Int32 State , String? LocalAddress , Int32 LocalPort , String? RemoteAddress , Int32 RemotePort , Int32 OwningPid);