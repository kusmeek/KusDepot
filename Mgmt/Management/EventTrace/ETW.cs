namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.Management")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.Management",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void BackupAKStart(String? catalog)                     { WriteEvent(1,catalog); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void BackupAKError(String? message , String? catalog)   { WriteEvent(2,message,catalog); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void BackupAKSuccess(String? catalog)                   { WriteEvent(3,catalog); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void RestoreAKStart(String? catalog)                    { WriteEvent(4,catalog); }

    [Event(5 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void RestoreAKError(String? message , String? catalog)  { WriteEvent(5,message,catalog); }

    [Event(6 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void RestoreAKSuccess(String? catalog , String? newest) { WriteEvent(6,catalog,newest); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void BackupDCStart()                                    { WriteEvent(7); }

    [Event(8 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void BackupDCError(String? message)                     { WriteEvent(8,message); }

    [Event(9 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void BackupDCSuccess()                                  { WriteEvent(9); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void RestoreDCStart()                                   { WriteEvent(10); }

    [Event(11 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void RestoreDCError(String? message)                    { WriteEvent(11,message); }

    [Event(12 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void RestoreDCSuccess(String? newest)                   { WriteEvent(12,newest); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void BackupUStart()                                     { WriteEvent(13); }

    [Event(14 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void BackupUError(String? message)                      { WriteEvent(14,message); }

    [Event(15 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void BackupUSuccess()                                   { WriteEvent(15); }

    [Event(16 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void RestoreUStart()                                    { WriteEvent(16); }

    [Event(17 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void RestoreUError(String? message)                     { WriteEvent(17,message); }

    [Event(18 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void RestoreUSuccess(String? newest)                    { WriteEvent(18,newest); }

    [Event(19 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnActivateStart()                                  { WriteEvent(19); }

    [Event(20 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnActivateSuccess()                                { WriteEvent(20); }

    [Event(21 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnDeactivateStart()                                { WriteEvent(21); }

    [Event(22 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnDeactivateSuccess()                              { WriteEvent(22); }
}