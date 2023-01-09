namespace KusDepot
{
    /** <include file='ICache.xml' path='ICache/interface[@Name="ICache"]'/> */
    public interface ICache
    {
        /** <include file='ICache.xml' path='ICache/interface/method[@Name="AccessItem"]'/> */
        public DataItem? AccessItem(Guid? guid);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="AccessTool"]'/> */
        public Tool? AccessTool(Guid? guid);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="AllocateItemGuid"]'/> */
        public Guid? AllocateItemGuid();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="AllocateToolGuid"]'/> */
        public Guid? AllocateToolGuid();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ExistsGuid"]'/> */
        public Boolean Exists(Guid? guid);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ExistsName"]'/> */
        public Boolean Exists(String? name);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="GetData"]'/> */
        public List<DataSet>? GetData();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="SearchItemNames"]'/> */
        public List<Guid>? SearchItemNames(List<String>? names);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="SearchItemTags"]'/> */
        public List<Guid>? SearchItemTags(List<String>? tags);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="SearchToolNames"]'/> */
        public List<Guid>? SearchToolNames(List<String>? names);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="SearchToolTags"]'/> */
        public List<Guid>? SearchToolTags(List<String>? tags);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ShowItemGuids"]'/> */
        public List<Guid>? ShowItemGuids();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ShowToolGuids"]'/> */
        public List<Guid>? ShowToolGuids();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ShowItemNames"]'/> */
        public List<String>? ShowItemNames();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ShowItemTags"]'/> */
        public List<String>? ShowItemTags();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ShowToolNames"]'/> */
        public List<String>? ShowToolNames();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="ShowToolTags"]'/> */
        public List<String>? ShowToolTags();

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="StoreData"]'/> */
        public Boolean Store(List<DataSet>? data);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="StoreItems"]'/> */
        public Boolean Store(List<DataItem>? items);

        /** <include file='ICache.xml' path='ICache/interface/method[@Name="StoreTools"]'/> */
        public Boolean Store(List<Tool>? tools);
    }

}