namespace KusDepot;

/**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/main/*'/>*/
public record Descriptor
{
    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Application"]/*'/>*/
    public String? Application        {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ApplicationVersion"]/*'/>*/
    public String? ApplicationVersion {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Artist"]/*'/>*/
    public String? Artist             {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn             {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName  {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ID"]/*'/>*/
    public Guid? ID                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Locator"]/*'/>*/
    public String? Locator            {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Modified"]/*'/>*/
    public String? Modified           {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Name"]/*'/>*/
    public String? Name               {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Notes"]/*'/>*/
    public HashSet<String>? Notes     {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ObjectType"]/*'/>*/
    public String? ObjectType         {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Purpose"]/*'/>*/
    public String? Purpose            {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ServiceVersion"]/*'/>*/
    public String? ServiceVersion     {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Tags"]/*'/>*/
    public HashSet<String>? Tags      {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Title"]/*'/>*/
    public String? Title              {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Type"]/*'/>*/
    public String? Type               {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Version"]/*'/>*/
    public String? Version            {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Year"]/*'/>*/
    public String? Year               {get;init;}
}