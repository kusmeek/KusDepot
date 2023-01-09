namespace KusDepot.Exams
{
    /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]'/> */
    [Parallelizable,TestFixture]
    public class GenericItemExam
    {
        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "IComparable{GenericItem}.CompareTo"]'/> */
        [Test]
        public void CompareTo()
        {
            GenericItem _0 = new GenericItem();
            Thread.Sleep(100);
            GenericItem _1 = new GenericItem();

            Check.That(new GenericItem().CompareTo(null)).IsEqualTo(1);
            Check.That(_0.CompareTo(_0)).IsEqualTo(0);
            Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
            Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
        }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            GenericItem _0 = new GenericItem(new List<Object>{},new Guid(), new List<String>{}, new List<String>{},DataType.SGML);

            Check.That(_0).IsInstanceOfType(typeof(GenericItem));
        }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "EqualsObject"]'/> */
        [Test]
        public void EqualsObject()
        {
            Check.That((Object)new GenericItem().Equals(null)).IsEqualTo(false);
            Check.That((Object)new GenericItem().Equals(new Object())).IsEqualTo(false);
        }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "IEquatable{GenericItem}.Equals"]'/> */
        [Test]
        public void EqualsInterface()
        {
            GenericItem _0 = new GenericItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GenericItem _1 = new GenericItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GenericItem _2 = new GenericItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(new GenericItem().Equals(null)).IsEqualTo(false);
            Check.That(new GenericItem().Equals(new Object())).IsEqualTo(false);
            Check.That(_0.Equals(_0)).IsEqualTo(true);
            Check.That(_0.Equals(_1)).IsEqualTo(true);
            Check.That(_0.Equals(_2)).IsEqualTo(false);
        }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            GenericItem _0 = new GenericItem(null,new Guid(),null,null,null);
            GenericItem _1 = new GenericItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GenericItem _2 = new GenericItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid(); GenericItem _1 = new GenericItem();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsNotNull();
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'GenericItemExam.xml' path = 'GenericItemExam/class[@Name = "GenericItemExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {
            GenericItem _0 = new GenericItem();
            
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

    }

}