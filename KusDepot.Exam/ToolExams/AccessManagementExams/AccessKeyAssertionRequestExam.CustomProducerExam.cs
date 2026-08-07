namespace KusDepot.Exams.Security;

public partial class AccessKeyAssertionRequestExam
{
    private static (String ToolSchemaID,String AccessKeyRealmID) GetManifestIdentityForCustomExam(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)!;
        Object identity = field.GetValue(manager)!;
        return (
            identity.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String ?? String.Empty,
            identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String ?? String.Empty);
    }

    private static Byte[] GetRealmKeyForCustomExam(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (Byte[])field.GetValue(manager)!;
    }

    private sealed class CustomAssertionGenerator : IAccessKeyAssertionGenerator
    {
        public AccessKeyAssertionGeneratorContext? LastContext { get; private set; }

        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            LastContext = context;

            return [new CustomAccessKeyAssertion()
            {
                Format = "exam/custom/v1",
                Value = $"{context.Subject}:{context.RequestedKeyType}"
            }];
        }
    }

    private sealed record class CustomAccessKeyAssertion : AccessKeyAssertion
    {
        private static readonly Boolean Registered = EnsureRegistered();

        public CustomAccessKeyAssertion()
        {
            AssertionType = "custom"; _ = Registered;
        }

        public String Value { get; init; } = String.Empty;

        private const String SerializationIdentifierValue = "kusdepot.exam.custom-assertion/v1";

        protected override String SerializationIdentifier => SerializationIdentifierValue;

        public override AccessKeyAssertionSummary ToSummary()
        {
            return new CustomAccessKeyAssertionSummary()
            {
                AssertionType = this.AssertionType,
                Format = this.Format,
                Value = this.Value
            };
        }

        protected override Byte[] SerializePayload()
        {
            _ = Registered;

            return DataContractUtility.Serialize(new CustomAccessKeyAssertionPayloadContract()
            {
                AssertionType = this.AssertionType,
                Format = this.Format,
                Value = this.Value
            });
        }

        private static Boolean EnsureRegistered()
        {
            return RegisterDeserializer(SerializationIdentifierValue,DeserializePayload);
        }

        private static AccessKeyAssertion? DeserializePayload(ReadOnlyMemory<Byte> payload)
        {
            try
            {
                CustomAccessKeyAssertionPayloadContract? contract = DataContractUtility.Deserialize<CustomAccessKeyAssertionPayloadContract>(payload.ToArray()); if(contract is null) { return null; }

                return new CustomAccessKeyAssertion()
                {
                    AssertionType = contract.AssertionType,
                    Format = contract.Format,
                    Value = contract.Value
                };
            }
            catch { return null; }
        }

        [DataContract(Name = "CustomAccessKeyAssertionPayloadContract" , Namespace = "KusDepot.Exams.Security")]
        private sealed record class CustomAccessKeyAssertionPayloadContract
        {
            [DataMember(Name = "AssertionType" , EmitDefaultValue = true , IsRequired = true)]
            public String AssertionType { get; init; } = String.Empty;

            [DataMember(Name = "Format" , EmitDefaultValue = true , IsRequired = true)]
            public String Format { get; init; } = String.Empty;

            [DataMember(Name = "Value" , EmitDefaultValue = true , IsRequired = true)]
            public String Value { get; init; } = String.Empty;
        }
    }

    private sealed record class CustomAccessKeyAssertionSummary : AccessKeyAssertionSummary
    {
        public String Value { get; init; } = String.Empty;
    }

    [Test]
    public void GenerateAccessKey_WithCustomGeneratorAndCustomAssertion_PersistsAndRecoversFullAssertionFlow()
    {
        Tool tool = new();
        CustomAssertionGenerator generator = new();
        AccessManagerOptions options = new(keyassertiongenerator: new DefaultAccessKeyAssertionGenerator([generator]));
        AccessManager manager = new(options,tool);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "generator-subject",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Check.That(generator.LastContext).IsNotNull();
        Check.That(generator.LastContext!.Value.Subject).IsEqualTo("generator-subject");
        Check.That(generator.LastContext!.Value.RequestedKeyType).IsEqualTo(nameof(ClientKey));

        Byte[] realmkey = GetRealmKeyForCustomExam(manager);
        var identity = GetManifestIdentityForCustomExam(manager);

        Check.That(AccessKeySecret.TryReadAssertions(key!.Key!,realmkey,identity.ToolSchemaID,identity.AccessKeyRealmID,out ImmutableArray<AccessKeyAssertion> assertions)).IsTrue();
        Check.That(assertions.Length).IsEqualTo(1);
        Check.That(assertions[0]).IsInstanceOf<CustomAccessKeyAssertion>();

        CustomAccessKeyAssertion assertion = (CustomAccessKeyAssertion)assertions[0];
        Check.That(assertion.AssertionType).IsEqualTo("custom");
        Check.That(assertion.Format).IsEqualTo("exam/custom/v1");
        Check.That(assertion.Value).IsEqualTo("generator-subject:ClientKey");

        Check.That(AccessKeySecret.TryReadClaims(key.Key!,realmkey,identity.ToolSchemaID,identity.AccessKeyRealmID,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.AssertionSummaries.Length).IsEqualTo(1);
        Check.That(claims.AssertionSummaries[0]).IsInstanceOf<CustomAccessKeyAssertionSummary>();

        CustomAccessKeyAssertionSummary summary = (CustomAccessKeyAssertionSummary)claims.AssertionSummaries[0];
        Check.That(summary.AssertionType).IsEqualTo("custom");
        Check.That(summary.Format).IsEqualTo("exam/custom/v1");
        Check.That(summary.Value).IsEqualTo("generator-subject:ClientKey");
    }
}
