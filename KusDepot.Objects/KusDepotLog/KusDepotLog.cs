namespace KusDepot;

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
        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogInitializeFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Critical"]/*'/>*/
    public static void Critical(String? message , params Object?[] args)
    {
        try { Logger?.LogCritical(message,args); }

        catch ( Exception _ ) { Logger?.LogCritical(_,KusDepotLogCriticalFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="CriticalEx"]/*'/>*/
    public static void Critical(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogCritical(message,args); } else { Logger?.LogCritical(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogCritical(_,KusDepotLogCriticalFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Debug"]/*'/>*/
    public static void Debug(String? message , params Object?[] args)
    {
        try { Logger?.LogDebug(message,args); }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogDebugFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="DebugEx"]/*'/>*/
    public static void Debug(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogDebug(message,args); } else { Logger?.LogDebug(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogDebugFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Error"]/*'/>*/
    public static void Error(String? message , params Object?[] args)
    {
        try { Logger?.LogError(message,args); }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogErrorFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="ErrorEx"]/*'/>*/
    public static void Error(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogError(message,args); } else { Logger?.LogError(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogErrorFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Fatal"]/*'/>*/
    public static void Fatal(String? message , params Object?[] args)
    {
        try { Logger?.LogCritical(message,args); }

        catch ( Exception _ ) { Logger?.LogCritical(_,KusDepotLogCriticalFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="FatalEx"]/*'/>*/
    public static void Fatal(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogCritical(message,args); } else { Logger?.LogCritical(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogCritical(_,KusDepotLogCriticalFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Information"]/*'/>*/
    public static void Information(String? message , params Object?[] args)
    {
        try { Logger?.LogInformation(message,args); }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogInformationFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="InformationEx"]/*'/>*/
    public static void Information(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogInformation(message,args); } else { Logger?.LogInformation(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogInformationFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Trace"]/*'/>*/
    public static void Trace(String? message , params Object?[] args)
    {
        try { Logger?.LogTrace(message,args); }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogTraceFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="TraceEx"]/*'/>*/
    public static void Trace(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogTrace(message,args); } else { Logger?.LogTrace(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogTraceFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Verbose"]/*'/>*/
    public static void Verbose(String? message , params Object?[] args)
    {
        try { Logger?.LogTrace(message,args); }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogVerboseFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="VerboseEx"]/*'/>*/
    public static void Verbose(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogTrace(message,args); } else { Logger?.LogTrace(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogVerboseFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="Warning"]/*'/>*/
    public static void Warning(String? message , params Object?[] args)
    {
        try { Logger?.LogWarning(message,args); }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogWarningFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='KusDepotLog.xml' path='KusDepotLog/class[@name="KusDepotLog"]/method[@name="WarningEx"]/*'/>*/
    public static void Warning(Exception? exception , String? message , params Object?[] args)
    {
        try { if(exception is null) { Logger?.LogWarning(message,args); } else { Logger?.LogWarning(exception,message,args); } }

        catch ( Exception _ ) { Logger?.LogError(_,KusDepotLogWarningFail); if(NoExceptions) { return; } throw; }
    }
}