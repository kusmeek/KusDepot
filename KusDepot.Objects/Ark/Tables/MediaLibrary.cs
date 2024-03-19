namespace KusDepot;

/**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/main/*'/>*/
internal sealed class MediaLibrary : DataTable
{
    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/constructor[@name="Constructor"]/*'/>*/
    internal MediaLibrary(String? name) : base(name)
    {
        DataColumn? _0;

        _0 = this.Columns.Add("Album",typeof(String));             _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Application",typeof(String));       _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Artist",typeof(String));            _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("BornOn",typeof(String));            _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("DistinguishedName",typeof(String)); _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("ID",typeof(Guid));                  _0.AllowDBNull = false;_0.DefaultValue = null; _0.Unique = true;

        _0 = this.Columns.Add("Modified",typeof(String));          _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Name",typeof(String));              _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Title",typeof(String));             _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Type",typeof(String));              _0.AllowDBNull = true; _0.DefaultValue = null;

        _0 = this.Columns.Add("Year",typeof(String));              _0.AllowDBNull = true; _0.DefaultValue = null;

        this.PrimaryKey = new DataColumn[] { this.Columns["ID"]! };
    }

    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/method[@name="AddUpdate"]/*'/>*/
    internal Boolean AddUpdate(String? it)
    {
        if(it is null) { return false; }

        MultiMediaItem? _0 = null; Object[] _u = new Object[11];

        try
        {
            if(MultiMediaItem.TryParse(it,null,out _0))
            {
                if(_0 is not null)
                {
                    _u.SetValue(_0.GetApplication(),1);
                    _u.SetValue(_0.GetArtists()?.FirstOrDefault(),2);
                    _u.SetValue(_0.GetBornOn()?.ToString("O"),3);
                    _u.SetValue(_0.GetDistinguishedName(),4);
                    _u.SetValue(_0.GetID(),5);
                    _u.SetValue(_0.GetModified()?.ToString("O"),6);
                    _u.SetValue(_0.GetName(),7);
                    _u.SetValue(_0.GetTitle(),8);
                    _u.SetValue(_0.GetType(),9);
                    _u.SetValue(_0.GetYear()?.ToString(CultureInfo.InvariantCulture),10);
                }
            }
            else { return false; }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[5]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/method[@name="AddUpdateDescriptor"]/*'/>*/
    internal Boolean AddUpdate(Descriptor? descriptor)
    {
        if(descriptor is null || !String.Equals(descriptor.ObjectType,typeof(MultiMediaItem).Name,StringComparison.Ordinal)) { return false; }

        Descriptor? _d = descriptor; Object[] _u = new Object[11];

        try
        {
            _u.SetValue(_d.Application,1);
            _u.SetValue(_d.Artist,2);
            _u.SetValue(_d.BornOn,3);
            _u.SetValue(_d.DistinguishedName,4);
            _u.SetValue(_d.ID,5);
            _u.SetValue(_d.Modified,6);
            _u.SetValue(_d.Name,7);
            _u.SetValue(_d.Title,8);
            _u.SetValue(_d.Type,9);
            _u.SetValue(_d.Year,10);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[5]));

            if(_r is not null) { _r.ItemArray = _u; this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            _r = this.NewRow(); _r.ItemArray = _u; this.Rows.Add(_r); _r.AcceptChanges(); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true;
        }
        catch ( NoNullAllowedException ) { return false; }

        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/method[@name="Exists"]/*'/>*/
    internal Boolean? Exists(Guid? id)
    {
        if(id is null) { return null; } if(Equals(this.Rows.Count,0)) { return false; }

        try
        {
            return this.Rows.Cast<DataRow>().Any(_=>Guid.Equals(_["ID"],id));
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/method[@name="Remove"]/*'/>*/
    internal Boolean Remove(String? it)
    {
        if(it is null) { return false; }

        if(this.Rows.Count == 0) { return true; }

        MultiMediaItem? _0 = null; Object[] _u = new Object[11];

        try
        {
            if(MultiMediaItem.TryParse(it,null,out _0))
            {
                if(_0 is not null) { _u.SetValue(_0.GetID(),5); }
            }
            else { return false; }

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[5]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/method[@name="RemoveDescriptor"]/*'/>*/
    internal Boolean Remove(Descriptor? descriptor)
    {
        if(descriptor is null || !String.Equals(descriptor.ObjectType,typeof(MultiMediaItem).Name,StringComparison.Ordinal)) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[11];

        try
        {
            _u.SetValue(descriptor.ID,5);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[5]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='MediaLibrary.xml' path='MediaLibrary/class[@name="MediaLibrary"]/method[@name="RemoveGuid"]/*'/>*/
    internal Boolean Remove(Guid? id)
    {
        if(id is null) { return false; } if(this.Rows.Count == 0) { return true; }

        Object[] _u = new Object[11];

        try
        {
            _u.SetValue(id,5);

            DataRow? _r = this.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],_u[5]));

            if(_r is not null) { this.Rows.Remove(_r); this.AcceptChanges(); this.DataSet!.AcceptChanges(); return true; }

            return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }
}