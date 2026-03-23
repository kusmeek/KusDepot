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

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateBinaryItemAction"]/*'/>*/
    public static BinaryItem? CreateBinaryItem(Action<BinaryItem>? configure = null)
    {
        try { BinaryItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateBinaryItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateBinaryItemActionAsync"]/*'/>*/
    public static async Task<BinaryItem?> CreateBinaryItemAsync(Func<BinaryItem,Task>? configure = null)
    {
        try { BinaryItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateBinaryItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateCodeItem"]/*'/>*/
    public static CodeItem? CreateCodeItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new CodeItem(content,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateCodeItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateCodeItemAction"]/*'/>*/
    public static CodeItem? CreateCodeItem(Action<CodeItem>? configure = null)
    {
        try { CodeItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateCodeItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateCodeItemActionAsync"]/*'/>*/
    public static async Task<CodeItem?> CreateCodeItemAsync(Func<CodeItem,Task>? configure = null)
    {
        try { CodeItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateCodeItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateDataSetItem"]/*'/>*/
    public static DataSetItem? CreateDataSetItem(IEnumerable<DataItem>? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new DataSetItem(content,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateDataSetItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateDataSetItemAction"]/*'/>*/
    public static DataSetItem? CreateDataSetItem(Action<DataSetItem>? configure = null)
    {
        try { DataSetItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateDataSetItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateDataSetItemActionAsync"]/*'/>*/
    public static async Task<DataSetItem?> CreateDataSetItemAsync(Func<DataSetItem,Task>? configure = null)
    {
        try { DataSetItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateDataSetItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGenericItem"]/*'/>*/
    public static GenericItem? CreateGenericItem(IEnumerable<Object>? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try { return new GenericItem(content,file,id,name,notes,tags,type); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGenericItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGenericItemAction"]/*'/>*/
    public static GenericItem? CreateGenericItem(Action<GenericItem>? configure = null)
    {
        try { GenericItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGenericItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGenericItemActionAsync"]/*'/>*/
    public static async Task<GenericItem?> CreateGenericItemAsync(Func<GenericItem,Task>? configure = null)
    {
        try { GenericItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGenericItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGuidReferenceItem"]/*'/>*/
    public static GuidReferenceItem? CreateGuidReferenceItem(Guid? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try { return new GuidReferenceItem(content,file,id,name,notes,tags); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGuidReferenceItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGuidReferenceItemAction"]/*'/>*/
    public static GuidReferenceItem? CreateGuidReferenceItem(Action<GuidReferenceItem>? configure = null)
    {
        try { GuidReferenceItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGuidReferenceItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateGuidReferenceItemActionAsync"]/*'/>*/
    public static async Task<GuidReferenceItem?> CreateGuidReferenceItemAsync(Func<GuidReferenceItem,Task>? configure = null)
    {
        try { GuidReferenceItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateGuidReferenceItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMSBuildItem"]/*'/>*/
    public static MSBuildItem? CreateMSBuildItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try { return new MSBuildItem(content,file,id,name,notes,tags); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMSBuildItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMSBuildItemAction"]/*'/>*/
    public static MSBuildItem? CreateMSBuildItem(Action<MSBuildItem>? configure = null)
    {
        try { MSBuildItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMSBuildItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMSBuildItemActionAsync"]/*'/>*/
    public static async Task<MSBuildItem?> CreateMSBuildItemAsync(Func<MSBuildItem,Task>? configure = null)
    {
        try { MSBuildItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMSBuildItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMultiMediaItem"]/*'/>*/
    public static MultiMediaItem? CreateMultiMediaItem(Byte[]? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? artists = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? title = null , String? type = null , Int32? year = null)
    {
        try { return new MultiMediaItem(content,file,id,name,artists,notes,tags,title,type,year); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMultiMediaItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMultiMediaItemAction"]/*'/>*/
    public static MultiMediaItem? CreateMultiMediaItem(Action<MultiMediaItem>? configure = null)
    {
        try { MultiMediaItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMultiMediaItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateMultiMediaItemActionAsync"]/*'/>*/
    public static async Task<MultiMediaItem?> CreateMultiMediaItemAsync(Func<MultiMediaItem,Task>? configure = null)
    {
        try { MultiMediaItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateMultiMediaItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateTextItem"]/*'/>*/
    public static TextItem? CreateTextItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null , String? language = null)
    {
        try { return new TextItem(content,file,id,name,notes,tags,type,language); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateTextItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateTextItemAction"]/*'/>*/
    public static TextItem? CreateTextItem(Action<TextItem>? configure = null)
    {
        try { TextItem _ = new(); configure?.Invoke(_); return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateTextItemFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataFactory.xml' path='DataFactory/class[@name="DataFactory"]/method[@name="CreateTextItemActionAsync"]/*'/>*/
    public static async Task<TextItem?> CreateTextItemAsync(Func<TextItem,Task>? configure = null)
    {
        try { TextItem _ = new(); if(configure is not null) { await configure(_).ConfigureAwait(false); } return _; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateTextItemFail); if(NoExceptions) { return null; } throw; }
    }
}