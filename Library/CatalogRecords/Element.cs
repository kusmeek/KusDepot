namespace KusDepot.Data;

/**<include file='Element.xml' path='Element/record[@name="Element"]/main/*'/>*/
public record Element
{
    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Application"]/*'/>*/
    public String? Application        {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ApplicationVersion"]/*'/>*/
    public String? ApplicationVersion {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName  {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ID"]/*'/>*/
    public Guid? ID                   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Modified"]/*'/>*/
    public String? Modified           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Name"]/*'/>*/
    public String? Name               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes            {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ObjectType"]/*'/>*/
    public String? ObjectType         {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="ServiceVersion"]/*'/>*/
    public String? ServiceVersion     {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Type"]/*'/>*/
    public String? Type               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="Element"]/property[@name="Version"]/*'/>*/
    public String? Version            {get;init;}
}

/**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/main/*'/>*/
public record ElementRequest
{
    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Application"]/*'/>*/
    public String? Application        {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="ApplicationVersion"]/*'/>*/
    public String? ApplicationVersion {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName  {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="ID"]/*'/>*/
    public Guid? ID                   {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Modified"]/*'/>*/
    public String? Modified           {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Name"]/*'/>*/
    public String? Name               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes            {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="ObjectType"]/*'/>*/
    public String? ObjectType         {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="ServiceVersion"]/*'/>*/
    public String? ServiceVersion     {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags             {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Type"]/*'/>*/
    public String? Type               {get;init;}

    /**<include file='Element.xml' path='Element/record[@name="ElementRequest"]/property[@name="Version"]/*'/>*/
    public String? Version            {get;init;}
}

/**<include file='Element.xml' path='Element/record[@name="ElementResponse"]/main/*'/>*/
public record ElementResponse
{
    /**<include file='Element.xml' path='Element/record[@name="ElementResponse"]/property[@name="Elements"]/*'/>*/
    public Element[] Elements {get;init;} = Array.Empty<Element>();
}