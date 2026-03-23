namespace KusDepot.Security;

/**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.AccessManagerState")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "AccessManagerState" , Namespace = "KusDepot.Security")]

public sealed class AccessManagerState
{
    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/field[@name="id"]/*'/>*/ 
    private Guid? id;

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/field[@name="certificate"]/*'/>*/ 
    private Byte[]? certificate;

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/field[@name="accesskeys"]/*'/>*/
    private Dictionary<String,HashSet<AccessKeyToken>>? accesskeys;

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/property[@name="AccessKeys"]/*'/>*/
    [JsonPropertyName("AccessKeys")] [JsonRequired] [Id(0)]
    [DataMember(Name = "AccessKeys" , EmitDefaultValue = true , IsRequired = true)]
    public Dictionary<String,HashSet<AccessKeyToken>>? AccessKeys
    {
        get
        {
            try
            {
                if(accesskeys is null) { return null; }

                return accesskeys.ToDictionary(p => new String(p.Key), p => p.Value.Select(_ => _.Clone()).ToHashSet());
            }
            catch ( Exception ) { if(NoExceptions) { return null; } throw; }
        }
        set => accesskeys ??= value;
    }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/property[@name="Certificate"]/*'/>*/
    [JsonPropertyName("Certificate")] [JsonRequired] [Id(1)]
    [DataMember(Name = "Certificate" , EmitDefaultValue = true , IsRequired = true)]
    public Byte[]? Certificate { get => certificate.CloneByteArray(); set => certificate ??= value; }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired] [Id(2)]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID { get => id; set => id ??= value; }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/method[@name="Clear"]/*'/>*/
    public void Clear()
    {
        try
        {
            if(this.accesskeys is not null)
            {
                foreach(var s in this.accesskeys.Values) { foreach(var t in s) { t.Clear(); } s.Clear(); }

                this.accesskeys.Clear(); this.accesskeys = null;
            }

            if(this.certificate is not null) { ZeroMemory(this.certificate); }

            this.id = Guid.Empty;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ClearFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/method[@name="Deserialize"]/*'/>*/
    public static AccessManagerState? Deserialize(Byte[] input , IFormatProvider? format = null)
    {
        try
        {
            if(Array.Empty<Byte>().SequenceEqual(input)) { return null; }

            return DataContractUtility.ParseBase64<AccessManagerState>(input.ToBase64FromByteArray(),SerializationData.ForType(typeof(AccessManagerState)));
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize()
    {
        try
        {
            var s = DataContractUtility.ToBase64String(this,SerializationData.ForType(typeof(AccessManagerState)));

            return s.ToByteArrayFromBase64();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }
}