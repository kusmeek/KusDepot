namespace KusDepot.Logging;

/**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/main/*'/>*/
public static class LoggerExtensions
{
    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Critical_Id_Ex"]/*'/>*/
    public static void Critical(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogCritical(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Critical_Id"]/*'/>*/
    public static void Critical(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogCritical(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Critical_Ex"]/*'/>*/
    public static void Critical(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogCritical(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Critical"]/*'/>*/
    public static void Critical(this ILogger? logger , String? message , params Object?[] args) => logger?.LogCritical(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Debug_Id_Ex"]/*'/>*/
    public static void Debug(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogDebug(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Debug_Id"]/*'/>*/
    public static void Debug(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogDebug(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Debug_Ex"]/*'/>*/
    public static void Debug(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogDebug(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Debug"]/*'/>*/
    public static void Debug(this ILogger? logger , String? message , params Object?[] args) => logger?.LogDebug(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Error_Id_Ex"]/*'/>*/
    public static void Error(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogError(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Error_Id"]/*'/>*/
    public static void Error(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogError(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Error_Ex"]/*'/>*/
    public static void Error(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogError(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Error"]/*'/>*/
    public static void Error(this ILogger? logger , String? message , params Object?[] args) => logger?.LogError(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Fatal_Id_Ex"]/*'/>*/
    public static void Fatal(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogCritical(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Fatal_Id"]/*'/>*/
    public static void Fatal(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogCritical(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Fatal_Ex"]/*'/>*/
    public static void Fatal(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogCritical(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Fatal"]/*'/>*/
    public static void Fatal(this ILogger? logger , String? message , params Object?[] args) => logger?.LogCritical(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Information_Id_Ex"]/*'/>*/
    public static void Information(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogInformation(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Information_Id"]/*'/>*/
    public static void Information(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogInformation(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Information_Ex"]/*'/>*/
    public static void Information(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogInformation(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Information"]/*'/>*/
    public static void Information(this ILogger? logger , String? message , params Object?[] args) => logger?.LogInformation(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Trace_Id_Ex"]/*'/>*/
    public static void Trace(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogTrace(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Trace_Id"]/*'/>*/
    public static void Trace(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogTrace(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Trace_Ex"]/*'/>*/
    public static void Trace(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogTrace(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Trace"]/*'/>*/
    public static void Trace(this ILogger? logger , String? message , params Object?[] args) => logger?.LogTrace(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Verbose_Id_Ex"]/*'/>*/
    public static void Verbose(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogTrace(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Verbose_Id"]/*'/>*/
    public static void Verbose(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogTrace(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Verbose_Ex"]/*'/>*/
    public static void Verbose(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogTrace(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Verbose"]/*'/>*/
    public static void Verbose(this ILogger? logger , String? message , params Object?[] args) => logger?.LogTrace(message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Warning_Id_Ex"]/*'/>*/
    public static void Warning(this ILogger? logger , EventId eventid , Exception? exception , String? message , params Object?[] args) => logger?.LogWarning(eventid,exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Warning_Id"]/*'/>*/
    public static void Warning(this ILogger? logger , EventId eventid , String? message , params Object?[] args) => logger?.LogWarning(eventid,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Warning_Ex"]/*'/>*/
    public static void Warning(this ILogger? logger , Exception? exception , String? message , params Object?[] args) => logger?.LogWarning(exception,message,args);

    /**<include file = 'LoggerExtensions.xml' path = 'LoggerExtensions/class[@name = "LoggerExtensions"]/method[@name = "Warning"]/*'/>*/
    public static void Warning(this ILogger? logger , String? message , params Object?[] args) => logger?.LogWarning(message,args);
}