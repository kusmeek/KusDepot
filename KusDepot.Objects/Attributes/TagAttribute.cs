namespace KusDepot
{
    /** <include file = 'TagAttribute.xml' path = 'TagAttribute/class[@Name = "TagAttribute"]'/> */
    [AttributeUsage( (AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue) , AllowMultiple = true)]
    public class TagAttribute : Attribute
    {
        /** <include file = 'TagAttribute.xml' path = 'TagAttribute/class[@Name = "TagAttribute"]/field[@Name = "Tag"]'/> */
        public readonly String? Tag;

        /** <include file = 'TagAttribute.xml' path = 'TagAttribute/class[@Name = "TagAttribute"]/constructor[@Name = "Constructor"]'/> */
        public TagAttribute(String? tag)
        {
            List<String?> AllTags = new List<String?>{ };

            Func<FieldInfo,String?> _0 = (FieldInfo field) => (String?)field.GetValue(null);

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

            try
            {
                if( (tag is not null) && (AllTags.Contains(tag)) )
                {
                    this.Tag = tag;
                }

                else
                {
                    throw new Exception("KusDepot.TagAttribute.Constructor");
                }
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.TagAttribute.Constructor",ie); }
        }

    }

}