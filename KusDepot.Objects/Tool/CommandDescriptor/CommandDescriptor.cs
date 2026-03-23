namespace KusDepot;

/**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "CommandDescriptor" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("KusDepot.CommandDescriptor")] [Immutable]

public sealed record class CommandDescriptor : IEquatable<CommandDescriptor> , IParsable<CommandDescriptor>
{
    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/property[@name="Type"]/*'/>*/
    [JsonPropertyName("Type")] [JsonRequired]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Type                    {get;init;}

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/property[@name="Specifications"]/*'/>*/
    [JsonPropertyName("Specifications")] [JsonRequired]
    [DataMember(Name = "Specifications" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Specifications          {get;init;}

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/property[@name="ExtendedData"]/*'/>*/
    [JsonPropertyName("ExtendedData")] [JsonRequired]
    [DataMember(Name = "ExtendedData" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public List<String>? ExtendedData      {get;init;}

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/property[@name="RegisteredHandles"]/*'/>*/
    [JsonPropertyName("RegisteredHandles")] [JsonRequired]
    [DataMember(Name = "RegisteredHandles" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public List<String>? RegisteredHandles {get;init;}

    ///<inheritdoc/>
    public override String ToString()
    {
        try { return JsonUtility.ToJsonString<CommandDescriptor>(this); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; }
    }

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(CommandDescriptor? other) { return ReferenceEquals(this,other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return RuntimeHelpers.GetHashCode(this); }

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/method[@name="Create"]/*'/>*/
    public static CommandDescriptor Create(ICommand command) => new() { Type = command?.GetType().FullName };

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out CommandDescriptor result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail); return false; }
    }

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/method[@name="IParsable{CommandDescriptor}.Parse"]/*'/>*/
    static CommandDescriptor IParsable<CommandDescriptor>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='CommandDescriptor.xml' path='CommandDescriptor/record[@name="CommandDescriptor"]/method[@name="Parse"]/*'/>*/
    public static CommandDescriptor? Parse(String input)
    {
        try { return String.IsNullOrEmpty(input) ? null : JsonUtility.Parse<CommandDescriptor>(input); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; }
    }
}