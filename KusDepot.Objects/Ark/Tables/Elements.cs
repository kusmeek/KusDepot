namespace KusDepot;

/**<include file='Elements.xml' path='Elements/class[@name="Elements"]/main/*'/>*/
public class Elements : DataTable
{
    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/constructor[@name="Constructor"]/*'/>*/
    public Elements(String? name) : base(name)
    {
        DataColumn? _0;

        _0 = this.Columns.Add("Application",typeof(String));        _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ApplicationVersion",typeof(String)); _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("BornOn",typeof(String));             _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("DistinguishedName",typeof(String));  _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ID",typeof(Guid));                   _0.AllowDBNull = false;_0.DefaultValue = null; _0.Unique = true;

        _0 = this.Columns.Add("Modified",typeof(String));           _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Name",typeof(String));               _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ObjectType",typeof(String));         _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ServiceVersion",typeof(String));     _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Type",typeof(String));               _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Version",typeof(String));            _0.AllowDBNull = true; _0.DefaultValue = null;

        this.PrimaryKey = new DataColumn[] { this.Columns["ID"]! };
    }

    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/method[@name="AddUpdate"]/*'/>*/
    internal Boolean AddUpdate(String? it)
    {
        if(it is null) { return false; }

        Tool? _0 = null; GuidReferenceItem? _1 = null; GenericItem? _2 = null; TextItem? _3 = null; CodeItem? _4 = null; MSBuildItem? _5 = null; BinaryItem? _6 = null; MultiMediaItem? _7 = null;

        Object[] _u = new Object[11];

        try
        {
            try { if(Tool.TryParse(it,null,out _0))              { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GuidReferenceItem.TryParse(it,null,out _1)) { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GenericItem.TryParse(it,null,out _2))       { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(TextItem.TryParse(it,null,out _3))          { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(CodeItem.TryParse(it,null,out _4))          { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MSBuildItem.TryParse(it,null,out _5))       { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(BinaryItem.TryParse(it,null,out _6))        { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MultiMediaItem.TryParse(it,null,out _7))    { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }

            return false;

            AddUpdate:
            if(_0 is not null) { _u.SetValue(_0.GetApplication(),0); _u.SetValue(_0.GetApplicationVersion()?.ToString(),1); _u.SetValue(_0.GetBornOn()?.ToString("O"),2); _u.SetValue(_0.GetDistinguishedName(),3); _u.SetValue(_0.GetID(),4); _u.SetValue(_0.GetModified()?.ToString("O"),5); _u.SetValue(_0.GetName(),6); _u.SetValue(((Object)_0).GetType().Name,7); _u.SetValue(_0.GetServiceVersion()?.ToString(),8); _u.SetValue(_0.GetType().ToString(),9); _u.SetValue(_0.GetVersion()?.ToString(),10); }
            if(_1 is not null) { _u.SetValue(_1.GetApplication(),0); _u.SetValue(_1.GetApplicationVersion()?.ToString(),1); _u.SetValue(_1.GetBornOn()?.ToString("O"),2); _u.SetValue(_1.GetDistinguishedName(),3); _u.SetValue(_1.GetID(),4); _u.SetValue(_1.GetModified()?.ToString("O"),5); _u.SetValue(_1.GetName(),6); _u.SetValue(((Object)_1).GetType().Name,7); _u.SetValue(_1.GetServiceVersion()?.ToString(),8); _u.SetValue(_1.GetType(),9); _u.SetValue(_1.GetVersion()?.ToString(),10); }
            if(_2 is not null) { _u.SetValue(_2.GetApplication(),0); _u.SetValue(_2.GetApplicationVersion()?.ToString(),1); _u.SetValue(_2.GetBornOn()?.ToString("O"),2); _u.SetValue(_2.GetDistinguishedName(),3); _u.SetValue(_2.GetID(),4); _u.SetValue(_2.GetModified()?.ToString("O"),5); _u.SetValue(_2.GetName(),6); _u.SetValue(((Object)_2).GetType().Name,7); _u.SetValue(_2.GetServiceVersion()?.ToString(),8); _u.SetValue(_2.GetType(),9); _u.SetValue(_2.GetVersion()?.ToString(),10); }
            if(_3 is not null) { _u.SetValue(_3.GetApplication(),0); _u.SetValue(_3.GetApplicationVersion()?.ToString(),1); _u.SetValue(_3.GetBornOn()?.ToString("O"),2); _u.SetValue(_3.GetDistinguishedName(),3); _u.SetValue(_3.GetID(),4); _u.SetValue(_3.GetModified()?.ToString("O"),5); _u.SetValue(_3.GetName(),6); _u.SetValue(((Object)_3).GetType().Name,7); _u.SetValue(_3.GetServiceVersion()?.ToString(),8); _u.SetValue(_3.GetType(),9); _u.SetValue(_3.GetVersion()?.ToString(),10); }
            if(_4 is not null) { _u.SetValue(_4.GetApplication(),0); _u.SetValue(_4.GetApplicationVersion()?.ToString(),1); _u.SetValue(_4.GetBornOn()?.ToString("O"),2); _u.SetValue(_4.GetDistinguishedName(),3); _u.SetValue(_4.GetID(),4); _u.SetValue(_4.GetModified()?.ToString("O"),5); _u.SetValue(_4.GetName(),6); _u.SetValue(((Object)_4).GetType().Name,7); _u.SetValue(_4.GetServiceVersion()?.ToString(),8); _u.SetValue(_4.GetType(),9); _u.SetValue(_4.GetVersion()?.ToString(),10); }
            if(_5 is not null) { _u.SetValue(_5.GetApplication(),0); _u.SetValue(_5.GetApplicationVersion()?.ToString(),1); _u.SetValue(_5.GetBornOn()?.ToString("O"),2); _u.SetValue(_5.GetDistinguishedName(),3); _u.SetValue(_5.GetID(),4); _u.SetValue(_5.GetModified()?.ToString("O"),5); _u.SetValue(_5.GetName(),6); _u.SetValue(((Object)_5).GetType().Name,7); _u.SetValue(_5.GetServiceVersion()?.ToString(),8); _u.SetValue(_5.GetType(),9); _u.SetValue(_5.GetVersion()?.ToString(),10); }
            if(_6 is not null) { _u.SetValue(_6.GetApplication(),0); _u.SetValue(_6.GetApplicationVersion()?.ToString(),1); _u.SetValue(_6.GetBornOn()?.ToString("O"),2); _u.SetValue(_6.GetDistinguishedName(),3); _u.SetValue(_6.GetID(),4); _u.SetValue(_6.GetModified()?.ToString("O"),5); _u.SetValue(_6.GetName(),6); _u.SetValue(((Object)_6).GetType().Name,7); _u.SetValue(_6.GetServiceVersion()?.ToString(),8); _u.SetValue(_6.GetType(),9); _u.SetValue(_6.GetVersion()?.ToString(),10); }
            if(_7 is not null) { _u.SetValue(_7.GetApplication(),0); _u.SetValue(_7.GetApplicationVersion()?.ToString(),1); _u.SetValue(_7.GetBornOn()?.ToString("O"),2); _u.SetValue(_7.GetDistinguishedName(),3); _u.SetValue(_7.GetID(),4); _u.SetValue(_7.GetModified()?.ToString("O"),5); _u.SetValue(_7.GetName(),6); _u.SetValue(((Object)_7).GetType().Name,7); _u.SetValue(_7.GetServiceVersion()?.ToString(),8); _u.SetValue(_7.GetType(),9); _u.SetValue(_7.GetVersion()?.ToString(),10); }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[4]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/method[@name="AddUpdateDescriptor"]/*'/>*/
    internal Boolean AddUpdate(Descriptor? descriptor)
    {
        if(descriptor is null) { return false; }

        Descriptor _d = descriptor; Object[] _u = new Object[11];

        List<String> _n = new List<String>(){typeof(GuidReferenceItem).Name,typeof(TextItem).Name,typeof(CodeItem).Name,typeof(GenericItem).Name,typeof(BinaryItem).Name,typeof(MultiMediaItem).Name,typeof(MSBuildItem).Name};

        try
        {
            if(_n.Contains(_d.ObjectType!)) { _u.SetValue(_d.Application,0); _u.SetValue(_d.ApplicationVersion,1); _u.SetValue(_d.BornOn,2); _u.SetValue(_d.DistinguishedName,3); _u.SetValue(_d.ID,4); _u.SetValue(_d.Modified,5); _u.SetValue(_d.Name,6); _u.SetValue(_d.ObjectType,7); _u.SetValue(_d.ServiceVersion,8); _u.SetValue(_d.Type,9); _u.SetValue(_d.Version,10); }
            else                            { _u.SetValue(_d.Application,0); _u.SetValue(_d.ApplicationVersion,1); _u.SetValue(_d.BornOn,2); _u.SetValue(_d.DistinguishedName,3); _u.SetValue(_d.ID,4); _u.SetValue(_d.Modified,5); _u.SetValue(_d.Name,6); _u.SetValue(_d.ObjectType,7); _u.SetValue(_d.ServiceVersion,8); _u.SetValue(_d.ObjectType,9); _u.SetValue(_d.Version,10); }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[4]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/method[@name="Exists"]/*'/>*/
    internal Boolean? Exists(Guid? id)
    {
        if(id is null) { return null; } if(Equals(this.Rows.Count,0)) { return false; }

        try
        {
            return this.Rows.Cast<DataRow>().Any(_=>Guid.Equals(_["ID"],id));
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/method[@name="Remove"]/*'/>*/
    internal Boolean Remove(String? it)
    {
        if(it is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Tool? _0 = null; GuidReferenceItem? _1 = null; GenericItem? _2 = null; TextItem? _3 = null; CodeItem? _4 = null; MSBuildItem? _5 = null; BinaryItem? _6 = null; MultiMediaItem? _7 = null;

        Object[] _u = new Object[11];

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

            return false;

            Remove:
            if(_0 is not null) { _u.SetValue(_0.GetID(),4); }
            if(_1 is not null) { _u.SetValue(_1.GetID(),4); }
            if(_2 is not null) { _u.SetValue(_2.GetID(),4); }
            if(_3 is not null) { _u.SetValue(_3.GetID(),4); }
            if(_4 is not null) { _u.SetValue(_4.GetID(),4); }
            if(_5 is not null) { _u.SetValue(_5.GetID(),4); }
            if(_6 is not null) { _u.SetValue(_6.GetID(),4); }
            if(_7 is not null) { _u.SetValue(_7.GetID(),4); }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[4]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/method[@name="RemoveDescriptor"]/*'/>*/
    internal Boolean Remove(Descriptor? descriptor)
    {
        if(descriptor is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[11];

        try
        {
            _u.SetValue(descriptor.ID,4);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[4]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='Elements.xml' path='Elements/class[@name="Elements"]/method[@name="RemoveGuid"]/*'/>*/
    internal Boolean Remove(Guid? id)
    {
        if(id is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[11];

        try
        {
            _u.SetValue(id,4);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[4]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }
}