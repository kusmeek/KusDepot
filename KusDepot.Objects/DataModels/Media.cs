namespace KusDepot.Data.Models;

/**<include file='Media.xml' path='Media/record[@name="Media"]/main/*'/>*/
[DataContract(Name = "Media" , Namespace = "KusDepot")]
public sealed record Media
{
    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Album"]/*'/>*/
    [DataMember(Name = "Album" , EmitDefaultValue = true , IsRequired = true)]
    public String? Album             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Artist"]/*'/>*/
    [DataMember(Name = "Artist" , EmitDefaultValue = true , IsRequired = true)]
    public String? Artist            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ContentHash"]/*'/>*/
    [DataMember(Name = "ContentHash" , EmitDefaultValue = true , IsRequired = true)]
    public String? ContentHash       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes           {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Title"]/*'/>*/
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)]
    public String? Title             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Year"]/*'/>*/
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)]
    public String? Year              {get;init;}
}

/**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/main/*'/>*/
[DataContract(Name = "MediaQuery" , Namespace = "KusDepot")]
public sealed record MediaQuery
{
    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Album"]/*'/>*/
    [DataMember(Name = "Album" , EmitDefaultValue = true , IsRequired = true)]
    public String? Album             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Artist"]/*'/>*/
    [DataMember(Name = "Artist" , EmitDefaultValue = true , IsRequired = true)]
    public String? Artist            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="ContentHash"]/*'/>*/
    [DataMember(Name = "ContentHash" , EmitDefaultValue = true , IsRequired = true)]
    public String? ContentHash       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes           {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Title"]/*'/>*/
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)]
    public String? Title             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaQuery"]/property[@name="Year"]/*'/>*/
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)]
    public String? Year              {get;init;}
}

/**<include file='Media.xml' path='Media/record[@name="MediaResponse"]/main/*'/>*/
[DataContract(Name = "MediaResponse" , Namespace = "KusDepot")]
public sealed record MediaResponse
{
    /**<include file='Media.xml' path='Media/record[@name="MediaResponse"]/property[@name="Media"]/*'/>*/
    [DataMember(Name = "Media" , EmitDefaultValue = true , IsRequired = true)]
    public Media[] Media {get;init;} = Array.Empty<Media>();
}