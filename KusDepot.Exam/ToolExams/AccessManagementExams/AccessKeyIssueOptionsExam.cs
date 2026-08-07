namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class AccessKeyIssueOptionsExam
{
    [Test]
    public void AccessKeyIssueOptions_Stores_Configured_Values()
    {
        AccessKeyIssueOptions options = new()
        {
            Subject = "subject-a",
            Operations = [1,7],
            Lifetime = TimeSpan.FromMinutes(5),
            Audiences = ["aud-b","aud-a"],
            Scopes = ["scope.z","scope.a"]
        };

        Check.That(options.Subject).IsEqualTo("subject-a");
        Check.That(options.Operations!.Value.SequenceEqual([1,7])).IsTrue();
        Check.That(options.Lifetime).IsEqualTo(TimeSpan.FromMinutes(5));
        Check.That(options.Audiences!.Value.SequenceEqual(["aud-b","aud-a"])).IsTrue();
        Check.That(options.Scopes!.Value.SequenceEqual(["scope.z","scope.a"])).IsTrue();
    }

    [Test]
    public void AccessKeyIssueOptionsCreate_Stores_Configured_Values()
    {
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            subject: "subject-a",
            operations: [1,7],
            lifetime: TimeSpan.FromMinutes(5),
            audiences: ["aud-b","aud-a"],
            scopes: ["scope.z","scope.a"]);

        Check.That(options.Subject).IsEqualTo("subject-a");
        Check.That(options.Operations!.Value.SequenceEqual([1,7])).IsTrue();
        Check.That(options.Lifetime).IsEqualTo(TimeSpan.FromMinutes(5));
        Check.That(options.Audiences!.Value.SequenceEqual(["aud-b","aud-a"])).IsTrue();
        Check.That(options.Scopes!.Value.SequenceEqual(["scope.z","scope.a"])).IsTrue();
    }

    [Test]
    public void AccessKeyIssueOptions_Defaults_Are_Null()
    {
        AccessKeyIssueOptions options = new();

        Check.That(options.Subject).IsNull();
        Check.That(options.Operations).IsNull();
        Check.That(options.Lifetime).IsNull();
        Check.That(options.Audiences).IsNull();
        Check.That(options.Scopes).IsNull();
    }

    [Test]
    public void AccessKeyIssueOptions_Builder()
    {
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create();
        AccessKeyIssueOptions updated = options
            .AddOperation(1)
            .AddOperation(7)
            .SetLifetime(TimeSpan.FromMinutes(3))
            .AddAudience("aud-a")
            .AddAudience("aud-b")
            .AddScope("scope.a")
            .AddScope("scope.z");

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.Operations!.Value.SequenceEqual([1,7])).IsTrue();
        Check.That(updated.Lifetime).IsEqualTo(TimeSpan.FromMinutes(3));
        Check.That(updated.Audiences!.Value.SequenceEqual(["aud-a","aud-b"])).IsTrue();
        Check.That(updated.Scopes!.Value.SequenceEqual(["scope.a","scope.z"])).IsTrue();
    }

    [Test]
    public void AccessKeyIssueOptions_Setters()
    {
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            subject: "subject-a",
            operations: [1],
            lifetime: TimeSpan.FromMinutes(1),
            audiences: ["aud-a"],
            scopes: ["scope.a"]);
        AccessKeyIssueOptions updated = options
            .SetOperations([2,3])
            .SetAudiences(["aud-b"])
            .SetScopes(["scope.b"])
            .SetLifetime(TimeSpan.FromMinutes(9));

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.Subject).IsEqualTo("subject-a");
        Check.That(updated.Operations!.Value.SequenceEqual([2,3])).IsTrue();
        Check.That(updated.Lifetime).IsEqualTo(TimeSpan.FromMinutes(9));
        Check.That(updated.Audiences!.Value.SequenceEqual(["aud-b"])).IsTrue();
        Check.That(updated.Scopes!.Value.SequenceEqual(["scope.b"])).IsTrue();
    }
}
