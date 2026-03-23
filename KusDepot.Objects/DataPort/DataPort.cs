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

            Type it = item.GetType(); if(it == typeof(GenericItem) || it == typeof(MSBuildItem)) { return false; }

            String? dir = Path.GetDirectoryName(path); if(String.IsNullOrEmpty(dir)) { dir = Environment.CurrentDirectory; }

            String? fn = Path.GetFileName(path); if(String.IsNullOrEmpty(fn)) { fn = item.GetName() ?? item.GetID().ToString(); }

            if(String.IsNullOrEmpty(fn)) { return false; } String? output = null; output = Path.Combine(dir,fn); if(File.Exists(output)) { return false; }

            Byte[]? bytes = null;

            switch(item)
            {
                case BinaryItem b: { bytes = b.GetContent(); break; }

                case MultiMediaItem m: { bytes = m.GetContent(); break; }

                case TextItem t: { bytes = t.GetContent().ToByteArrayFromUTF16String(); break; }

                case CodeItem c: { bytes = c.GetContent().ToByteArrayFromUTF16String(); break; }

                case GuidReferenceItem g: { bytes = g.GetContent().HasValue ? g.GetContent()!.Value.ToByteArray() : null; break; }

                default: break;
            }

            if(bytes is null) { bytes = Array.Empty<Byte>(); }

            FileStream stream = new FileStream(output,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Options = FileOptions.Asynchronous, Share = FileShare.None});

            ValueTask task = stream.WriteAsync(bytes.AsMemory(0,bytes.Length),cancel); await task.ConfigureAwait(false);

            if(task.IsCompletedSuccessfully && stream.Length == bytes.Length) { await stream.DisposeAsync().ConfigureAwait(false); return true; }

            await stream.DisposeAsync().ConfigureAwait(false); File.Delete(output); return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ExportFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Import"]/*'/>*/
    public static async Task<DataItem?> Import(String? path , Action<DataItem>? configure = null , CancellationToken cancel = default)
    {
        try
        {
            if(path is null || !File.Exists(path)) { return null; } Boolean contentStreamed = default; Byte[] bytes = Array.Empty<Byte>();

            using FileStream stream = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read}); if(stream.Length > DataImportBufferLimit) { contentStreamed = true; }

            if(contentStreamed is false) { bytes = new Byte[stream.Length]; if(await stream.ReadAsync(bytes.AsMemory(0,bytes.Length),cancel).ConfigureAwait(false) != stream.Length) {  return null; } }

            String name = Path.GetFileName(path); String extension = Path.GetExtension(path).TrimStart('.').ToUpperInvariant(); String? datatype = typeof(DataTypes).GetField(extension)?.GetValue(null)?.ToString();

            String? security = null; if(OperatingSystem.IsWindows()) { security = new FileSecurity(path,AccessControlSections.All).GetSecurityDescriptorSddlForm(AccessControlSections.All); }

            DataItem? item = null;

            switch(datatype)
            {
                case var t when String.Equals(t,DataTypes.GUID,Ordinal):
                {
                    GuidReferenceItem gri = new();
                    gri.SetContent(new Guid(bytes));
                    item = gri;
                    break;
                }

                case var t when DataValidation.IsValidCodeDataType(t):
                {
                    CodeItem ci = new();
                    if(contentStreamed is true) { if(await ci.SetContentStreamed(true,cancel:cancel) is false) { return null; } } else { ci.SetContent(bytes.TryGetStringFromByteArray()); }
                    item = ci;
                    break;
                }

                case var t when DataValidation.IsValidTextDataType(t):
                {
                    TextItem ti = new();
                    if(contentStreamed is true) { if(await ti.SetContentStreamed(true,cancel:cancel) is false) { return null; } } else { ti.SetContent(bytes.TryGetStringFromByteArray()); }
                    item = ti;
                    break;
                }

                case var t when DataValidation.IsValidMultiMediaDataType(t):
                {
                    MultiMediaItem mi = new();
                    if(contentStreamed is true) { if(await mi.SetContentStreamed(true,cancel:cancel) is false) { return null; } } else { mi.SetContent(bytes); }
                    item = mi;
                    break;
                }

                case var t when DataValidation.IsValidBinaryDataType(t):
                {
                    BinaryItem bi = new();
                    if(contentStreamed is true) { if(await bi.SetContentStreamed(true,cancel:cancel) is false) { return null; } } else { bi.SetContent(bytes); }
                    item = bi;
                    break;
                }

                default: { return null; }
            }

            item.SetFILE(path); item.SetName(name); item.SetSecurityDescriptor(security); item.SetDataType(datatype);

            configure?.Invoke(item);

            return item;
        }
        catch ( OutOfMemoryException _ ) { KusDepotLog.Error(_,ImportFail); throw; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ImportFail); if(NoExceptions) { return null; } throw; }
    }
}