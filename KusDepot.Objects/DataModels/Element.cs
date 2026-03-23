namespace KusDepot.Data.Models;

/**<include file='Element.xml' path='Element/record[@name="Element"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "Element" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.Element")] [GenerateSerializer] [Immutable]

public sealed record class Element
{
    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonRequired]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application        {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonRequired]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonRequired]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? BornOn             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonRequired]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public Boolean? ContentStreamed   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonRequired]
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? DataType           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonRequired]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? DistinguishedName  {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonRequired]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public String? FILE               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public Guid? ID                   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonRequired]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? Modified           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String? Name               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonRequired]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String[]? Notes            {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonRequired]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? ObjectType         {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonRequired]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? ServiceVersion     {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonRequired]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? Size               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonRequired]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String[]? Tags             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? Version            {get;init;}
}

/**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ElementQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.ElementQuery")] [GenerateSerializer] [Immutable]

public sealed record ElementQuery
{
    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonRequired]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application        {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonRequired]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonRequired]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? BornOn             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonRequired]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public Boolean? ContentStreamed   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonRequired]
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? DataType           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonRequired]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? DistinguishedName  {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonRequired]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public String? FILE               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public Guid? ID                   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonRequired]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? Modified           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String? Name               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonRequired]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String[]? Notes            {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonRequired]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? ObjectType         {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonRequired]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? ServiceVersion     {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonRequired]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? Size               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonRequired]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String[]? Tags             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? Version            {get;init;}
}

/**<include file='Element.xml' path='Element/record[@name="ElementResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ElementResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.ElementResponse")] [GenerateSerializer] [Immutable]

public sealed record ElementResponse
{
    /**<include file='Element.xml' path='Element/record[@name="ElementResponse"]/property[@name="Elements"]/*'/>*/
    [JsonPropertyName("Elements")] [JsonRequired]
    [DataMember(Name = "Elements" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Element[] Elements {get;init;} = Array.Empty<Element>();
}