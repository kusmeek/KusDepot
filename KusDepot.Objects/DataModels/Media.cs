namespace KusDepot.Data.Models;

/**<include file='Media.xml' path='Media/record[@name="Media"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "Media" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.Media")] [GenerateSerializer] [Immutable]

public sealed record class Media
{
    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Album"]/*'/>*/
    [JsonPropertyName("Album")] [JsonInclude]
    [DataMember(Name = "Album" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Album             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonInclude]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Application       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Artist"]/*'/>*/
    [JsonPropertyName("Artist")] [JsonInclude]
    [DataMember(Name = "Artist" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Artist            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonInclude]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? BornOn            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonInclude]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public Boolean? ContentStreamed  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonInclude]
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? DataType          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonInclude]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public String? DistinguishedName {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonInclude]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? FILE              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonInclude]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public Guid? ID                  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonInclude]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String? Modified          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonInclude]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String? Name              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonInclude]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String[]? Notes           {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ObjectFILE"]/*'/>*/
    [JsonPropertyName("ObjectFILE")] [JsonInclude]
    [DataMember(Name = "ObjectFILE" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? ObjectFILE        {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonInclude]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? Size              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonInclude]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String[]? Tags            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Title"]/*'/>*/
    [JsonPropertyName("Title")] [JsonInclude]
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? Title             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Year"]/*'/>*/
    [JsonPropertyName("Year")] [JsonInclude]
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String? Year              {get;init;}
}

/**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "MediaQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.MediaQuery")] [GenerateSerializer] [Immutable]

public sealed record MediaQuery
{
    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Album"]/*'/>*/
    [JsonPropertyName("Album")] [JsonInclude]
    [DataMember(Name = "Album" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Album             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonInclude]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Application       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Artist"]/*'/>*/
    [JsonPropertyName("Artist")] [JsonInclude]
    [DataMember(Name = "Artist" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Artist            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonInclude]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? BornOn            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonInclude]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public Boolean? ContentStreamed  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonInclude]
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? DataType          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonInclude]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public String? DistinguishedName {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonInclude]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? FILE              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonInclude]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public Guid? ID                  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonInclude]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String? Modified          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonInclude]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String? Name              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonInclude]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String[]? Notes           {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="ObjectFILE"]/*'/>*/
    [JsonPropertyName("ObjectFILE")] [JsonInclude]
    [DataMember(Name = "ObjectFILE" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? ObjectFILE        {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonInclude]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? Size              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonInclude]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String[]? Tags            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Title"]/*'/>*/
    [JsonPropertyName("Title")] [JsonInclude]
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? Title             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Year"]/*'/>*/
    [JsonPropertyName("Year")] [JsonInclude]
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String? Year              {get;init;}
}

/**<include file='Media.xml' path='Media/record[@name="MediaResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "MediaResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.MediaResponse")] [GenerateSerializer] [Immutable]

public sealed record MediaResponse
{
    /**<include file='Media.xml' path='Media/record[@name="MediaResponse"]/property[@name="Media"]/*'/>*/
    [JsonPropertyName("Media")] [JsonInclude]
    [DataMember(Name = "Media" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Media[] Media {get;init;} = Array.Empty<Media>();
}