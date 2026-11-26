namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class DataContractUtilityExam
{
    [Test]
    public void ToFile_ExistingDestination_IsNotOverwritten()
    {
        String path = Path.Combine(Path.GetTempPath(),$"DataContractUtilityExam-Existing-{Guid.NewGuid():N}.bin");
        Byte[] original = Encoding.UTF8.GetBytes("preserve");
        File.WriteAllBytes(path,original);

        try
        {
            Check.That(DataContractUtility.ToFile(path,"replacement")).IsFalse();
            Check.That(File.ReadAllBytes(path)).IsEqualTo(original);
        }
        finally { File.Delete(path); }
    }

    [Test]
    public void ToFile_Roundtrip()
    {
        String path = Path.Combine(Path.GetTempPath(),$"DataContractUtilityExam-Roundtrip-{Guid.NewGuid():N}.bin");

        try
        {
            Check.That(DataContractUtility.ToFile(path,"roundtrip")).IsTrue();
            Check.That(DataContractUtility.FromFile<String>(path)).IsEqualTo("roundtrip");
        }
        finally { if(File.Exists(path)) { File.Delete(path); } }
    }
}
