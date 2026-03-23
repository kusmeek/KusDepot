namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable]
public class DataPortExam
{
    [Test]
    public async Task Export()
    {
        using RandomNumberGenerator _0 = RandomNumberGenerator.Create();

        Byte[] _1 = new Byte[1024]; _0.GetBytes(_1);

        String _2 = TestCaseDataGenerator.GenerateUnicodeString(1024);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\TextItem.txt";

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse(); 

        Check.That(await DataPort.Export(new TextItem(_2),_3)).IsTrue();

        Check.That(File.Exists(_3)).IsTrue();

        TextItem? _t = (TextItem?)await DataPort.Import(_3,i => i.SetName("Name"));

        Check.That(_t?.GetContent()).IsEqualTo(_2); Check.That(_t?.GetName()).IsEqualTo("Name");

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse();

        String _4 = Environment.GetEnvironmentVariable("TEMP") + "\\CodeItem.cpp";

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse(); 

        Check.That(await DataPort.Export(new CodeItem(_2),_4)).IsTrue();

        Check.That(File.Exists(_4)).IsTrue();

        CodeItem? _c = (CodeItem?)await DataPort.Import(_4);

        Check.That(_c?.GetContent()).IsEqualTo(_2);

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse();

        String _5 = Environment.GetEnvironmentVariable("TEMP") + "\\MultiMediaItem.mp4";

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse(); 

        Check.That(await DataPort.Export(new MultiMediaItem(_1),_5)).IsTrue();

        Check.That(File.Exists(_5)).IsTrue();

        MultiMediaItem? _m = (MultiMediaItem?)await DataPort.Import(_5);

        Check.That(_m?.GetContent()).IsEqualTo(_1);

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse();

        String _6 = Environment.GetEnvironmentVariable("TEMP") + "\\BinaryItem.iso";

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse(); 

        Check.That(await DataPort.Export(new BinaryItem(_1),_6)).IsTrue();

        Check.That(File.Exists(_6)).IsTrue();

        BinaryItem? _b = (BinaryItem?)await DataPort.Import(_6);

        Check.That(_b?.GetContent()).IsEqualTo(_1);

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse();

        String _7 = Environment.GetEnvironmentVariable("TEMP") + "\\GuidReferenceItem2.guid";

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse(); 

        Guid _g = Guid.NewGuid();

        Check.That(await DataPort.Export(new GuidReferenceItem(_g),_7)).IsTrue();

        Check.That(File.Exists(_7)).IsTrue();

        GuidReferenceItem? _gri = (GuidReferenceItem?)await DataPort.Import(_7);

        Check.That(_gri?.GetContent()).IsEqualTo(_g);

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse();
    }

    [Test]
    public async Task Import()
    {
        using RandomNumberGenerator _0 = RandomNumberGenerator.Create();

        Byte[] _1 = new Byte[1024]; _0.GetBytes(_1);

        String _2 = TestCaseDataGenerator.GenerateUnicodeString(1024);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\TextItem2.txt";

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse(); File.WriteAllBytes(_3,_2.ToByteArrayFromUTF16String());

        TextItem? _t = (TextItem?)await DataPort.Import(_3);

        Check.That(_t?.GetContent()).IsEqualTo(_2);

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse();

        String _4 = Environment.GetEnvironmentVariable("TEMP") + "\\CodeItem.c";

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse(); File.WriteAllBytes(_4,_2.ToByteArrayFromUTF16String());

        CodeItem ? _c = (CodeItem?)await DataPort.Import(_4);

        Check.That(_c?.GetContent()).IsEqualTo(_2);

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse();

        String _5 = Environment.GetEnvironmentVariable("TEMP") + "\\MultiMediaItem.mp3";

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse(); File.WriteAllBytes(_5,_1);

        MultiMediaItem? _m = (MultiMediaItem?)await DataPort.Import(_5);

        Check.That(_m?.GetContent()).IsEqualTo(_1);

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse();

        String _6 = Environment.GetEnvironmentVariable("TEMP") + "\\BLOB197346825";

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse(); File.WriteAllBytes(_6,_1);

        BinaryItem? _b = (BinaryItem?)await DataPort.Import(_6);

        Check.That(_b?.GetContent()).IsEqualTo(_1);

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse();

        String _7 = Environment.GetEnvironmentVariable("TEMP") + "\\GuidReferenceItem4.guid";

        Guid _g = Guid.NewGuid();

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse(); File.WriteAllBytes(_7,_g.ToByteArray());

        GuidReferenceItem? _gr = (GuidReferenceItem?)await DataPort.Import(_7);

        Check.That(_gr?.GetContent()).IsEqualTo(_g);

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse();
    }
}