namespace KusDepot;

/**<include file='Serialization.xml' path='Serialization/class[@name="Serialization"]/main/*'/>*/
internal static class Serialization
{
    /**<include file='Serialization.xml' path='Serialization/class[@name="Serialization"]/method[@name="GetAllKnownTypes"]/*'/>*/
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

            _.Add(typeof(CommandWorkflow));
            _.Add(typeof(CommandDetails));
            _.Add(typeof(KusDepotCab));
            _.Add(typeof(Descriptor));

            _.UnionWith(GetDataItemKnownTypes());
            _.UnionWith(GetSecurityKnownTypes());

            return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetAllKnownTypesFail); throw; }
    }

    /**<include file='Serialization.xml' path='Serialization/class[@name="Serialization"]/method[@name="GetDataItemKnownTypes"]/*'/>*/
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

    /**<include file='Serialization.xml' path='Serialization/class[@name="Serialization"]/method[@name="GetSecurityKnownTypes"]/*'/>*/
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