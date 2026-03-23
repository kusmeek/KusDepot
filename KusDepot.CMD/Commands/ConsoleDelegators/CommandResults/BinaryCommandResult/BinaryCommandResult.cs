namespace KusDepot.Commands;

/**<include file='BinaryCommandResult.xml' path='BinaryCommandResult/class[@name="BinaryCommandResult"]/main/*'/>*/
public sealed class BinaryCommandResult : CommandResult
{
    /**<include file='BinaryCommandResult.xml' path='BinaryCommandResult/class[@name="BinaryCommandResult"]/property[@name="StandardErrorBytes"]/*'/>*/
    public Byte[] StandardErrorBytes { get; }

    /**<include file='BinaryCommandResult.xml' path='BinaryCommandResult/class[@name="BinaryCommandResult"]/property[@name="StandardOutputBytes"]/*'/>*/
    public Byte[] StandardOutputBytes { get; }

    /**<include file='BinaryCommandResult.xml' path='BinaryCommandResult/class[@name="BinaryCommandResult"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryCommandResult(Int32 exitcode , DateTimeOffset starttime , DateTimeOffset exittime , Byte[] standardoutputbytes , Byte[] standarderrorbytes) : base(exitcode,starttime,exittime)
    {
        StandardOutputBytes = standardoutputbytes ?? Array.Empty<Byte>(); StandardErrorBytes = standarderrorbytes ?? Array.Empty<Byte>();
    }

    /**<include file='BinaryCommandResult.xml' path='BinaryCommandResult/class[@name="BinaryCommandResult"]/method[@name="Deconstruct"]/*'/>*/
    public void Deconstruct(out Int32 exitcode , out Byte[] standardoutputbytes , out Byte[] standarderrorbytes)
    {
        exitcode = ExitCode; standardoutputbytes = StandardOutputBytes; standarderrorbytes  = StandardErrorBytes;
    }

    /**<include file='BinaryCommandResult.xml' path='BinaryCommandResult/class[@name="BinaryCommandResult"]/method[@name="ImplicitBytes"]/*'/>*/
    public static implicit operator Byte[](BinaryCommandResult result) => result.StandardOutputBytes;
}