namespace KusDepot;

/**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/main/*'/>*/
public sealed class CommandResult
{
    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/property[@name="Code"]/*'/>*/
    public ActivityRecordCode Code { get; }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/property[@name="Exception"]/*'/>*/
    public Exception? Exception { get; }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/property[@name="HasOutput"]/*'/>*/
    public Boolean HasOutput { get; }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/property[@name="Message"]/*'/>*/
    public String? Message { get; }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/property[@name="Output"]/*'/>*/
    public Object? Output { get; }

    private CommandResult(ActivityRecordCode code , Object? output , Boolean hasoutput , String? message , Exception? exception)
    {
        if(code is not (ActivityRecordCode.Success or ActivityRecordCode.Failed or ActivityRecordCode.Canceled or ActivityRecordCode.TimedOut or ActivityRecordCode.Faulted))
        {
            throw new ArgumentOutOfRangeException(nameof(code));
        }

        Code = code; Output = output; HasOutput = hasoutput; Message = message; Exception = exception;
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="Canceled"]/*'/>*/
    public static CommandResult Canceled(OperationCanceledException? exception = null , String? message = null)
    {
        return new(ActivityRecordCode.Canceled,null,false,message,exception);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="CanceledWithOutput"]/*'/>*/
    public static CommandResult CanceledWithOutput(Object? output , OperationCanceledException? exception = null , String? message = null)
    {
        return new(ActivityRecordCode.Canceled,output,true,message,exception);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="Failed"]/*'/>*/
    public static CommandResult Failed(String? message = null)
    {
        return new(ActivityRecordCode.Failed,null,false,message,null);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="FailedWithOutput"]/*'/>*/
    public static CommandResult FailedWithOutput(Object? output , String? message = null)
    {
        return new(ActivityRecordCode.Failed,output,true,message,null);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="Faulted"]/*'/>*/
    public static CommandResult Faulted(Exception exception , String? message = null)
    {
        ArgumentNullException.ThrowIfNull(exception); return new(ActivityRecordCode.Faulted,null,false,message ?? exception.Message,exception);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="FaultedWithOutput"]/*'/>*/
    public static CommandResult FaultedWithOutput(Exception exception , Object? output , String? message = null)
    {
        ArgumentNullException.ThrowIfNull(exception); return new(ActivityRecordCode.Faulted,output,true,message ?? exception.Message,exception);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="Success"]/*'/>*/
    public static CommandResult Success(String? message = null)
    {
        return new(ActivityRecordCode.Success,null,false,message,null);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="SuccessWithOutput"]/*'/>*/
    public static CommandResult SuccessWithOutput(Object? output , String? message = null)
    {
        return new(ActivityRecordCode.Success,output,true,message,null);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="TimedOut"]/*'/>*/
    public static CommandResult TimedOut(OperationCanceledException? exception = null , String? message = null)
    {
        return new(ActivityRecordCode.TimedOut,null,false,message,exception);
    }

    /**<include file='CommandResult.xml' path='CommandResult/class[@name="CommandResult"]/method[@name="TimedOutWithOutput"]/*'/>*/
    public static CommandResult TimedOutWithOutput(Object? output , OperationCanceledException? exception = null , String? message = null)
    {
        return new(ActivityRecordCode.TimedOut,output,true,message,exception);
    }
}
