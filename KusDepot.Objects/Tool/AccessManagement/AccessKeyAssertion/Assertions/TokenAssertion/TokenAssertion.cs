namespace KusDepot.Security.Assertions;

/**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Assertions.TokenAssertion")] [Immutable]

public sealed record class TokenAssertion : AccessKeyAssertion
{
    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String> Audiences { get; init; } = [];

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="AuthenticationContextClassReference"]/*'/>*/
    [JsonPropertyName("AuthenticationContextClassReference")] [JsonRequired] [Id(1)]
    public String? AuthenticationContextClassReference { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="AuthenticationMethods"]/*'/>*/
    [JsonPropertyName("AuthenticationMethods")] [JsonRequired] [Id(2)]
    public ImmutableArray<String> AuthenticationMethods { get; init; } = [];

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="AuthenticationTime"]/*'/>*/
    [JsonPropertyName("AuthenticationTime")] [JsonRequired] [Id(3)]
    public DateTimeOffset? AuthenticationTime { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="AuthorizedParty"]/*'/>*/
    [JsonPropertyName("AuthorizedParty")] [JsonRequired] [Id(4)]
    public String? AuthorizedParty { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="ClientID"]/*'/>*/
    [JsonPropertyName("ClientID")] [JsonRequired] [Id(5)]
    public String? ClientID { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="IssuedAt"]/*'/>*/
    [JsonPropertyName("IssuedAt")] [JsonRequired] [Id(6)]
    public DateTimeOffset? IssuedAt { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="Issuer"]/*'/>*/
    [JsonPropertyName("Issuer")] [JsonRequired] [Id(7)]
    public String? Issuer { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="JwtID"]/*'/>*/
    [JsonPropertyName("JwtID")] [JsonRequired] [Id(8)]
    public String? JwtID { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="NotAfter"]/*'/>*/
    [JsonPropertyName("NotAfter")] [JsonRequired] [Id(9)]
    public DateTimeOffset? NotAfter { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="NotBefore"]/*'/>*/
    [JsonPropertyName("NotBefore")] [JsonRequired] [Id(10)]
    public DateTimeOffset? NotBefore { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="ObjectID"]/*'/>*/
    [JsonPropertyName("ObjectID")] [JsonRequired] [Id(11)]
    public String? ObjectID { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(12)]
    public ImmutableArray<String> Scopes { get; init; } = [];

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonRequired] [Id(13)]
    public String? SessionID { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(14)]
    public String? Subject { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="TenantID"]/*'/>*/
    [JsonPropertyName("TenantID")] [JsonRequired] [Id(15)]
    public String? TenantID { get; init; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="Token"]/*'/>*/
    [JsonPropertyName("Token")] [JsonRequired] [Id(16)]
    public Byte[] Token
    {
        get => token.CloneByteArray();

        set => token = token.Length == 0 ? value?.CloneByteArray() ?? Array.Empty<Byte>() : token;
    }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/field[@name="token"]/*'/>*/
    [NonSerialized]
    private Byte[] token = Array.Empty<Byte>();

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="TokenType"]/*'/>*/
    [JsonPropertyName("TokenType")] [JsonRequired] [Id(17)]
    public TokenKeyType TokenType { get; init; }

    static TokenAssertion() { RegisterDeserializer(SerializationIdentifierValue,DeserializePayload); }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public TokenAssertion() { AssertionType = "token"; }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/property[@name="SerializationIdentifier"]/*'/>*/
    protected override String SerializationIdentifier => SerializationIdentifierValue;

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/field[@name="SerializationIdentifierValue"]/*'/>*/
    private const String SerializationIdentifierValue = "kusdepot.security.assertions.token/v1";

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/method[@name="Copy"]/*'/>*/
    public TokenAssertion Copy() => this with { Token = this.Token };

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/method[@name="HasAuthenticationMethod"]/*'/>*/
    public Boolean HasAuthenticationMethod(String? authenticationmethod)
    {
        return String.IsNullOrWhiteSpace(authenticationmethod) is false && this.AuthenticationMethods.Contains(authenticationmethod,StringComparer.Ordinal);
    }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/method[@name="IsExpiredAt"]/*'/>*/
    public Boolean IsExpiredAt(DateTimeOffset when)
    {
        return this.NotAfter is not null && when > this.NotAfter.Value;
    }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/method[@name="SerializePayload"]/*'/>*/
    protected override Byte[] SerializePayload()
    {
        return DataContractUtility.Serialize(new TokenAssertionPayloadContract()
        {
            AuthenticationContextClassReference = this.AuthenticationContextClassReference,
            Audiences = this.Audiences.ToArray(),
            AuthenticationMethods = this.AuthenticationMethods.ToArray(),
            AuthenticationTime = this.AuthenticationTime,
            AuthorizedParty = this.AuthorizedParty,
            ClientID = this.ClientID,
            Format = this.Format,
            IssuedAt = this.IssuedAt,
            Issuer = this.Issuer,
            JwtID = this.JwtID,
            NotAfter = this.NotAfter,
            NotBefore = this.NotBefore,
            ObjectID = this.ObjectID,
            Scopes = this.Scopes.ToArray(),
            SessionID = this.SessionID,
            Subject = this.Subject,
            TenantID = this.TenantID,
            Token = this.Token,
            TokenType = this.TokenType
        });
    }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/method[@name="ToSummary"]/*'/>*/
    public override AccessKeyAssertionSummary ToSummary()
    {
        return new TokenAssertionSummary()
        {
            AssertionType = this.AssertionType,
            AuthenticationContextClassReference = this.AuthenticationContextClassReference,
            AuthenticationMethods = this.AuthenticationMethods,
            AuthenticationTime = this.AuthenticationTime,
            Audiences = this.Audiences,
            AuthorizedParty = this.AuthorizedParty,
            ClientID = this.ClientID,
            Format = this.Format,
            IssuedAt = this.IssuedAt,
            Issuer = this.Issuer,
            JwtID = this.JwtID,
            NotAfter = this.NotAfter,
            NotBefore = this.NotBefore,
            ObjectID = this.ObjectID,
            Scopes = this.Scopes,
            SessionID = this.SessionID,
            Subject = this.Subject,
            TenantID = this.TenantID
        };
    }

    /**<include file='TokenAssertion.xml' path='TokenAssertion/record[@name="TokenAssertion"]/method[@name="DeserializePayload"]/*'/>*/
    private static AccessKeyAssertion? DeserializePayload(ReadOnlyMemory<Byte> payload)
    {
        try
        {
            TokenAssertionPayloadContract? contract = DataContractUtility.Deserialize<TokenAssertionPayloadContract>(payload.ToArray()); if(contract is null) { return null; }

            return new TokenAssertion()
            {
                AuthenticationContextClassReference = contract.AuthenticationContextClassReference,
                Audiences = contract.Audiences.ToImmutableArray(),
                AuthenticationMethods = contract.AuthenticationMethods.ToImmutableArray(),
                AuthenticationTime = contract.AuthenticationTime,
                AuthorizedParty = contract.AuthorizedParty,
                ClientID = contract.ClientID,
                Format = contract.Format,
                IssuedAt = contract.IssuedAt,
                Issuer = contract.Issuer,
                JwtID = contract.JwtID,
                NotAfter = contract.NotAfter,
                NotBefore = contract.NotBefore,
                ObjectID = contract.ObjectID,
                Scopes = contract.Scopes.ToImmutableArray(),
                SessionID = contract.SessionID,
                Subject = contract.Subject,
                TenantID = contract.TenantID,
                Token = contract.Token,
                TokenType = contract.TokenType
            };
        }
        catch { return null; }
    }

    [DataContract(Name = "TokenAssertionPayloadContract" , Namespace = "KusDepot.Security.Assertions")]
    private sealed record class TokenAssertionPayloadContract
    {
        [DataMember(Name = "Audiences" , EmitDefaultValue = true , IsRequired = true)]
        public String[] Audiences { get; init; } = Array.Empty<String>();

        [DataMember(Name = "AuthenticationContextClassReference" , EmitDefaultValue = true , IsRequired = true)]
        public String? AuthenticationContextClassReference { get; init; }

        [DataMember(Name = "AuthenticationMethods" , EmitDefaultValue = true , IsRequired = true)]
        public String[] AuthenticationMethods { get; init; } = Array.Empty<String>();

        [DataMember(Name = "AuthenticationTime" , EmitDefaultValue = true , IsRequired = true)]
        public DateTimeOffset? AuthenticationTime { get; init; }

        [DataMember(Name = "AuthorizedParty" , EmitDefaultValue = true , IsRequired = true)]
        public String? AuthorizedParty { get; init; }

        [DataMember(Name = "ClientID" , EmitDefaultValue = true , IsRequired = true)]
        public String? ClientID { get; init; }

        [DataMember(Name = "Format" , EmitDefaultValue = true , IsRequired = true)]
        public String Format { get; init; } = String.Empty;

        [DataMember(Name = "IssuedAt" , EmitDefaultValue = true , IsRequired = true)]
        public DateTimeOffset? IssuedAt { get; init; }

        [DataMember(Name = "Issuer" , EmitDefaultValue = true , IsRequired = true)]
        public String? Issuer { get; init; }

        [DataMember(Name = "JwtID" , EmitDefaultValue = true , IsRequired = true)]
        public String? JwtID { get; init; }

        [DataMember(Name = "NotAfter" , EmitDefaultValue = true , IsRequired = true)]
        public DateTimeOffset? NotAfter { get; init; }

        [DataMember(Name = "NotBefore" , EmitDefaultValue = true , IsRequired = true)]
        public DateTimeOffset? NotBefore { get; init; }

        [DataMember(Name = "ObjectID" , EmitDefaultValue = true , IsRequired = true)]
        public String? ObjectID { get; init; }

        [DataMember(Name = "Scopes" , EmitDefaultValue = true , IsRequired = true)]
        public String[] Scopes { get; init; } = Array.Empty<String>();

        [DataMember(Name = "SessionID" , EmitDefaultValue = true , IsRequired = true)]
        public String? SessionID { get; init; }

        [DataMember(Name = "Subject" , EmitDefaultValue = true , IsRequired = true)]
        public String? Subject { get; init; }

        [DataMember(Name = "TenantID" , EmitDefaultValue = true , IsRequired = true)]
        public String? TenantID { get; init; }

        [DataMember(Name = "Token" , EmitDefaultValue = true , IsRequired = true)]
        public Byte[] Token { get; init; } = Array.Empty<Byte>();

        [DataMember(Name = "TokenType" , EmitDefaultValue = true , IsRequired = true)]
        public TokenKeyType TokenType { get; init; }
    }
}