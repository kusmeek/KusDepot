namespace KusDepot.Security;

/**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/main/*'/>*/
public class KeyMaker
{
    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/field[@name="Aliases"]/*'/>*/
    private readonly Dictionary<String,String> Aliases = new(StringComparer.Ordinal);

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/field[@name="Certificate"]/*'/>*/
    private X509Certificate2? Certificate;

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/field[@name="Factories"]/*'/>*/
    private readonly Dictionary<String,Func<KeyMaterial,AccessKey?>> Factories = new(StringComparer.Ordinal);

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/field[@name="ID"]/*'/>*/
    private Guid? ID;

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/field[@name="Key"]/*'/>*/
    private Byte[]? Key;

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/finalizer[@name="Finalizer"]/*'/>*/
    ~KeyMaker() { this.Reset(); }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsClientKey"]/*'/>*/
    public ClientKey? AsClientKey()
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new ClientKey(this.Key!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsCommandKey"]/*'/>*/
    public CommandKey? AsCommandKey()
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new CommandKey(this.Key!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsExecutiveKey"]/*'/>*/
    public ExecutiveKey? AsExecutiveKey()
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new ExecutiveKey(this.Key!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsHostKey"]/*'/>*/
    public HostKey? AsHostKey()
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new HostKey(this.Key!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsManagerKey"]/*'/>*/
    public ManagerKey? AsManagerKey()
    {
        if(this.ValidMaterial(false,true) is false) { return null; }

        try { return new ManagerKey(this.Certificate!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsMyHostKey"]/*'/>*/
    public MyHostKey? AsMyHostKey()
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new MyHostKey(this.Key!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsOwnerKey"]/*'/>*/
    public OwnerKey? AsOwnerKey()
    {
        if(this.ValidMaterial(false,true) is false) { return null; }

        try { return new OwnerKey(this.Certificate!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsServiceKey"]/*'/>*/
    public ServiceKey? AsServiceKey()
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new ServiceKey(this.Key!,this.ID!.Value); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AsTokenKey"]/*'/>*/
    public TokenKey? AsTokenKey(TokenKeyType type)
    {
        if(this.ValidMaterial(true,false) is false) { return null; }

        try { return new TokenKey(this.Key!,this.ID!.Value,type); }

        catch { return null; } finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="Create"]/*'/>*/
    public static KeyMaker Create() => new KeyMaker();

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="CreateKeyMaterial"]/*'/>*/
    private KeyMaterial CreateKeyMaterial()
    {
        return new(this.ID,this.Key,this.Certificate);
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="Reset"]/*'/>*/
    public KeyMaker Reset()
    {
        if(this.Key is not null)
        {
            ZeroMemory(this.Key); this.Key = null;
        }

        this.Certificate?.Dispose(); this.Certificate = null; this.ID = null; return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="NormalizeRequestedKeyType"]/*'/>*/
    private static String? NormalizeRequestedKeyType(String? requestedkeytype)
    {
        return String.IsNullOrWhiteSpace(requestedkeytype) ? null : requestedkeytype.Trim();
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="Register"]/*'/>*/
    public KeyMaker Register(String? requestedkeytype , Func<KeyMaterial,AccessKey?> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        String? normalized = NormalizeRequestedKeyType(requestedkeytype); if(normalized is null) { return this; }

        this.Factories[normalized] = factory; return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="RegisterAlias"]/*'/>*/
    public KeyMaker RegisterAlias(String? alias , String? requestedkeytype)
    {
        String? normalizedalias = NormalizeRequestedKeyType(alias); if(normalizedalias is null) { return this; }

        String? normalizedtype = ResolveRequestedKeyType(requestedkeytype) ?? NormalizeRequestedKeyType(requestedkeytype); if(normalizedtype is null) { return this; }

        this.Aliases[normalizedalias] = normalizedtype; return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="ResolveRequestedKeyType"]/*'/>*/
    private String? ResolveRequestedKeyType(String? requestedkeytype)
    {
        String? normalized = NormalizeRequestedKeyType(requestedkeytype); if(normalized is null) { return null; }

        return this.Aliases.TryGetValue(normalized,out String? canonical) ? canonical : normalized;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="TryMake"]/*'/>*/
    public Boolean TryMake(String? requestedkeytype , out AccessKey? key)
    {
        key = null;

        try
        {
            String? resolved = ResolveRequestedKeyType(requestedkeytype); if(resolved is null) { return false; }

            if(!this.Factories.TryGetValue(resolved,out Func<KeyMaterial,AccessKey?>? factory)) { return false; }

            key = factory(CreateKeyMaterial()); return key is not null;
        }
        catch { key = null; return false; }

        finally { this.Reset(); }
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="ValidMaterial"]/*'/>*/
    private Boolean ValidMaterial(Boolean needskey , Boolean needscertificate)
    {
        if(this.ID is null) { return false; }

        if(needscertificate && this.Certificate is null) { return false; }

        if(needskey)
        {
            if(this.Key is null) { return false; }

            if(this.Key.Length < 2048 || this.Key.Length > 16384) { return false; }

            if(this.Key.All(b => b == 0)) { return false; }
        }

        return true;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithCertificateKey"]/*'/>*/
    public KeyMaker WithCertificateKey(X509Certificate2 certificate)
    {
        this.Certificate = DeserializeCertificate(SerializeCertificate(certificate)); return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithCertificateKeyName"]/*'/>*/
    public KeyMaker WithCertificateKeyName(StoreLocation location , StoreName store , String name)
    {
        try
        {
            using X509Store s = new(store,location); s.Open(OpenFlags.ReadOnly);

            this.Certificate = s.Certificates.Find(X509FindType.FindBySubjectName,name,false).FirstOrDefault();
        }
        catch { this.Certificate = null; } return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithCertificateKeyThumb"]/*'/>*/
    public KeyMaker WithCertificateKeyThumb(StoreLocation location , StoreName store , String thumbprint)
    {
        try
        {
            using X509Store s = new(store,location); s.Open(OpenFlags.ReadOnly);

            this.Certificate = s.Certificates.Find(X509FindType.FindByThumbprint,thumbprint,false).FirstOrDefault();
        }
        catch { this.Certificate = null; } return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="AutoID"]/*'/>*/
    public KeyMaker AutoID() { this.ID = Guid.NewGuid(); return this; }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithID"]/*'/>*/
    public KeyMaker WithID(Guid id) { this.ID = id; return this; }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithKey"]/*'/>*/
    public KeyMaker WithKey(ReadOnlySpan<Byte> key)
    {
        this.Key = key.ToArray(); return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithOwnedKey"]/*'/>*/
    internal KeyMaker WithOwnedKey(Byte[]? key)
    {
        if(!ReferenceEquals(this.Key,key) && this.Key is not null)
        {
            ZeroMemory(this.Key);
        }

        this.Key = key; return this;
    }

    /**<include file='KeyMaker.xml' path='KeyMaker/class[@name="KeyMaker"]/method[@name="WithRandomKey"]/*'/>*/
    public KeyMaker WithRandomKey(Int32 size = 4096)
    {
        if(size < 2048 || size > 16384) { return this; }

        this.Key = new Byte[size]; RandomNumberGenerator.Fill(this.Key); return this;
    }
}