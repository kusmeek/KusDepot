using Serilog;

namespace KusDepot.Dapr;

public static class CommandUtility
{
    public static unsafe Int32? GetListeningProcess(String? processname , String? localaddress , Int32? port)
    {
        IntPtr data = IntPtr.Zero; if(port is null || port < 0 || port > 65535) { return null; }

        UInt32 buffersize = 0; const UInt32 margin = 24 * 1000;

        try
        {
            if( String.IsNullOrWhiteSpace(processname) || String.IsNullOrWhiteSpace(localaddress) ) { return null; }

            Process[] processes = Process.GetProcessesByName(processname); if(Equals(processes.Length,0)) { return null; }

            _ = PInvoke.GetExtendedTcpTable(Span<Byte>.Empty,ref buffersize,false,(UInt16)ADDRESS_FAMILY.AF_INET,TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_LISTENER,0);

            if(buffersize == 0 ) { return null; } buffersize = buffersize > Int32.MaxValue - margin ? Int32.MaxValue : buffersize + margin;

            data = new(NativeMemory.Alloc(buffersize));

            UInt32 result = PInvoke.GetExtendedTcpTable(new Span<Byte>(data.ToPointer(),(Int32)buffersize),ref buffersize,false,(UInt16)ADDRESS_FAMILY.AF_INET,TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_LISTENER,0);

            if(result != (UInt32)WIN32_ERROR.NO_ERROR) { return null; }

            MIB_TCPTABLE_OWNER_PID table = Marshal.PtrToStructure<MIB_TCPTABLE_OWNER_PID>(data);

            MIB_TCPROW_OWNER_PID[] rows = new MIB_TCPROW_OWNER_PID[table.EntryCount]; IntPtr row = (IntPtr)(data.ToInt64() + Marshal.SizeOf(table.EntryCount));

            for(Int32 i = 0; i < table.EntryCount; i++) { rows[i] = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(row); row = (IntPtr)(row.ToInt64() + Marshal.SizeOf<MIB_TCPROW_OWNER_PID>()); }

            MIB_TCPROW_PID[] dataset = rows.Select(_ => _.ToMIB_TCPROW_PID()).ToArray();

            return processes.FirstOrDefault(p => { return p.Id == dataset.FirstOrDefault(r => Equals(r.LocalAddress,localaddress) && Equals(r.LocalPort,port))?.OwningPid; })?.Id;
        }
        catch ( Exception _ ) { Log.Error(_,GetListeningProcessFail); return null; }

        finally { if(data != IntPtr.Zero) { NativeMemory.Free(data.ToPointer()); } }
    }

    public static Boolean TerminateProcess(Int32? processid)
    {
        if( processid is null || processid < 0 ) { return false; }

        HANDLE handle = HANDLE.Null;

        try
        {
            handle = PInvoke.OpenProcess(PROCESS_ACCESS_RIGHTS.PROCESS_TERMINATE,false,(UInt32)processid);

            if(Equals(handle,HANDLE.Null)) { return false; }

            return PInvoke.TerminateProcess(handle,0);
        }
        catch ( Exception _ ) { Log.Error(_,TerminateProcessFail); return false; }

        finally { if(Equals(handle,IntPtr.Zero)) { PInvoke.CloseHandle(handle); } }
    }

    public const String GetListeningProcessFail = @"GetListeningProcess Failed";
    public const String TerminateProcessFail    = @"TerminateProcess Failed";
}