namespace KusDepot.Exams;

internal static class TestCaseDataGenerator
{
    private static String?[] DataTypes;

    private static String?[] Tags;

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

        Type _22 = typeof(HardwareType);
        FieldInfo[] _21 = _22.GetFields();

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
        AllTags.AddRange(_21.Select(_0).ToArray());

        Tags = AllTags.ToArray();

        Type _31 = typeof(DataType);

        FieldInfo[] _32 = _31.GetFields();

        DataTypes = _32.Select(_0).ToArray();
    }

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

    public static String GenerateUnicodeString(Int32 length)
    {
        StringBuilder _s = new StringBuilder(length);
        Byte[] _b = new Byte[4];

        using(RandomNumberGenerator _r = RandomNumberGenerator.Create())
        {
            for(Int32 i = 0; i < length; i++)
            {
                Int32 _cp;
                do
                {
                    _r.GetBytes(_b);
                    _cp = BitConverter.ToInt32(_b, 0) & 0x10FFFF;
                }
                while(!IsValidCodePoint(_cp));

                _s.Append(Char.ConvertFromUtf32(_cp));
            }
        }

        return _s.ToString();
    }

    private static Boolean IsValidCodePoint(Int32 cp)
    {
        return (cp < 0xD800 || cp > 0xDFFF) && (cp & 0xFFFE) != 0xFFFE;
    }
}