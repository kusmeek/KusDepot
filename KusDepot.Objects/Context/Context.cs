namespace KusDepot;

/**<include file='Context.xml' path='Context/class[@name="Context"]/main/*'/>*/
[DataContract(Name = "Context" , Namespace = "KusDepot")]
public abstract class Context
{
    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="AppDomainID"]/*'/>*/
    [DataMember(Name = "AppDomainID" , EmitDefaultValue = true , IsRequired = true)]
    protected Int64? AppDomainID;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="AppDomainUID"]/*'/>*/
    [DataMember(Name = "AppDomainUID" , EmitDefaultValue = true , IsRequired = true)]
    protected Int64? AppDomainUID;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="AssemblyVersion"]/*'/>*/
    [DataMember(Name = "AssemblyVersion" , EmitDefaultValue = true , IsRequired = true)]
    protected Version? AssemblyVersion;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="CPUID"]/*'/>*/
    [DataMember(Name = "CPUID" , EmitDefaultValue = true , IsRequired = true)]
    protected String? CPUID;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="MachineID"]/*'/>*/
    [DataMember(Name = "MachineID" , EmitDefaultValue = true , IsRequired = true)]
    protected String? MachineID;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="ProcessID"]/*'/>*/
    [DataMember(Name = "ProcessID" , EmitDefaultValue = true , IsRequired = true)]
    protected Int64? ProcessID;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/field[@name="ThreadID"]/*'/>*/
    [DataMember(Name = "ThreadID" , EmitDefaultValue = true , IsRequired = true)]
    protected Int32? ThreadID;

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainID"]/*'/>*/
    public virtual Int64? GetAppDomainID() { return this.AppDomainID; }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainUID"]/*'/>*/
    public virtual Int64? GetAppDomainUID() { return this.AppDomainUID; }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetAssemblyVersion"]/*'/>*/
    public virtual Version? GetAssemblyVersion() { return this.AssemblyVersion is null ? null : new Version(this.AssemblyVersion.ToString()); }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetCPUID"]/*'/>*/
    public virtual String? GetCPUID() { return this.CPUID is null ? null : new String(this.CPUID); }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetMachineID"]/*'/>*/
    public virtual String? GetMachineID() { return this.MachineID is null ? null : new String(this.MachineID); }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetProcessID"]/*'/>*/
    public virtual Int64? GetProcessID() { return this.ProcessID; }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="GetThreadID"]/*'/>*/
    public virtual Int32? GetThreadID() { return this.ThreadID; }

    /**<include file='Context.xml' path='Context/class[@name="Context"]/method[@name="Initialize"]/*'/>*/
    public virtual Boolean Initialize()
    {
        try
        {
            this.AppDomainID     = AppDomain.CurrentDomain.Id;
            this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            this.MachineID       = Environment.MachineName;
            this.ProcessID       = Environment.ProcessId;
            this.ThreadID        = Native.GetCurrentThreadId() ?? Environment.CurrentManagedThreadId;

            if(new List<Object?>(){this.AppDomainID,this.AssemblyVersion,this.MachineID,this.ProcessID,this.ThreadID}.Any(_=>_ is null)) { throw new InitializationException(); }

            return true;
        }
        catch ( InitializationException ) { if(NoExceptions) { return false; } throw; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } throw new InitializationException(_.Message,_); }
    }
}