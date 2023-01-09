namespace KusDepot.Exams
{
    /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]'/> */
    [Parallelizable,TestFixture]
    public class MultiMediaItemExam
    {
        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/field[@Name = "ValidDataTypes"]'/> */
        private static readonly String?[] ValidDataTypes = new String?[]
        {
            DataType.AVI,
            DataType.BAR,
            DataType.BMP,
            DataType.FLAC,
            DataType.GIF,
            DataType.IMAX,
            DataType.JPEG,
            DataType.JPG,
            DataType.MIDI,
            DataType.MOV,
            DataType.M4A,
            DataType.MP3,
            DataType.MP4,
            DataType.MPEG,
            DataType.MPG,
            DataType.OGA,
            DataType.OGV,
            DataType.OGX,
            DataType.PNG,
            DataType.QRC,
            DataType.SVG,
            DataType.TIFF,
            DataType.WAV,
            DataType.WMA,
            DataType.WEBA,
            DataType.WEBM,
            DataType.WEBP,
            DataType.WMV
        };

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "IComparable{MultiMediaItem}.CompareTo"]'/> */
        [Test]
        public void CompareTo()
        {
            MultiMediaItem _0 = new MultiMediaItem();
            Thread.Sleep(100);
            MultiMediaItem _1 = new MultiMediaItem();

            Check.That(new MultiMediaItem().CompareTo(null)).IsEqualTo(1);
            Check.That(_0.CompareTo(_0)).IsEqualTo(0);
            Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
            Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            StringBuilder _0 = new StringBuilder();
            Random _1 = new Random();
            Byte[] _2 = new Byte[8192];
            String _5 = "MultiMediaExam";
            _1.NextBytes(_2);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            String? _3 = ValidDataTypes[_1.Next(0,ValidDataTypes.Length-1)];
            MultiMediaItem _4 = new MultiMediaItem(_2,new List<String>(),Double.Pi,Decimal.One,new Guid(),Single.Tau,Language.ell,new List<String>{},new MemoryStream(),new List<String>{},_5,_3!.ToString(),Single.E);

            Check.That(_4).IsInstanceOfType(typeof(MultiMediaItem));
            Check.That(_4.Content).IsEqualTo(_2);
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "EqualsObject"]'/> */
        [Test]
        public void EqualsObject()
        {
            Check.That((Object)new MultiMediaItem().Equals(null)).IsEqualTo(false);
            Check.That((Object)new MultiMediaItem().Equals(new Object())).IsEqualTo(false);
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "IEquatable{MultiMediaItem}.Equals"]'/> */
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
            MultiMediaItem _3 = new MultiMediaItem(_2,null,null,null,_11,null,null,null,null,null,null,null,null);
            MultiMediaItem _15 = new MultiMediaItem(_2,null,null,null,_11,null,null,null,null,null,null,null,null);
            MultiMediaItem _4 = new MultiMediaItem(_2,null,null,null,_14,null,null,null,null,null,null,null,null);
            MultiMediaItem _5 = new MultiMediaItem(_2,null,null,null,null,null,null,null,null,null,null,null,null);
            MultiMediaItem _9 = new MultiMediaItem(_8,null,null,null,_11,null,null,null,null,null,null,null,null);

            Check.That(new MultiMediaItem().Equals(null)).IsEqualTo(false);
            Check.That(new MultiMediaItem().Equals(new Object())).IsEqualTo(false);
            Check.That(_3.Equals(_4)).IsEqualTo(false);
            Check.That(_3.Equals(_5)).IsEqualTo(false);
            Check.That(_3.Equals(_9)).IsEqualTo(false);
            Check.That(_3.Equals(_15)).IsTrue();
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            MultiMediaItem _0 = new MultiMediaItem(null,null,null,null,new Guid(),null,null,null,null,null,null,null,null);
            MultiMediaItem _1 = new MultiMediaItem(null,null,null,null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null,null,null,null,null,null);
            MultiMediaItem _2 = new MultiMediaItem(null,null,null,null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null,null,null,null,null,null);

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid(); MultiMediaItem _1 = new MultiMediaItem();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.AssemblyVersion).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsNotNull();
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {
            MultiMediaItem _0 = new MultiMediaItem();
            
            Check.That(_0.AppDomainID).IsNotNull();
            Check.That(_0.AppDomainUID).IsNull();
            Check.That(_0.AssemblyVersion).IsNotNull();
            Check.That(_0.Artists).IsNull();
            Check.That(_0.Bitrate).IsNull();
            Check.That(_0.BornOn).IsNotNull();
            Check.That(_0.Content).IsNull();
            Check.That(_0.CPUID).IsNull();
            Check.That(_0.DomainID).IsNull();
            Check.That(((Object?)(_0.Extension))).IsNull();
            Check.That(_0.Duration).IsNull();
            Check.That(_0.Framerate).IsNull();
            Check.That(_0.GPS).IsNull();
            Check.That(_0.ID).IsNotNull();
            Check.That(_0.Language).IsNull();
            Check.That(_0.Source).IsNull();
            Check.That(_0.Title).IsNull();
            Check.That(_0.MachineID).IsNotNull();
            Check.That(_0.Modified).IsNull();
            Check.That(_0.Name).IsNull();
            Check.That(_0.Notes).IsNull();
            Check.That(_0.ProcessID).IsNotNull();
            Check.That(_0.Tags).IsNull();
            Check.That(_0.Type).IsNull();
            Check.That(_0.URI).IsNull();
            Check.That(_0.Volume).IsNull();
        }

        /** <include file = 'MultiMediaItemExam.xml' path = 'MultiMediaItemExam/class[@Name = "MultiMediaItemExam"]/method[@Name = "Validate"]'/> */
        [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTypeTestCases))]
        public void Validate(String? type)
        {
            Random _0 = new Random();
            Byte[] _1 = new Byte[8192];
            _0.NextBytes(_1);
            MultiMediaItem _2 = new MultiMediaItem(_1,null,null,null,null,null,null,null,null,null,null,type,null);

            if(ValidDataTypes.Contains(type))
            {
                Check.That(MultiMediaItem.Validate(_2)).IsTrue();
            }

            else
            {
                Check.That(MultiMediaItem.Validate(_2)).IsFalse();
            }

        }

    }

}