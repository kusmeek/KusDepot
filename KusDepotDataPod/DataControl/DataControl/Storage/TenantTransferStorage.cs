namespace DataPodServices.DataControl;

/**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/main/*'/>*/
internal sealed class TenantTransferStorage : ITenantTransferStorage
{
    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="SHA512MetadataKey"]/*'/>*/
    private const String SHA512MetadataKey = "datacontrol_sha512";

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="LengthMetadataKey"]/*'/>*/
    private const String LengthMetadataKey = "datacontrol_length";

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="StagedBlockSize"]/*'/>*/
    private const Int32 StagedBlockSize = 8 * MiB;

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="container"]/*'/>*/
    private readonly BlobContainerClient container;

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="connectionString"]/*'/>*/
    private readonly String connectionString;

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="path"]/*'/>*/
    private readonly TransferStoragePath path;

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/field[@name="itemid"]/*'/>*/
    private readonly Guid itemid;

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/constructor[@name="TenantTransferStorage"]/*'/>*/
    internal TenantTransferStorage(StorageSilo silo , Guid sessionid , Guid itemid , Guid itemInstanceToken)
    {
        ArgumentNullException.ThrowIfNull(silo);

        if(String.IsNullOrWhiteSpace(silo.ConnectionString)) { throw new ArgumentException("A tenant storage connection string is required.",nameof(silo)); }

        if(itemid == Guid.Empty) { throw new ArgumentException("A published item identifier is required.",nameof(itemid)); }

        if(itemInstanceToken == Guid.Empty) { throw new ArgumentException("An item instance token is required.",nameof(itemInstanceToken)); }

        this.itemid = itemid; this.connectionString = silo.ConnectionString; this.path = TransferStoragePath.Create(itemInstanceToken,sessionid);

        this.container = new BlobContainerClient(silo.ConnectionString,itemid.ToString());
    }

    /// <inheritdoc/>
    public Task<Boolean> DeleteTransferAsync(CancellationToken cancel = default)
    {
        return DeleteTransferCoreAsync(cancel);
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="DeleteInstanceTransfersAsync"]/*'/>*/
    internal static async Task<Boolean> DeleteInstanceTransfersAsync(StorageSilo silo , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(silo);

        if(String.IsNullOrWhiteSpace(silo.ConnectionString)) { throw new ArgumentException("A tenant storage connection string is required.",nameof(silo)); }

        if(itemid == Guid.Empty) { throw new ArgumentException("A published item identifier is required.",nameof(itemid)); }

        String prefix = TransferStoragePath.ForInstance(itemInstanceToken).Prefix;

        BlobContainerClient container = new(silo.ConnectionString,itemid.ToString());

        if((await container.ExistsAsync(cancel)).Value is false) { return true; }

        await foreach(BlobItem blob in container.GetBlobsAsync(BlobTraits.None,BlobStates.None,prefix,cancel))
        {
            await container.DeleteBlobIfExistsAsync(blob.Name,DeleteSnapshotsOption.IncludeSnapshots,cancellationToken:cancel);
        }

        return true;
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenObjectAsync(CancellationToken cancel = default)
    {
        return OpenAsync(this.path.ObjectName,cancel);
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenRangeAsync(DataItemTransferRange range , CancellationToken cancel = default)
    {
        ValidateRange(range);

        return OpenAsync(this.path.CreateRangeName(range),cancel);
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenChunkAsync(Guid operationid , CancellationToken cancel = default)
    {
        ValidateOperationID(operationid);

        return OpenAsync(this.path.CreateChunkName(operationid),cancel);
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenPublicationObjectAsync(Guid operationid , CancellationToken cancel = default)
    {
        ValidateOperationID(operationid);

        return OpenAsync(this.path.CreatePublicationObjectName(operationid),cancel);
    }

    /// <inheritdoc/>
    public async Task<PublishedArtifactReference?> PublishObjectAsync(TenantTransferBlobReference source , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        ValidateBlobReference(source);

        await EnsureContainerAsync(cancel);

        BlockBlobClient sourceBlob = CreateBlockBlob(source.BlobName);

        BlockBlobClient destination = CreateBlockBlob(this.path.PublishedObjectName);

        if(!await sourceBlob.ExistsAsync(cancel)) { return null; }

        BlobProperties sourceProperties = await sourceBlob.GetPropertiesAsync(cancellationToken:cancel);

        if(!VerifyMetadata(sourceProperties,source.SHA512,source.Length)) { return null; }

        if(await destination.ExistsAsync(cancel))
        {
            return await VerifyArtifactAsync(destination,source.Length,source.SHA512,cancel);
        }

        try
        {
            Uri sourceUri = sourceBlob.GenerateSasUri(BlobSasPermissions.Read,DateTimeOffset.UtcNow.AddMinutes(15));

            await destination.SyncUploadFromUriAsync(sourceUri,new BlobSyncUploadFromUriOptions
            {
                Metadata = CreateMetadata(source.SHA512,source.Length),
                SourceConditions = new BlobRequestConditions { IfMatch = sourceProperties.ETag },
                DestinationConditions = new BlobRequestConditions { IfNoneMatch = ETag.All },
            },cancel);
        }
        catch ( RequestFailedException error ) when(error.Status == 409 || error.Status == 412)
        {
            return await VerifyArtifactAsync(destination,source.Length,source.SHA512,cancel);
        }

        return new PublishedArtifactReference { BlobName = destination.Name , Length = source.Length , SHA512 = source.SHA512.ToArray() };
    }

    /// <inheritdoc/>
    public async Task<PublishedArtifactReference?> TryLoadPublishedObjectAsync(CancellationToken cancel = default)
    {
        BlockBlobClient blob = CreateBlockBlob(this.path.PublishedObjectName);

        return await blob.ExistsAsync(cancel) ? await GetReferenceAsync(blob,cancel) : null;
    }

    /// <inheritdoc/>
    public async Task<PublishedArtifactReference?> TryLoadPublishedStreamAsync(CancellationToken cancel = default)
    {
        BlockBlobClient blob = CreateBlockBlob(this.path.PublishedStreamName);

        return await blob.ExistsAsync(cancel) ? await GetReferenceAsync(blob,cancel) : null;
    }

    /// <inheritdoc/>
    public async Task<PublishedArtifactReference?> TryLoadPublishedIntegrityAsync(CancellationToken cancel = default)
    {
        BlockBlobClient blob = CreateBlockBlob(this.path.PublishedIntegrityName);

        return await blob.ExistsAsync(cancel) ? await GetReferenceAsync(blob,cancel) : null;
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="GetReferenceAsync"]/*'/>*/
    private static async Task<PublishedArtifactReference> GetReferenceAsync(BlockBlobClient blob , CancellationToken cancel)
    {
        BlobProperties properties = await blob.GetPropertiesAsync(cancellationToken:cancel);

        return new PublishedArtifactReference { BlobName = blob.Name , Length = properties.ContentLength , SHA512 = ReadHash(properties.Metadata) };
    }

    /// <inheritdoc/>
    public async Task<PublishedArtifactReference?> AssembleStreamAsync(TransferBlockPlan plan , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if(plan.TotalLength < 0 || plan.Blocks is null) { throw new ArgumentException("A valid transfer block plan is required.",nameof(plan)); }

        TenantTransferStorageLimits.ValidateBudget(Guid.Empty,plan.Blocks.Count,0);

        await EnsureContainerAsync(cancel);

        BlockBlobClient destination = CreateBlockBlob(this.path.PublishedStreamName);

        List<String> blockids = new(plan.Blocks.Count);

        foreach(TransferBlock block in plan.Blocks)
        {
            ValidateBlock(block);

            BlockBlobClient source = CreateBlockBlob(block.Source.BlobName);

            if(!await source.ExistsAsync(cancel)) { return null; }

            Uri sourceUri = source.GenerateSasUri(BlobSasPermissions.Read,DateTimeOffset.UtcNow.AddMinutes(15));

            await destination.StageBlockFromUriAsync(sourceUri,block.BlockID,new StageBlockFromUriOptions
            {
                SourceRange = new HttpRange(block.SourceOffset,block.Length),
            },cancel);

            blockids.Add(block.BlockID);
        }

        if(blockids.Count is 0 && plan.TotalLength is not 0) { return null; }

        await destination.CommitBlockListAsync(blockids,new CommitBlockListOptions
        {
            Metadata = new Dictionary<String,String>
            {
                [LengthMetadataKey] = plan.TotalLength.ToString(CultureInfo.InvariantCulture),
            },
        },cancel);

        BlobProperties properties = await destination.GetPropertiesAsync(cancellationToken:cancel);

        if(properties.ContentLength != plan.TotalLength) { return null; }

        return new PublishedArtifactReference
        {
            BlobName = destination.Name,
            Length = properties.ContentLength,
            SHA512 = ReadHash(properties.Metadata),
        };
    }

    /// <inheritdoc/>
    public async Task<PublishedArtifactReference?> SaveIntegrityAsync(Stream payload , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        await using MemoryStream copy = new();

        await payload.CopyToAsync(copy,cancel); copy.Position = 0;

        TenantTransferBlobReference? saved = await SaveFinalAsync(this.path.PublishedIntegrityName,copy,cancel);

        return saved is null ? null : new PublishedArtifactReference { BlobName = saved.BlobName , Length = saved.Length , SHA512 = saved.SHA512 };
    }

    /// <inheritdoc/>
    public Task<TenantTransferBlobReference?> SaveObjectAsync(Stream payload , CancellationToken cancel = default)
    {
        return SaveAsync(this.path.ObjectName,payload,false,cancel);
    }

    /// <inheritdoc/>
    public Task<TenantTransferBlobReference?> ReplaceObjectAsync(Stream payload , CancellationToken cancel = default)
    {
        return SaveAsync(this.path.ObjectName,payload,true,cancel);
    }

    /// <inheritdoc/>
    public Task<TenantTransferBlobReference?> SaveRangeAsync(DataItemTransferRange range , Stream payload , CancellationToken cancel = default)
    {
        ValidateRange(range);

        return SaveAsync(this.path.CreateRangeName(range),payload,false,cancel);
    }

    /// <inheritdoc/>
    public Task<TenantTransferBlobReference?> SaveVerifiedRangeAsync(DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default)
    {
        ValidateRange(range);

        ValidateExpectedLength(range,expectedLength);

        return SaveVerifiedAsync(this.path.CreateRangeName(range),payload,expectedLength,expectedSHA512,cancel);
    }

    /// <inheritdoc/>
    public Task<TenantTransferBlobReference?> SaveVerifiedChunkAsync(Guid operationid , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default)
    {
        ValidateRange(range); ValidateOperationID(operationid);

        ValidateExpectedLength(range,expectedLength);

        return SaveVerifiedAsync(this.path.CreateChunkName(operationid),payload,expectedLength,expectedSHA512,cancel);
    }

    /// <inheritdoc/>
    public Task<TenantTransferBlobReference?> SavePublicationObjectAsync(Guid operationid , Stream payload , CancellationToken cancel = default)
    {
        ValidateOperationID(operationid);

        return SaveAsync(this.path.CreatePublicationObjectName(operationid),payload,false,cancel);
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="DeleteTransferCoreAsync"]/*'/>*/
    private async Task<Boolean> DeleteTransferCoreAsync(CancellationToken cancel)
    {
        if((await this.container.ExistsAsync(cancel)).Value is false) { return true; }

        await foreach(BlobItem blob in this.container.GetBlobsAsync(BlobTraits.None,BlobStates.None,String.Concat(this.path.Prefix,"/"),cancel))
        {
            await this.container.DeleteBlobIfExistsAsync(blob.Name,DeleteSnapshotsOption.IncludeSnapshots,cancellationToken:cancel);
        }

        return true;
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="SaveAsync"]/*'/>*/
    private async Task<TenantTransferBlobReference?> SaveAsync(String name , Stream payload , Boolean replace , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(payload); if(payload.CanSeek) { payload.Position = 0; }

        await EnsureContainerAsync(cancel);

        BlockBlobClient blob = CreateBlockBlob(name);

        await using HashingReadStream hashing = new(payload,leaveopen:true);

        List<String> blockids = await StagePayloadAsync(blob,hashing,cancel);

        Byte[] digest = hashing.GetHashAndReset(); Int64 length = hashing.BytesRead;

        try
        {
            await CommitPayloadAsync(blob,blockids,digest,length,replace,cancel);
        }
        catch ( RequestFailedException error ) when(!replace && (error.Status == 409 || error.Status == 412))
        {
            BlobProperties existing = await blob.GetPropertiesAsync(cancellationToken:cancel);

            Byte[] existingHash = ReadHash(existing.Metadata);

            if(existing.ContentLength != length || existingHash.SequenceEqual(digest) is false)
            {
                throw new InvalidOperationException(String.Concat("The immutable tenant transfer Blob already exists with different content: ",name));
            }

            return new TenantTransferBlobReference { BlobName = name , Length = existing.ContentLength , SHA512 = existingHash };
        }

        return new TenantTransferBlobReference { BlobName = name , Length = length , SHA512 = digest };
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="SaveVerifiedAsync"]/*'/>*/
    private async Task<TenantTransferBlobReference?> SaveVerifiedAsync(String name , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(payload); ArgumentNullException.ThrowIfNull(expectedSHA512);

        if(expectedLength < 0 || expectedSHA512.Length != 64) { throw new ArgumentException("A non-negative expected length and a SHA-512 digest are required."); }

        if(payload.CanSeek) { payload.Position = 0; }

        await EnsureContainerAsync(cancel);

        BlockBlobClient blob = CreateBlockBlob(name);

        await using HashingReadStream hashing = new(payload,leaveopen:true);

        List<String> blockids = await StagePayloadAsync(blob,hashing,cancel);

        Byte[] digest = hashing.GetHashAndReset(); Int64 length = hashing.BytesRead;

        if(length != expectedLength || digest.AsSpan().SequenceEqual(expectedSHA512) is false)
        {
            throw new OperationFailedException("The staged tenant transfer Blob does not match the declared content.",length != expectedLength ? StorageVerificationFailureCode.InvalidLength : StorageVerificationFailureCode.InvalidHash);
        }

        try
        {
            await CommitPayloadAsync(blob,blockids,digest,length,false,cancel);
        }
        catch ( RequestFailedException error ) when(error.Status == 409 || error.Status == 412)
        {
            BlobProperties existing = await blob.GetPropertiesAsync(cancellationToken:cancel);

            if(!VerifyMetadata(existing,expectedSHA512,expectedLength))
            {
                throw new OperationFailedException("The staged tenant transfer Blob does not match the declared content.",GetVerificationFailureCode(existing,expectedLength));
            }

            return new TenantTransferBlobReference { BlobName = name , Length = existing.ContentLength , SHA512 = expectedSHA512.ToArray() };
        }

        return new TenantTransferBlobReference { BlobName = name , Length = length , SHA512 = digest };
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="StagePayloadAsync"]/*'/>*/
    private static async Task<List<String>> StagePayloadAsync(BlockBlobClient blob , HashingReadStream payload , CancellationToken cancel)
    {
        List<String> blockids = new(); Guid operationid = Guid.NewGuid(); Byte[] buffer = ArrayPool<Byte>.Shared.Rent(StagedBlockSize);

        try
        {
            while(true)
            {
                Int32 count = 0;

                while(count < StagedBlockSize)
                {
                    Int32 read = await payload.ReadAsync(buffer.AsMemory(count,StagedBlockSize - count),cancel);

                    if(read is 0) { break; }

                    count += read;
                }

                if(count is 0) { break; }

                if(blockids.Count >= TenantTransferStorageLimits.MaximumBlockCount) { throw new InvalidOperationException("The tenant transfer payload exceeds the maximum supported Blob size for the configured staging block size."); }

                String encodedBlockID = CreateBlockID(operationid,blockids.Count);

                using MemoryStream block = new(buffer,0,count,false,true);

                await blob.StageBlockAsync(encodedBlockID,block,cancellationToken:cancel);

                blockids.Add(encodedBlockID);
            }

            return blockids;
        }
        finally
        {
            ArrayPool<Byte>.Shared.Return(buffer,clearArray:true);
        }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="CreateBlockID"]/*'/>*/
    private static String CreateBlockID(Guid operationid , Int32 index)
    {
        Span<Byte> blockid = stackalloc Byte[20];

        operationid.TryWriteBytes(blockid);

        BitConverter.TryWriteBytes(blockid[16..],index);

        return Convert.ToBase64String(blockid);
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="CommitPayloadAsync"]/*'/>*/
    private static async Task CommitPayloadAsync(BlockBlobClient blob , List<String> blockids , Byte[] digest , Int64 length , Boolean replace , CancellationToken cancel)
    {
        if(blockids.Count is 0)
        {
            await using MemoryStream empty = new();

            await blob.UploadAsync(empty,CreateUploadOptions(digest,length,replace),cancel);

            return;
        }

        await blob.CommitBlockListAsync(blockids,new CommitBlockListOptions
        {
            Metadata = CreateMetadata(digest,length),
            Conditions = replace ? null : new BlobRequestConditions { IfNoneMatch = ETag.All },
        },cancel);
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="GetVerificationFailureCode"]/*'/>*/
    private static StorageVerificationFailureCode GetVerificationFailureCode(BlobProperties properties , Int64 expectedLength)
    {
        return properties.ContentLength != expectedLength
            ? StorageVerificationFailureCode.InvalidLength
            : StorageVerificationFailureCode.InvalidHash;
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="DeleteStagedBlobAsync"]/*'/>*/
    private async Task DeleteStagedBlobAsync(String name)
    {
        try
        {
            await this.container.DeleteBlobIfExistsAsync(name,DeleteSnapshotsOption.IncludeSnapshots,cancellationToken:CancellationToken.None);
        }
        catch ( Exception error ) when(error is not OperationCanceledException)
        {
        }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="ValidateExpectedLength"]/*'/>*/
    private static void ValidateExpectedLength(DataItemTransferRange range , Int64 expectedLength)
    {
        if(expectedLength != range.Length) { throw new ArgumentException("The expected payload length must match the logical transfer range length.",nameof(expectedLength)); }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="SaveFinalAsync"]/*'/>*/
    private async Task<TenantTransferBlobReference?> SaveFinalAsync(String name , Stream payload , CancellationToken cancel)
    {
        return await SaveAsync(name,payload,true,cancel) is TenantTransferBlobReference saved
            ? saved
            : null;
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="OpenAsync"]/*'/>*/
    private async Task<Stream?> OpenAsync(String name , CancellationToken cancel)
    {
        await EnsureContainerAsync(cancel);

        try
        {
            BlobDownloadStreamingResult result = await this.container.GetBlobClient(name).DownloadStreamingAsync(cancellationToken:cancel);

            return new ContentLengthStream(result.Content,result.Details.ContentLength);
        }
        catch ( RequestFailedException error ) when(error.Status == 404)
        {
            return null;
        }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="EnsureContainerAsync"]/*'/>*/
    private async Task EnsureContainerAsync(CancellationToken cancel)
    {
        await this.container.CreateIfNotExistsAsync(cancellationToken:cancel);
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="VerifyArtifactAsync"]/*'/>*/
    private static async Task<PublishedArtifactReference?> VerifyArtifactAsync(BlockBlobClient blob , Int64 length , Byte[] hash , CancellationToken cancel)
    {
        BlobProperties properties = await blob.GetPropertiesAsync(cancellationToken:cancel);

        return VerifyMetadata(properties,hash,length)
            ? new PublishedArtifactReference { BlobName = blob.Name , Length = properties.ContentLength , SHA512 = hash.ToArray() }
            : null;
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="VerifyMetadata"]/*'/>*/
    private static Boolean VerifyMetadata(BlobProperties properties , Byte[] hash , Int64 length)
    {
        return properties.ContentLength == length && properties.Metadata.TryGetValue(SHA512MetadataKey,out String? actualHash) &&
               String.Equals(actualHash,Convert.ToHexString(hash),OrdinalIgnoreCase);
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="CreateUploadOptions"]/*'/>*/
    private static BlobUploadOptions CreateUploadOptions(Byte[] hash , Int64 length , Boolean replace)
    {
        return new BlobUploadOptions
        {
            Metadata = CreateMetadata(hash,length),
            Conditions = replace ? null : new BlobRequestConditions { IfNoneMatch = ETag.All },
        };
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="CreateMetadata"]/*'/>*/
    private static Dictionary<String,String> CreateMetadata(Byte[] hash , Int64 length)
    {
        return new Dictionary<String,String>
        {
            [SHA512MetadataKey] = Convert.ToHexString(hash),
            [LengthMetadataKey] = length.ToString(CultureInfo.InvariantCulture),
        };
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="ReadHash"]/*'/>*/
    private static Byte[] ReadHash(IDictionary<String,String> metadata)
    {
        if(metadata.TryGetValue(SHA512MetadataKey,out String? value) is false || String.IsNullOrWhiteSpace(value)) { return Array.Empty<Byte>(); }

        try
        {
            Byte[] hash = Convert.FromHexString(value);

            return hash.Length == 64 ? hash : Array.Empty<Byte>();
        }
        catch ( FormatException )
        {
            return Array.Empty<Byte>();
        }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="ValidateRange"]/*'/>*/
    private static void ValidateRange(DataItemTransferRange range)
    {
        if(range is null || !range.Validate() || range.Length <= 0) { throw new ArgumentException("A non-empty valid transfer range is required.",nameof(range)); }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="ValidateOperationID"]/*'/>*/
    private static void ValidateOperationID(Guid operationid)
    {
        if(operationid == Guid.Empty) { throw new ArgumentException("A transfer operation identifier is required.",nameof(operationid)); }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="ValidateBlobReference"]/*'/>*/
    private static void ValidateBlobReference(TenantTransferBlobReference reference)
    {
        if(String.IsNullOrWhiteSpace(reference.BlobName) || reference.Length < 0 || reference.SHA512.Length != 64) { throw new ArgumentException("A valid tenant transfer Blob reference is required.",nameof(reference)); }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="ValidateBlock"]/*'/>*/
    private static void ValidateBlock(TransferBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);

        if(String.IsNullOrWhiteSpace(block.BlockID) || block.Length <= 0 || block.Length > TenantTransferStorageLimits.MaximumBlockSizeBytes || block.SourceOffset < 0 || block.SourceOffset > block.Source.Length || block.Length > block.Source.Length - block.SourceOffset)
        {
            throw new ArgumentException("A valid tenant transfer block is required.",nameof(block));
        }
    }

    /**<include file='TenantTransferStorage.xml' path='TenantTransferStorage/class[@name="TenantTransferStorage"]/method[@name="CreateBlockBlob"]/*'/>*/
    private BlockBlobClient CreateBlockBlob(String name)
    {
        return new BlockBlobClient(this.connectionString,this.itemid.ToString(),name);
    }
}
