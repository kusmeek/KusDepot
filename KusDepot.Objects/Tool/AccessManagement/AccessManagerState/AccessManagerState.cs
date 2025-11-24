namespace KusDepot;

/**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/main/*'/>*/
[DataContract(Name = "AccessManagerState" , Namespace = "KusDepot")]
public sealed class AccessManagerState
{
    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/field[@name="id"]/*'/>*/ 
    private Guid? id;

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/field[@name="certificate"]/*'/>*/ 
    private Byte[]? certificate;

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/field[@name="accesskeys"]/*'/>*/
    private Dictionary<String,HashSet<AccessKeyToken>>? accesskeys;

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/property[@name="AccessKeys"]/*'/>*/
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
    [DataMember(Name = "Certificate" , EmitDefaultValue = true , IsRequired = true)]
    public Byte[]? Certificate { get => certificate.CloneByteArray(); set => certificate ??= value; }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/property[@name="ID"]/*'/>*/
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

            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(AccessManagerState),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            AccessManagerState? _2 = _1.ReadObject(_0) as AccessManagerState; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManagerState.xml' path='AccessManagerState/class[@name="AccessManagerState"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(AccessManagerState),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }
}