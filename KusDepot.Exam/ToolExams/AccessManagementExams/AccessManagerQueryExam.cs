namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessManagerQueryExam
{
    [Test]
    public void AccessManagerQuery_Create_Configures_All_Values()
    {
        AccessManagerQuery query = AccessManagerQuery.Create(
            subjects:["subject-a"],
            protectedoperations:[ProtectedOperation.GetOutputIDs],
            audiences:["aud-a"],
            scopes:["scope-a"],
            includeexpired:false);

        Check.That(query.Subjects).IsNotNull();
        Check.That(query.Subjects!.Value.SequenceEqual(["subject-a"])).IsTrue();
        Check.That(query.ProtectedOperations).IsNotNull();
        Check.That(query.ProtectedOperations!.Value.SequenceEqual([ProtectedOperation.GetOutputIDs])).IsTrue();
        Check.That(query.Audiences).IsNotNull();
        Check.That(query.Audiences!.Value.SequenceEqual(["aud-a"])).IsTrue();
        Check.That(query.Scopes).IsNotNull();
        Check.That(query.Scopes!.Value.SequenceEqual(["scope-a"])).IsTrue();
        Check.That(query.IncludeExpired).IsFalse();
    }

    [Test]
    public void AccessManagerQuery_SetSubjects()
    {
        AccessManagerQuery query = AccessManagerQuery.Create();

        AccessManagerQuery updated = query.SetSubjects(["subject-a"]);

        Check.That(updated).IsSameReferenceAs(query);
        Check.That(updated.Subjects).IsNotNull();
        Check.That(updated.Subjects!.Value.SequenceEqual(["subject-a"])).IsTrue();
    }

    [Test]
    public void AccessManagerQuery_SetProtectedOperations()
    {
        AccessManagerQuery query = AccessManagerQuery.Create();

        AccessManagerQuery updated = query.SetProtectedOperations([ProtectedOperation.GetOutputIDs]);

        Check.That(updated).IsSameReferenceAs(query);
        Check.That(updated.ProtectedOperations).IsNotNull();
        Check.That(updated.ProtectedOperations!.Value.SequenceEqual([ProtectedOperation.GetOutputIDs])).IsTrue();
    }

    [Test]
    public void AccessManagerQuery_SetAudiences()
    {
        AccessManagerQuery query = AccessManagerQuery.Create();

        AccessManagerQuery updated = query.SetAudiences(["aud-a"]);

        Check.That(updated).IsSameReferenceAs(query);
        Check.That(updated.Audiences).IsNotNull();
        Check.That(updated.Audiences!.Value.SequenceEqual(["aud-a"])).IsTrue();
    }

    [Test]
    public void AccessManagerQuery_SetScopes()
    {
        AccessManagerQuery query = AccessManagerQuery.Create();

        AccessManagerQuery updated = query.SetScopes(["scope-a"]);

        Check.That(updated).IsSameReferenceAs(query);
        Check.That(updated.Scopes).IsNotNull();
        Check.That(updated.Scopes!.Value.SequenceEqual(["scope-a"])).IsTrue();
    }

    [Test]
    public void AccessManagerQuery_SetIncludeExpired()
    {
        AccessManagerQuery query = AccessManagerQuery.Create();

        AccessManagerQuery updated = query.SetIncludeExpired(false);

        Check.That(updated).IsSameReferenceAs(query);
        Check.That(updated.IncludeExpired).IsFalse();
    }

}
