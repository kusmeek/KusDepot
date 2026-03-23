namespace KusDepot.AI;

public enum ToolPodResultKind
{
    [Description(ResultKindVoid)]
    Void,

    [Description(ResultKindValue)]
    Value,

    [Description(ResultKindReference)]
    Reference,

    [Description(ResultKindToolValue)]
    ToolValue,

    [Description(ResultKindError)]
    Error
}