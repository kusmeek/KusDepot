namespace KusDepot.Commands;

/**<include file='CommandDetailsExtensions.xml' path='CommandDetailsExtensions/class[@name="CommandDetailsExtensions"]/main/*'/>*/
public static class CommandDetailsExtensions
{
    /**<include file='CommandDetailsExtensions.xml' path='CommandDetailsExtensions/class[@name="CommandDetailsExtensions"]/method[@name="SetBinaryConsoleObserver"]/*'/>*/
    public static CommandDetails SetBinaryConsoleObserver(this CommandDetails details , BinaryCommandObserver? observer = null , Boolean overwrite = false)
    {
        try
        {
            if(details is null || observer is null) { return details!; }

            details.SetArgument("BinaryCommandObserver",observer,overwrite);
        }
        catch {} return details;
    }

    /**<include file='CommandDetailsExtensions.xml' path='CommandDetailsExtensions/class[@name="CommandDetailsExtensions"]/method[@name="SetConsoleObserver"]/*'/>*/
    public static CommandDetails SetConsoleObserver(this CommandDetails details , ConsoleCommandObserver? observer = null , Boolean overwrite = false)
    {
        try
        {
            if(details is null || observer is null) { return details!; }

            details.SetArgument("ConsoleCommandObserver",observer,overwrite);
        }
        catch {} return details;
    }
}