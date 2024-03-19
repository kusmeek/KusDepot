namespace Watch;

/**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/main/*'/>*/
public interface IWatch : IActor
{
    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="GetElapsed"]/*'/>*/
    public Task<TimeSpan?> GetElapsed();

    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="GetStarted"]/*'/>*/
    public Task<DateTimeOffset?> GetStarted();

    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="GetStopped"]/*'/>*/
    public Task<DateTimeOffset?> GetStopped();

    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="GetTime"]/*'/>*/
    public Task<DateTimeOffset> GetTime();

    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="Reset"]/*'/>*/
    public Task<Boolean> Reset();

    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="Start"]/*'/>*/
    public Task<Boolean> Start();

    /**<include file='IWatch.xml' path='IWatch/interface[@name="IWatch"]/method[@name="Stop"]/*'/>*/
    public Task<Boolean> Stop();
}