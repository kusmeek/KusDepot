namespace KusDepot.Exams.DataItems;

public static class DataItemSecurityContextExamHelpers
{
    /// <summary>Create a `DataItemSecurityContext` for exam scenarios by issuing a management key and projecting it into the new security context model.</summary>
    public static DataItemSecurityContext CreateContext(DataItem item , String subject = "TestKey" , Boolean includeSelfRecipient = true)
    {
        ArgumentNullException.ThrowIfNull(item);

        ManagementKey? key = item.CreateManagementKey(subject) ?? throw new InvalidOperationException("Unable to create management key for exam context.");
        return CreateContext(item,key,null,includeSelfRecipient,subject);
    }

    /// <summary>Create a `DataItemSecurityContext` from an existing management key so tests can reuse the same key for lock operations and context-based crypto operations.</summary>
    public static DataItemSecurityContext CreateContext(DataItem item , ManagementKey key , Boolean includeSelfRecipient = true , String? displayName = null)
    {
        return CreateContext(item,key,null,includeSelfRecipient,displayName);
    }

    /// <summary>Create a `DataItemSecurityContext` with an optional distinct signing key so tests can exercise separate protection and signing identities.</summary>
    public static DataItemSecurityContext CreateContext(DataItem item , ManagementKey key , ManagementKey? signingKey , Boolean includeSelfRecipient = true , String? displayName = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(key);

        Guid objectId = EnsureItemId(item);
        X509Certificate2 certificate = Utility.DeserializeCertificate(key.Key) ?? throw new InvalidOperationException("Unable to deserialize management key certificate for exam context.");
        X509Certificate2 signingCertificate = signingKey is null ? certificate : Utility.DeserializeCertificate(signingKey.Key) ?? throw new InvalidOperationException("Unable to deserialize signing certificate for exam context.");
        Guid signingObjectId = signingKey is null ? objectId : Guid.NewGuid();

        DataSecurityObject localObject = new(objectId,certificate,displayName);
        DataSecurityObject signingObject = new(signingObjectId,signingCertificate,signingKey is null ? displayName : $"{displayName ?? objectId.ToString("N")}-Signer");
        ImmutableArray<DataSecurityObject> knownObjects = [localObject];
        ImmutableArray<DataSecurityRecipient> recipients = [new DataSecurityRecipient(certificate,objectId)];

        return new DataItemSecurityContext(
            localObject,
            signingObject,
            knownObjects,
            recipients,
            purpose: $"Exam:{item.GetType().Name}:{displayName ?? objectId.ToString("N")}",
            includeselfrecipient: includeSelfRecipient);
    }

    /// <summary>Create a management key and matching `DataItemSecurityContext` together for tests that need both legacy lock operations and the new context-based API.</summary>
    public static (ManagementKey Key , DataItemSecurityContext Context) CreateContextPair(DataItem item , String subject = "TestKey" , Boolean includeSelfRecipient = true)
    {
        ArgumentNullException.ThrowIfNull(item);

        ManagementKey key = item.CreateManagementKey(subject) ?? throw new InvalidOperationException("Unable to create management key for exam context.");
        return (key,CreateContext(item,key,includeSelfRecipient,subject));
    }

    private static Guid EnsureItemId(DataItem item)
    {
        Guid? id = item.GetID();
        if(id is not null && id != Guid.Empty) { return id.Value; }

        Guid created = Guid.NewGuid();
        if(!item.SetID(created)) { throw new InvalidOperationException("Unable to assign identifier required for exam security context."); }

        return created;
    }
}

public sealed class SimpleSecurityIdentityDirectory : ISecurityIdentityDirectory
{
    private readonly Dictionary<Guid,DataSecurityObject> identitiesByObjectId;
    private readonly Dictionary<String,DataSecurityObject> identitiesByThumbprint;

    public SimpleSecurityIdentityDirectory(IEnumerable<DataSecurityObject>? identities)
    {
        this.identitiesByObjectId = new();
        this.identitiesByThumbprint = new(StringComparer.OrdinalIgnoreCase);

        if(identities is null) { return; }

        foreach(DataSecurityObject identity in identities)
        {
            ArgumentNullException.ThrowIfNull(identity);

            this.identitiesByObjectId[identity.ObjectId] = identity;
            this.identitiesByThumbprint[NormalizeThumbprint(identity.Thumbprint)] = identity;
        }
    }

    public ValueTask<DataSecurityObject?> ResolveIdentity(Guid objectid , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        this.identitiesByObjectId.TryGetValue(objectid,out DataSecurityObject? identity);
        return ValueTask.FromResult(identity);
    }

    public ValueTask<DataSecurityObject?> ResolveIdentity(String thumbprint , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        if(String.IsNullOrWhiteSpace(thumbprint)) { return ValueTask.FromResult<DataSecurityObject?>(null); }

        this.identitiesByThumbprint.TryGetValue(NormalizeThumbprint(thumbprint),out DataSecurityObject? identity);
        return ValueTask.FromResult(identity);
    }

    private static String NormalizeThumbprint(String thumbprint)
    {
        return thumbprint.Replace(" ",String.Empty,StringComparison.Ordinal).ToUpperInvariant();
    }
}
