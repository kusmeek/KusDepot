namespace KusDepot.Logging;

/**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/main/*'/>*/
public static class KusDepotLog
{
    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/property[@name="Logger"]/*'/>*/
    public static ILogger? Logger { get; private set; }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Initialize"]/*'/>*/
    public static Boolean Initialize(ILoggerFactory? factory)
    {
        try
        {
            if(factory is null) { return false; }

            Logger = factory.CreateLogger("KusDepot"); return Logger is not null;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Critical"]/*'/>*/
    public static void Critical(String? message , params Object?[] args)
    {
        try { Logger?.Critical(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="CriticalEx"]/*'/>*/
    public static void Critical(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Critical(message,args); } else { Logger?.Critical(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Debug"]/*'/>*/
    public static void Debug(String? message , params Object?[] args)
    {
        try { Logger?.Debug(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="DebugEx"]/*'/>*/
    public static void Debug(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Debug(message,args); } else { Logger?.Debug(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Error"]/*'/>*/
    public static void Error(String? message , params Object?[] args)
    {
        try { Logger?.Error(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="ErrorEx"]/*'/>*/
    public static void Error(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Error(message,args); } else { Logger?.Error(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Fatal"]/*'/>*/
    public static void Fatal(String? message , params Object?[] args)
    {
        try { Logger?.Fatal(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="FatalEx"]/*'/>*/
    public static void Fatal(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Fatal(message,args); } else { Logger?.Fatal(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Information"]/*'/>*/
    public static void Information(String? message , params Object?[] args)
    {
        try { Logger?.Information(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="InformationEx"]/*'/>*/
    public static void Information(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Information(message,args); } else { Logger?.Information(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Trace"]/*'/>*/
    public static void Trace(String? message , params Object?[] args)
    {
        try { Logger?.Trace(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="TraceEx"]/*'/>*/
    public static void Trace(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Trace(message,args); } else { Logger?.Trace(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Verbose"]/*'/>*/
    public static void Verbose(String? message , params Object?[] args)
    {
        try { Logger?.Verbose(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="VerboseEx"]/*'/>*/
    public static void Verbose(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Verbose(message,args); } else { Logger?.Verbose(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Warning"]/*'/>*/
    public static void Warning(String? message , params Object?[] args)
    {
        try { Logger?.Warning(message,args); }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="WarningEx"]/*'/>*/
    public static void Warning(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.Warning(message,args); } else { Logger?.Warning(exception,message,args); } }

        catch ( Exception ) { if(NoExceptions) { return; } throw; }
    }
}