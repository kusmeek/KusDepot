namespace KusDepot;

/**<include file='TagAttribute.xml' path='TagAttribute/class[@name="TagAttribute"]/main/*'/>*/
[AttributeUsage( (AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue) , AllowMultiple = true)]
public sealed class TagAttribute : Attribute
{
    /**<include file='TagAttribute.xml' path='TagAttribute/class[@name="TagAttribute"]/property[@name="Tag"]/*'/>*/
    public String Tag {get;}

    /**<include file='TagAttribute.xml' path='TagAttribute/class[@name="TagAttribute"]/constructor[@name="Constructor"]/*'/>*/
    public TagAttribute(String? tag)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(tag);

            List<String?> AllTags = new List<String?>();

            Func<FieldInfo,String?> _ = (field) => (String?)field.GetValue(null);

            AllTags.AddRange(typeof(ArchitectureType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(BrowserType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(DatabaseType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(DeploymentType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(FrameworkVersionType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(HardwareType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(HostType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(Language).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(OperatingSystemType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(PlatformType).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(ServiceReference).GetFields().Select(_).ToArray());
            AllTags.AddRange(typeof(UsageType).GetFields().Select(_).ToArray());

            if(AllTags.Contains(tag)) { this.Tag = tag; }

            else { throw new ArgumentException("Invalid Tag",tag); }
        }
        catch { throw; }
    }
}