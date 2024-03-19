namespace Wasm;

internal static class StartUp
{
    private static async Task Main(String[] args)
    {
        WebAssemblyHostBuilder _0 = WebAssemblyHostBuilder.CreateDefault(args);

        _0.RootComponents.Add<App>("#app"); _0.RootComponents.Add<HeadOutlet>("head::after");

        _0.Services.AddScoped(sp => {return new HttpClient(){BaseAddress = new Uri(_0.HostEnvironment.BaseAddress)};});

        await _0.Build().RunAsync().ConfigureAwait(false);
    }
}