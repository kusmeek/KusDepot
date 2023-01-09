namespace KusDepot.Exams
{
    /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]'/> */
    [Parallelizable,TestFixture]
    public class MSBuildItemExam
    {
        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "IComparable{MSBuildItem}.CompareTo"]'/> */
        [Test]
        public void CompareTo()
        {
            MSBuildItem _0 = new MSBuildItem();
            Thread.Sleep(100);
            MSBuildItem _1 = new MSBuildItem();

            Check.That(new MSBuildItem().CompareTo(null)).IsEqualTo(1);
            Check.That(_0.CompareTo(_0)).IsEqualTo(0);
            Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
            Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
        }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            StringBuilder _0 = new StringBuilder();
            Random _1 = new Random();
            Byte[] _2 = new Byte[8192];
            _1.NextBytes(_2);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            MSBuildItem _3 = new MSBuildItem(_0.ToString(),new Guid(),new List<String>{},new List<String>{});

            Check.That(_3).IsInstanceOfType(typeof(MSBuildItem));
            Check.That(_3.Content).IsEqualTo(_0.ToString());
        }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "EqualsObject"]'/> */
        [Test]
        public void EqualsObject()
        {
            Check.That((Object)new MSBuildItem().Equals(null)).IsEqualTo(false);
            Check.That((Object)new MSBuildItem().Equals(new Object())).IsEqualTo(false);
        }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "IEquatable{MSBuildItem}.Equals"]'/> */
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
            MSBuildItem _3 = new MSBuildItem(_0.ToString(),_11,null,null);
            MSBuildItem _15 = new MSBuildItem(_0.ToString(),_11,null,null);
            MSBuildItem _4 = new MSBuildItem(_0.ToString(),_14,null,null);
            MSBuildItem _5 = new MSBuildItem(_0.ToString(),null,null,null);
            MSBuildItem _9 = new MSBuildItem(_6.ToString(),_11,null,null);

            Check.That(new MSBuildItem().Equals(null)).IsEqualTo(false);
            Check.That(new MSBuildItem().Equals(new Object())).IsEqualTo(false);
            Check.That(_3.Equals(_4)).IsEqualTo(false);
            Check.That(_3.Equals(_5)).IsEqualTo(false);
            Check.That(_3.Equals(_9)).IsEqualTo(false);
            Check.That(_3.Equals(_15)).IsTrue();
        }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            MSBuildItem _0 = new MSBuildItem(null,new Guid(),null,null);
            MSBuildItem _1 = new MSBuildItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null);
            MSBuildItem _2 = new MSBuildItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null);

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid(); MSBuildItem _1 = new MSBuildItem();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.AssemblyVersion).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsNotNull();
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {        
            MSBuildItem _0 = new MSBuildItem();
            
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

        /** <include file = 'MSBuildItemExam.xml' path = 'MSBuildItemExam/class[@Name = "MSBuildItemExam"]/method[@Name = "Validate"]'/> */
        [Test]
        public void Validate()
        {
            StringBuilder _0 = new StringBuilder();
            Random _1 = new Random();
            Byte[] _2 = new Byte[8192];
            _1.NextBytes(_2);
            foreach (Byte b in _2) {_0.Append(Convert.ToChar(b));}
            MSBuildItem _3 = new MSBuildItem(_0.ToString(),null,null,null);

            Check.That(MSBuildItem.Validate(_3)).IsTrue();
        }

    }

}