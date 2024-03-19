namespace KusDepot;

/**<include file='Tags.xml' path='Tags/class[@name="Tags"]/main/*'/>*/
internal sealed class Tags : DataTable
{
    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/constructor[@name="Constructor"]/*'/>*/
    internal Tags(String? name , Int32 capacity) : base(name)
    {
        DataColumn? _0;

        _0 = this.Columns.Add("ID",typeof(Guid)); _0.AllowDBNull = false; _0.DefaultValue = null; _0.Unique = true;

        for( Int32 _n = 0 ; _n < capacity ; _n++ ) { _0 = this.Columns.Add("Tag" + _n.ToString(CultureInfo.InvariantCulture),typeof(String)); _0.AllowDBNull = true; _0.DefaultValue = null; }

        this.PrimaryKey = new DataColumn[] { this.Columns["ID"]! };
    }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="AddUpdate"]/*'/>*/
    internal Boolean AddUpdate(String? it)
    {
        if(it is null) { return false; }

        Tool? _0 = null; GuidReferenceItem? _1 = null; GenericItem? _2 = null; TextItem? _3 = null; CodeItem? _4 = null; MSBuildItem? _5 = null; BinaryItem? _6 = null; MultiMediaItem? _7 = null;

        Object[] _u = new Object[this.GetTagCapacity()+1];

        try
        {
            try { if(Tool.TryParse(it,null,out _0))              { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GuidReferenceItem.TryParse(it,null,out _1)) { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GenericItem.TryParse(it,null,out _2))       { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(TextItem.TryParse(it,null,out _3))          { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(CodeItem.TryParse(it,null,out _4))          { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MSBuildItem.TryParse(it,null,out _5))       { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(BinaryItem.TryParse(it,null,out _6))        { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MultiMediaItem.TryParse(it,null,out _7))    { goto GetData; } } catch ( SerializationException ) { } catch ( XmlException ) { }

            return false;

            GetData:
            if(_0 is not null) { _u.SetValue(_0.GetID(),0); String[]? _t = _0.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_1 is not null) { _u.SetValue(_1.GetID(),0); String[]? _t = _1.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_2 is not null) { _u.SetValue(_2.GetID(),0); String[]? _t = _2.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_3 is not null) { _u.SetValue(_3.GetID(),0); String[]? _t = _3.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_4 is not null) { _u.SetValue(_4.GetID(),0); String[]? _t = _4.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_5 is not null) { _u.SetValue(_5.GetID(),0); String[]? _t = _5.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_6 is not null) { _u.SetValue(_6.GetID(),0); String[]? _t = _6.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }
            if(_7 is not null) { _u.SetValue(_7.GetID(),0); String[]? _t = _7.GetTags()?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); } }

            AddUpdate:
            _u = _u.TakeWhile(_=>_ is not null).ToArray(); if(_u.Length == 0) { return false; }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[0]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="AddUpdateDescriptor"]/*'/>*/
    internal Boolean AddUpdate(Descriptor? descriptor)
    {
        if(descriptor is null) { return false; }

        Descriptor _d = descriptor; Object[] _u = new Object[this.GetTagCapacity()+1];

        try
        {
            _u.SetValue(_d.ID,0); String[]? _t = _d.Tags?.ToArray(); if( _t is null ) { goto AddUpdate; } if(this.GetTagCapacity() < _t.Length) { return false; } for(Int32 _ = 0; _ < _t.Length; _++) { _u.SetValue(_t[_],_+1); }

            AddUpdate:
            _u = _u.TakeWhile(_=>_ is not null).ToArray(); if(_u.Length == 0) { return false; }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[0]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="Exists"]/*'/>*/
    internal Boolean? Exists(Guid? id)
    {
        if(id is null) { return null; } if(Equals(this.Rows.Count,0)) { return false; }

        try
        {
            return this.Rows.Cast<DataRow>().Any(_=>Guid.Equals(_["ID"],id));
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="GetTagCapacity"]/*'/>*/
    private Int32 GetTagCapacity() { return this.Columns.Cast<DataColumn>().Count(_=>_.ColumnName.StartsWith("Tag",StringComparison.Ordinal)); }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="Remove"]/*'/>*/
    internal Boolean Remove(String? it)
    {
        if(it is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Tool? _0 = null; GuidReferenceItem? _1 = null; GenericItem? _2 = null; TextItem? _3 = null; CodeItem? _4 = null; MSBuildItem? _5 = null; BinaryItem? _6 = null; MultiMediaItem? _7 = null;

        Object[] _u = new Object[1];

        try
        {
            try { if(Tool.TryParse(it,null,out _0))              { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GuidReferenceItem.TryParse(it,null,out _1)) { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GenericItem.TryParse(it,null,out _2))       { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(TextItem.TryParse(it,null,out _3))          { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(CodeItem.TryParse(it,null,out _4))          { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MSBuildItem.TryParse(it,null,out _5))       { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(BinaryItem.TryParse(it,null,out _6))        { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MultiMediaItem.TryParse(it,null,out _7))    { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }

            Remove:
            if(_0 is not null) { _u.SetValue(_0.GetID(),0); }
            if(_1 is not null) { _u.SetValue(_1.GetID(),0); }
            if(_2 is not null) { _u.SetValue(_2.GetID(),0); }
            if(_3 is not null) { _u.SetValue(_3.GetID(),0); }
            if(_4 is not null) { _u.SetValue(_4.GetID(),0); }
            if(_5 is not null) { _u.SetValue(_5.GetID(),0); }
            if(_6 is not null) { _u.SetValue(_6.GetID(),0); }
            if(_7 is not null) { _u.SetValue(_7.GetID(),0); }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[0]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="RemoveDescriptor"]/*'/>*/
    internal Boolean Remove(Descriptor? descriptor)
    {
        if(descriptor is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[1];

        try
        {
            _u.SetValue(descriptor.ID,0);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[0]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Tags.xml' path='Tags/class[@name="Tags"]/method[@name="RemoveGuid"]/*'/>*/
    internal Boolean Remove(Guid? id)
    {
        if(id is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[1];

        try
        {
            _u.SetValue(id,0);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[0]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }
}