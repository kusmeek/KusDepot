namespace KusDepot.Reflection;

/**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/main/*'/>*/
internal sealed class TypeRequest
{
    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/constructor[@name="Constructor"]/*'/>*/
    internal TypeRequest(String typename , String? assemblyname = null , Version? version = null , Boolean hasversion = false , String? culture = null , Boolean hasculture = false , String? publickeytoken = null , Boolean haspublickeytoken = false)
    {
        TypeName = typename; AssemblyName = assemblyname; Version = version; HasVersion = hasversion;
        Culture = culture; HasCulture = hasculture; PublicKeyToken = publickeytoken;
        HasPublicKeyToken = haspublickeytoken;
    }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="TypeName"]/*'/>*/
    internal String TypeName { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="AssemblyName"]/*'/>*/
    internal String? AssemblyName { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="Version"]/*'/>*/
    internal Version? Version { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="HasVersion"]/*'/>*/
    internal Boolean HasVersion { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="Culture"]/*'/>*/
    internal String? Culture { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="HasCulture"]/*'/>*/
    internal Boolean HasCulture { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="PublicKeyToken"]/*'/>*/
    internal String? PublicKeyToken { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="HasPublicKeyToken"]/*'/>*/
    internal Boolean HasPublicKeyToken { get; }

    /**<include file='TypeRequest.xml' path='TypeRequest/class[@name="TypeRequest"]/property[@name="HasAssemblyIdentity"]/*'/>*/
    internal Boolean HasAssemblyIdentity => String.IsNullOrWhiteSpace(AssemblyName) is false;
}