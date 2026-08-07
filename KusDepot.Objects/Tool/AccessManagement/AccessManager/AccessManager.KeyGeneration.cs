namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager : IAccessManager
{
    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GenerateAccessKey)]
    public virtual TKey? GenerateAccessKey<TKey>(AccessKeyIssueOptions? options = null , AccessKey? key = null) where TKey : AccessKey
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return null; }

            return IssueAccessKey<TKey>(options);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateAccessKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GenerateAccessKey)]
    public virtual async Task<TKey?> GenerateAccessKeyAsync<TKey>(AccessKeyIssueOptions? options = null , AccessKey? key = null , CancellationToken cancel = default) where TKey : AccessKey
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            if(Tool?.GetLocked() is true && await AccessCheckAsync(key,nameof(GenerateAccessKey),cancel).ConfigureAwait(false) is false) { return null; }

            return await IssueAccessKeyAsync<TKey>(options,null,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateAccessKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateRealmKey"]/*'/>*/
    private Byte[]? GenerateRealmKey()
    {
        try
        {
            Byte[] realmkey = new Byte[AccessKeySecret.SymmetricKeySize]; RandomNumberGenerator.Fill(realmkey); return realmkey;
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateRealmKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }
}