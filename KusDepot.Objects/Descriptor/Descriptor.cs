namespace KusDepot;

/**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/main/*'/>*/
[DataContract(Name = "Descriptor" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("KusDepot.Descriptor")] [Immutable]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed record class Descriptor : IEquatable<Descriptor> , IParsable<Descriptor>
{
    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonRequired]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application                  {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonRequired]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion           {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Artist"]/*'/>*/
    [JsonPropertyName("Artist")] [JsonRequired]
    [DataMember(Name = "Artist" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Artist                       {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonRequired]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? BornOn                       {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Commands"]/*'/>*/
    [JsonPropertyName("Commands")] [JsonRequired]
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public HashSet<CommandDescriptor>? Commands {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonRequired]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Boolean? ContentStreamed             {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonRequired]
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? DataType                     {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonRequired]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? DistinguishedName            {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonRequired]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public String? FILE                         {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public Guid? ID                             {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="LiveStream"]/*'/>*/
    [JsonPropertyName("LiveStream")] [JsonRequired]
    [DataMember(Name = "LiveStream" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public Boolean LiveStream                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonRequired]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? Modified                     {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public String? Name                         {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonRequired]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public HashSet<String>? Notes               {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonRequired]
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public String? ObjectType                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Services"]/*'/>*/
    [JsonPropertyName("Services")] [JsonRequired]
    [DataMember(Name = "Services" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public HashSet<ToolDescriptor>? Services    {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonRequired]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(17)]
    public String? ServiceVersion               {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Size"]/*'/>*/
    [JsonPropertyName("Size")] [JsonRequired]
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(18)]
    public String? Size                         {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonRequired]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(19)]
    public HashSet<String>? Tags                {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Title"]/*'/>*/
    [JsonPropertyName("Title")] [JsonRequired]
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)] [Id(20)]
    public String? Title                        {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(21)]
    public String? Version                      {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Year"]/*'/>*/
    [JsonPropertyName("Year")] [JsonRequired]
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)] [Id(22)]
    public String? Year                         {get;init;}

    ///<inheritdoc/>
    public override String ToString()
    {
        try { return JsonUtility.ToJsonString<Descriptor>(this); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; }
    }

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(Descriptor? other) { return ReferenceEquals(this,other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return RuntimeHelpers.GetHashCode(this); }

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out Descriptor result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail); return false; }
    }

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/method[@name="IParsable{Descriptor}.Parse"]/*'/>*/
    static Descriptor IParsable<Descriptor>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/method[@name="Parse"]/*'/>*/
    public static Descriptor? Parse(String input)
    {
        try { return String.IsNullOrEmpty(input) ? null : JsonUtility.Parse<Descriptor>(input); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; }
    }
}

/**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/main/*'/>*/
public static class DescriptorExtensions
{
    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsBinaryItem"]/*'/>*/
    public static Boolean IsBinaryItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(BinaryItem).FullName!,
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsCodeItem"]/*'/>*/
    public static Boolean IsCodeItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(CodeItem).FullName!,
        }
        .Any(_ => descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsCommand"]/*'/>*/
    public static Boolean IsCommand(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(BinaryItem).FullName!,
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal)) && descriptor?.Commands is not null;
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsDataItem"]/*'/>*/
    public static Boolean IsDataItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(KeySet).FullName!,
            typeof(TextItem).FullName!,
            typeof(CodeItem).FullName!,
            typeof(BinaryItem).FullName!,
            typeof(GenericItem).FullName!,
            typeof(MSBuildItem).FullName!,
            typeof(DataSetItem).FullName!,
            typeof(MultiMediaItem).FullName!,
            typeof(GuidReferenceItem).FullName!
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsDataSetItem"]/*'/>*/
    public static Boolean IsDataSetItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(DataSetItem).FullName!,
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsGenericItem"]/*'/>*/
    public static Boolean IsGenericItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(GenericItem).FullName!,
        }
        .Any(_ => descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsGuidReferenceItem"]/*'/>*/
    public static Boolean IsGuidReferenceItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(GuidReferenceItem).FullName!,
        }
        .Any(_ => descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsKeySet"]/*'/>*/
    public static Boolean IsKeySet(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(KeySet).FullName!,
        }
        .Any(_ => descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsMSBuildItem"]/*'/>*/
    public static Boolean IsMSBuildItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(MSBuildItem).FullName!,
        }
        .Any(_ => descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsMultiMediaItem"]/*'/>*/
    public static Boolean IsMultiMediaItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(MultiMediaItem).FullName!,
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal));
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsService"]/*'/>*/
    public static Boolean IsService(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(BinaryItem).FullName!,
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal)) && descriptor?.Services is not null;
    }

    /**<include file='Descriptor.xml' path='Descriptor/class[@name="DescriptorExtensions"]/method[@name="IsTextItem"]/*'/>*/
    public static Boolean IsTextItem(this Descriptor descriptor)
    {
        return new String[]
        {
            typeof(TextItem).FullName!,
        }
        .Any(_=>descriptor.ObjectType!.Contains(_,Ordinal));
    }
}