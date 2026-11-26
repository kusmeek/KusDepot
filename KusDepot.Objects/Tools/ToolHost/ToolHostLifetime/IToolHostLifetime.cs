namespace KusDepot;

/**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/main/*'/>*/
public interface IToolHostLifetime : IHostApplicationLifetime
{
    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/property[@name="LifeID"]/*'/>*/
    Guid? LifeID { get; }

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/property[@name="ToolHost"]/*'/>*/
    IToolHost? ToolHost { get; }

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/property[@name="ApplicationStarting"]/*'/>*/
    CancellationToken ApplicationStarting { get; }

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="Bind"]/*'/>*/
    Guid Bind(IToolHost toolhost);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="Dispose"]/*'/>*/
    void Dispose(Guid lifekey);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStarting"]/*'/>*/
    void NotifyStarting(Guid lifekey);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStarted"]/*'/>*/
    void NotifyStarted(Guid lifekey);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStopping"]/*'/>*/
    void NotifyStopping(Guid lifekey);

    /**<include file='IToolHostLifetime.xml' path='IToolHostLifetime/interface[@name="IToolHostLifetime"]/method[@name="NotifyStopped"]/*'/>*/
    void NotifyStopped(Guid lifekey);
}