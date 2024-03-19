namespace KusDepot;

/**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/main/*'/>*/
internal sealed class ActiveServices : DataTable
{
    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/constructor[@name="Constructor"]/*'/>*/
    internal ActiveServices(String? name) : base(name)
    {
        DataColumn? _0;

        _0 = this.Columns.Add("ActorID",typeof(Guid));              _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ActorNameID",typeof(String));        _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Application",typeof(String));        _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ApplicationVersion",typeof(String)); _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("BornOn",typeof(String));             _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("DistinguishedName",typeof(String));  _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ID",typeof(Guid));                   _0.AllowDBNull = false;_0.DefaultValue = null; _0.Unique = true;

        _0 = this.Columns.Add("Interfaces",typeof(String));         _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Modified",typeof(String));           _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Name",typeof(String));               _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Purpose",typeof(String));            _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Registered",typeof(String));         _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ServiceVersion",typeof(String));     _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Url",typeof(String));                _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Version",typeof(String));            _0.AllowDBNull = true; _0.DefaultValue = null;

        this.PrimaryKey = new DataColumn[] { this.Columns["ID"]! };
    }

    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/method[@name="AddUpdate"]/*'/>*/
    internal Boolean AddUpdate(String? it)
    {
        if(it is null) { return false; }

        Tool? _0 = null; Object[] _u = new Object[15];

        try
        {
            if(Tool.TryParse(it,null,out _0))
            {
                if(_0 is not null)
                {
                    _u.SetValue(_0.GetApplication(),2);
                    _u.SetValue(_0.GetApplicationVersion()?.ToString(),3);
                    _u.SetValue(_0.GetBornOn()?.ToString("O"),4);
                    _u.SetValue(_0.GetDistinguishedName(),5);
                    _u.SetValue(_0.GetID(),6);
                    _u.SetValue(typeof(ITool).ToString(),7);
                    _u.SetValue(_0.GetModified()?.ToString("O"),8);
                    _u.SetValue(_0.GetName(),9);
                    _u.SetValue(_0.GetPurpose(),10);
                    _u.SetValue(DateTimeOffset.Now.ToString("O"),11);
                    _u.SetValue(_0.GetServiceVersion()?.ToString(),12);
                    _u.SetValue(_0.GetLocator(),13);
                    _u.SetValue(_0.GetVersion()?.ToString(),14);
                }
            }
            else { return false; }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[6]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/method[@name="AddUpdateDescriptor"]/*'/>*/
    internal Boolean AddUpdate(Descriptor? descriptor)
    {
        if(descriptor is null || !String.Equals(descriptor.ObjectType,typeof(Tool).Name,StringComparison.Ordinal)) { return false; }

        Descriptor _d = descriptor; Object[] _u = new Object[15];

        try
        {
            _u.SetValue(_d.Application,2);
            _u.SetValue(_d.ApplicationVersion,3);
            _u.SetValue(_d.BornOn,4);
            _u.SetValue(_d.DistinguishedName,5);
            _u.SetValue(_d.ID,6);
            _u.SetValue(typeof(ITool).ToString(),7);
            _u.SetValue(_d.Modified,8);
            _u.SetValue(_d.Name,9);
            _u.SetValue(_d.Purpose,10);
            _u.SetValue(DateTimeOffset.Now.ToString("O"),11);
            _u.SetValue(_d.ServiceVersion,12);
            _u.SetValue(_d.Locator,13);
            _u.SetValue(_d.Version,14);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[6]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/method[@name="Exists"]/*'/>*/
    internal Boolean? Exists(Guid? id)
    {
        if(id is null) { return null; } if(Equals(this.Rows.Count,0)) { return false; }

        try
        {
            return this.Rows.Cast<DataRow>().Any(_=>Guid.Equals(_["ID"],id));
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/method[@name="Remove"]/*'/>*/
    internal Boolean Remove(String? it)
    {
        if(it is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Tool? _0 = null; Object[] _u = new Object[15];

        try
        {
            if(Tool.TryParse(it,null,out _0))
            {
                if(_0 is not null) { _u.SetValue(_0.GetID(),6); }
            }
            else { return false; }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[6]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/method[@name="RemoveDescriptor"]/*'/>*/
    internal Boolean Remove(Descriptor? descriptor)
    {
        if(descriptor is null || !String.Equals(descriptor.ObjectType,typeof(Tool).Name,StringComparison.Ordinal)) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[15];

        try
        {
            _u.SetValue(descriptor.ID,6);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[6]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='ActiveServices.xml' path='ActiveServices/class[@name="ActiveServices"]/method[@name="RemoveGuid"]/*'/>*/
    internal Boolean Remove(Guid? id)
    {
        if(id is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[15];

        try
        {
            _u.SetValue(id,6);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[6]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }
}