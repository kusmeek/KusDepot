namespace KusDepot.Exams
{
    /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]'/> */
    [Parallelizable,TestFixture]
    public class TextItemExam
    {
        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/field[@Name = "ValidDataTypes"]'/> */
        private static readonly String?[] ValidDataTypes = new String?[]
        {
            DataType.ADML,
            DataType.ADMX,
            DataType.AHK,
            DataType.API,
            DataType.ARM,
            DataType.ASM,
            DataType.AU3,
            DataType.BAT,
            DataType.BICEP,
            DataType.C,
            DataType.CFG,
            DataType.CMD,
            DataType.CONFIG,
            DataType.CPP,
            DataType.CS,
            DataType.CSHTML,
            DataType.CSPROJ,
            DataType.CSS,
            DataType.CSV,
            DataType.CUE,
            DataType.DAT,
            DataType.DOCX,
            DataType.FS,
            DataType.GEO,
            DataType.GO,
            DataType.GPX,
            DataType.HTM,
            DataType.HTML,
            DataType.ICS,
            DataType.INF,
            DataType.INI,
            DataType.JS,
            DataType.JSON,
            DataType.KQL,
            DataType.LOG,
            DataType.LUA,
            DataType.MJS,
            DataType.MOF,
            DataType.NFO,
            DataType.PDF,
            DataType.PPTX,
            DataType.PS1,
            DataType.PY,
            DataType.RAZOR,
            DataType.REG,
            DataType.RFC,
            DataType.RS,
            DataType.RTF,
            DataType.SGML,
            DataType.SH,
            DataType.SLN,
            DataType.SQL,
            DataType.TS,
            DataType.TXT,
            DataType.VB,
            DataType.VSD,
            DataType.WAT,
            DataType.XAML,
            DataType.XHTML,
            DataType.XLSX,
            DataType.XML,
            DataType.YAML,
            DataType.YML
        };

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "IComparable{TextItem}.CompareTo"]'/> */
        [Test]
        public void CompareTo()
        {
            TextItem _0 = new TextItem();
            Thread.Sleep(100);
            TextItem _1 = new TextItem();

            Check.That(new TextItem().CompareTo(null)).IsEqualTo(1);
            Check.That(_0.CompareTo(_0)).IsEqualTo(0);
            Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
            Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            StringBuilder _0 = new StringBuilder();
            Random _1 = new Random();
            Byte[] _2 = new Byte[8192];
            _1.NextBytes(_2);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            String? _3 = ValidDataTypes[_1.Next(0,ValidDataTypes.Length-1)];
            TextItem _4 = new TextItem(_0.ToString(),new Guid(),Language.eng,new List<String>{},new List<String>{},_3!.ToString());

            Check.That(_4).IsInstanceOfType(typeof(TextItem));
            Check.That(_4.Content).IsEqualTo(_0.ToString());
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "EqualsObject"]'/> */
        [Test]
        public void EqualsObject()
        {
            Check.That((Object)new TextItem().Equals(null)).IsEqualTo(false);
            Check.That((Object)new TextItem().Equals(new Object())).IsEqualTo(false);
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "IEquatable{TextItem}.Equals"]'/> */
        [Test]
        public void EqualsInterface()
        {
            StringBuilder _0 = new StringBuilder();
            StringBuilder _6 = new StringBuilder();
            Random _1 = new Random();
            Random _7 = new Random();
            Byte[] _2 = new Byte[8192];
            Byte[] _8 = new Byte[8192];
            Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
            Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
            _1.NextBytes(_2);
            _7.NextBytes(_8);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            foreach (Byte b in _8) {_6.Append(Convert.ToChar(b));}
            TextItem _3 = new TextItem(_0.ToString(),_11,null,null,null,null);
            TextItem _15 = new TextItem(_0.ToString(),_11,null,null,null,null);
            TextItem _4 = new TextItem(_0.ToString(),_14,null,null,null,null);
            TextItem _5 = new TextItem(_0.ToString(),null,null,null,null,null);
            TextItem _9 = new TextItem(_6.ToString(),_11,null,null,null,null);
            TextItem _10 = new TextItem(_6.ToString(),_14,null,null,null,null);

            Check.That(new TextItem().Equals(null)).IsEqualTo(false);
            Check.That(new TextItem().Equals(new Object())).IsEqualTo(false);
            Check.That(_3.Equals(_4)).IsEqualTo(false);
            Check.That(_3.Equals(_5)).IsEqualTo(false);
            Check.That(_3.Equals(_9)).IsEqualTo(false);
            Check.That(_3.Equals(_15)).IsTrue();
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            TextItem _0 = new TextItem(null,new Guid(),null,null,null,null);
            TextItem _1 = new TextItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null,null);
            TextItem _2 = new TextItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null,null);

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid();
            TextItem _1 = new TextItem();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.AssemblyVersion).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsNotNull();
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {
            TextItem _0 = new TextItem();

            Check.That(_0.AppDomainID).IsNotNull();
            Check.That(_0.AppDomainUID).IsNull();
            Check.That(_0.AssemblyVersion).IsNotNull();
            Check.That(_0.BornOn).IsNotNull();
            Check.That(_0.Content).IsNull();
            Check.That(_0.CPUID).IsNull();
            Check.That(_0.DomainID).IsNull();
            Check.That(((Object?)(_0.Extension))).IsNull();
            Check.That(_0.GPS).IsNull();
            Check.That(_0.ID).IsNotNull();
            Check.That(_0.Language).IsNull();
            Check.That(_0.MachineID).IsNotNull();
            Check.That(_0.Modified).IsNull();
            Check.That(_0.Name).IsNull();
            Check.That(_0.Notes).IsNull();
            Check.That(_0.ProcessID).IsNotNull();
            Check.That(_0.Tags).IsNull();
            Check.That(_0.Type).IsNull();
            Check.That(_0.URI).IsNull();
        }

        /** <include file = 'TextItemExam.xml' path = 'TextItemExam/class[@Name = "TextItemExam"]/method[@Name = "Validate"]'/> */
        [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTypeTestCases))]
        public void Validate(String? type)
        {
            StringBuilder _0 = new StringBuilder();
            Random _1 = new Random();
            Byte[] _2 = new Byte[8192];
            _1.NextBytes(_2);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            TextItem _3 = new TextItem(_0.ToString(),null,null,null,null,type);

            if(ValidDataTypes.Contains(type))
            {
                Check.That(TextItem.Validate(_3)).IsTrue();
            }

            else
            {
                Check.That(TextItem.Validate(_3)).IsFalse();
            }
        }

    }

}