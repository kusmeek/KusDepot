namespace Watch;

internal static class StartUp
{
    private static void Main()
    {
        try
        {
            ActorRuntime.RegisterActorAsync<Watch>( (context,type) => { return new ActorService(context,type); }).GetAwaiter().GetResult();

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception ) { throw; }
    }
}