namespace KusDepot;

/**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/main/*'/>*/
public interface IMetaBase : ICommon
{
    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="AddNotes"]/*'/>*/
    Boolean AddNotes(IEnumerable<String>? notes);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="AddTags"]/*'/>*/
    Boolean AddTags(IEnumerable<String>? tags);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetApplication"]/*'/>*/
    String? GetApplication();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetApplicationVersion"]/*'/>*/
    Version? GetApplicationVersion();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetBornOn"]/*'/>*/
    DateTimeOffset? GetBornOn();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetDescriptor"]/*'/>*/
    Descriptor? GetDescriptor();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetDistinguishedName"]/*'/>*/
    String? GetDistinguishedName();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetDomainID"]/*'/>*/
    String? GetDomainID();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetExtension"]/*'/>*/
    Dictionary<String,Object?>? GetExtension();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetFILE"]/*'/>*/
    String? GetFILE();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetLinks"]/*'/>*/
    Dictionary<String,GuidReferenceItem>? GetLinks();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetModified"]/*'/>*/
    DateTimeOffset? GetModified();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetName"]/*'/>*/
    String? GetName();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetNotes"]/*'/>*/
    HashSet<String>? GetNotes();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetSecurityDescriptor"]/*'/>*/
    String? GetSecurityDescriptor();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetServiceVersion"]/*'/>*/
    Version? GetServiceVersion();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetTags"]/*'/>*/
    HashSet<String>? GetTags();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetVersion"]/*'/>*/
    Version? GetVersion();

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="RemoveNote"]/*'/>*/
    Boolean RemoveNote(String? note);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="RemoveTag"]/*'/>*/
    Boolean RemoveTag(String? tag);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetApplication"]/*'/>*/
    Boolean SetApplication(String? application);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="GetApplicationVersion"]/*'/>*/
    Boolean SetApplicationVersion(Version? applicationversion);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetBornOn"]/*'/>*/
    Boolean SetBornOn(DateTimeOffset? bornon);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetDistinguishedName"]/*'/>*/
    Boolean SetDistinguishedName(String? distinguishedname);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetDomainID"]/*'/>*/
    Boolean SetDomainID(String? domainid);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetExtension"]/*'/>*/
    Boolean SetExtension(IDictionary<String,Object?>? extension);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetFILE"]/*'/>*/
    Boolean SetFILE(String? file);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetLinks"]/*'/>*/
    Boolean SetLinks(IDictionary<String,GuidReferenceItem>? links);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetModified"]/*'/>*/
    Boolean SetModified(DateTimeOffset? modified);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetName"]/*'/>*/
    Boolean SetName(String? name);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetSecurityDescriptor"]/*'/>*/
    Boolean SetSecurityDescriptor(String? securitydescriptor);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetServiceVersion"]/*'/>*/
    Boolean SetServiceVersion(Version? serviceversion);

    /**<include file='IMetaBase.xml' path='IMetaBase/interface[@name="IMetaBase"]/method[@name="SetVersion"]/*'/>*/
    Boolean SetVersion(Version? version);
}