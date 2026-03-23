namespace KusDepot.Builders;

/**<include file='RegisteredCommand.xml' path='RegisteredCommand/record[@name="RegisteredCommand"]/main/*'/>*/
public sealed record RegisteredCommand
{
    /**<include file='RegisteredCommand.xml' path='RegisteredCommand/record[@name="RegisteredCommand"]/property[@name="Command"]/*'/>*/
    public required ICommand Command { get; init; }

    /**<include file='RegisteredCommand.xml' path='RegisteredCommand/record[@name="RegisteredCommand"]/property[@name="Handle"]/*'/>*/
    public required String Handle { get; init; }

    /**<include file='RegisteredCommand.xml' path='RegisteredCommand/record[@name="RegisteredCommand"]/property[@name="Permissions"]/*'/>*/
    public ImmutableArray<Int32>? Permissions { get; init; }
}