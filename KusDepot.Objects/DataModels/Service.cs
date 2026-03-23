namespace KusDepot.Data.Models;

/**<include file='Service.xml' path='Service/record[@name="Service"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "Service" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.Service")] [GenerateSerializer] [Immutable]

public sealed record class Service
{
    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonRequired]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application           {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonRequired]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion    {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonRequired]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? BornOn                {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonRequired]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public Boolean? ContentStreamed      {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonRequired]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? DistinguishedName     {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonRequired]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? FILE                  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Guid? ID                      {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonRequired]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? Modified              {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? Name                  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonRequired]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String[]? Notes               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonRequired]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String? ObjectType            {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceInterfaces"]/*'/>*/
    [JsonPropertyName("ServiceInterfaces")] [JsonRequired]
    [DataMember(Name = "ServiceInterfaces" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? ServiceInterfaces     {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceSpecifications"]/*'/>*/
    [JsonPropertyName("ServiceSpecifications")] [JsonRequired]
    [DataMember(Name = "ServiceSpecifications" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? ServiceSpecifications {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceType"]/*'/>*/
    [JsonPropertyName("ServiceType")] [JsonRequired]
    [DataMember(Name = "ServiceType" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? ServiceType           {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonRequired]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String? ServiceVersion        {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonRequired]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? Size                  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonRequired]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String[]? Tags                {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="Service"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(17)]
    public String? Version               {get;init;}
}

/**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ServiceQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.ServiceQuery")] [GenerateSerializer] [Immutable]

public sealed record ServiceQuery
{
    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonRequired]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application           {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonRequired]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion    {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonRequired]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? BornOn                {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonRequired]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public Boolean? ContentStreamed      {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonRequired]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? DistinguishedName     {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonRequired]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? FILE                  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Guid? ID                      {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonRequired]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? Modified              {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? Name                  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonRequired]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String[]? Notes               {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonRequired]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String? ObjectType            {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceInterfaces"]/*'/>*/
    [JsonPropertyName("ServiceInterfaces")] [JsonRequired]
    [DataMember(Name = "ServiceInterfaces" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? ServiceInterfaces     {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceSpecifications"]/*'/>*/
    [JsonPropertyName("ServiceSpecifications")] [JsonRequired]
    [DataMember(Name = "ServiceSpecifications" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? ServiceSpecifications {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceType"]/*'/>*/
    [JsonPropertyName("ServiceType")] [JsonRequired]
    [DataMember(Name = "ServiceType" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? ServiceType           {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonRequired]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String? ServiceVersion        {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonRequired]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? Size                  {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonRequired]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String[]? Tags                {get;init;}

    /**<include file='Service.xml' path='Service/record[@name="ServiceQuery"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(17)]
    public String? Version               {get;init;}
}

/**<include file='Service.xml' path='Service/record[@name="ServiceResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ServiceResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.ServiceResponse")] [GenerateSerializer] [Immutable]

public sealed record class ServiceResponse
{
    /**<include file='Service.xml' path='Service/record[@name="ServiceResponse"]/property[@name="Services"]/*'/>*/
    [JsonPropertyName("Services")] [JsonRequired]
    [DataMember(Name = "Services" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Service[] Services {get;init;} = Array.Empty<Service>();
}