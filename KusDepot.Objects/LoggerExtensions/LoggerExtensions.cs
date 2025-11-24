namespace KusDepot;

/**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/main/*'/>*/
public static class LoggerExtensions 
{
    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Critical"]/*'/>*/
    public static void Critical(this ILogger? logger , String? message , params Object?[] args) => logger?.LogCritical(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "CriticalEx"]/*'/>*/
    public static void Critical(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogCritical(message,args); } else { logger?.LogCritical(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Debug"]/*'/>*/
    public static void Debug(this ILogger? logger , String? message , params Object?[] args) => logger?.LogDebug(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "DebugEx"]/*'/>*/
    public static void Debug(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogDebug(message,args); } else { logger?.LogDebug(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Error"]/*'/>*/
    public static void Error(this ILogger? logger , String? message , params Object?[] args) => logger?.LogError(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "ErrorEx"]/*'/>*/
    public static void Error(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogError(message,args); } else { logger?.LogError(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Fatal"]/*'/>*/
    public static void Fatal(this ILogger? logger , String? message , params Object?[] args) => logger?.LogCritical(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "FatalEx"]/*'/>*/
    public static void Fatal(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogCritical(message,args); } else { logger?.LogCritical(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Information"]/*'/>*/
    public static void Information(this ILogger? logger , String? message , params Object?[] args) => logger?.LogInformation(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "InformationEx"]/*'/>*/
    public static void Information(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogInformation(message,args); } else { logger?.LogInformation(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Trace"]/*'/>*/
    public static void Trace(this ILogger? logger , String? message , params Object?[] args) => logger?.LogTrace(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "TraceEx"]/*'/>*/
    public static void Trace(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogTrace(message,args); } else { logger?.LogTrace(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Verbose"]/*'/>*/
    public static void Verbose(this ILogger? logger , String? message , params Object?[] args) => logger?.LogTrace(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "VerboseEx"]/*'/>*/
    public static void Verbose(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogTrace(message,args); } else { logger?.LogTrace(exception,message,args); }
    }

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Warning"]/*'/>*/
    public static void Warning(this ILogger? logger , String? message , params Object?[] args) => logger?.LogWarning(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "WarningEx"]/*'/>*/
    public static void Warning(this ILogger? logger , Exception? exception , String? message , params Object?[] args)
    {
        if(exception is null) { logger?.LogWarning(message,args); } else { logger?.LogWarning(exception,message,args); }
    }
}