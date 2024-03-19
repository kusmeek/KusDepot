namespace WebTool;

internal static class Server
{
    private static void Main(String[] args)
    {
        WebApplicationBuilder _0 = WebApplication.CreateBuilder(args);
        {
            _0.WebHost.UseUrls("http://localhost:57015");
        }

        WebApplication _1 = _0.Build();
        {
            _1.UseStaticFiles(); _1.MapGet("/NewTool", ( ) => {return new Tool().ToString();});

            _1.Run();
        }
    }
}