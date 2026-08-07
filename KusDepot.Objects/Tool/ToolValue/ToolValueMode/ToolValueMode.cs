namespace KusDepot;

/**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.ToolValueMode")]
[DataContract(Name = "ToolValueMode" , Namespace = "KusDepot")]

public enum ToolValueMode
{
    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Parse"]/*'/>*/
    [EnumMember(Value = nameof(Parse))]
    Parse = 0,

    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Build"]/*'/>*/
    [EnumMember(Value = nameof(Build))]
    Build = 1,

    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Custom"]/*'/>*/
    [EnumMember(Value = nameof(Custom))]
    Custom = 2,

    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Unhandled"]/*'/>*/
    [EnumMember(Value = nameof(Unhandled))]
    Unhandled = Int32.MaxValue
}