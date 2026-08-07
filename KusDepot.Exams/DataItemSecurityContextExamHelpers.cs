using System.Collections.Immutable;
using KusDepot.Security.Data;

namespace KusDepot.Exams;

public static class DataItemSecurityContextExamHelpers
{
    public static DataItemSecurityContext CreateContext(DataItem item , String subject = "TestKey" , Boolean includeSelfRecipient = true)
    {
        ArgumentNullException.ThrowIfNull(item);

        ManagementKey? key = item.CreateManagementKey(subject) ?? throw new InvalidOperationException("Unable to create management key for exam context.");
        return CreateContext(item,key,null,includeSelfRecipient,subject);
    }

    public static DataItemSecurityContext CreateContext(DataItem item , ManagementKey key , Boolean includeSelfRecipient = true , String? displayName = null)
    {
        return CreateContext(item,key,null,includeSelfRecipient,displayName);
    }

    public static DataItemSecurityContext CreateContext(DataItem item , ManagementKey key , ManagementKey? signingKey , Boolean includeSelfRecipient = true , String? displayName = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(key);

        Guid objectId = EnsureItemId(item);
        X509Certificate2 certificate = DeserializeCertificate(key.Key) ?? throw new InvalidOperationException("Unable to deserialize management key certificate for exam context.");
        X509Certificate2 signingCertificate = signingKey is null ? certificate : DeserializeCertificate(signingKey.Key) ?? throw new InvalidOperationException("Unable to deserialize signing certificate for exam context.");
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

    private static Guid EnsureItemId(DataItem item)
    {
        Guid? id = item.GetID();
        if(id is not null && id != Guid.Empty) { return id.Value; }

        Guid created = Guid.NewGuid();
        if(!item.SetID(created)) { throw new InvalidOperationException("Unable to assign identifier required for exam security context."); }

        return created;
    }
}
