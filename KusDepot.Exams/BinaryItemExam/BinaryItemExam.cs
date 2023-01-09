namespace KusDepot.Exams
{
    /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]'/> */
    [Parallelizable,TestFixture]
    public class BinaryItemExam
    {
        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/field[@Name = "ValidDataTypes"]'/> */
        private static readonly String?[] ValidDataTypes = new String?[]
        {
            DataType.ACCDB,
            DataType.APPX,
            DataType.BAML,
            DataType.BIN,
            DataType.CAB,
            DataType.CAD,
            DataType.CER,
            DataType.CIL,
            DataType.COM,
            DataType.DAT,
            DataType.DAW,
            DataType.DB,
            DataType.DIT,
            DataType.DKR,
            DataType.DLL,
            DataType.DMP,
            DataType.EDB,
            DataType.EOT,
            DataType.EPUB,
            DataType.ETL,
            DataType.EVTX,
            DataType.EXE,
            DataType.EXT,
            DataType.GZ ,
            DataType.ICO,
            DataType.ISO,
            DataType.JAR,
            DataType.JAVA,
            DataType.KEY,
            DataType.KML,
            DataType.LDF,
            DataType.MDB,
            DataType.MDF,
            DataType.MSIL,
            DataType.MSIX,
            DataType.NDF,
            DataType.OTF,
            DataType.P7B,
            DataType.PCAP,
            DataType.PDB,
            DataType.PSD1,
            DataType.PSM1,
            DataType.PSSC,
            DataType.PEM,
            DataType.PFX,
            DataType.PHP,
            DataType.RAR,
            DataType.RLIB,
            DataType.SCR,
            DataType.SO,
            DataType.SUO,
            DataType.SYS,
            DataType.TAR,
            DataType.TTF,
            DataType.VHDX,
            DataType.WASM,
            DataType.WIM,
            DataType.WINMD,
            DataType.WOFF,
            DataType.XAP,
            DataType.ZIP
        };

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "IComparable{BinaryItem}.CompareTo"]'/> */
        [Test]
        public void CompareTo()
        {
            BinaryItem _0 = new BinaryItem();
            Thread.Sleep(100);
            BinaryItem _1 = new BinaryItem();

            Check.That(new BinaryItem().CompareTo(null)).IsEqualTo(1);
            Check.That(_0.CompareTo(_0)).IsEqualTo(0);
            Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
            Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            StringBuilder _0 = new StringBuilder();
            Random _1 = new Random();
            Byte[] _2 = new Byte[8192];
            _1.NextBytes(_2);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            String? _3 = ValidDataTypes[_1.Next(0,ValidDataTypes.Length-1)];
            BinaryItem _4 = new BinaryItem(_2,new Guid(),new List<String>{},new List<String>{},_3!.ToString());

            Check.That(_4).IsInstanceOfType(typeof(BinaryItem));
            Check.That(_4.Content).IsEqualTo(_2);
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "EqualsObject"]'/> */
        [Test]
        public void EqualsObject()
        {
            Check.That((Object)new BinaryItem().Equals(null)).IsEqualTo(false);
            Check.That((Object)new BinaryItem().Equals(new Object())).IsEqualTo(false);
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "IEquatable{BinaryItem}.Equals"]'/> */
        [Test]
        public void EqualsInterface()
        {
            Random _1 = new Random();
            Random _7 = new Random();
            Byte[] _2 = new Byte[8192];
            Byte[] _8 = new Byte[8192];
            Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
            Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
            _1.NextBytes(_2);
            _7.NextBytes(_8);
            BinaryItem _3 = new BinaryItem(_2,_11,null,null,null);
            BinaryItem _15 = new BinaryItem(_2,_11,null,null,null);
            BinaryItem _4 = new BinaryItem(_2,_14,null,null,null);
            BinaryItem _5 = new BinaryItem(_2,null,null,null,null);
            BinaryItem _9 = new BinaryItem(_8,_11,null,null,null);

            Check.That(new BinaryItem().Equals(null)).IsEqualTo(false);
            Check.That(new BinaryItem().Equals(new Object())).IsEqualTo(false);
            Check.That(_3.Equals(_4)).IsEqualTo(false);
            Check.That(_3.Equals(_5)).IsEqualTo(false);
            Check.That(_3.Equals(_9)).IsEqualTo(false);
            Check.That(_3.Equals(_15)).IsTrue();
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            BinaryItem _0 = new BinaryItem(null,new Guid(),null,null,null);
            BinaryItem _1 = new BinaryItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            BinaryItem _2 = new BinaryItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid(); BinaryItem _1 = new BinaryItem();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.AssemblyVersion).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsNotNull();
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {
            BinaryItem _0 = new BinaryItem();

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
            Check.That(_0.MachineID).IsNotNull();
            Check.That(_0.Modified).IsNull();
            Check.That(_0.Name).IsNull();
            Check.That(_0.Notes).IsNull();
            Check.That(_0.ProcessID).IsNotNull();
            Check.That(_0.Tags).IsNull();
            Check.That(_0.Type).IsNull();
            Check.That(_0.URI).IsNull();
        }

        /** <include file = 'BinaryItemExam.xml' path = 'BinaryItemExam/class[@Name = "BinaryItemExam"]/method[@Name = "Validate"]'/> */
        [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTypeTestCases))]
        public void Validate(String? type)
        {
            Random _0 = new Random();
            Byte[] _1 = new Byte[8192];
            _0.NextBytes(_1);
            BinaryItem _2 = new BinaryItem(_1,null,null,null,type);

            if(ValidDataTypes.Contains(type))
            {
                Check.That(BinaryItem.Validate(_2)).IsTrue();
            }

            else
            {
                Check.That(BinaryItem.Validate(_2)).IsFalse();
            }
        }

    }

}