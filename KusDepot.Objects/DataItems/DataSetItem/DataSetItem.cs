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
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetDataType(type); this.AddNotes(notes); this.AddTags(tags);
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

                    if(di.Content is null) { di.Content = new(new IDEquality()); }

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

                    if(this.Content is null) { this.Content = new(new IDEquality()); }

                    if(nc.Any( i => this.Content.Any( d => Guid.Equals(d.GetID(),i.GetID())))) { return false; }

                    if(this.Content.TryAddByID(clones, out var _) is false) { return false; }

                    if(UpdateSecureHash(DataSecurityCNKey,this.GetIntegrityHash_NoSync()))
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
                    if(this.ContentStreamed)
                    {
                        var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = File.OpenRead(f);

                        return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                    }

                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = File.OpenRead(f);

                    return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                }

                if(this.Content is null) { return false; }

                return await CheckDataHashCore(await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),null,cancel).ConfigureAwait(false);
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

            await this.AcquireLocksAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await DecryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!await UpdateSecureHashAsync(DataSecurityCNKey,o,cancel).ConfigureAwait(false)) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))  { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null;
                    }

                    return true;
                }
                else
                {
                    MemoryStream? o = null; Byte[]? ea = null;

                    try
                    {
                        var i = this.DataEncrypted && this.EncryptedData is not null ? this.EncryptedData : null;

                        o = (await DecryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream; if(o is null) { return false; }

                        ea = o.ToArray(); var _ = Deserialize(ea) as DataSetItem; if(_ is null)                                       { return false; }

                        if(_.Content is not null)
                        {
                            if(!_.Content.TryCloneDistinctByID(out var cloned)) { return false; } this.Content = cloned;
                        }

                        await _.WipeDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);

                        if(!await UpdateSecureHashAsync(DataSecurityCNKey,
                            await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),false,cancel).ConfigureAwait(false)) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))  { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null; this.EncryptedData = null;

                        return true;
                    }
                    finally
                    {
                        if(o is not null) { await ZeroMemoryAsync(o,default).ConfigureAwait(false); await o.DisposeAsync().ConfigureAwait(false); } if(ea is not null) { ZeroMemory(ea); }
                    }
                }
            }
            finally { this.ReleaseLocks(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> EncryptData(ManagementKey? managementkey , Boolean sign = true , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || this.Locked) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.AcquireLocksAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await EncryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!await UpdateSecureHashAsync(DataSecurityEDKey,o,cancel).ConfigureAwait(false)) { return false; }
                        if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

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

                            if(!await UpdateSecureHashAsync(DataSecurityEDKey,this.EncryptedData,false,cancel).ConfigureAwait(false)) { return false; }
                            if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                            if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                            this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                            await this.WipeDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);
                        }

                        return true;
                    }
                    finally
                    {
                        if(i is not null) { ZeroMemory(i); }
                    }
                }
            }
            finally { this.ReleaseLocks(); }
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

                if(this.Content is null && _ is null) { return true; }

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

            return DataContractUtility.FromFile<DataSetItem>(path,SerializationData.ForType(typeof(DataSetItem)));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetContent"]/*'/>*/
    public HashSet<DataItem>? GetContent()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Content is null) { return null; }

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
                if(this.DataEncrypted && (managementkey is null || CheckSecrets(managementkey.Key,this.Secrets) is false)) { return null; }

                if(this.DataEncrypted)
                {
                    if(this.ContentStreamed)
                    {
                        var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return null; }

                        d = await this.DecryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false);
                    }
                    else if(Array.Empty<Byte>().AsSpan().SequenceEqual(this.EncryptedData.AsSpan()) is false)
                    {
                        d = await this.DecryptDataCore(this.EncryptedData,null,managementkey,cancel).ConfigureAwait(false);
                    }

                    if(d is not MemoryStream m) { return null; } var _ = Deserialize(m.ToArray()) as DataSetItem; if(_ is null) { return null; }

                    try
                    {
                        if(_.Content is null) { return new() { DataSet = null }; }

                        if(!_.Content.TryCloneDistinctByID(out var ec)) { return null; }

                        return new() { DataSet = ec };
                    }
                    finally { await _.WipeDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false); _ = null; }
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
            if(id is null) { return null; }

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
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return null; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return null; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null) { return null; }

                    if(di.Content is null) { return null; }

                    if(!di.Content.TryCloneDistinctByID(out var cloned)) { return null; }

                    if(Equals(di.Content.Count,cloned.Count) is false) { return null; } return cloned;
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
                DataType           = this.GetDataType(),
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Size               = (this.ContentStreamed ? GetFileSize(this.FILE) : this.Content?.Count).ToStringInvariant(),
                Tags               = this.GetTags(),
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
    public override async Task<Int32> GetHashCodeAsync(CancellationToken cancel = default)
    {
        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try { return await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Int32 GetHashCode_NoSync()
    {
        try
        {
            return BitConverter.ToInt32(this.GetIntegrityHash_NoSync(),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            return BitConverter.ToInt32(await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected internal override Byte[] GetIntegrityHash_NoSync()
    {
        try
        {
            var c = this.Content?.Order()
                .SelectMany(d => d.GetIntegrityHash_NoSync()).ToArray() ?? Array.Empty<Byte>();

            return SHA512.HashData(c);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected internal override async Task<Byte[]> GetIntegrityHash_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            var array = this.Content?.Order()?.ToArray();

            if(array is null)
            {
                using var m = new MemoryStream(Array.Empty<Byte>(),false);

                return await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false);
            }

            var tasks = new Task<Byte[]>[array.Length];

            for(Int32 i = 0 ; i < array.Length ; i++)
            {
                tasks[i] = array[i].GetIntegrityHash_NoSyncAsync(cancel);
            }

            var hashes = await Task.WhenAll(tasks).ConfigureAwait(false);

            var contenthashes = hashes.SelectMany(h => h).ToArray();

            using var buffer = new MemoryStream(contenthashes,false);

            return await SHA512.HashDataAsync(buffer,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); throw; }
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
            return DataContractUtility.ParseBase64<DataSetItem>(input,SerializationData.ForType(typeof(DataSetItem)));
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

                        if(UpdateSecureHash(DataSecurityCNKey,this.GetIntegrityHash_NoSync()))
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

                    if(UpdateSecureHash(DataSecurityCNKey,this.GetIntegrityHash_NoSync()))
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
    public override async Task<Boolean> SetContentStreamed(Boolean streamed , ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        try
        {
            if(this.Locked || this.DataEncrypted) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(streamed is false)
                {
                    if(this.ContentStreamed is false) { return true; }

                    this.ContentStreamed = false; MN();

                    if(await UpdateSecureHashAsync(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel:cancel).ConfigureAwait(false))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }

                var f = this.GetFILE();

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false)
                {
                    this.ContentStreamed = false; MN(); return await RefreshContentHashAsync().ConfigureAwait(false);
                }

                await using FileStream s = new(f,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.Asynchronous|FileOptions.SequentialScan});

                var h = await SHA512.HashDataAsync(s,cancel).ConfigureAwait(false);

                if(await UpdateSecureHashAsync(DataSecurityCNKey,h,true,cancel).ConfigureAwait(false))
                {
                    this.ContentStreamed = true; MN(); return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                }

                this.ContentStreamed = false; MN(); return await RefreshContentHashAsync().ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }

        async Task<Boolean> RefreshContentHashAsync()
        {
            if(await UpdateSecureHashAsync(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel:cancel).ConfigureAwait(false))
            {
                UpdateSecureSignature(DataSecurityCNKey,managementkey);
            }

            return false;
        }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataSetItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(DataSetItem)),out item);
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
                        if(this.DataEncrypted is false) { return false; }

                        if(this.ContentStreamed)
                        {
                            return await VerifyDataCore(field,managementkey,null,s,cancel).ConfigureAwait(false);
                        }

                        if(this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.Content is null && this.ContentStreamed is false) { return false; }

                        return this.ContentStreamed ?
                            await VerifyDataCore(field,managementkey,null,s,cancel).ConfigureAwait(false) :
                            await VerifyDataCore(field,managementkey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),null,cancel).ConfigureAwait(false);
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
    public override async Task<Boolean> WipeData(ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        try
        {
            return await this.WipeDataCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataCore(ManagementKey? managementkey, HashSet<Object> visited, CancellationToken cancel = default)
    {
        if(await base.WipeDataCore(managementkey,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

        await this.AcquireLocksAsync(cancel).ConfigureAwait(false);

        try
        {
            if(this.Content is not null)
            {
                foreach(var i in this.Content) { await i.WipeDataCore(managementkey,visited,cancel).ConfigureAwait(false); }

                this.Content.Clear(); this.Content = null;
            }

            if(this.ContentStreamed)
            {
                var f = this.GetFILE();

                if(File.Exists(f))
                {
                    using(var fs = new FileStream(f,new FileStreamOptions{Mode = FileMode.Open , Access = FileAccess.Write , Share = FileShare.None , Options = FileOptions.Asynchronous}))
                    {
                        await WipeMemoryAsync(fs,cancel).ConfigureAwait(false);
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
        finally { this.ReleaseLocks(); }
    }

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            if(await base.WipeDataInMemoryOnlyCore(managementkey,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            if(requirelocks) { await this.AcquireLocksAsync(cancel).ConfigureAwait(false); }

            try
            {
                if(this.Content is not null)
                {
                    foreach(var i in this.Content)
                    {
                        await i.WipeDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false);
                    }

                    this.Content.Clear(); this.Content = null;
                }

                return true;
            }
            finally { if(requirelocks) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}