namespace KusDepot;

/**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/main/*'/>*/
public interface IToolHostLifetime : IHostApplicationLifetime , IDisposable
{
    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/property[@name="ApplicationStarting"]/*'/>*/
    CancellationToken ApplicationStarting { get; }

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStarting"]/*'/>*/
    void NotifyStarting(Guid lifeid);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStarted"]/*'/>*/
    void NotifyStarted(Guid lifeid);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStopping"]/*'/>*/
    void NotifyStopping(Guid lifeid);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStopped"]/*'/>*/
    void NotifyStopped(Guid lifeid);
}