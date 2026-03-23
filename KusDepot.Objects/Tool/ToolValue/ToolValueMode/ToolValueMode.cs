namespace KusDepot;

/**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.ToolValueMode")]
[DataContract(Name = "ToolValueMode" , Namespace = "KusDepot")]

public enum ToolValueMode
{
    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Parse"]/*'/>*/
    [EnumMember(Value = nameof(Parse))] [Id(0)]
    Parse = 0,

    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Build"]/*'/>*/
    [EnumMember(Value = nameof(Build))] [Id(1)]
    Build = 1,

    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Custom"]/*'/>*/
    [EnumMember(Value = nameof(Custom))] [Id(2)]
    Custom = 2,

    /**<include file='ToolValueMode.xml' path='ToolValueMode/enum[@name="ToolValueMode"]/value[@name="Unhandled"]/*'/>*/
    [EnumMember(Value = nameof(Unhandled))] [Id(3)]
    Unhandled = Int32.MaxValue
}