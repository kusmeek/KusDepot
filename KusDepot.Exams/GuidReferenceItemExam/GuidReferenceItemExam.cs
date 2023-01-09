namespace KusDepot.Exams
{
    /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]'/> */
    [Parallelizable,TestFixture]
    public class GuidReferenceItemExam
    {
        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "IComparable{GuidReferenceItem}.CompareTo"]'/> */
        [Test]
        public void CompareTo()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);
            GuidReferenceItem _3 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(new GuidReferenceItem().CompareTo(null)).IsEqualTo(1);
            Check.That(_1.CompareTo(_1)).IsEqualTo(0);
            Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
            Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
            Check.That(_1.CompareTo(_2)).IsStrictlyNegative();
            Check.That(_2.CompareTo(_1)).IsStrictlyPositive();
            Check.That(_2.CompareTo(_3)).IsEqualTo(0);
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            GuidReferenceItem _0 = new GuidReferenceItem(new Guid(),new Guid(),new List<String>{},new List<String>{});

            Check.That(_0).IsInstanceOfType(typeof(GuidReferenceItem));
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "EqualsObject"]'/> */
        [Test]
        public void EqualsObject()
        {
            Check.That((Object)new GuidReferenceItem().Equals(null)).IsEqualTo(false);
            Check.That((Object)new GuidReferenceItem().Equals(new Object())).IsEqualTo(false);
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "IEquatable{GuidReferenceItem}.Equals"]'/> */
        [Test]
        public void EqualsInterface()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);

            Check.That(new GuidReferenceItem().Equals(null)).IsEqualTo(false);
            Check.That(new GuidReferenceItem().Equals(new Object())).IsEqualTo(false);
            Check.That(_0.Equals(_1)).IsEqualTo(false);
            Check.That(_1.Equals(_0)).IsEqualTo(false);
            Check.That(_1.Equals(_1)).IsEqualTo(true);
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            GuidReferenceItem _0 = new GuidReferenceItem(null,new Guid(),null,null);
            GuidReferenceItem _1 = new GuidReferenceItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null);

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid(); GuidReferenceItem _1 = new GuidReferenceItem();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.AssemblyVersion).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsNotNull();
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "OperatorEQ"]'/> */
        [Test]
        public void OperatorEQ()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0 == _0).IsTrue();
            Check.That(_1 == _1).IsTrue();
            Check.That(_0 == _1).IsFalse();
            Check.That(_1 == _2).IsFalse();
            Check.That(_2 == _1).IsFalse();
            Check.That(_1 == null).IsFalse();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "OperatorNE"]'/> */
        [Test]
        public void OperatorNE()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0 != _0).IsFalse();
            Check.That(_1 != _1).IsFalse();
            Check.That(_0 != _1).IsTrue();
            Check.That(_1 != _2).IsTrue();
            Check.That(_2 != _1).IsTrue();
            Check.That(_1 != null).IsFalse();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "OperatorLT"]'/> */
        [Test]
        public void OperatorLT()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0 < _0).IsFalse();
            Check.That(_1 < _1).IsFalse();
            Check.That(_0 < _1).IsTrue();
            Check.That(_1 < _2).IsTrue();
            Check.That(_2 < _1).IsFalse();
            Check.That(_1 < null).IsFalse();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "OperatorLTE"]'/> */
        [Test]
        public void OperatorLTE()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0 <= _0).IsTrue();
            Check.That(_1 <= _1).IsTrue();
            Check.That(_0 <= _1).IsTrue();
            Check.That(_1 <= _2).IsTrue();
            Check.That(_2 <= _1).IsFalse();
            Check.That(_1 <= null).IsFalse();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "OperatorGT"]'/> */
        [Test]
        public void OperatorGT()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0 > _0).IsFalse();
            Check.That(_1 > _1).IsFalse();
            Check.That(_0 > _1).IsFalse();
            Check.That(_1 > _2).IsFalse();
            Check.That(_2 > _1).IsTrue();
            Check.That(_1 > null).IsFalse();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "OperatorGTE"]'/> */
        [Test]
        public void OperatorGTE()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();
            GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
            GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

            Check.That(_0 >= _0).IsTrue();
            Check.That(_1 >= _1).IsTrue();
            Check.That(_0 >= _1).IsFalse();
            Check.That(_1 >= _2).IsFalse();
            Check.That(_2 >= _1).IsTrue();
            Check.That(_1 >= null).IsFalse();
        }

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {
            GuidReferenceItem _0 = new GuidReferenceItem();

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

        /** <include file = 'GuidReferenceItemExam.xml' path = 'GuidReferenceItemExam/class[@Name = "GuidReferenceItemExam"]/method[@Name = "Validate"]'/> */
        [Test]
        public void Validate()
        {
            GuidReferenceItem _0 = new GuidReferenceItem(new Guid(),new Guid(),null,null);

            Check.That(GuidReferenceItem.Validate(_0)).IsTrue();
        }

    }

}