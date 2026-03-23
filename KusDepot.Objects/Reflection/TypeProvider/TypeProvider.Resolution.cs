namespace KusDepot.Reflection;

public sealed partial class TypeProvider
{
    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryResolveNoSync"]/*'/>*/
    private Boolean TryResolve_NoSync()
    {
        try
        {
            if(Value is not null) { return false; }

            if(TryGetEffectiveRequests_NoSync(out List<TypeRequest>? r) is false || r is null || Equals(r.Count,0)) { return false; }

            if(Mode is not Isolated)
            {
                foreach(TypeRequest _ in r)
                {
                    if(TryResolveInDefaultContext_NoSync(_,out Type? t) && t is not null)
                    {
                        Value = t; return true;
                    }
                }
            }

            Boolean hasprobesources = (AssemblyPaths is not null && !Equals(AssemblyPaths.Count,0)) ||
                (AssemblyDirectories is not null && !Equals(AssemblyDirectories.Count,0)) ||
                (AssemblyImages is not null && !Equals(AssemblyImages.Count,0));

            if(hasprobesources is false && Context is null) { return false; }

            foreach(TypeRequest _ in r)
            {
                if(TryResolveFromImagesInActiveContext_NoSync(_,out Type? t) && t is not null)
                {
                    Value = t; return true;
                }

                if(TryResolveFromPathsInActiveContext_NoSync(_,out t))
                {
                    if(t is not null) { Value = t; return true; }
                }
            }

            return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryResolveNoSyncFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryGetEffectiveRequestsNoSync"]/*'/>*/
    private Boolean TryGetEffectiveRequests_NoSync(out List<TypeRequest>? requests)
    {
        requests = null;

        try
        {
            if(TryParseRequest_NoSync(out TypeRequest? r) is false || r is null) { return false; }

            List<TypeRequest> v = new();

            if(r.HasAssemblyIdentity)
            {
                v.Add(r); requests = v; return true;
            }

            if(AssemblyNames is null || Equals(AssemblyNames.Count,0))
            {
                v.Add(r); requests = v; return true;
            }

            foreach(AssemblyName a in AssemblyNames)
            {
                String? c = null; Boolean hc = false; String? pkt = null; Boolean hpkt = false;

                if(String.IsNullOrWhiteSpace(a.CultureName) is false) { c = a.CultureName; hc = true; }

                Byte[]? t = a.GetPublicKeyToken(); if(t is not null && t.Length > 0) { pkt = Convert.ToHexStringLower(t); hpkt = true; }

                TypeRequest _ = new(r.TypeName,a.Name,a.Version,a.Version is not null,c,hc,pkt,hpkt);

                if(v.Any(__ => TypeRequestsEqual(__,_))) { continue; }

                v.Add(_);
            }

            if(Equals(v.Count,0)) { return false; }

            requests = v; return true;
        }
        catch ( Exception _ ) { requests = null; KusDepotLog.Error(_,TryGetEffectiveRequestsFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryResolveInDefaultContextNoSync"]/*'/>*/
    private Boolean TryResolveInDefaultContext_NoSync(TypeRequest request , out Type? type)
    {
        type = null;

        try
        {
            if(String.IsNullOrWhiteSpace(Name)) { return false; }

            type = Type.GetType(Name,throwOnError:false,ignoreCase:false);

            if(type is not null && MatchesResolvedType(request,type)) { return true; }

            type = null;

            foreach(Assembly a in AssemblyLoadContext.Default.Assemblies)
            {
                if(MatchesRequestAssembly(request,a.GetName()) is false) { continue; }

                if(TryResolveInAssembly(a,request.TypeName,out type) is false || type is null) { continue; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { type = null; KusDepotLog.Error(_,TryResolveInDefaultContextFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="EnsureActiveContextNoSync"]/*'/>*/
    private AssemblyLoadContext? EnsureActiveContext_NoSync()
    {
        try
        {
            if(Context is not null) { return Context; }

            AssemblyLoadContext c = new($"TypeProvider:{Guid.NewGuid():N}",isCollectible:true);

            ContextResolver = (context,assemblyname) => ResolveContextDependency(context,assemblyname);

            c.Resolving += ContextResolver; Context = c; OwnsContext = true; return Context;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EnsureActiveContextFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="UnloadOwnedContextNoSync"]/*'/>*/
    private Boolean UnloadOwnedContext_NoSync()
    {
        try
        {
            if(Context is null) { ContextResolver = null; OwnsContext = false; return true; }

            if(OwnsContext is false) { return false; }

            AssemblyLoadContext? c = Context; Func<AssemblyLoadContext,AssemblyName,Assembly?>? r = ContextResolver;

            Context = null; ContextResolver = null; OwnsContext = false;

            try { if(c is not null && r is not null) { c.Resolving -= r; } } catch {}

            try { c?.Unload(); } catch {}

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnloadOwnedContextFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="EnumerateCandidateAssemblyPathsNoSync"]/*'/>*/
    private List<String> EnumerateCandidateAssemblyPaths_NoSync(TypeRequest request)
    {
        try
        {
            List<String> c = new();

            if(AssemblyPaths is not null && !Equals(AssemblyPaths.Count,0))
            {
                if(request.HasAssemblyIdentity)
                {
                    foreach(String p in AssemblyPaths)
                    {
                        if(TryGetAssemblyName(p,out AssemblyName? a) is false || a is null) { continue; }

                        if(MatchesRequestAssembly(request,a) is false) { continue; }

                        if(c.Contains(p,StringComparer.OrdinalIgnoreCase)) { continue; }

                        c.Add(p);
                    }
                }
                else
                {
                    foreach(String p in AssemblyPaths)
                    {
                        if(c.Contains(p,StringComparer.OrdinalIgnoreCase)) { continue; }

                        c.Add(p);
                    }
                }
            }

            if(AssemblyDirectories is not null && !Equals(AssemblyDirectories.Count,0))
            {
                foreach(String d in AssemblyDirectories)
                {
                    String[] f;

                    try { f = Directory.GetFiles(d,"*.dll",SearchOption.TopDirectoryOnly); }
                    catch { continue; }

                    foreach(String p in f)
                    {
                        if(c.Contains(p,StringComparer.OrdinalIgnoreCase)) { continue; }

                        if(request.HasAssemblyIdentity)
                        {
                            if(TryGetAssemblyName(p,out AssemblyName? a) is false || a is null) { continue; }

                            if(MatchesRequestAssembly(request,a) is false) { continue; }
                        }

                        c.Add(p);
                    }
                }
            }

            return c;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EnumerateCandidateAssemblyPathsFail); if(NoExceptions) { return []; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="GetAssemblyDirectoriesNoSync"]/*'/>*/
    private HashSet<String> GetAssemblyDirectories_NoSync()
    {
        try
        {
            HashSet<String> d = new(StringComparer.OrdinalIgnoreCase);

            if(AssemblyPaths is not null && !Equals(AssemblyPaths.Count,0))
            {
                foreach(String p in AssemblyPaths)
                {
                    String? _ = Path.GetDirectoryName(p); if(!String.IsNullOrWhiteSpace(_)) { _ = Path.GetFullPath(_); d.Add(_); }
                }
            }

            if(AssemblyDirectories is not null && !Equals(AssemblyDirectories.Count,0))
            {
                foreach(String p in AssemblyDirectories) { d.Add(p); }
            }

            return d;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetAssemblyDirectoriesFail); if(NoExceptions) { return []; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryResolveFromPathsInActiveContextNoSync"]/*'/>*/
    private Boolean TryResolveFromPathsInActiveContext_NoSync(TypeRequest request , out Type? type)
    {
        type = null;

        try
        {
            AssemblyLoadContext? c = EnsureActiveContext_NoSync(); if(c is null) { return false; }

            Assembly? la = TryFindLoadedByRequestInContext(c,request);

            if(la is not null && TryResolveInAssembly(la,request.TypeName,out type) && type is not null) { return true; }

            foreach(String p in EnumerateCandidateAssemblyPaths_NoSync(request))
            {
                Assembly? a = TryFindLoadedByPathInContext(c,p);

                if(a is null) { try { a = c.LoadFromAssemblyPath(p); } catch { a = TryFindLoadedByPathInContext(c,p); } }

                if(a is null) { continue; }

                if(MatchesRequestAssembly(request,a.GetName()) is false) { continue; }

                if(TryResolveInAssembly(a,request.TypeName,out type) is false || type is null) { continue; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { type = null; KusDepotLog.Error(_,TryResolveFromPathsInActiveContextFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryResolveFromImagesInActiveContextNoSync"]/*'/>*/
    private Boolean TryResolveFromImagesInActiveContext_NoSync(TypeRequest request , out Type? type)
    {
        type = null;

        try
        {
            AssemblyLoadContext? c = Context;

            if(c is null)
            {
                if(AssemblyImages is null || Equals(AssemblyImages.Count,0)) { return false; }

                c = EnsureActiveContext_NoSync(); if(c is null) { return false; }
            }

            Assembly? la = TryFindLoadedByRequestInContext(c,request);

            if(la is not null && TryResolveInAssembly(la,request.TypeName,out type) && type is not null) { return true; }

            if(AssemblyImages is null || Equals(AssemblyImages.Count,0)) { return false; }

            foreach(Byte[] i in AssemblyImages)
            {
                if(request.HasAssemblyIdentity)
                {
                    if(TryReadAssemblyName(i,out AssemblyName? n) is false || n is null) { continue; }

                    if(MatchesRequestAssembly(request,n) is false) { continue; }
                }

                Assembly? a = null;

                try
                {
                    using MemoryStream m = new(i,writable:false); a = c.LoadFromStream(m);
                }
                catch
                {
                    a = TryFindLoadedByRequestInContext(c,request);
                }

                if(a is null) { continue; }

                if(MatchesRequestAssembly(request,a.GetName()) is false) { continue; }

                if(TryResolveInAssembly(a,request.TypeName,out type) is false || type is null) { continue; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { type = null; KusDepotLog.Error(_,TryResolveFromImagesInActiveContextFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="ResolveContextDependency"]/*'/>*/
    private Assembly? ResolveContextDependency(AssemblyLoadContext context , AssemblyName assemblyname)
    {
        try
        {
            Assembly? la = TryFindLoadedByRequestedAssemblyInContext(context,assemblyname); if(la is not null) { return la; }

            la = TryLoadImageByRequestedAssemblyInContext(context,assemblyname); if(la is not null) { return la; }

            foreach(String d in GetAssemblyDirectories_NoSync())
            {
                String p = Path.Combine(d,$"{assemblyname.Name}.dll");

                if(File.Exists(p) is false) { continue; }

                if(TryGetAssemblyName(p,out AssemblyName? a) is false || a is null) { continue; }

                if(MatchesRequestedAssembly(a,assemblyname) is false) { continue; }

                try { return context.LoadFromAssemblyPath(p); }
                catch
                {
                    la = TryFindLoadedByPathInContext(context,p); if(la is not null) { return la; }

                    la = TryFindLoadedByRequestedAssemblyInContext(context,assemblyname); if(la is not null) { return la; }
                }
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ResolveContextDependencyFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryLoadImageByRequestedAssemblyInContext"]/*'/>*/
    private Assembly? TryLoadImageByRequestedAssemblyInContext(AssemblyLoadContext context , AssemblyName requested)
    {
        try
        {
            if(AssemblyImages is null || Equals(AssemblyImages.Count,0)) { return null; }

            Assembly? la = TryFindLoadedByRequestedAssemblyInContext(context,requested); if(la is not null) { return la; }

            foreach(Byte[] i in AssemblyImages)
            {
                if(TryReadAssemblyName(i,out AssemblyName? a) is false || a is null) { continue; }

                if(MatchesRequestedAssembly(a,requested) is false) { continue; }

                try
                {
                    using MemoryStream m = new(i,writable:false); return context.LoadFromStream(m);
                }
                catch
                {
                    la = TryFindLoadedByRequestedAssemblyInContext(context,requested); if(la is not null) { return la; }
                }
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryLoadImageByRequestedAssemblyInContextFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryResolveInAssembly"]/*'/>*/
    private static Boolean TryResolveInAssembly(Assembly assembly , String typename , out Type? type)
    {
        type = null;

        try
        {
            type = assembly.GetType(typename,throwOnError:false,ignoreCase:false);

            return type is not null && String.Equals(type.FullName,typename,Ordinal);
        }
        catch ( Exception _ ) { type = null; KusDepotLog.Error(_,TryResolveInAssemblyFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="MatchesResolvedType"]/*'/>*/
    private static Boolean MatchesResolvedType(TypeRequest request , Type type)
    {
        try
        {
            if(String.Equals(type.FullName,request.TypeName,Ordinal) is false) { return false; }

            return MatchesRequestAssembly(request,type.Assembly.GetName());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,MatchesResolvedTypeFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TypeRequestsEqual"]/*'/>*/
    private static Boolean TypeRequestsEqual(TypeRequest left , TypeRequest right)
    {
        try
        {
            if(String.Equals(left.TypeName,right.TypeName,Ordinal) is false) { return false; }

            if(String.Equals(left.AssemblyName,right.AssemblyName,Ordinal) is false) { return false; }

            if(Equals(left.Version,right.Version) is false) { return false; }

            if(!Equals(left.HasVersion,right.HasVersion)) { return false; }

            if(String.Equals(left.Culture,right.Culture,Ordinal) is false) { return false; }

            if(!Equals(left.HasCulture,right.HasCulture)) { return false; }

            if(String.Equals(left.PublicKeyToken,right.PublicKeyToken,Ordinal) is false) { return false; }

            if(!Equals(left.HasPublicKeyToken,right.HasPublicKeyToken)) { return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TypeRequestsEqualFail); if(NoExceptions) { return false; } throw; }
    }
}