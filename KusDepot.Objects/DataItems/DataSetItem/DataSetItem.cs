namespace KusDepot;

/**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.DataSetItem")]
[DataContract(Name = "DataSetItem" , Namespace = "KusDepot")]
public sealed class DataSetItem : DataItem , IComparable<DataSetItem> , IEquatable<DataSetItem> , IExtensibleDataObject , IParsable<DataSetItem>
{
    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/content[@name="Content"]/*'/>*/
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private HashSet<DataItem>? Content;

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public DataSetItem() : this(null,null,null,null,null,null,null) {}

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/constructor[@name="Constructor"]/*'/>*/
    public DataSetItem(IEnumerable<DataItem>? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetType(type); this.AddNotes(notes); this.AddTags(tags);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="AddDataItems"]/*'/>*/
    public Boolean AddDataItems(IEnumerable<DataItem>? data , ManagementKey? managementkey = null)
    {
        try
        {
            if( data is null || (this.Locked && this.CheckManager(managementkey) is false) ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    if(!data.TryDistinctByID(out var nc) || Equals(nc.Count,0)) { return Equals(nc?.Count,0); }

                    if(nc.Any( _ => { return _.GetID() is null || Equals(_.GetID(),Guid.Empty); })) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null) { return false; }

                    if(di.Content is null) { di.Content = new(); }

                    if(nc.Any( _ => di.Content.Any( d => Guid.Equals(d.GetID(),_.GetID())))) { return false; }

                    if(di.Content.TryAddByID(nc,out var _) is false) { return false; }

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData)) { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                    return true;
                }
                else
                {
                    if(!data.TryDistinctByID(out var nc) || Equals(nc.Count,0)) { return Equals(nc?.Count,0); }

                    if(nc.Any( _ => { return _.GetID() is null || Equals(_.GetID(),Guid.Empty); })) { return false; }

                    if(!nc.TryCloneDistinctByID(out var clones)) { return false; }

                    if(this.Content is null) { this.Content = new(); }

                    if(nc.Any( i => this.Content.Any( d => Guid.Equals(d.GetID(),i.GetID())))) { return false; }

                    if(this.Content.TryAddByID(clones, out var _) is false) { return false; }

                    if(UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> CheckDataHash(CancellationToken cancel = default)
    {
        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = File.OpenRead(f);

                    return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                }

                if(this.Content is null) { return false; }

                return await CheckDataHashCore(BitConverter.GetBytes(this.GetHashCode_NoSync()),null,cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(DataSetItem? other)
    {
        try
        {
            if(ReferenceEquals(this,other))              { return 0; }

            DateTimeOffset? _ = other?.GetBornOn();

            if(this.BornOn is null     && _ is null)     { return 0; }
            if(this.BornOn is not null && _ is null)     { return 1; }
            if(this.BornOn is null     && _ is not null) { return -1; }

            return this.BornOn!.Value.CompareTo(_!.Value);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareToFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> DecryptData(ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || this.Locked) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await DecryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!UpdateSecureHash(DataSecurityCNKey,SHA512.HashData(o),true)) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))      { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>()))     { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null;
                    }

                    return true;
                }
                else
                {
                    MemoryStream? o = null; Byte[]? oa = null;

                    try
                    {
                        var i = this.DataEncrypted && this.EncryptedData is not null ? this.EncryptedData : null;

                        o = (await DecryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream; if(o is null) { return false; }

                        oa = o.ToArray(); var _ = Deserialize(oa) as DataSetItem; if(_ is null)                                       { return false; }

                        if(_.Content is not null)
                        {
                            if(!_.Content.TryCloneDistinctByID(out var cloned)) { return false; } this.Content = cloned;
                        }

                        await _.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);

                        if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync()))) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))                               { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>()))                              { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null; this.EncryptedData = null;

                        return true;
                    }
                    finally
                    {
                        if(o is not null) { await ZeroMemoryAsync(o,default).ConfigureAwait(false); await o.DisposeAsync().ConfigureAwait(false); } if(oa is not null) { ZeroMemory(oa); }
                    }
                }
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> EncryptData(ManagementKey? managementkey , Boolean sign = true , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || this.Locked) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await EncryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!UpdateSecureHash(DataSecurityEDKey,SHA512.HashData(o),true))       { return false; }
                        if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                        this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);
                    }

                    return true;
                }
                else
                {
                    Byte[]? i = null;

                    try
                    {
                        i = this.Content is not null ? this.Serialize() : null;

                        using(var o = await EncryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false) as MemoryStream)
                        {
                            if(o is null) { return false; }

                            this.EncryptedData = o.ToArray();

                            if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))            { return false; }
                            if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                            if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                            this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                            await this.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);
                        }

                        return true;
                    }
                    finally
                    {
                        if(i is not null) { ZeroMemory(i); }
                    }
                }
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(DataSetItem? a , DataSetItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(DataSetItem? a , DataSetItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public Boolean Equals(DataSetItem? other)
    {
        try
        {
            if(other is null) { return false; } if(ReferenceEquals(this,other)) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { return false; }

            try
            {
                HashSet<DataItem>? _ = other.GetContent();

                if(this.Content is null && _ is null) { return false; }

                if(this.Content is null && _ is not null) { return false; }

                if(this.Content is not null && _ is null) { return false; }

                var a = this.Content!.Select(i => i.GetID()).ToHashSet(); var b = _!.Select(i => i.GetID()).ToHashSet();

                if(a.Count != b.Count) { return false; }

                return a.SetEquals(b);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="FromFile"]/*'/>*/
    public static DataSetItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(DataSetItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return _1.ReadObject(_2) as DataSetItem;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetContent"]/*'/>*/
    public HashSet<DataItem>? GetContent()
    {
        try
        {
            if(this.Content is null || this.DataMasked is true) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(!this.Content.TryCloneDistinctByID(out var o)) { return null; }

                if(Equals(this.Content.Count,o.Count) is false) { return null; } return o;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<DataContent?> GetDataContent(ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        Stream? d = null;

        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.DataMasked is false) { goto ReturnContent; }

                if(managementkey is null || CheckSecrets(managementkey.Key,this.Secrets) is false) { return null; }

                ReturnContent:

                if(this.DataEncrypted && Array.Empty<Byte>().SequenceEqual(this.EncryptedData) is false)
                {
                    d = await this.DecryptDataCore(this.EncryptedData,null,managementkey,cancel).ConfigureAwait(false);

                    if(d is not MemoryStream m) { return null; } var _ = Deserialize(m.ToArray()) as DataSetItem; if(_ is null) { return null; }

                    try
                    {
                        if(_.Content is null) { return new() { DataSet = null }; }

                        if(!_.Content.TryCloneDistinctByID(out var ec)) { return null; }

                        return new() { DataSet = ec };
                    }
                    finally { await _.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false); _ = null; }
                }

                if(this.Content is null) { return null; }

                if(!this.Content.TryCloneDistinctByID(out var c)) { return null; }

                return new() { DataSet = c };
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(d is not null) { await ZeroMemoryAsync(d,default).ConfigureAwait(false); await d.DisposeAsync().ConfigureAwait(false); } }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetDataItem"]/*'/>*/
    public DataItem? GetDataItem(Guid? id , ManagementKey? managementkey = null)
    {
        try
        {
            if( id is null || this.DataMasked is true ) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return null; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return null; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null) { return null; }

                    return di.Content?.FirstOrDefault(_=>_.GetID().Equals(id));
                }

                if(this.Content is null) { return null; }

                return this.Content.FirstOrDefault(_=>_.GetID().Equals(id))?.Clone();
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetDataItems"]/*'/>*/
    public HashSet<DataItem>? GetDataItems(ManagementKey? managementkey = null)
    {
        try
        {
            if(this.DataMasked is true) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return null; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return null; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null) { return null; }

                    return di.Content;
                }

                if(this.Content is null) { return null; }

                if(!this.Content.TryCloneDistinctByID(out var o)) { return null; }

                if(Equals(this.Content?.Count,o.Count) is false) { return null; } return o;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Descriptor? GetDescriptor()
    {
        try
        {
            return this.GetID() is null ? null :

            new()
            {
                Application        = this.GetApplication(),
                ApplicationVersion = this.GetApplicationVersion()?.ToString(),
                BornOn             = this.GetBornOn()?.ToString("O"),
                ContentStreamed    = this.GetContentStreamed(),
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Size               = this.Content?.Count.ToStringInvariant(),
                Tags               = this.GetTags(),
                Type               = this.GetType(),
                Version            = this.GetVersion()?.ToString()
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.GetHashCode_NoSync(); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Int32 GetHashCode_NoSync()
    {
        try
        {
            var i = this.GetID()!.Value.ToByteArray();
            var c = this.Content?.Order()
                .SelectMany(d => BitConverter.GetBytes(d.GetHashCode())).ToArray() ?? Array.Empty<Byte>();

            return BitConverter.ToInt32(SHA512.HashData(i.Concat(c).ToArray()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static DataSetItem IParsable<DataSetItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="Parse"]/*'/>*/
    public static new DataSetItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(DataSetItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            DataSetItem? _2 = _1.ReadObject(_0) as DataSetItem; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="RemoveDataItem"]/*'/>*/
    public Boolean RemoveDataItem(Guid? id , ManagementKey? managementkey = null)
    {
        try
        {
            if( id is null || (this.Locked && this.CheckManager(managementkey) is false) ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null)           { return false; }

                    if(di.Content is null)                                                            { return true; }

                    DataItem? _ = di.Content.FirstOrDefault(_=>_.GetID().Equals(id)); if(_ is null)   { return true; }

                    if(di.Content.Remove(_))
                    {
                        if(Equals(di.Content.Count,0)) { di.Content = null; }

                        var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)      { return false; }

                        this.EncryptedData = eb; this.Content = null; MN();

                        if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                        if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                        return true;
                    }
                    return false;
                }
                else
                {
                    if(this.Content is null) { return true; }

                    DataItem? _ = this.Content.FirstOrDefault(_=>_.GetID().Equals(id));

                    if(_ is null) { return true; }

                    if(this.Content.Remove(_))
                    {
                        if(Equals(this.Content.Count,0)) { this.Content = null; } MN();

                        if(UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                        }
                        else { return false; }
                    }
                    return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(IEnumerable<DataItem>? content , ManagementKey? managementkey = null)
    {
        try
        {
            if( content is null || (this.Locked && this.CheckManager(managementkey) is false) ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    if(!content.TryDistinctByID(out var nc)) { return false; }

                    if(Equals(nc.Count,0))
                    {
                        this.EncryptedData = null; this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityEDKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityEDKey,managementkey);
                        }
                        else { return false; }
                    }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null)           { return false; }

                    di.Content = nc;

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)          { return false; }

                    this.EncryptedData = eb; this.Content = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if(!content.TryDistinctByID(out var nc) || nc.Any(i => ReferenceEquals(i,this))) { return false; }

                    if(!nc.TryCloneDistinctByID(out var __)) { return false; }

                    if(Equals(__.Count,0))
                    {
                        this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityCNKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                        }
                        else { return false; }
                    }

                    this.Content = __; MN();

                    if(UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean SetContentStreamed(Boolean streamed , ManagementKey? managementkey = null)
    {
        try
        {
            if(this.Locked || this.DataEncrypted) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(streamed is false)
                {
                    if(this.ContentStreamed is false) { return true; }

                    this.ContentStreamed = false; MN();

                    if(UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }

                if(String.IsNullOrEmpty(this.GetFILE()) is true || File.Exists(this.GetFILE()) is false)
                {
                    this.ContentStreamed = false; MN(); return RefreshContentHash();
                }

                using Stream s = File.OpenRead(this.GetFILE()!);

                if(UpdateSecureHash(DataSecurityCNKey,SHA512.HashData(s),true))
                {
                    this.ContentStreamed = true; MN(); return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                }

                else { this.ContentStreamed = false; MN(); return RefreshContentHash(); }

                Boolean RefreshContentHash()
                {
                    if(UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync()))) { UpdateSecureSignature(DataSecurityCNKey,managementkey); }

                    return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataSetItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(DataSetItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            DataSetItem? _2 = _1.ReadObject(_0) as DataSetItem; if( _2 is not null ) { item = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> VerifyData(String? field , ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || String.IsNullOrEmpty(field)) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                using var s = this.GetContentStream_NoSync();

                switch(field)
                {
                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false || this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.Content is null && this.ContentStreamed is false) { return false; }

                        return await VerifyDataCore(field,managementkey,BitConverter.GetBytes(this.GetHashCode_NoSync()),s,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityHCKey:
                    {
                        return await VerifyDataCore(field,managementkey,null,null,cancel).ConfigureAwait(false);
                    }

                    default: return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> ZeroData(ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        try
        {
            return await this.ZeroDataCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    internal override async Task<Boolean> ZeroDataCore(ManagementKey? managementkey, HashSet<Object> visited, CancellationToken cancel = default)
    {
        if(await base.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

        await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            if(this.Content is not null)
            {
                foreach(var i in this.Content) { await i.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false); }

                this.Content.Clear(); this.Content = null;
            }

            if(this.ContentStreamed)
            {
                var f = this.GetFILE();

                if(File.Exists(f))
                {
                    using(var fs = new FileStream(f,FileMode.Open,FileAccess.Write,FileShare.None))
                    {
                        await ZeroMemoryAsync(fs, cancel).ConfigureAwait(false);
                    }
                    File.Delete(f); this.FILE = null;
                }

                this.ContentStreamed = false;
            }

            if(this.EncryptedData is not null)
            {
                ZeroMemory(this.EncryptedData); this.EncryptedData = null;

                this.DataEncrypted = false; this.DataEncryptInfo = null;
            }

            if(this.SecureHashes is not null)
            {
                foreach (var _ in this.SecureHashes.Values) { ZeroMemory(_); }

                this.SecureHashes.Clear(); this.SecureHashes = null;
            }

            if(this.SecureSignatures is not null)
            {
                foreach (var _ in this.SecureSignatures.Values) { ZeroMemory(_); }

                this.SecureSignatures.Clear(); this.SecureSignatures = null;
            }

            MN(); return true;
        }
        finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="ZeroDataInMemoryOnly"]/*'/>*/
    private async Task<Boolean> ZeroDataInMemoryOnly(ManagementKey? managementkey , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            return await this.ZeroDataInMemoryOnlyCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),requirelocks,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataInMemoryOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    internal override async Task<Boolean> ZeroDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            if(await base.ZeroDataInMemoryOnlyCore(managementkey,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            if(requirelocks) { await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false); }

            try
            {
                if(this.Content is not null)
                {
                    foreach(var i in this.Content)
                    {
                        await i.ZeroDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false);
                    }

                    this.Content.Clear(); this.Content = null;
                }

                return true;
            }
            finally { if(requirelocks) { this.Sync.Data.Release(); this.Sync.Meta.Release(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}