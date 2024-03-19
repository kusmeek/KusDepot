namespace KusDepot;

public static class Native
{
    public static Int32? GetCurrentThreadId() { if(OperatingSystem.IsWindowsVersionAtLeast(5,1,2600)) { return (Int32)PInvoke.GetCurrentThreadId(); } return null; }

    public static Boolean Message(String? msg)
    {
        if(msg is null) { return false; }

        try
        {
            if(OperatingSystem.IsWindowsVersionAtLeast(5)) { PInvoke.MessageBox(HWND.Null,msg,"Alert",Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_YESNOCANCEL); return true; }

            return false;
        }
        catch ( Exception ) { return false; }
    }

    public static Boolean ScreenSleep()
    {
        try
        {
            if(OperatingSystem.IsWindowsVersionAtLeast(5)) { PInvoke.SendMessage((HWND)0xFFFF,0x0112,0xF170,2); return true; }

            return false;
        }
        catch ( Exception ) { return false; }
    }

    public static UInt64 UpTime()
    {
        try
        {
            if(OperatingSystem.IsWindowsVersionAtLeast(5)) { return PInvoke.timeGetTime(); }

            return 0;
        }
        catch ( Exception ) { return 0; }
    }
}