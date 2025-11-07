namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.Blob")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.Blob",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void DeleteStart(String? id , String? version)                          { WriteEvent(1,id,version); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void DeleteError(String? message , String? id , String? version = null) { WriteEvent(2,message,id,version); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive)]
    public void DeleteContainerSuccess(String? id)                                 { WriteEvent(3,id); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void DeleteSuccess(String? id , String? version = null)                 { WriteEvent(4,id,version); }

    [Event(5 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void ExistsStart(String? id , String? version)                          { WriteEvent(5,id,version); }

    [Event(6 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void ExistsError(String? message , String? id , String? version = null) { WriteEvent(6,message,id,version); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void ExistsSuccess(String? id , String? version = null)                 { WriteEvent(7,id,version); }
    
    [Event(8 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetStart(String? id , String? version)                             { WriteEvent(8,id,version); }

    [Event(9 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetError(String? message , String? id , String? version = null)    { WriteEvent(9,message,id,version); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void GetSuccess(String? id , String? version = null)                    { WriteEvent(10,id,version); }

    [Event(11 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void StoreStart(String? id)                                             { WriteEvent(11,id); }

    [Event(12 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void StoreError(String? message , String? id)                           { WriteEvent(12,message,id); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void StoreSuccess(String? id)                                           { WriteEvent(13,id); }

    [Event(14 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnActivateStart()                                                  { WriteEvent(14); }

    [Event(15 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnActivateSuccess()                                                { WriteEvent(15); }

    [Event(16 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnDeactivateStart()                                                { WriteEvent(16); }

    [Event(17 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnDeactivateSuccess()                                              { WriteEvent(17); }
}