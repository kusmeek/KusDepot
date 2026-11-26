namespace KusDepot;

/**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/main/*'/>*/
public static class DataPort
{
    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Export"]/*'/>*/
    public static async Task<Boolean> Export(DataItem? item , String? path = null , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        String? staging = null;
        Stream? source = null;
        Boolean streamed = false;
        Boolean encrypted = false;
        Byte[]? bytes = null;
        try
        {
            if(item is null || File.Exists(path)) { return false; }

            cancel.ThrowIfCancellationRequested();

            if(item is not (BinaryItem or MultiMediaItem or TextItem or CodeItem or GuidReferenceItem)) { return false; }

            String? dir = Path.GetDirectoryName(path); if(String.IsNullOrEmpty(dir)) { dir = Environment.CurrentDirectory; }

            String? fn = Path.GetFileName(path); if(String.IsNullOrEmpty(fn)) { fn = item.GetName() ?? item.GetID().ToString(); }

            if(String.IsNullOrEmpty(fn)) { return false; } String? output = null; output = Path.Combine(dir,fn); if(File.Exists(output)) { return false; }

            streamed = item.GetContentStreamed(); encrypted = item.GetDataEncrypted(); DataItem? stagedItem = null; DataContent? protectedContent = null;

            if(streamed)
            {
                if(item is GuidReferenceItem) { return false; }

                source = item.GetContentStream(); if(source is null) { return false; }

                if(encrypted)
                {
                    if(security is null) { return false; }

                    stagedItem = item.Clone(); if(stagedItem is null) { return false; }
                }
            }
            else
            {
                if(encrypted)
                {
                    if(security is null) { return false; }

                    protectedContent = await item.GetDataContent(security,cancel).ConfigureAwait(false); if(protectedContent is null) { return false; }
                }

                switch(item)
                {
                    case BinaryItem b:
                    {
                        bytes = encrypted ? protectedContent!.Binary : b.GetContent();

                        if(encrypted && bytes is null) { return false; }

                        break;
                    }
                    case MultiMediaItem m:
                    {
                        if(encrypted)
                        {
                            if(protectedContent!.MultiMedia is null || !protectedContent.MultiMedia.TryGetValue("Content",out Object? content) || content is not Byte[] binary) { return false; }

                            bytes = binary;
                        }
                        else { bytes = m.GetContent(); }

                        break;
                    }
                    case TextItem t:
                    {
                        String? content = encrypted ? protectedContent!.Text : t.GetContent();

                        if(encrypted && content is null) { return false; }

                        bytes = new UTF8Encoding(false,true).GetBytes(content ?? String.Empty);

                        break;
                    }
                    case CodeItem c:
                    {
                        String? content = encrypted ? protectedContent!.Code : c.GetContent();

                        if(encrypted && content is null) { return false; }

                        bytes = new UTF8Encoding(false,true).GetBytes(content ?? String.Empty);

                        break;
                    }
                    case GuidReferenceItem g:
                    {
                        if(encrypted)
                        {
                            if(protectedContent!.GuidReference is null || !protectedContent.GuidReference.TryGetValue("Content",out Object? content) || content is not null and not Guid) { return false; }

                            bytes = content is Guid id ? id.ToByteArray() : Array.Empty<Byte>();
                        }
                        else
                        {
                            Guid? id = g.GetContent(); bytes = id.HasValue ? id.Value.ToByteArray() : null;
                        }
                        break;
                    }
                    default: return false;
                }
            }

            if(bytes is null) { bytes = Array.Empty<Byte>(); }

            output = Path.GetFullPath(output);

            String temporary = Path.Combine(Path.GetDirectoryName(output)!, $".kusdepot-export-{Guid.NewGuid():N}.tmp");

            await using(FileStream stream = new(temporary,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Options = FileOptions.Asynchronous, Share = FileShare.None}))
            {
                staging = temporary;

                if(source is not null)
                {
                    Int64? sourceLength = source.CanSeek ? source.Length : null;

                    await source.CopyToAsync(stream,cancel).ConfigureAwait(false);

                    if(sourceLength.HasValue && stream.Length != sourceLength.Value) { return false; }
                }
                else
                {
                    await stream.WriteAsync(bytes.AsMemory(),cancel).ConfigureAwait(false);

                    if(stream.Length != bytes.Length) { return false; }
                }

                await stream.FlushAsync(cancel).ConfigureAwait(false);
            }

            if(source is not null)
            {
                Stream input = source; source = null;

                await input.DisposeAsync().ConfigureAwait(false);
            }

            if(stagedItem is not null)
            {
                if(!stagedItem.SetFILE(staging) || !await stagedItem.DecryptData(security,cancel).ConfigureAwait(false)) { return false; }
            }

            cancel.ThrowIfCancellationRequested();

            File.Move(staging,output,overwrite:false); staging = null; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ExportFail); if(NoExceptions) { return false; } throw; }
        finally
        {
            source?.Dispose();

            if(encrypted && !streamed && bytes is not null) { ZeroMemory(bytes); }

            if(staging is not null)
            {
                try { File.Delete(staging); } catch ( Exception _ ) { KusDepotLog.Error(_,ExportFail); }
            }
        }
    }

    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="Import"]/*'/>*/
    public static async Task<DataItem?> Import(String? path , Action<DataItem>? configure = null , CancellationToken cancel = default)
    {
        try
        {
            if(path is null || !File.Exists(path)) { return null; } Boolean contentStreamed = default; Byte[] bytes = Array.Empty<Byte>();

            using FileStream stream = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read}); if(stream.Length > DataImportBufferLimit) { contentStreamed = true; }

            if(contentStreamed is false) { bytes = new Byte[stream.Length]; if(await stream.ReadAsync(bytes.AsMemory(0,bytes.Length),cancel).ConfigureAwait(false) != stream.Length) { return null; } }

            String name = Path.GetFileName(path); String extension = Path.GetExtension(path).TrimStart('.').ToUpperInvariant(); String? datatype = typeof(DataTypes).GetField(extension)?.GetValue(null)?.ToString();

            if(contentStreamed && (DataValidation.IsValidCodeDataType(datatype) || DataValidation.IsValidTextDataType(datatype)))
            {
                await ValidateTextEncodingAsync(stream,cancel).ConfigureAwait(false);
            }

            String? security = null;

            if(OperatingSystem.IsWindows())
            {
                try { security = new FileSecurity(path,AccessControlSections.All).GetSecurityDescriptorSddlForm(AccessControlSections.All); }

                catch ( PrivilegeNotHeldException ) { security = new FileSecurity(path,AccessControlSections.Access|AccessControlSections.Owner|AccessControlSections.Group).GetSecurityDescriptorSddlForm(AccessControlSections.Access|AccessControlSections.Owner|AccessControlSections.Group); }
            }

            DataItem? item = null;

            switch(datatype)
            {
                case var t when String.Equals(t,DataTypes.GUID,Ordinal):
                {
                    GuidReferenceItem gri = new();

                    if(gri.SetFILE(path) is false) { return null; }

                    gri.SetContent(new Guid(bytes),null);

                    item = gri;

                    break;
                }

                case var t when DataValidation.IsValidCodeDataType(t):
                {
                    CodeItem ci = new();

                    if(ci.SetFILE(path) is false) { return null; }

                    if(contentStreamed is true) { if(await ci.SetContentStreamed(true,null,cancel).ConfigureAwait(false) is false) { return null; } } else { ci.SetContent(bytes.TryGetStringFromByteArray(),null); }

                    item = ci;

                    break;
                }

                case var t when DataValidation.IsValidTextDataType(t):
                {
                    TextItem ti = new();

                    if(ti.SetFILE(path) is false) { return null; }

                    if(contentStreamed is true) { if(await ti.SetContentStreamed(true,null,cancel).ConfigureAwait(false) is false) { return null; } } else { ti.SetContent(bytes.TryGetStringFromByteArray(),null); }

                    item = ti;

                    break;
                }

                case var t when DataValidation.IsValidMultiMediaDataType(t):
                {
                    MultiMediaItem mi = new();

                    if(mi.SetFILE(path) is false) { return null; }

                    if(contentStreamed is true) { if(await mi.SetContentStreamed(true,null,cancel).ConfigureAwait(false) is false) { return null; } } else { mi.SetContent(bytes,null); }

                    item = mi;

                    break;
                }

                case var t when DataValidation.IsValidBinaryDataType(t):
                {
                    BinaryItem bi = new();

                    if(bi.SetFILE(path) is false) { return null; }

                    if(contentStreamed is true) { if(await bi.SetContentStreamed(true,null,cancel).ConfigureAwait(false) is false) { return null; } } else { bi.SetContent(bytes,null); }

                    item = bi;

                    break;
                }

                default: { return null; }
            }

            if(item is null) { return null; }

            item.SetName(name); item.SetSecurityDescriptor(security); item.SetDataType(datatype);

            configure?.Invoke(item);

            return item;
        }
        catch ( OutOfMemoryException _ ) { KusDepotLog.Error(_,ImportFail); throw; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ImportFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataPort.xml' path='DataPort/class[@name="DataPort"]/method[@name="ValidateTextEncodingAsync"]/*'/>*/
    private static async Task ValidateTextEncodingAsync(FileStream stream , CancellationToken cancel)
    {
        Byte[] preamble = new Byte[4]; Int32 preambleLength = 0;

        while(preambleLength < preamble.Length)
        {
            Int32 read = await stream.ReadAsync(preamble.AsMemory(preambleLength),cancel).ConfigureAwait(false);

            if(read == 0) { break; }

            preambleLength += read;
        }

        Encoding encoding = ResolveTextEncoding(preamble.AsSpan(0,preambleLength),out Int32 offset);

        try
        {
            stream.Position = offset;

            Decoder decoder = encoding.GetDecoder();

            Byte[] input = new Byte[8*KiB];

            Char[] output = new Char[encoding.GetMaxCharCount(input.Length)];

            while(true)
            {
                Int32 read = await stream.ReadAsync(input.AsMemory(),cancel).ConfigureAwait(false);

                if(read == 0) { break; }

                Int32 consumed = 0;

                while(consumed < read)
                {
                    decoder.Convert(input,consumed,read - consumed,output,0,output.Length,false,out Int32 bytesUsed,out _,out _);

                    if(bytesUsed == 0) { throw new FormatException(InvalidTextEncoding); }

                    consumed += bytesUsed;
                }
            }

            decoder.Convert(Array.Empty<Byte>(),0,0,output,0,output.Length,true,out _,out _,out _);
        }
        catch ( DecoderFallbackException _ ) { throw new FormatException(InvalidTextEncoding,_); }
    }
}