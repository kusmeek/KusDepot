namespace KusDepot.Sync;

/**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/main/*'/>*/
public readonly struct LockState
{
    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/field[@name="First"]/*'/>*/
    public readonly DataItem? First;

    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/field[@name="Second"]/*'/>*/
    public readonly DataItem? Second;

    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/field[@name="FirstMeta"]/*'/>*/
    public readonly Boolean FirstMeta;

    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/field[@name="FirstData"]/*'/>*/
    public readonly Boolean FirstData;

    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/field[@name="SecondMeta"]/*'/>*/
    public readonly Boolean SecondMeta;

    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/field[@name="SecondData"]/*'/>*/
    public readonly Boolean SecondData;

    /**<include file='LockState.xml' path='LockState/struct[@name="LockState"]/constructor[@name="Constructor"]/*'/>*/
    public LockState(DataItem? first , DataItem? second , Boolean firstmeta , Boolean firstdata , Boolean secondmeta , Boolean seconddata)
    {
        First      = first;
        Second     = second;
        FirstMeta  = firstmeta;
        FirstData  = firstdata;
        SecondMeta = secondmeta;
        SecondData = seconddata;
    }
}