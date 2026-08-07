namespace KusDepot;

public abstract partial class DataItem
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ApplyAssertionState"]/*'/>*/
    protected Boolean ApplyAssertionState(String field , DataItemSecurityContext? security)
    {
        if(security is null)
        {
            ClearAssertion(field);

            return true;
        }

        return UpdateFieldAssertion(field,security);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="BuildProtectionInfo"]/*'/>*/
    protected DataProtectionInfo BuildProtectionInfo(DataItemSecurityContext security , DataFieldState fieldstate)
    {
        ImmutableArray<DataProtectionRecipientSummary> recipients = GetProtectionRecipients(security);

        return new DataProtectionInfo
        {
            ProtectedAt = DateTimeOffset.UtcNow,
            ProtectedByObjectId = security.LocalObject.ObjectId,
            ProtectedByThumbprint = security.LocalObject.Thumbprint,
            Purpose = security.Purpose,
            Recipients = recipients,
            Assertions = GetProtectionAssertions(),
            HasMultipleRecipients = recipients.Length > 1,
            ProtectionMode = fieldstate.ToString()
        };
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ClearAssertion"]/*'/>*/
    protected void ClearAssertion(String field)
    {
        if(this.FieldAssertions is null || !this.FieldAssertions.Remove(field) || this.FieldAssertions.Count != 0) { return; }

        this.FieldAssertions = null;
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetProtectionAssertions"]/*'/>*/
    protected ImmutableArray<DataProtectionAssertionSummary> GetProtectionAssertions()
    {
        if(this.FieldAssertions is null || this.FieldAssertions.Count == 0) { return []; }

        ImmutableArray<DataProtectionAssertionSummary>.Builder builder = ImmutableArray.CreateBuilder<DataProtectionAssertionSummary>(this.FieldAssertions.Count);

        foreach(var pair in this.FieldAssertions)
        {
            if(pair.Value is null || pair.Value.Length == 0) { continue; }
            if(!DataFieldAssertionFactory.TryRead(pair.Value,out DataFieldAssertion? assertion) || assertion is null) { continue; }

            builder.Add(new DataProtectionAssertionSummary
            {
                Field = assertion.Data.Field,
                FieldState = assertion.Data.FieldState,
                IssuerObjectId = assertion.Data.IssuerObjectId,
                Thumbprint = assertion.Data.Thumbprint,
                CreatedAt = assertion.Data.CreatedAt
            });
        }

        return builder.ToImmutable();
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetProtectionRecipients"]/*'/>*/
    protected static ImmutableArray<DataProtectionRecipientSummary> GetProtectionRecipients(DataItemSecurityContext security)
    {
        List<DataProtectionRecipientSummary> recipients = [];
        IEnumerable<DataSecurityRecipient> source = security.Recipients.IsDefaultOrEmpty ? [new DataSecurityRecipient(security.LocalObject.Certificate,security.LocalObject.ObjectId)] : security.Recipients;

        foreach(var recipient in source)
        {
            Byte[] publickeyhash = recipient.PublicKeyHash ?? (recipient.Certificate is null ? Array.Empty<Byte>() : SHA256.HashData(recipient.Certificate.GetPublicKey()));

            recipients.Add(new DataProtectionRecipientSummary
            {
                ObjectId = recipient.ObjectId,
                Thumbprint = recipient.Thumbprint ?? recipient.Certificate?.Thumbprint,
                PublicKeyHash = ImmutableArray.Create(publickeyhash)
            });
        }

        if(security.IncludeSelfRecipient && recipients.All(_=>_.ObjectId != security.LocalObject.ObjectId))
        {
            recipients.Add(new DataProtectionRecipientSummary
            {
                ObjectId = security.LocalObject.ObjectId,
                Thumbprint = security.LocalObject.Thumbprint,
                PublicKeyHash = ImmutableArray.Create(SHA256.HashData(security.LocalObject.Certificate.GetPublicKey()))
            });
        }

        return [.. recipients];
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> ProtectData(DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            return this.GetDataEncrypted() ? this.ValidateData(security,cancel) : this.EncryptData(security,cancel);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ProtectDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Task.FromResult(false); } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> UnProtectData(DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            if(security is null) { return false; }

            if(this.GetDataEncrypted() && !await this.DecryptData(security,cancel).ConfigureAwait(false)) { return false; }

            return await this.ValidateData(security,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnProtectDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}
