namespace KusDepot;

/**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/main/*'/>*/
public sealed class ActivityRecord
{
    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/property[@name="Message"]/*'/>*/
    public String? Message { get; private set; }

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/property[@name="Code"]/*'/>*/
    public ActivityRecordCode Code { get; private set; }

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/property[@name="Exception"]/*'/>*/
    public Exception? Exception { get; }

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/property[@name="HasOutput"]/*'/>*/
    public Boolean HasOutput { get; }

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/property[@name="TimeStamp"]/*'/>*/
    public DateTimeOffset TimeStamp { get; private set; }

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/constructor[@name="Constructor"]/*'/>*/
    public ActivityRecord(ActivityRecordCode code , String? message = null) : this(code,message,null,false) {}

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/constructor[@name="ConstructorWithException"]/*'/>*/
    public ActivityRecord(ActivityRecordCode code , String? message , Exception? exception) : this(code,message,exception,false) {}

    /**<include file='ActivityRecord.xml' path='ActivityRecord/class[@name="ActivityRecord"]/constructor[@name="ConstructorWithOutput"]/*'/>*/
    public ActivityRecord(ActivityRecordCode code , String? message , Exception? exception , Boolean hasoutput)
    {
        this.TimeStamp = DateTimeOffset.Now; this.Code = code; this.Message = message; this.Exception = exception; this.HasOutput = hasoutput;
    }
}

/**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/main/*'/>*/
public enum ActivityRecordCode
{
    /**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/field[@name="None"]/*'/>*/
    None = 0,

    /**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/field[@name="Success"]/*'/>*/
    Success = 1,

    /**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/field[@name="Failed"]/*'/>*/
    Failed = 4,

    /**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/field[@name="Canceled"]/*'/>*/
    Canceled = 7,

    /**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/field[@name="TimedOut"]/*'/>*/
    TimedOut = 9,

    /**<include file='ActivityRecord.xml' path='ActivityRecord/enum[@name="ActivityRecordCode"]/field[@name="Faulted"]/*'/>*/
    Faulted = 11
}