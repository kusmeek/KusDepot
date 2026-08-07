namespace KusDepot.Security;

/**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessManagerQuery")]

public sealed class AccessManagerQuery
{
    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String>? Audiences { get; set; }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/property[@name="IncludeExpired"]/*'/>*/
    [JsonPropertyName("IncludeExpired")] [JsonRequired] [Id(1)]
    public Boolean IncludeExpired { get; set; } = true;

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/property[@name="ProtectedOperations"]/*'/>*/
    [JsonPropertyName("ProtectedOperations")] [JsonRequired] [Id(2)]
    public ImmutableArray<Int32>? ProtectedOperations { get; set; }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(3)]
    public ImmutableArray<String>? Scopes { get; set; }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/property[@name="Subjects"]/*'/>*/
    [JsonPropertyName("Subjects")] [JsonRequired] [Id(4)]
    public ImmutableArray<String>? Subjects { get; set; }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/method[@name="Create"]/*'/>*/
    public static AccessManagerQuery Create(IEnumerable<String>? subjects = null , IEnumerable<Int32>? protectedoperations = null , IEnumerable<String>? audiences = null , IEnumerable<String>? scopes = null , Boolean includeexpired = true)
    {
        return new()
        {
            Audiences = audiences?.ToImmutableArray(),
            IncludeExpired = includeexpired,
            ProtectedOperations = protectedoperations?.ToImmutableArray(),
            Scopes = scopes?.ToImmutableArray(),
            Subjects = subjects?.ToImmutableArray()
        };
    }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/method[@name="SetAudiences"]/*'/>*/
    public AccessManagerQuery SetAudiences(IEnumerable<String>? audiences = null)
    {
        this.Audiences = audiences?.ToImmutableArray(); return this;
    }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/method[@name="SetIncludeExpired"]/*'/>*/
    public AccessManagerQuery SetIncludeExpired(Boolean includeexpired = true)
    {
        this.IncludeExpired = includeexpired; return this;
    }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/method[@name="SetProtectedOperations"]/*'/>*/
    public AccessManagerQuery SetProtectedOperations(IEnumerable<Int32>? protectedoperations = null)
    {
        this.ProtectedOperations = protectedoperations?.ToImmutableArray(); return this;
    }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/method[@name="SetScopes"]/*'/>*/
    public AccessManagerQuery SetScopes(IEnumerable<String>? scopes = null)
    {
        this.Scopes = scopes?.ToImmutableArray(); return this;
    }

    /**<include file='AccessManagerQuery.xml' path='AccessManagerQuery/class[@name="AccessManagerQuery"]/method[@name="SetSubjects"]/*'/>*/
    public AccessManagerQuery SetSubjects(IEnumerable<String>? subjects = null)
    {
        this.Subjects = subjects?.ToImmutableArray(); return this;
    }
}