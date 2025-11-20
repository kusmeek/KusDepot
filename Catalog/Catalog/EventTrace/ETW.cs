namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.Catalog")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.Catalog",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GNStart(Guid id)                        { WriteEvent(1,id); }

    [Event(2 , Level = EventLevel.Error , Opcode = EventOpcode.Stop)]
    public void GNError(Guid id)                        { WriteEvent(2,id); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void GNSuccess(Guid id , Int32 count)        { WriteEvent(3,id,count); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GTStart(Guid id)                        { WriteEvent(4,id); }

    [Event(5 , Level = EventLevel.Error , Opcode = EventOpcode.Stop)]
    public void GTError(Guid id)                        { WriteEvent(5,id); }

    [Event(6 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void GTSuccess(Guid id , Int32 count)        { WriteEvent(6,id,count); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SSVStart()                              { WriteEvent(7); }

    [Event(8 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SSVError(String? message)               { WriteEvent(8,message); }

    [Event(9 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SSVSuccess(Int32 count)                 { WriteEvent(9,count); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SCStart()                               { WriteEvent(10); }

    [Event(11 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SCError(String? message)                { WriteEvent(11,message); }

    [Event(12 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SCSuccess(Int32 count)                  { WriteEvent(12,count); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SELStart()                              { WriteEvent(13); }

    [Event(14 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SELError(String? message)               { WriteEvent(14,message); }

    [Event(15 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SELSuccess(Int32 count)                 { WriteEvent(15,count); }

    [Event(16 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SMStart()                               { WriteEvent(16); }

    [Event(17 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SMError(String? message)                { WriteEvent(17,message); }

    [Event(18 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SMSuccess(Int32 count)                  { WriteEvent(18,count); }

    [Event(19 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SNStart()                               { WriteEvent(19); }

    [Event(20 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SNError(String? message)                { WriteEvent(20,message); }

    [Event(21 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SNSuccess()                             { WriteEvent(21); }

    [Event(22 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void STGStart()                              { WriteEvent(22); }

    [Event(23 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void STGError(String? message)               { WriteEvent(23,message); }

    [Event(24 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void STGSuccess()                            { WriteEvent(24); }
}