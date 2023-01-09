namespace KusDepot.Exams
{
    /** <include file = 'TestCaseDataGenerator.xml' path = 'TestCaseDataGenerator/class[@Name = "TestCaseDataGenerator"]'/> */
    internal static class TestCaseDataGenerator
    {
        /** <include file = 'TestCaseDataGenerator.xml' path = 'TestCaseDataGenerator/class[@Name = "TestCaseDataGenerator"]/field[@Name = "DataTypes"]'/> */
        private static String?[] DataTypes;

        /** <include file = 'TestCaseDataGenerator.xml' path = 'TestCaseDataGenerator/class[@Name = "TestCaseDataGenerator"]/field[@Name = "Tags"]'/> */
        private static String?[] Tags;

        /** <include file = 'TestCaseDataGenerator.xml' path = 'TestCaseDataGenerator/class[@Name = "TestCaseDataGenerator"]/constructor[@Name = "Constructor"]'/> */
        static TestCaseDataGenerator()
        {
            Func<FieldInfo,String?> _0 = (FieldInfo field) => (String?)field.GetValue(null);

            List<String?> AllTags = new List<String?>{ };

            Type _1 = typeof(ArchitectureType);
            FieldInfo[] _11 = _1.GetFields();

            Type _2 = typeof(BrowserType);
            FieldInfo[] _12 = _2.GetFields();

            Type _3 = typeof(DatabaseType);
            FieldInfo[] _13 = _3.GetFields();

            Type _4 = typeof(DeploymentType);
            FieldInfo[] _14 = _4.GetFields();

            Type _5 = typeof(FrameworkVersionType);
            FieldInfo[] _15 = _5.GetFields();

            Type _6 = typeof(HostType);
            FieldInfo[] _16 = _6.GetFields();

            Type _7 = typeof(Language);
            FieldInfo[] _17 = _7.GetFields();

            Type _8 = typeof(OperatingSystemType);
            FieldInfo[] _18 = _8.GetFields();

            Type _9 = typeof(PlatformType);
            FieldInfo[] _19 = _9.GetFields();

            Type _10 = typeof(UsageType);
            FieldInfo[] _20 = _10.GetFields();

            AllTags.AddRange(_11.Select(_0).ToArray());
            AllTags.AddRange(_12.Select(_0).ToArray());
            AllTags.AddRange(_13.Select(_0).ToArray());
            AllTags.AddRange(_14.Select(_0).ToArray());
            AllTags.AddRange(_15.Select(_0).ToArray());
            AllTags.AddRange(_16.Select(_0).ToArray());
            AllTags.AddRange(_17.Select(_0).ToArray());
            AllTags.AddRange(_18.Select(_0).ToArray());
            AllTags.AddRange(_19.Select(_0).ToArray());
            AllTags.AddRange(_20.Select(_0).ToArray());

            Tags = AllTags.ToArray();

            Type _21 = typeof(DataType);

            FieldInfo[] _22 = _21.GetFields();

            DataTypes = _22.Select(_0).ToArray();
        }

        /** <include file = 'TestCaseDataGenerator.xml' path = 'TestCaseDataGenerator/class[@Name = "TestCaseDataGenerator"]/property[@Name = "DataTypeTestCases"]'/> */
        public static IEnumerable DataTypeTestCases
        {
            get
            {
                foreach(String? type in DataTypes)
                {
                    yield return new TestCaseData(type);
                }
            }
        }

        /** <include file = 'TestCaseDataGenerator.xml' path = 'TestCaseDataGenerator/class[@Name = "TestCaseDataGenerator"]/property[@Name = "TagTestCases"]'/> */
        public static IEnumerable TagTestCases
        {
            get
            {
                foreach(String? tag in Tags)
                {
                    yield return new TestCaseData(tag);
                }
            }
        }
    }

}