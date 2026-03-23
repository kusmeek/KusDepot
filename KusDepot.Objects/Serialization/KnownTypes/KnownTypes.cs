namespace KusDepot.Serialization;

/**<include file='KnownTypes.xml' path='KnownTypes/class[@name="KnownTypes"]/main/*'/>*/
internal static class KnownTypes
{
    /**<include file='KnownTypes.xml' path='KnownTypes/class[@name="KnownTypes"]/method[@name="GetAllKnownTypes"]/*'/>*/
    internal static IEnumerable<Type> GetAllKnownTypes()
    {
        try
        {
            var _ = new HashSet<Type>();

            _.Add(typeof(Uri));
            _.Add(typeof(Guid));
            _.Add(typeof(Guid?));
            _.Add(typeof(Version));
            _.Add(typeof(DateTime));
            _.Add(typeof(TimeSpan));
            _.Add(typeof(BigInteger));
            _.Add(typeof(DateTimeOffset));

            _.Add(typeof(Char[]));
            _.Add(typeof(Char?[]));
            _.Add(typeof(Int32[]));
            _.Add(typeof(Int32?[]));
            _.Add(typeof(Byte[]));
            _.Add(typeof(Byte?[]));
            _.Add(typeof(SByte[]));
            _.Add(typeof(SByte?[]));
            _.Add(typeof(Int16[]));
            _.Add(typeof(Int16?[]));
            _.Add(typeof(UInt16[]));
            _.Add(typeof(UInt16?[]));
            _.Add(typeof(UInt32[]));
            _.Add(typeof(UInt32?[]));
            _.Add(typeof(Int64[]));
            _.Add(typeof(Int64?[]));
            _.Add(typeof(UInt64[]));
            _.Add(typeof(UInt64?[]));
            _.Add(typeof(Single[]));
            _.Add(typeof(Single?[]));
            _.Add(typeof(Double[]));
            _.Add(typeof(Double?[]));
            _.Add(typeof(Decimal[]));
            _.Add(typeof(Decimal?[]));

            _.Add(typeof(HashSet<Guid>));
            _.Add(typeof(HashSet<Guid?>));
            _.Add(typeof(HashSet<DataItem>));

            _.Add(typeof(Stack<Object>));
            _.Add(typeof(Stack<Object?>));
            _.Add(typeof(Queue<Object>));
            _.Add(typeof(Queue<Object?>));

            _.Add(typeof(List<Object>));
            _.Add(typeof(List<Object?>));
            _.Add(typeof(List<String>));
            _.Add(typeof(List<String?>));

            _.Add(typeof(Dictionary<String,Object>));
            _.Add(typeof(Dictionary<String,Object?>));
            _.Add(typeof(Dictionary<String,String>));
            _.Add(typeof(Dictionary<String,String?>));
            _.Add(typeof(Dictionary<String,Int32>));
            _.Add(typeof(Dictionary<String,Int32?>));
            _.Add(typeof(SortedList<Int32,MSBuildItem>));

            _.Add(typeof(CommandDescriptor));
            _.Add(typeof(CommandWorkflow));
            _.Add(typeof(CommandDetails));
            _.Add(typeof(ToolDescriptor));
            _.Add(typeof(KusDepotCab));
            _.Add(typeof(Descriptor));
            _.Add(typeof(ToolOutput));
            _.Add(typeof(ToolValue));
            _.Add(typeof(ToolInput));
            _.Add(typeof(ToolData));

            _.UnionWith(GetDataItemKnownTypes());
            _.UnionWith(GetSecurityKnownTypes());

            return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetAllKnownTypesFail); throw; }
    }

    /**<include file='KnownTypes.xml' path='KnownTypes/class[@name="KnownTypes"]/method[@name="GetDataItemKnownTypes"]/*'/>*/
    internal static IEnumerable<Type> GetDataItemKnownTypes()
    {
        try
        {
            return new HashSet<Type>
            {
                typeof(KeySet),
                typeof(TextItem),
                typeof(CodeItem),
                typeof(BinaryItem),
                typeof(MSBuildItem),
                typeof(GenericItem),
                typeof(DataSetItem),
                typeof(MultiMediaItem),
                typeof(GuidReferenceItem)
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataItemKnownTypesFail); throw; }
    }

    /**<include file='KnownTypes.xml' path='KnownTypes/class[@name="KnownTypes"]/method[@name="GetSecurityKnownTypes"]/*'/>*/
    internal static IEnumerable<Type> GetSecurityKnownTypes()
    {
        try
        {
            return new HashSet<Type>
            {
                typeof(KeySet),
                typeof(HostKey),
                typeof(TokenKey),
                typeof(OwnerKey),
                typeof(MyHostKey),
                typeof(AccessKey),
                typeof(ClientKey),
                typeof(ManagerKey),
                typeof(ServiceKey),
                typeof(CommandKey),
                typeof(ExecutiveKey),
                typeof(ManagementKey),
                typeof(ServiceRequest),
                typeof(StandardRequest),
                typeof(ManagementRequest)
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetSecurityKnownTypesFail); throw; }
    }
}