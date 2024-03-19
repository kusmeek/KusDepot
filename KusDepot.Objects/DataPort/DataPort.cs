namespace KusDepot;

/**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/main/*'/>*/
public static class DataPort
{
    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Export"]/*'/>*/
    public static async Task<Boolean> Export(DataItem? item , String? path=null , CancellationToken? token = null)
    {
        try
        {
            if(item is null || File.Exists(path)) { return false; }

            Type _it = ((Object)item).GetType(); if(_it == typeof(GenericItem) || _it == typeof(MSBuildItem)) { return false; }

            String? _dir = Path.GetDirectoryName(path); if(String.IsNullOrEmpty(_dir)) { _dir = Environment.CurrentDirectory; }

            String? _fn = Path.GetFileName(path); if(String.IsNullOrEmpty(_fn)) { _fn = item.GetName() ?? item.GetID().ToString(); }

            if(String.IsNullOrEmpty(_fn)) { return false; } String? _out = null; _out = Path.Combine(_dir,_fn); if(File.Exists(_out)) { return false; }

            Byte[]? _b = null;

            switch(item)
            {
                case BinaryItem b: { _b = b.GetContent(); break; }

                case MultiMediaItem m: { _b = m.GetContent(); break; }

                case TextItem t: { _b = t.GetContent().ToByteArrayFromUTF16String(); break; }

                case CodeItem c: { _b = c.GetContent().ToByteArrayFromUTF16String(); break; }

                case GuidReferenceItem g: { _b = g.GetContent().HasValue ? g.GetContent()!.Value.ToByteArray() : null; break; }
            }

            if(_b is null) { _b = Array.Empty<Byte>(); }

            FileStream _s = new FileStream(_out,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Options = FileOptions.Asynchronous, Share = FileShare.None});

            ValueTask _t = _s.WriteAsync(_b.AsMemory(0,_b.Length),token.GetValueOrDefault()); await _t.ConfigureAwait(false);

            if(_t.IsCompletedSuccessfully && _s.Length == _b.Length) { await _s.DisposeAsync().ConfigureAwait(false); return true; }

            await _s.DisposeAsync().ConfigureAwait(false); File.Delete(_out); return false;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return false; } throw; }
    }

    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Import"]/*'/>*/
    public static async Task<DataItem?>? Import(String? path , CancellationToken? token = null)
    {
        try
        {
            if(path is null || !File.Exists(path)) { return null; }
                        
            FileStream _s = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            Byte[] _b = new Byte[_s.Length]; if(await _s.ReadAsync(_b.AsMemory(0,_b.Length),token.GetValueOrDefault()).ConfigureAwait(false) != _s.Length) { await _s.DisposeAsync().ConfigureAwait(false); return null; } await _s.DisposeAsync().ConfigureAwait(false);

            String _n = Path.GetFileName(path); String _e = Path.GetExtension(path).TrimStart('.').ToUpperInvariant(); String? _t = typeof(DataType).GetField(_e)?.GetValue(null)?.ToString();

            Dictionary<String,Byte[]> _h = new Dictionary<String,Byte[]>(); _h.Add("Content",SHA512.HashData(_b));

            FileSecurity? _se = null; String? _sd = null; if(OperatingSystem.IsWindows()) { _se = new FileSecurity(path,AccessControlSections.All); _sd = _se.GetSecurityDescriptorSddlForm(AccessControlSections.All); }

            if(Settings.GuidReferenceItemValidDataTypes.Contains(_t))
            {
                Guid _g = new Guid(_b);

                GuidReferenceItem _gri = new GuidReferenceItem(); _gri.SetContent(_g); _gri.SetFILE(path); _gri.SetName(_n); _gri.SetSecureHashes(_h); _gri.SetSecurityDescriptor(_sd); _gri.SetType(_t);

                if(GuidReferenceItem.Validate(_gri)) { return _gri; } return null;
            }

            if(Settings.CodeItemValidDataTypes.Contains(_t))
            {
                String _cc = _b.TryGetStringFromByteArray();

                CodeItem _ci = new CodeItem(); _ci.SetContent(_cc); _ci.SetFILE(path); _ci.SetName(_n); _ci.SetSecureHashes(_h); _ci.SetSecurityDescriptor(_sd); _ci.SetType(_t);

                if(CodeItem.Validate(_ci)) { return _ci; } return null;
            }

            if(Settings.TextItemValidDataTypes.Contains(_t))
            {
                String _tc = _b.TryGetStringFromByteArray();

                TextItem _ti = new TextItem(); _ti.SetContent(_tc); _ti.SetFILE(path); _ti.SetName(_n); _ti.SetSecureHashes(_h); _ti.SetSecurityDescriptor(_sd); _ti.SetType(_t);

                if(TextItem.Validate(_ti)) { return _ti; } return null;
            }

            if(Settings.MultiMediaItemValidDataTypes.Contains(_t))
            {
                MultiMediaItem _mi = new MultiMediaItem(); _mi.SetContent(_b); _mi.SetFILE(path); _mi.SetName(_n); _mi.SetSecureHashes(_h); _mi.SetSecurityDescriptor(_sd); _mi.SetType(_t);

                if(MultiMediaItem.Validate(_mi)) { return _mi; } return null;
            }

            if(Settings.BinaryItemValidDataTypes.Contains(_t))
            {
                BinaryItem _bi = new BinaryItem(); _bi.SetContent(_b); _bi.SetFILE(path); _bi.SetName(_n); _bi.SetSecureHashes(_h); _bi.SetSecurityDescriptor(_sd); _bi.SetType(_t);

                if(BinaryItem.Validate(_bi)) { return _bi; } return null;
            }

            return null;
        }
        catch ( OutOfMemoryException ) { throw; }

        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }
}