namespace KusDepot;

/**<include file='Ark.xml' path='Ark/class[@name="Ark"]/main/*'/>*/
public sealed class Ark : DataSet
{
    /** <include file='Ark.xml' path='Ark/class[@name="Ark"]/field[@name="TagsTableName"]/*'/>*/
    public const String TagsTableName = "Tags";

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/field[@name="NotesTableName"]/*'/>*/
    public const String NotesTableName = "Notes";

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/field[@name="ElementsTableName"]/*'/>*/
    public const String ElementsTableName = "Elements";

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/field[@name="MediaLibraryTableName"]/*'/>*/
    public const String MediaLibraryTableName = "MediaLibrary";
    
    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/field[@name="ActiveServicesTableName"]/*'/>*/
    public const String ActiveServicesTableName = "ActiveServices";

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/field[@name="Sync"]/*'/>*/
    private readonly Object Sync = new Object();

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/constructor[@name="Constructor"]/*'/>*/
    public Ark(Int32 capacity = 100) : base("Ark")
    {
        this.CaseSensitive = false;

        Int32 _ = capacity < 1 ? 100 : capacity;

        this.Tables.Add(new Tags(TagsTableName,_));

        this.Tables.Add(new Notes(NotesTableName,_));

        this.Tables.Add(new Elements(ElementsTableName));

        this.Tables.Add(new MediaLibrary(MediaLibraryTableName));

        this.Tables.Add(new ActiveServices(ActiveServicesTableName));
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="AddUpdate"]/*'/>*/
    public Boolean AddUpdate(String? it)
    {
        if(it is null) { return false; }

        Tool? _0 = null; GuidReferenceItem? _1 = null; GenericItem? _2 = null; TextItem? _3 = null; CodeItem? _4 = null; MSBuildItem? _5 = null; BinaryItem? _6 = null; MultiMediaItem? _7 = null;

        Boolean _ = true;

        try
        {
            if(!TryEnter(this.Sync,SyncTime)) { throw ArkSyncException; }

            try { if(Tool.TryParse(it,null,out _0))              { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GuidReferenceItem.TryParse(it,null,out _1)) { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GenericItem.TryParse(it,null,out _2))       { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(TextItem.TryParse(it,null,out _3))          { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(CodeItem.TryParse(it,null,out _4))          { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MSBuildItem.TryParse(it,null,out _5))       { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(BinaryItem.TryParse(it,null,out _6))        { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MultiMediaItem.TryParse(it,null,out _7))    { goto AddUpdate; } } catch ( SerializationException ) { } catch ( XmlException ) { }

            return false;

            AddUpdate:
            _ = _ & ((Tags)this.Tables[TagsTableName]!).AddUpdate(it);                                           if(!_) { return false; }
            _ = _ & ((Notes)this.Tables[NotesTableName]!).AddUpdate(it);                                         if(!_) { return false; }
            _ = _ & ((Elements)this.Tables[ElementsTableName]!).AddUpdate(it);                                   if(!_) { return false; }
            if(_7 is not null) { _ = _ & ((MediaLibrary)this.Tables[MediaLibraryTableName]!).AddUpdate(it);}     if(!_) { return false; }
            if(_0 is not null) { _ = _ & ((ActiveServices)this.Tables[ActiveServicesTableName]!).AddUpdate(it);} if(!_) { return false; }
            return _;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="AddUpdateDescriptor"]/*'/>*/
    public Boolean AddUpdate(Descriptor? descriptor)
    {
        if(descriptor is null) { return false; }

        List<String> _n = new List<String>(){typeof(GuidReferenceItem).Name,typeof(TextItem).Name,typeof(CodeItem).Name,typeof(GenericItem).Name,typeof(BinaryItem).Name,typeof(MultiMediaItem).Name,typeof(MSBuildItem).Name};

        Boolean _ = true; Descriptor _d = descriptor;

        try
        {
            if(!TryEnter(this.Sync,SyncTime)) { throw ArkSyncException; }

            _ = _ & ((Tags)this.Tables[TagsTableName]!).AddUpdate(_d);                                                                                                              if(!_) { return false; }
            _ = _ & ((Notes)this.Tables[NotesTableName]!).AddUpdate(_d);                                                                                                            if(!_) { return false; }
            _ = _ & ((Elements)this.Tables[ElementsTableName]!).AddUpdate(_d);                                                                                                      if(!_) { return false; }
            if(String.Equals(_d.ObjectType,typeof(MultiMediaItem).Name,StringComparison.Ordinal)) { _ = _ & ((MediaLibrary)this.Tables[MediaLibraryTableName]!).AddUpdate(_d);}     if(!_) { return false; }
            if(!_n.Contains(_d.ObjectType!))                             { _ = _ & ((ActiveServices)this.Tables[ActiveServicesTableName]!).AddUpdate(_d);}                          if(!_) { return false; }
            return _;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="Exists"]/*'/>*/
    public Boolean? Exists(Guid? id)
    {
        if(id is null) { return false; }

        List<String> _n = new List<String>(){typeof(GuidReferenceItem).Name,typeof(TextItem).Name,typeof(CodeItem).Name,typeof(GenericItem).Name,typeof(BinaryItem).Name,typeof(MultiMediaItem).Name,typeof(MSBuildItem).Name};

        try
        {
            DataRow? _r = ((Elements)this.Tables[ElementsTableName]!).Rows.Find(id);

            if(_r is null) { return false; } String? _s = _r!["ObjectType"]!.ToString();

            Boolean _ = true; Boolean? __;

            __ = ((Tags)this.Tables[TagsTableName]!).Exists(id);                        if(__ is null) { return null; } _ = _ & (Boolean)__; if(!_) { return false; }
            __ = ((Notes)this.Tables[NotesTableName]!).Exists(id);                      if(__ is null) { return null; } _ = _ & (Boolean)__; if(!_) { return false; }
            __ = ((Elements)this.Tables[ElementsTableName]!).Exists(id);                if(__ is null) { return null; } _ = _ & (Boolean)__; if(!_) { return false; }

            if(String.Equals(_s,typeof(MultiMediaItem).Name,StringComparison.Ordinal))
            { __ = ((MediaLibrary)this.Tables[MediaLibraryTableName]!).Exists(id);}     if(__ is null) { return null; } _ = _ & (Boolean)__; if(!_) { return false; }

            if(!_n.Contains(_s!))
            { __ = ((ActiveServices)this.Tables[ActiveServicesTableName]!).Exists(id);} if(__ is null) { return null; } _ = _ & (Boolean)__; if(!_) { return false; }

            return _;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="GetBytes"]/*'/>*/
    public static Byte[] GetBytes(Ark? ark)
    {
        if(ark is null) { return Array.Empty<Byte>(); }

        try
        {
            if(!TryEnter(ark,SyncTime)) { throw ArkSyncException; }

            MemoryStream _ = new MemoryStream(); ark.WriteXml(_,XmlWriteMode.WriteSchema); return _.ToArray();
        }
        catch ( Exception ) { if(NoExceptions) { return Array.Empty<Byte>(); } throw; }

        finally { if(IsEntered(ark)) { Exit(ark); } }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="Parse"]/*'/>*/
    public static Ark? Parse(Byte[]? ark)
    {
        if(ark is null) { return null; }

        try
        {
            using(XmlReader _ = XmlReader.Create(new MemoryStream(ark)))
            {
                Ark __ = new Ark(); __.ReadXml(_,XmlReadMode.IgnoreSchema);

                return __;
            }
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="Remove"]/*'/>*/
    public Boolean Remove(String? it)
    {
        if(it is null) { return false; }

        Tool? _0 = null; GuidReferenceItem? _1 = null; GenericItem? _2 = null; TextItem? _3 = null; CodeItem? _4 = null; MSBuildItem? _5 = null; BinaryItem? _6 = null; MultiMediaItem? _7 = null;

        Boolean _ = true;

        try
        {
            if(!TryEnter(this.Sync,SyncTime)) { throw ArkSyncException; }

            try { if(Tool.TryParse(it,null,out _0))              { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GuidReferenceItem.TryParse(it,null,out _1)) { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(GenericItem.TryParse(it,null,out _2))       { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(TextItem.TryParse(it,null,out _3))          { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(CodeItem.TryParse(it,null,out _4))          { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MSBuildItem.TryParse(it,null,out _5))       { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(BinaryItem.TryParse(it,null,out _6))        { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }
            try { if(MultiMediaItem.TryParse(it,null,out _7))    { goto Remove; } } catch ( SerializationException ) { } catch ( XmlException ) { }

            return false;

            Remove:
            _ = _ & ((Tags)this.Tables[TagsTableName]!).Remove(it);                                           if(!_) { return false; }
            _ = _ & ((Notes)this.Tables[NotesTableName]!).Remove(it);                                         if(!_) { return false; }
            _ = _ & ((Elements)this.Tables[ElementsTableName]!).Remove(it);                                   if(!_) { return false; }
            if(_7 is not null) { _ = _ & ((MediaLibrary)this.Tables[MediaLibraryTableName]!).Remove(it);}     if(!_) { return false; }
            if(_0 is not null) { _ = _ & ((ActiveServices)this.Tables[ActiveServicesTableName]!).Remove(it);} if(!_) { return false; }
            return _;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="RemoveDescriptor"]/*'/>*/
    public Boolean Remove(Descriptor? descriptor)
    {
        if(descriptor is null) { return false; }

        List<String> _n = new List<String>(){typeof(GuidReferenceItem).Name,typeof(TextItem).Name,typeof(CodeItem).Name,typeof(GenericItem).Name,typeof(BinaryItem).Name,typeof(MultiMediaItem).Name,typeof(MSBuildItem).Name};

        Boolean _ = true; Descriptor _d = descriptor;

        try
        {
            if(!TryEnter(this.Sync,SyncTime)) { throw ArkSyncException; }

            _ = _ & ((Tags)this.Tables[TagsTableName]!).Remove(_d);                                                                                                              if(!_) { return false; }
            _ = _ & ((Notes)this.Tables[NotesTableName]!).Remove(_d);                                                                                                            if(!_) { return false; }
            _ = _ & ((Elements)this.Tables[ElementsTableName]!).Remove(_d);                                                                                                      if(!_) { return false; }
            if(String.Equals(_d.ObjectType,typeof(MultiMediaItem).Name,StringComparison.Ordinal)) { _ = _ & ((MediaLibrary)this.Tables[MediaLibraryTableName]!).Remove(_d);}     if(!_) { return false; }
            if(!_n.Contains(_d.ObjectType!))                             { _ = _ & ((ActiveServices)this.Tables[ActiveServicesTableName]!).Remove(_d);}                          if(!_) { return false; }
            return _;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="RemoveGuid"]/*'/>*/
    public Boolean Remove(Guid? id)
    {
        if(id is null) { return false; }

        Boolean _ = true;

        try
        {
            if(!TryEnter(this.Sync,SyncTime)) { throw ArkSyncException; }

            _ = _ & ((Tags)this.Tables[TagsTableName]!).Remove(id);                        if(!_) { return false; }
            _ = _ & ((Notes)this.Tables[NotesTableName]!).Remove(id);                      if(!_) { return false; }
            _ = _ & ((Elements)this.Tables[ElementsTableName]!).Remove(id);                if(!_) { return false; }

            if(((MediaLibrary)this.Tables[MediaLibraryTableName]!).Exists(id) is true)
            {_ = _ & ((MediaLibrary)this.Tables[MediaLibraryTableName]!).Remove(id);       if(!_) { return false; }}

            if(((ActiveServices)this.Tables[ActiveServicesTableName]!).Exists(id) is true)
            {_ = _ & ((ActiveServices)this.Tables[ActiveServicesTableName]!).Remove(id);   if(!_) { return false; }}
            return _;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    /**<include file='Ark.xml' path='Ark/class[@name="Ark"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(Byte[]? it , [NotNullWhen(true)] out Ark? ark)
    {
        ark = null; if(it is null || Array.Empty<Byte>().Equals(it)) { return false; }

        try
        {
            using(XmlReader _ = XmlReader.Create(new MemoryStream(it)))
            {
                Ark __ = new Ark(); __.ReadXml(_,XmlReadMode.IgnoreSchema);

                ark = __; return true;
            }
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }
}