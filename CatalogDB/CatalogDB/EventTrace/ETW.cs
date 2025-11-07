namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.CatalogDB")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.CatalogDB",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void AddUpdateStart(String? catalog , String? id)                   { WriteEvent(1,catalog,id); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void AddUpdateError(String? message , String? catalog , String? id) { WriteEvent(2,message,catalog,id); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void AddUpdateSuccess(String? catalog , String? id)                 { WriteEvent(3,catalog,id); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void ExistsStart(String? catalog , String? id)                      { WriteEvent(4,catalog,id); }

    [Event(5 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void ExistsError(String? message , String? catalog , String? id)    { WriteEvent(5,message,catalog,id); }

    [Event(6 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void ExistsSuccess(String? catalog , String? id)                    { WriteEvent(6,catalog,id); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void ExistsIDStart(String? catalog , String? id)                    { WriteEvent(7,catalog,id); }

    [Event(8 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void ExistsIDError(String? message , String? catalog , String? id)  { WriteEvent(8,message,catalog,id); }

    [Event(9 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void ExistsIDSuccess(String? catalog , String? id)                  { WriteEvent(9,catalog,id); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void RemoveStart(String? catalog , String? id)                      { WriteEvent(10,catalog,id); }

    [Event(11 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void RemoveError(String? message , String? catalog , String? id)    { WriteEvent(11,message,catalog,id); }

    [Event(12 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void RemoveSuccess(String? catalog , String? id)                    { WriteEvent(12,catalog,id); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void RemoveIDStart(String? catalog , String? id)                    { WriteEvent(13,catalog,id); }

    [Event(14 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void RemoveIDError(String? message , String? catalog , String? id)  { WriteEvent(14,message,catalog,id); }

    [Event(15 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void RemoveIDSuccess(String? catalog , String? id)                  { WriteEvent(15,catalog,id); }

    [Event(16 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SCStart()                                                      { WriteEvent(16); }

    [Event(17 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SCError(String? message)                                       { WriteEvent(17,message); }

    [Event(18 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SCSuccess(Int32 count)                                         { WriteEvent(18,count); }

    [Event(19 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SELStart()                                                     { WriteEvent(19); }

    [Event(20 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SELError(String? message)                                      { WriteEvent(20,message); }

    [Event(21 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SELSuccess(Int32 count)                                        { WriteEvent(21,count); }

    [Event(22 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SMStart()                                                      { WriteEvent(22); }

    [Event(23 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SMError(String? message)                                       { WriteEvent(23,message); }

    [Event(24 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SMSuccess(Int32 count)                                         { WriteEvent(24,count); }

    [Event(25 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SNStart()                                                      { WriteEvent(25); }

    [Event(26 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SNError(String? message)                                       { WriteEvent(26,message); }

    [Event(27 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SNSuccess()                                                    { WriteEvent(27); }

    [Event(28 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SSVStart()                                                     { WriteEvent(28); }

    [Event(29 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SSVError(String? message)                                      { WriteEvent(29,message); }

    [Event(30 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SSVSuccess(Int32 count)                                        { WriteEvent(30,count); }

    [Event(31 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void STGStart()                                                     { WriteEvent(31); }

    [Event(32 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void STGError(String? message)                                      { WriteEvent(32,message); }

    [Event(33 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void STGSuccess()                                                   { WriteEvent(33); }

    [Event(34 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnActivateStart(String? catalog)                               { WriteEvent(34,catalog); }

    [Event(35 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateError(String? message , String? catalog)             { WriteEvent(35,message,catalog); }

    [Event(36 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateSuccess(String? message , String? catalog)           { WriteEvent(36,message,catalog); }

    [Event(37 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnDeactivateStart(String? catalog)                             { WriteEvent(37,catalog); }

    [Event(38 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnDeactivateSuccess(String? catalog)                           { WriteEvent(38,catalog); }
}