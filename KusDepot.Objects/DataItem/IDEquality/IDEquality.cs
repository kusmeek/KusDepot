namespace KusDepot;

/**<include file='IDEquality.xml' path='IDEquality/class[@name="IDEquality"]/main/*'/>*/
public class IDEquality : IEqualityComparer<DataItem>
{
    /**<include file='IDEquality.xml' path='IDEquality/class[@name="IDEquality"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(DataItem? a , DataItem? b) { if(ReferenceEquals(a,b)) { return true; } return Guid.Equals(a?.GetID(),b?.GetID()); }

    /**<include file='IDEquality.xml' path='IDEquality/class[@name="IDEquality"]/method[@name="GetHashCode"]/*'/>*/
    public Int32 GetHashCode([DisallowNull] DataItem item) { return HashCode.Combine(item?.GetID()); }
}