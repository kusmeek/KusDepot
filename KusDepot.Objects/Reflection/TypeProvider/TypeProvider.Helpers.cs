namespace KusDepot.Reflection;

public sealed partial class TypeProvider
{
    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="AssemblyNamesEqual"]/*'/>*/
    private static Boolean AssemblyNamesEqual(AssemblyName left , AssemblyName right)
    {
        try
        {
            if(String.Equals(left.Name,right.Name,Ordinal) is false) { return false; }

            if(Equals(left.Version,right.Version) is false) { return false; }

            if(String.Equals(left.CultureName,right.CultureName,Ordinal) is false) { return false; }

            return String.Equals(GetPublicKeyTokenText(left),GetPublicKeyTokenText(right),Ordinal);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AssemblyNamesEqualFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="GetCultureText"]/*'/>*/
    private static String GetCultureText(AssemblyName assemblyname)
    {
        try { return String.IsNullOrWhiteSpace(assemblyname.CultureName) ? "neutral" : assemblyname.CultureName; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetCultureTextFail); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="GetPublicKeyTokenText"]/*'/>*/
    private static String GetPublicKeyTokenText(AssemblyName assemblyname)
    {
        try
        {
            Byte[]? t = assemblyname.GetPublicKeyToken(); if(t is null || Equals(t.Length,0)) { return "null"; }

            return Convert.ToHexStringLower(t);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetPublicKeyTokenTextFail); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="MatchesRequestAssembly"]/*'/>*/
    private static Boolean MatchesRequestAssembly(TypeRequest request , AssemblyName assemblyname)
    {
        try
        {
            if(request.HasAssemblyIdentity is false) { return true; }

            if(String.Equals(assemblyname.Name,request.AssemblyName,Ordinal) is false) { return false; }

            if(request.HasVersion && Equals(assemblyname.Version,request.Version) is false) { return false; }

            if(request.HasCulture && String.Equals(GetCultureText(assemblyname),request.Culture,Ordinal) is false) { return false; }

            if(request.HasPublicKeyToken && String.Equals(GetPublicKeyTokenText(assemblyname),request.PublicKeyToken,Ordinal) is false) { return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,MatchesRequestAssemblyFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="MatchesRequestedAssembly"]/*'/>*/
    private static Boolean MatchesRequestedAssembly(AssemblyName candidate , AssemblyName requested)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(requested.Name) || String.Equals(candidate.Name,requested.Name,Ordinal) is false) { return false; }

            if(requested.Version is not null && Equals(candidate.Version,requested.Version) is false) { return false; }

            if(String.IsNullOrWhiteSpace(requested.CultureName) is false && String.Equals(GetCultureText(candidate),GetCultureText(requested),Ordinal) is false) { return false; }

            Byte[]? pkt = requested.GetPublicKeyToken(); if(pkt is not null && pkt.Length > 0 && String.Equals(GetPublicKeyTokenText(candidate),GetPublicKeyTokenText(requested),Ordinal) is false) { return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,MatchesRequestedAssemblyFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryFindLoadedByPathInContext"]/*'/>*/
    private static Assembly? TryFindLoadedByPathInContext(AssemblyLoadContext context , String fullpath)
    {
        try
        {
            foreach(Assembly a in context.Assemblies)
            {
                try
                {
                    String l = a.Location;

                    if(String.IsNullOrWhiteSpace(l)) { continue; }

                    if(String.Equals(Path.GetFullPath(l),fullpath,OrdinalIgnoreCase)) { return a; }
                }
                catch {}
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryFindLoadedByPathInContextFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryFindLoadedByRequestInContext"]/*'/>*/
    private static Assembly? TryFindLoadedByRequestInContext(AssemblyLoadContext context , TypeRequest request)
    {
        try
        {
            foreach(Assembly a in context.Assemblies)
            {
                try
                {
                    if(MatchesRequestAssembly(request,a.GetName())) { return a; }
                }
                catch {}
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryFindLoadedByRequestInContextFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryFindLoadedByRequestedAssemblyInContext"]/*'/>*/
    private static Assembly? TryFindLoadedByRequestedAssemblyInContext(AssemblyLoadContext context , AssemblyName requested)
    {
        try
        {
            foreach(Assembly a in context.Assemblies)
            {
                try
                {
                    if(MatchesRequestedAssembly(a.GetName(),requested)) { return a; }
                }
                catch {}
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryFindLoadedByRequestedAssemblyInContextFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryGetAssemblyName"]/*'/>*/
    private static Boolean TryGetAssemblyName(String path , out AssemblyName? assemblyname)
    {
        assemblyname = null;

        try { assemblyname = AssemblyName.GetAssemblyName(path); return assemblyname is not null; }

        catch ( Exception _ ) { assemblyname = null; KusDepotLog.Error(_,TryGetAssemblyNameFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryNormalizeAssemblyName"]/*'/>*/
    private static Boolean TryNormalizeAssemblyName(AssemblyName? input , out AssemblyName? assemblyname)
    {
        assemblyname = null;

        try
        {
            if(input is null || String.IsNullOrWhiteSpace(input.Name)) { return false; }

            AssemblyName a = new() { Name = input.Name };

            if(input.Version is not null) { a.Version = input.Version; }

            if(String.IsNullOrWhiteSpace(input.CultureName) is false) { a.CultureName = input.CultureName; }

            Byte[]? t = input.GetPublicKeyToken(); if(t is not null && t.Length > 0) { a.SetPublicKeyToken([ .. t ]); }

            assemblyname = a; return true;
        }
        catch ( Exception _ ) { assemblyname = null; KusDepotLog.Error(_,TryNormalizeAssemblyNameFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryReadAssemblyName"]/*'/>*/
    private static Boolean TryReadAssemblyName(Byte[] image , out AssemblyName? assemblyname)
    {
        assemblyname = null;

        try
        {
            using MemoryStream m = new(image,writable:false); using PEReader p = new(m);

            if(p.HasMetadata is false) { return false; }

            MetadataReader r = PEReaderExtensions.GetMetadataReader(p); if(r.IsAssembly is false) { return false; }

            AssemblyDefinition d = r.GetAssemblyDefinition(); String n = r.GetString(d.Name);

            if(String.IsNullOrWhiteSpace(n)) { return false; }

            AssemblyName a = new() { Name = n, Version = d.Version };

            if(d.Culture.IsNil is false)
            {
                String c = r.GetString(d.Culture); if(String.IsNullOrWhiteSpace(c) is false) { a.CultureName = c; }
            }

            if(d.PublicKey.IsNil is false)
            {
                Byte[] k = r.GetBlobBytes(d.PublicKey); if(!Equals(k.Length,0)) { try { a.SetPublicKey(k); } catch {} }
            }

            assemblyname = a; return true;
        }
        catch ( Exception _ ) { assemblyname = null; KusDepotLog.Error(_,TryReadAssemblyNameFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TrySnapshotAssemblyImage"]/*'/>*/
    private static Boolean TrySnapshotAssemblyImage(MemoryStream? stream , out Byte[]? image)
    {
        image = null;

        try
        {
            if(stream is null || stream.CanRead is false) { return false; }

            Byte[] i = stream.ToArray(); if(Equals(i.Length,0)) { return false; }

            if(TryReadAssemblyName(i,out _) is false) { return false; }

            image = i; return true;
        }
        catch ( Exception _ ) { image = null; KusDepotLog.Error(_,TrySnapshotAssemblyImageFail); if(NoExceptions) { return false; } throw; }

        finally { try { stream?.Dispose(); } catch {} }
    }
}