namespace KusDepot;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public class AccessManager : IAccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="ID"]/*'/>*/
    protected Guid? ID;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Tool"]/*'/>*/
    protected ITool? Tool;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Logger"]/*'/>*/
    protected ILogger? Logger;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Certificate"]/*'/>*/
    protected X509Certificate2? Certificate;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="AccessKeys"]/*'/>*/
    protected Dictionary<String,HashSet<AccessKeyToken>> AccessKeys = new();

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/property[@name="MyNoExceptions"]/*'/>*/
    protected Boolean MyNoExceptions => !Tool?.MyExceptionsEnabled() ?? true;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/constructor[@name="ConstructorState"]/*'/>*/
    public AccessManager(AccessManagerState state)
    {
        try
        {
            this.ID = state.ID;

            if(state.AccessKeys is not null)
            {
                this.AccessKeys = new Dictionary<String,HashSet<AccessKeyToken>>();

                foreach(var s in state.AccessKeys)
                {
                    this.AccessKeys.Add(new String(s.Key),s.Value.Select(t => t.Clone()).ToHashSet());
                }
            }
            else { this.AccessKeys = new(); }

            this.Certificate = DeserializeCertificate(state.Certificate!); state.Clear();
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,this.GetType().Name,this.ID?.ToString()); if(MyNoExceptions) { return; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/constructor[@name="Constructor"]/*'/>*/
    public AccessManager(ITool? tool = null , ILogger? logger = null , X509Certificate2? certificate = null) { this.Initialize(tool,logger,certificate); }

    ///<inheritdoc/>
    public virtual Boolean AccessCheck(AccessKey? key = null , [CallerMemberName] String? operationname = null)
    {
        try
        {
            if(Equals(AccessKeys.Count,0)) { return false; }

            if(key is null || String.IsNullOrEmpty(operationname)) { return false; }

            Int32 op = ProtectedOperationResolver.ResolveIndex(operationname);

            if(op < 0 || op >= AccessKeySecret.MaxOperations) { return false; }

            if(AccessKeySecret.TryParse(
                key.Key,
                Certificate!,
                Tool!.GetID()!.Value,
                ID!.Value,
                out String? subject,
                out ReadOnlyMemory<Byte> bitmap,
                out DateTimeOffset issued,
                out DateTimeOffset? notafter,
                out AccessKeyToken token,
                out Boolean expired) is false)
            { return false; }

            if(expired) { return false; }

            Int32 bit = op % AccessKeySecret.BitsPerBlock;

            Int32 block = op / AccessKeySecret.BitsPerBlock;

            Int32 blockoffset = block * AccessKeySecret.BytesPerBlock;

            if(bitmap.Length < blockoffset + AccessKeySecret.BytesPerBlock) { return false; }

            UInt128 blockvalue = ReadUInt128BigEndian(bitmap.Span.Slice(blockoffset,AccessKeySecret.BytesPerBlock));

            Boolean allowed = ((blockvalue >> bit) & UInt128.One) == UInt128.One; if(allowed is false) { return false; }

            if(!TryEnter(AccessKeys!,SyncTime)) { throw SyncException; }

            if(AccessKeys.TryGetValue(subject!,out HashSet<AccessKeyToken>? set) is false) { return false; }

            return set.Contains(token);
        }
        catch ( Exception _ ) { Logger?.Error(_,AccessCheckFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="AddMembership"]/*'/>*/
    protected virtual Boolean AddMembership(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(AccessKeys!,SyncTime)) { throw SyncException; }

            if(AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? s)) { s.Add(token); return true; }

            return AccessKeys.TryAdd(subject,new HashSet<AccessKeyToken>{token});
        }
        catch ( Exception _ ) { Logger?.Error(_,AddMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.DestroySecrets)]
    public virtual Boolean DestroySecrets(AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return false; }

            if(!TryEnter(AccessKeys,SyncTime)) { throw SyncException; }

            foreach(HashSet<AccessKeyToken> s in AccessKeys.Values) { foreach(AccessKeyToken t in s) { t.Clear(); } s.Clear(); }

            AccessKeys.Clear(); Certificate?.Dispose(); Certificate = null;

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,DestroySecretsFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys)) { Exit(AccessKeys); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.ExportAccessManagerState)]
    public virtual AccessManagerState? ExportAccessManagerState(AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return null; }

            return new ()
            {
                ID = this.ID,
                AccessKeys = this.AccessKeys.ToDictionary(k => new String(k.Key), k => k.Value.Select(t => t.Clone()).ToHashSet()),
                Certificate = SerializeCertificate(this.Certificate!)
            };
        }
        catch ( Exception _ ) { Logger?.Error(_,ExportFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateClientKey"]/*'/>*/
    protected virtual ClientKey? GenerateClientKey()
    {
        try
        {
            String s = Guid.NewGuid().ToString();

            if(TryIssueToken(ProtectedOperationSets.Client,s,out Byte[]? k,out AccessKeyToken t) is false) { return null; }

            if(AddMembership(s,t) is false) { return null; }

            return new ClientKey(k!);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateClientKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GenerateCommandKey)]
    public virtual CommandKey? GenerateCommandKey(ICommand? command = null , AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return null; }

            Guid? i = command?.GetID(); if(i is null || Equals(i,Guid.Empty) ) { return null; } String s = i.Value.ToString();

            if(TryIssueToken(ProtectedOperationSets.Command,s,out Byte[]? k,out AccessKeyToken t) is false) { return null; }

            if(AddMembership(s,t) is false) { return null; }

            return new CommandKey(k!);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateCommandKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateExecutiveKey"]/*'/>*/
    protected virtual ExecutiveKey? GenerateExecutiveKey()
    {
        try
        {
            String s = Guid.NewGuid().ToString();

            if(TryIssueToken(ProtectedOperationSets.Executive,s,out Byte[]? k,out AccessKeyToken t) is false) { return null; }

            if(AddMembership(s,t) is false) { return null; }

            return new ExecutiveKey(k!);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateExecutiveKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateHostKey"]/*'/>*/
    protected virtual HostKey? GenerateHostKey()
    {
        try
        {
            String s = Guid.NewGuid().ToString();

            if(TryIssueToken(ProtectedOperationSets.Host,s,out Byte[]? k,out AccessKeyToken t) is false) { return null; }

            if(AddMembership(s,t) is false) { return null; }

            return new HostKey(k!);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateHostKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GenerateHostedServiceKey)]
    public virtual MyHostKey? GenerateHostedServiceKey(ITool? tool = null , AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return null; }

            Guid? i = tool?.GetID(); if(i is null || Equals(i,Guid.Empty) ) { return null; } String s = i.Value.ToString();

            if(TryIssueToken(ProtectedOperationSets.MyHost,s,out Byte[]? k,out AccessKeyToken t) is false) { return null; }

            if(AddMembership(s,t) is false) { return null; }

            return new MyHostKey(k!);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateHostedServiceKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateServiceKey"]/*'/>*/
    protected virtual ServiceKey? GenerateServiceKey(ITool? tool = null)
    {
        try
        {
            Guid? i = tool?.GetID(); if(i is null || Equals(i,Guid.Empty) ) { return null; } String s = i.Value.ToString();

            if(TryIssueToken(ProtectedOperationSets.Service,s,out Byte[]? k,out AccessKeyToken t) is false) { return null; }

            if(AddMembership(s,t) is false) { return null; }

            return new ServiceKey(k!);
        }
        catch ( Exception _ ) { Logger?.Error(_,GenerateServiceKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.ImportAccessManagementKeys)]
    public virtual Boolean ImportAccessManagementKeys(AccessManagerState? state , AccessKey? key = null)
    {
        try
        {
            if(state is null) { return true; }

            Dictionary<String,HashSet<AccessKeyToken>>? i = state?.AccessKeys;

            if(i is null || Equals(i.Count,0) ) { return true; }

            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return false; }

            if(!TryEnter(AccessKeys,SyncTime)) { throw SyncException; }

            foreach(KeyValuePair<String,HashSet<AccessKeyToken>> s in i)
            {
                if(AccessKeys.TryGetValue(s.Key,out HashSet<AccessKeyToken>? t)) { t.UnionWith(s.Value); }

                else { AccessKeys.Add(s.Key,s.Value); }
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,ImportAccessManagementKeysFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys)) { Exit(AccessKeys); } }
    }

    ///<inheritdoc/>
    public virtual Boolean Initialize(ITool? tool = null , ILogger? logger = null , X509Certificate2? certificate = null)
    {
        try
        {
            if(this.ID.HasValue) { this.Tool = tool; this.Logger = logger; return true; }

            if( Tool is not null || tool is null || this.Logger is not null ) { return false; } ID = Guid.NewGuid();

            Tool = tool; this.Logger = logger; this.Certificate ??= certificate ?? CreateCertificate(Tool,"AccessManager"); return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,InitializeFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="RemoveMembership"]/*'/>*/
    protected virtual Boolean RemoveMembership(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(AccessKeys!,SyncTime)) { throw SyncException; }

            if(!AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? s)) { return false; }

            if(s.Remove(token)) { if(Equals(s.Count,0)) { AccessKeys.Remove(subject); } return true; }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    ///<inheritdoc/>
    public virtual AccessKey? RequestAccess(AccessRequest? request = null)
    {
        try
        {
            if(request is null || Tool is null) { return null; }

            if(request is ManagementRequest)
            {
                ManagementKey? k = ManagementKey.Parse((request as ManagementRequest)!.Data,null);

                if(Tool.CheckManager(k) is true || Tool.CheckOwner(k) is true) { return GenerateExecutiveKey(); }

                k = new OwnerKey(DeserializeCertificate((request as ManagementRequest)!.Data.ToByteArrayFromBase64())!);

                if(Tool.CheckOwner(k) is true) { return GenerateExecutiveKey(); }

                k = new ManagerKey(DeserializeCertificate((request as ManagementRequest)!.Data.ToByteArrayFromBase64())!);

                if(Tool.CheckManager(k) is true) { return GenerateExecutiveKey(); }
            }

            if(Tool.GetLocked() is not false) { return null; }

            switch(request)
            {
                case HostRequest:
                {
                    if((request as HostRequest)!.External) { return GenerateHostKey(); }

                    if((request as HostRequest)!.Host!.IsHosting(Tool,Tool) ) { return GenerateHostKey(); }

                    return null;
                }

                case ServiceRequest:
                {
                    if(ReferenceEquals(Tool,(request as ServiceRequest)!.Tool)) { return GenerateExecutiveKey(); }

                    return GenerateServiceKey((request as ServiceRequest)!.Tool);
                }

                case StandardRequest: { return GenerateClientKey(); }

                default: { return null; }
            }
        }
        catch ( Exception _ ) { Logger?.Error(_,RequestAccessFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean RevokeAccess(AccessKey? key)
    {
        try
        {
            if(key is null) { return false; }

            if(AccessKeySecret.TryParse(
                key.Key,
                Certificate!,
                Tool!.GetID()!.Value,
                ID!.Value,
                out String? s,
                out ReadOnlyMemory<Byte> _,
                out DateTimeOffset _,
                out DateTimeOffset? _,
                out AccessKeyToken t,
                out Boolean _) is false)
            { return false; }

            return RemoveMembership(s!,t);
        }
        catch ( Exception _ ) { Logger?.Error(_,RevokeAccessFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryIssueToken"]/*'/>*/
    protected virtual Boolean TryIssueToken(IEnumerable<Int32> ops , String subject , out Byte[]? ciphertext , out AccessKeyToken token)
    {
        ciphertext = null; token = default;

        try { return AccessKeySecret.TryCreate(Certificate!,Tool!.GetID()!.Value,ID!.Value,subject,ops,lifetime:null,out ciphertext,out token); }

        catch ( Exception _ ) { Logger?.Error(_,TryIssueTokenFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }
}