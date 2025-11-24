namespace KusDepot;

/**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/main/*'/>*/
[GenerateSerializer] [Alias("ManagementKey")]
[DataContract(Name = "ManagementKey" , Namespace = "KusDepot")]
public abstract class ManagementKey : SecurityKey
{
    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/property[@name="Thumbprint"]/*'/>*/
    [DataMember(Name = "Thumbprint" , EmitDefaultValue = true , IsRequired = true)] [NotNull] [Id(0)]
    public String? Thumbprint { get; protected set; }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/property[@name="CanSign"]/*'/>*/
    [IgnoreDataMember]
    public Boolean CanSign
    {
        get
        {
            try
            {
                if(this.Key is null) { return false; }

                using var c = DeserializeCertificate(this.Key); if(c is null) { return false; }

                using var k = c.GetRSAPrivateKey(); return k is not null;
            }
            catch { return false; }
        }
    }

    ///<inheritdoc/>
    public override Boolean Equals(SecurityKey? other) { return this.Equals(other as ManagementKey); }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="EqualsObject"]/*'/>*/
    public override Boolean Equals(Object? other) { return this.Equals(other as ManagementKey); }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="IEquatable{ManagementKey}.Equals"]/*'/>*/
    public Boolean Equals(ManagementKey? other)
    {
        try
        {
            if(ReferenceEquals(this,other)) { return true; } if(other is null || Equals(this.GetType(),other.GetType()) is false) { return false; }

            return Equals(this.ID,other.ID) && String.Equals(this.Thumbprint,other.Thumbprint);
        }
        catch { return false; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { var _ = new HashCode(); _.Add(this.ID); _.Add(this.Thumbprint); return _.ToHashCode(); }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="ParseAny"]/*'/>*/
    public static new ManagementKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; } ManagementKey? k;

            try { k = Parse<OwnerKey>(input,format);   if(k is not null) { return k; } } catch { }

            try { k = Parse<ManagerKey>(input,format); if(k is not null) { return k; } } catch { }

            return null;
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="Parse"]/*'/>*/
    public static new TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : ManagementKey
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="TryParse"]/*'/>*/
    public static new Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : ManagementKey
    {
        result = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { result = _2; return true; }

            return false;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ManagementKey.xml' path='ManagementKey/record[@name="ManagementKeyWeb"]/main/*'/>*/
public record ManagementKeyWeb : SecurityKeyWeb
{
    /**<include file='ManagementKey.xml' path='ManagementKey/record[@name="ManagementKeyWeb"]/property[@name="Thumbprint"]/*'/>*/
    public String? Thumbprint { get; init; }
}