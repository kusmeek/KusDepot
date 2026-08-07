namespace KusDepot.Data.Models;

/**<include file='Command.xml' path='Command/record[@name="Command"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "Command" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.Command")] [GenerateSerializer] [Immutable]

public sealed record class Command
{
    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonInclude]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonInclude]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion    {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonInclude]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? BornOn                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="CommandHandle"]/*'/>*/
    [JsonPropertyName("CommandHandle")] [JsonInclude]
    [DataMember(Name = "CommandHandle" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? CommandHandle         {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="CommandSpecifications"]/*'/>*/
    [JsonPropertyName("CommandSpecifications")] [JsonInclude]
    [DataMember(Name = "CommandSpecifications" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? CommandSpecifications {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="CommandType"]/*'/>*/
    [JsonPropertyName("CommandType")] [JsonInclude]
    [DataMember(Name = "CommandType" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? CommandType           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonInclude]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Boolean? ContentStreamed      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonInclude]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? DistinguishedName     {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonInclude]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? FILE                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonInclude]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public Guid? ID                      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonInclude]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String? Modified              {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonInclude]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? Name                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonInclude]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String[]? Notes               {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ObjectFILE"]/*'/>*/
    [JsonPropertyName("ObjectFILE")] [JsonInclude]
    [DataMember(Name = "ObjectFILE" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? ObjectFILE            {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonInclude]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String? ObjectType            {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonInclude]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? ServiceVersion        {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonInclude]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String? Size                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonInclude]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(17)]
    public String[]? Tags                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="Command"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonInclude]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(18)]
    public String? Version               {get;init;}
}

/**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "CommandQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.CommandQuery")] [GenerateSerializer] [Immutable]

public sealed record CommandQuery
{
    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonInclude]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonInclude]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion    {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonInclude]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? BornOn                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="CommandHandle"]/*'/>*/
    [JsonPropertyName("CommandHandle")] [JsonInclude]
    [DataMember(Name = "CommandHandle" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? CommandHandle         {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="CommandSpecifications"]/*'/>*/
    [JsonPropertyName("CommandSpecifications")] [JsonInclude]
    [DataMember(Name = "CommandSpecifications" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? CommandSpecifications {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="CommandType"]/*'/>*/
    [JsonPropertyName("CommandType")] [JsonInclude]
    [DataMember(Name = "CommandType" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public String? CommandType           {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonInclude]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Boolean? ContentStreamed      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonInclude]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? DistinguishedName     {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonInclude]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? FILE                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonInclude]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public Guid? ID                      {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonInclude]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public String? Modified              {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonInclude]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? Name                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonInclude]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String[]? Notes               {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ObjectFILE"]/*'/>*/
    [JsonPropertyName("ObjectFILE")] [JsonInclude]
    [DataMember(Name = "ObjectFILE" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? ObjectFILE            {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonInclude]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String? ObjectType            {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonInclude]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? ServiceVersion        {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonInclude]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String? Size                  {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonInclude]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(17)]
    public String[]? Tags                {get;init;}

    /**<include file='Command.xml' path='Command/record[@name="CommandQuery"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonInclude]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(18)]
    public String? Version               {get;init;}
}

/**<include file='Command.xml' path='Command/record[@name="CommandResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "CommandResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.CommandResponse")] [GenerateSerializer] [Immutable]

public sealed record CommandResponse
{
    /**<include file='Command.xml' path='Command/record[@name="CommandResponse"]/property[@name="Commands"]/*'/>*/
    [JsonPropertyName("Commands")] [JsonInclude]
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Command[] Commands {get;init;} = Array.Empty<Command>();
}