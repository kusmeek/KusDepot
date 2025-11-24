namespace KusDepot;

/**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/main/*'/>*/
public static class DataPort
{
    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Export"]/*'/>*/
    public static async Task<Boolean> Export(DataItem? item , String? path = null , CancellationToken cancel = default)
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

                default: break;
            }

            if(_b is null) { _b = Array.Empty<Byte>(); }

            FileStream _s = new FileStream(_out,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Options = FileOptions.Asynchronous, Share = FileShare.None});

            ValueTask _t = _s.WriteAsync(_b.AsMemory(0,_b.Length),cancel); await _t.ConfigureAwait(false);

            if(_t.IsCompletedSuccessfully && _s.Length == _b.Length) { await _s.DisposeAsync().ConfigureAwait(false); return true; }

            await _s.DisposeAsync().ConfigureAwait(false); File.Delete(_out); return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ExportFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Import"]/*'/>*/
    public static async Task<DataItem?>? Import(String? path , CancellationToken cancel = default)
    {
        try
        {
            if(path is null || !File.Exists(path)) { return null; } Boolean cs = default; Byte[] _b = Array.Empty<Byte>();

            using FileStream _s = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read}); if(_s.Length > DataImportBufferLimit) { cs = true; }

            if(cs is false) { _b = new Byte[_s.Length]; if(await _s.ReadAsync(_b.AsMemory(0,_b.Length),cancel).ConfigureAwait(false) != _s.Length) {  return null; } }

            String _n = Path.GetFileName(path); String _e = Path.GetExtension(path).TrimStart('.').ToUpperInvariant(); String? _t = typeof(DataType).GetField(_e)?.GetValue(null)?.ToString();

            String? _sd = null; if(OperatingSystem.IsWindows()) { _sd = new FileSecurity(path,AccessControlSections.All).GetSecurityDescriptorSddlForm(AccessControlSections.All); }

            DataItem? item = null;

            switch(_t)
            {
                case var t when String.Equals(t,DataType.GUID,Ordinal):
                {
                    GuidReferenceItem _gri = new();
                    _gri.SetContent(new Guid(_b));
                    item = _gri;
                    break;
                }

                case var t when DataValidation.IsValidCodeDataType(t):
                {
                    CodeItem _ci = new();
                    if(cs is true) { if(_ci.SetContentStreamed(true) is false) { return null; } } else { _ci.SetContent(_b.TryGetStringFromByteArray()); }
                    item = _ci;
                    break;
                }

                case var t when DataValidation.IsValidTextDataType(t):
                {
                    TextItem _ti = new();
                    if(cs is true) { if(_ti.SetContentStreamed(true) is false) { return null; } } else { _ti.SetContent(_b.TryGetStringFromByteArray()); }
                    item = _ti;
                    break;
                }

                case var t when DataValidation.IsValidMultiMediaDataType(t):
                {
                    MultiMediaItem _mi = new();
                    if(cs is true) { if(_mi.SetContentStreamed(true) is false) { return null; } } else { _mi.SetContent(_b); }
                    item = _mi;
                    break;
                }

                case var t when DataValidation.IsValidBinaryDataType(t):
                {
                    BinaryItem _bi = new();
                    if(cs is true) { if(_bi.SetContentStreamed(true) is false) { return null; } } else { _bi.SetContent(_b); }
                    item = _bi;
                    break;
                }

                default: { return null; }
            }

            item.SetFILE(path); item.SetName(_n); item.SetSecurityDescriptor(_sd); item.SetType(_t);

            return item;
        }
        catch ( OutOfMemoryException _ ) { KusDepotLog.Error(_,ImportFail); throw; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ImportFail); if(NoExceptions) { return null; } throw; }
    }
}