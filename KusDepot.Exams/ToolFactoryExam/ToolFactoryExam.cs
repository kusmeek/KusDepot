namespace KusDepot.Exams
{
    /** <include file = 'ToolFactoryExam.xml' path = 'ToolFactoryExam/class[@Name = "ToolFactoryExam"]'/> */
    [Parallelizable,TestFixture]
    public class ToolFactoryExam
    {
        /** <include file = 'ToolFactoryExam.xml' path = 'ToolFactoryExam/class[@Name = "ToolFactoryExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'ToolFactoryExam.xml' path = 'ToolFactoryExam/class[@Name = "ToolFactoryExam"]/method[@Name = "Forge"]'/> */
        [Test]
        public void Forge()
        {
            List<DataItem> _0    = new List<DataItem>();
            GuidReferenceItem _1 = new GuidReferenceItem();
            GenericItem _2       = new GenericItem();
            TextItem _3          = new TextItem();
            MSBuildItem _4       = new MSBuildItem();
            BinaryItem _5        = new BinaryItem();
            MultiMediaItem _6    = new MultiMediaItem();
            List<Guid?> _8       = new List<Guid?>();
            List<Guid?> _10      = new List<Guid?>();
            _0.Add(_1); _8.Add(_1.ID);
            _0.Add(_2); _8.Add(_2.ID);
            _0.Add(_3); _8.Add(_3.ID);
            _0.Add(_4); _8.Add(_4.ID);
            _0.Add(_5); _8.Add(_5.ID);
            _0.Add(_6); _8.Add(_6.ID);
            String _11 = "ToolFactoryExamInputs";
            Queue<Object> _22 = new Queue<Object>();
            _22.Enqueue(_11);
            Guid _15 = new Guid();
            String _17 = "My Tool";
            String _18 = "ToolFactoryExamNotes";
            String _19 = "ToolFactoryExamObjectives";
            String _21 = "ToolFactoryExamPolicies";;
            List<String> _24 = new List<String>(){_18};
            List<Object> _25 = new List<Object>(){_19};
            List<Object> _26 = new List<Object>(){_21};
            String _28 = "ToolFactoryExamPurpose";
            String _32 = "ToolFactoryExamTags";
            List<String> _38 = new List<String>(){_32};
            Tool? _99 = ToolFactory.Forge(_0,_15,_22,_17,_24,_25,_26,_28,_38);
            List<DataItem>? _9   = _99!.Data;
            if(_9 is not null)
            {
                foreach(DataItem item in _9) { _10!.Add(item!.ID); }
            }
            
            Check.That(_99).IsInstanceOfType(typeof(Tool));
            Check.That(_10).ContainsExactly(_8.ToArray());
            Check.That(_99.ID).IsEqualTo(_15);
            Check.That(_99.Inputs!.Dequeue()).IsEqualTo(_11);
            Check.That(_99.Name).IsEqualTo(_17);
            Check.That(_99.Notes).ContainsExactly(_18);
            Check.That(_99.Objectives).ContainsExactly(_19);
            Check.That(_99.Policies).ContainsExactly(_21);
            Check.That(_99.Purpose).IsEqualTo(_28);
            Check.That(_99.Tags).ContainsExactly(_32);
        }

    }

}