namespace KusDepot;

/**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/main/*'/>*/
public static class DataFactory
{
    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateBinaryItem"]/*'/>*/
    public static BinaryItem? CreateBinaryItem(Byte[]? content = null , IEnumerable<String>? dllpaths = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new BinaryItem(content,dllpaths,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateBinaryItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateCodeItem"]/*'/>*/
    public static CodeItem? CreateCodeItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new CodeItem(content,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateCodeItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateDataSetItem"]/*'/>*/
    public static DataSetItem? CreateDataSetItem(IEnumerable<DataItem>? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new DataSetItem(content,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateDataSetItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGenericItem"]/*'/>*/
    public static GenericItem? CreateGenericItem(IEnumerable<Object>? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new GenericItem(content,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGenericItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGuidReferenceItem"]/*'/>*/
    public static GuidReferenceItem? CreateGuidReferenceItem(Guid? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try { return new GuidReferenceItem(content,file,id,name,notes,tags); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGuidReferenceItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMSBuildItem"]/*'/>*/
    public static MSBuildItem? CreateMSBuildItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try { return new MSBuildItem(content,file,id,name,notes,tags); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMSBuildItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMultiMediaItem"]/*'/>*/
    public static MultiMediaItem? CreateMultiMediaItem(Byte[]? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? artists = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? title = null , String? type = null , Int32? year = null)
    {
        try { return new MultiMediaItem(content,file,id,name,artists,notes,tags,title,type,year); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMultiMediaItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateTextItem"]/*'/>*/
    public static TextItem? CreateTextItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null , String? language = null)
    {
        try { return new TextItem(content,file,id,name,notes,tags,type,language); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateTextItemFail); if(NoExceptions) { return null; } throw; }
    }
}