namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.CoreCache")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.CoreCache",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void CleanUpStart()                                    { WriteEvent(1); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void CleanUpError(String? message , String? id = null) { WriteEvent(2,message,id); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive)]
    public void CleanUpRemoved(String? id)                        { WriteEvent(3,id); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void CleanUpSuccess()                                  { WriteEvent(4); }

    [Event(5 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void DeleteStart(String? id)                           { WriteEvent(5,id); }

    [Event(6 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void DeleteError(String? message , String? id)         { WriteEvent(6,message,id); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void DeleteSuccess(String? id)                         { WriteEvent(7,id); }

    [Event(8 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void ExistsStart(String? id)                           { WriteEvent(8,id); }

    [Event(9 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void ExistsError(String? message , String? id)         { WriteEvent(9,message,id); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void ExistsSuccess(String? id)                         { WriteEvent(10,id); }

    [Event(11 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetStart(String? id)                              { WriteEvent(11,id); }

    [Event(12 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetError(String? message , String? id)            { WriteEvent(12,message,id); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void GetSuccess(String? id)                            { WriteEvent(13,id); }

    [Event(14 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void StoreStart(String? id)                            { WriteEvent(14,id); }

    [Event(15 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void StoreError(String? message , String? id)          { WriteEvent(15,message,id); }

    [Event(16 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void StoreSuccess(String? id)                          { WriteEvent(16,id); }

    [Event(17 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnActivateStart()                                 { WriteEvent(17); }

    [Event(18 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateError(String? message)                  { WriteEvent(18,message); }

    [Event(19 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnActivateSuccess()                               { WriteEvent(19); }

    [Event(20 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnDeactivateStart()                               { WriteEvent(20); }

    [Event(21 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnDeactivateSuccess()                             { WriteEvent(21); }
}