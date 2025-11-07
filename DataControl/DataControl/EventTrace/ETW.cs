namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.Control")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.Control",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void CacheStart(String? id)                         { WriteEvent(1,id); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void CacheError(String? message , String? id)       { WriteEvent(2,message,id); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void CacheSuccess(String? message , String? id)     { WriteEvent(3,message,id); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void DeleteStart(String? id)                        { WriteEvent(4,id); }

    [Event(5 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void DeleteError(String? message , String? id)      { WriteEvent(5,message,id); }

    [Event(6 , Level = EventLevel.Error , Message = "{0}")]
    public void DeleteIssue(String? message , String? id)      { WriteEvent(6,message,id); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void DeleteSuccess(String? id)                      { WriteEvent(7,id); }

    [Event(8 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetStart(String? id)                           { WriteEvent(8,id); }

    [Event(9 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetError(String? message , String? id)         { WriteEvent(9,message,id); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void GetSuccess(String? id)                         { WriteEvent(10,id); }

    [Event(11 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetStreamStart(String? id)                     { WriteEvent(11,id); }

    [Event(12 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetStreamError(String? message , String? id)   { WriteEvent(12,message,id); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void GetStreamSuccess(String? id)                   { WriteEvent(13,id); }

    [Event(14 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void StoreStart(String? id)                         { WriteEvent(14,id); }

    [Event(15 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void StoreError(String? message , String? id)       { WriteEvent(15,message,id); }

    [Event(16 , Level = EventLevel.Error , Message = "{0}")]
    public void StoreIssue(String? message , String? id)       { WriteEvent(16,message,id); }

    [Event(17 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void StoreSuccess(String? id)                       { WriteEvent(17,id); }

    [Event(18 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void StoreStreamStart(String? id)                   { WriteEvent(18,id); }

    [Event(19 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void StoreStreamError(String? message , String? id) { WriteEvent(19,message,id); }

    [Event(20 , Level = EventLevel.Error , Message = "{0}")]
    public void StoreStreamIssue(String? message , String? id) { WriteEvent(20,message,id); }

    [Event(21 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void StoreStreamSuccess(String? id)                 { WriteEvent(21,id); }
}