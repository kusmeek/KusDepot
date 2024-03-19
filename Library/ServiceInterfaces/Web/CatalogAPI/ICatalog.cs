namespace KusDepot.Data;

/**<include file='ICatalog.xml' path='ICatalog/interface[@name="ICatalog"]/main/*'/>*/
internal interface ICatalog
{
    /**<include file='ICatalog.xml' path='ICatalog/interface[@name="ICatalog"]/method[@name="SearchActiveServices"]/*'/>*/
    internal ActiveServiceResponse SearchActiveServices(ActiveServiceRequest? search);

    /**<include file='ICatalog.xml' path='ICatalog/interface[@name="ICatalog"]/method[@name="SearchElements"]/*'/>*/
    internal ElementResponse SearchElements(ElementRequest? search);

    /**<include file='ICatalog.xml' path='ICatalog/interface[@name="ICatalog"]/method[@name="SearchMedia"]/*'/>*/
    internal MediaResponse SearchMedia(MediaRequest? search);

    /**<include file='ICatalog.xml' path='ICatalog/interface[@name="ICatalog"]/method[@name="SearchNotes"]/*'/>*/
    internal NoteResponse SearchNotes(NoteRequest? search);

    /**<include file='ICatalog.xml' path='ICatalog/interface[@name="ICatalog"]/method[@name="SearchTags"]/*'/>*/
    internal TagResponse SearchTags(TagRequest? search);
}