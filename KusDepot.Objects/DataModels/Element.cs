namespace KusDepot.Data.Models;

/**<include file='Element.xml' path='Element/record[@name="Element"]/main/*'/>*/
[DataContract(Name = "Element" , Namespace = "KusDepot")]
public sealed record Element
{
    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Application"]/*'/>*/        
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application        {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ApplicationVersion {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName  {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes            {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ObjectType         {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceVersion     {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    public String? Version            {get;init;}
}

/**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/main/*'/>*/
[DataContract(Name = "ElementQuery" , Namespace = "KusDepot")]
public sealed record ElementQuery
{
    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application        {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ApplicationVersion {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName  {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes            {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ObjectType         {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceVersion     {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementQuery"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    public String? Version            {get;init;}
}

/**<include file='Element.xml' path='Element/record[@name="ElementResponse"]/main/*'/>*/
[DataContract(Name = "ElementResponse" , Namespace = "KusDepot")]
public sealed record ElementResponse
{
    /**<include file='Element.xml' path='Element/record[@name="ElementResponse"]/property[@name="Elements"]/*'/>*/
    [DataMember(Name = "Elements" , EmitDefaultValue = true , IsRequired = true)]
    public Element[] Elements {get;init;} = Array.Empty<Element>();
}