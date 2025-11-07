namespace KusDepot.Data.Models;

/**<include file='Command.xml' path='Command/record[@name="Command"]/main/*'/>*/
[DataContract(Name = "Command" , Namespace = "KusDepot")]
public sealed record Command
{
    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ApplicationVersion    {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="CommandHandle"]/*'/>*/
    [DataMember(Name = "CommandHandle" , EmitDefaultValue = true , IsRequired = true)]
    public String? CommandHandle         {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="CommandSpecifications"]/*'/>*/
    [DataMember(Name = "CommandSpecifications" , EmitDefaultValue = true , IsRequired = true)]
    public String? CommandSpecifications {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="CommandType"]/*'/>*/
    [DataMember(Name = "CommandType" , EmitDefaultValue = true , IsRequired = true)]
    public String? CommandType           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ContentHash"]/*'/>*/
    [DataMember(Name = "ContentHash" , EmitDefaultValue = true , IsRequired = true)]
    public String? ContentHash           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName     {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified              {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes               {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ObjectType            {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceVersion        {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    public String? Version               {get;init;}
}

/**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/main/*'/>*/
[DataContract(Name = "CommandQuery" , Namespace = "KusDepot")]
public sealed record CommandQuery
{
    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    public String? Application           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ApplicationVersion    {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    public String? BornOn                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="CommandHandle"]/*'/>*/
    [DataMember(Name = "CommandHandle" , EmitDefaultValue = true , IsRequired = true)]
    public String? CommandHandle         {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="CommandSpecifications"]/*'/>*/
    [DataMember(Name = "CommandSpecifications" , EmitDefaultValue = true , IsRequired = true)]
    public String? CommandSpecifications {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="CommandType"]/*'/>*/
    [DataMember(Name = "CommandType" , EmitDefaultValue = true , IsRequired = true)]
    public String? CommandType           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ContentHash"]/*'/>*/
    [DataMember(Name = "ContentHash" , EmitDefaultValue = true , IsRequired = true)]
    public String? ContentHash           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)]
    public Boolean? ContentStreamed      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    public String? DistinguishedName     {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    public String? FILE                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID                      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    public String? Modified              {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    public String? Name                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes               {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)]
    public String? ObjectType            {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    public String? ServiceVersion        {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)]
    public String? Size                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    public String? Version               {get;init;}
}

/**<include file='Command.xml' path='Command/record[@name="CommandResponse"]/main/*'/>*/
[DataContract(Name = "CommandResponse" , Namespace = "KusDepot")]
public sealed record CommandResponse
{
    /**<include file='Command.xml' path='Command/record[@name="CommandResponse"]/property[@name="Commands"]/*'/>*/
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)]
    public Command[] Commands {get;init;} = Array.Empty<Command>();
}