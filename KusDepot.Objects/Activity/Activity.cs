namespace KusDepot;

/**<include file='Activity.xml' path='Activity/class[@name="Activity"]/main/*'/>*/
public sealed class Activity
{
    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="handle"]/*'/>*/
    private String? handle;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="id"]/*'/>*/
    private Guid? id;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="progress"]/*'/>*/
    private IProgress<Object>? progress;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="started"]/*'/>*/
    private DateTimeOffset? started;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="task"]/*'/>*/
    private Task<Object?>? task;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="token"]/*'/>*/
    private CancellationToken? token;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Handle"]/*'/>*/
    public String? Handle              { get { return this.handle;   } set { this.handle   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="ID"]/*'/>*/
    public Guid? ID                    { get { return this.id;       } set { this.id       ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Progress"]/*'/>*/
    public IProgress<Object>? Progress { get { return this.progress; } set { this.progress ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Started"]/*'/>*/
    public DateTimeOffset? Started     { get { return this.started;  } set { this.started  ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Task"]/*'/>*/
    public Task<Object?>? Task         { get { return this.task;     } set { this.task     ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Token"]/*'/>*/
    public CancellationToken? Token    { get { return this.token;    } set { this.token    ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/constructor[@name="Constructor"]/*'/>*/
    public Activity(Task<Object?>? task = null , String? handle = null , Guid? id = null , DateTimeOffset? started = null , IProgress<Object>? progress = null , CancellationToken? token = null)
    {
        this.handle = handle; this.id = id; this.progress = progress; this.started = started; this.task = task; this.token = token;
    }
}