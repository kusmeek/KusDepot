namespace KusDepot.Data.Models;

/**<include file='Service.xml' path='Service/record[@name="Service"]/main/*'/>*/
[DataContract(Name = "Service" , Namespace = "KusDepot")]
public sealed record Service
{
    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application        {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ApplicationVersion {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn             {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed   {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                   {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified           {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes            {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ObjectType         {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceInterfaces"]/*'/>*/
    [DataMember(Name = "ServiceInterfaces" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceInterfaces  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceType"]/*'/>*/
    [DataMember(Name = "ServiceType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceType        {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceVersion     {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags             {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    public String? Version            {get;init;}
}

/**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/main/*'/>*/
[DataContract(Name = "ServiceQuery" , Namespace = "KusDepot")]
public record ServiceQuery
{
    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application        {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ApplicationVersion {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn             {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed   {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                   {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified           {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes            {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ObjectType         {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceInterfaces"]/*'/>*/
    [DataMember(Name = "ServiceInterfaces" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceInterfaces  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceType"]/*'/>*/
    [DataMember(Name = "ServiceType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceType        {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceVersion     {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags             {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    public String? Version            {get;init;}
}

/**<include file='Service.xml' path='Service/record[@name="ServiceResponse"]/main/*'/>*/
[DataContract(Name = "ServiceResponse" , Namespace = "KusDepot")]
public record ServiceResponse
{
    /**<include file='Service.xml' path='Service/record[@name="ServiceResponse"]/property[@name="Services"]/*'/>*/
    [DataMember(Name = "Services" , EmitDefaultValue = true , IsRequired = true)]
    public Service[] Services {get;init;} = Array.Empty<Service>();
}