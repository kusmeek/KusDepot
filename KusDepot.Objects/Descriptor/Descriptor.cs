namespace KusDepot;

/**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/main/*'/>*/
[GenerateSerializer]
[Alias("KusDepot.Descriptor")]
[DataContract(Name = "Descriptor" , Namespace = "KusDepot")]
public sealed record Descriptor
{
    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Application                            {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? ApplicationVersion                     {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Artist"]/*'/>*/
    [DataMember(Name = "Artist" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Artist                                 {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? BornOn                                 {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Commands"]/*'/>*/
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public HashSet<Tuple<String,String,String>>? Commands {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Boolean? ContentStreamed                       {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? DistinguishedName                      {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    public String? FILE                                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    public Guid? ID                                       {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="LiveStream"]/*'/>*/
    [DataMember(Name = "LiveStream" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    public Boolean LiveStream                             {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    public String? Modified                               {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    public String? Name                                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    public HashSet<String>? Notes                         {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ObjectType"]/*'/>*/
    [DataMember(Name = "ObjectType" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    public String? ObjectType                             {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Services"]/*'/>*/
    [DataMember(Name = "Services" , EmitDefaultValue = true , IsRequired = true)] [Id(15)]
    public HashSet<Tuple<String,String>>? Services        {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(16)]
    public String? ServiceVersion                         {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Size"]/*'/>*/
    [DataMember(Name = "Size" , EmitDefaultValue = true , IsRequired = true)] [Id(17)]
    public String? Size                                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(18)]
    public HashSet<String>? Tags                          {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Title"]/*'/>*/
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)] [Id(19)]
    public String? Title                                  {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [Id(20)]
    public String? Type                                   {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(21)]
    public String? Version                                {get;init;}

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/property[@name="Year"]/*'/>*/
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)] [Id(22)]
    public String? Year                                   {get;init;}

    ///<inheritdoc/>
    public override String ToString() { try { return JsonSerializer.Serialize(this,new JsonSerializerOptions(){ WriteIndented = true }); } catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; } }

    /**<include file='Descriptor.xml' path='Descriptor/record[@name="Descriptor"]/method[@name="Parse"]/*'/>*/
    public static Descriptor? Parse(String input) { try { return String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<Descriptor>(input); } catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; } }
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