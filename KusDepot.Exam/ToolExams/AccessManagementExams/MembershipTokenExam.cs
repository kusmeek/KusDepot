namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class MembershipTokenExam
{
    private static Type MembershipTokenType => typeof(AccessKeySecret).Assembly.GetType("KusDepot.Security.MembershipToken",throwOnError: true)!;

    private static Object CreateMembershipToken(Byte[]? permissions = null , DateTimeOffset? notafter = null , String? toolschemaid = null , String? manifesthash = null ,
        String? accesskeyrealmid = null , Guid issuertoolid = default , Guid issueraccessmanagerid = default , ImmutableArray<String> audiences = default , ImmutableArray<String> scopes = default)
    {
        ConstructorInfo ctor = MembershipTokenType.GetConstructor([
            typeof(String),
            typeof(AccessKeyToken),
            typeof(Byte[]),
            typeof(DateTimeOffset?),
            typeof(String),
            typeof(String),
            typeof(String),
            typeof(Guid),
            typeof(Guid),
            typeof(ImmutableArray<String>),
            typeof(ImmutableArray<String>),
            typeof(ImmutableArray<AccessKeyAssertionSummary>)])!;

        return ctor.Invoke([
            "subject",
            default(AccessKeyToken),
            permissions!,
            notafter,
            toolschemaid,
            manifesthash,
            accesskeyrealmid,
            issuertoolid,
            issueraccessmanagerid,
            audiences,
            scopes,
            default(ImmutableArray<AccessKeyAssertionSummary>)]);
    }

    [Test]
    public void MembershipTokenAllows_Uses_FullBitmap()
    {
        Byte[] bitmap = new Byte[32];
        BinaryPrimitives.WriteUInt128BigEndian(bitmap.AsSpan(0,16),UInt128.One << 5);
        BinaryPrimitives.WriteUInt128BigEndian(bitmap.AsSpan(16,16),UInt128.One << 7);
        Object membership = CreateMembershipToken(bitmap);

        MethodInfo allows = MembershipTokenType.GetMethod("Allows")!;

        Check.That((Boolean)allows.Invoke(membership,[5])!).IsTrue();
        Check.That((Boolean)allows.Invoke(membership,[135])!).IsTrue();
        Check.That((Boolean)allows.Invoke(membership,[6])!).IsFalse();
        Check.That((Boolean)allows.Invoke(membership,[300])!).IsFalse();
    }

    [Test]
    public void MembershipTokenConstructor_Caches_SchemaIdentity()
    {
        DateTimeOffset notAfter = DateTimeOffset.Now.AddMinutes(5);
        Object membership = CreateMembershipToken(Array.Empty<Byte>(),notAfter,"KusDepot.Tools/Schema","HASH-ABC","Realm-A",Guid.NewGuid(),Guid.NewGuid(),["aud-a","aud-b"],["scope.a"]);

        Check.That(MembershipTokenType.GetProperty("Subject")!.GetValue(membership) as String).IsEqualTo("subject");
        Check.That((DateTimeOffset?)MembershipTokenType.GetProperty("NotAfter")!.GetValue(membership)).IsEqualTo(notAfter);
        Check.That(MembershipTokenType.GetProperty("ToolSchemaID")!.GetValue(membership) as String).IsEqualTo("KusDepot.Tools/Schema");
        Check.That(MembershipTokenType.GetProperty("ManifestHash")!.GetValue(membership) as String).IsEqualTo("HASH-ABC");
        Check.That(MembershipTokenType.GetProperty("AccessKeyRealmID")!.GetValue(membership) as String).IsEqualTo("Realm-A");
        Check.That(((ImmutableArray<String>)MembershipTokenType.GetProperty("Audiences")!.GetValue(membership)!).SequenceEqual(["aud-a","aud-b"])).IsTrue();
        Check.That(((ImmutableArray<String>)MembershipTokenType.GetProperty("Scopes")!.GetValue(membership)!).SequenceEqual(["scope.a"])).IsTrue();
    }

    [Test]
    public void MembershipTokenConstructor_NullSchemaIdentity_Uses_EmptyStrings()
    {
        Object membership = CreateMembershipToken();

        Check.That(MembershipTokenType.GetProperty("AccessKeyRealmID")!.GetValue(membership) as String).IsEqualTo(String.Empty);
        Check.That(((ImmutableArray<String>)MembershipTokenType.GetProperty("Audiences")!.GetValue(membership)!).IsEmpty).IsTrue();
        Check.That(((ImmutableArray<String>)MembershipTokenType.GetProperty("Scopes")!.GetValue(membership)!).IsEmpty).IsTrue();
        Check.That(MembershipTokenType.GetProperty("ToolSchemaID")!.GetValue(membership) as String).IsEqualTo(String.Empty);
        Check.That(MembershipTokenType.GetProperty("ManifestHash")!.GetValue(membership) as String).IsEqualTo(String.Empty);
    }
}
