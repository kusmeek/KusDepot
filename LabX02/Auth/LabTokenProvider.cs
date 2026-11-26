namespace LabX02;

internal static class LabTokenProvider
{
    public static async Task<String> GetToken(String mode)
    {
        EntraSetupRecord setup = await EntraSetupProvider.LoadAsync().ConfigureAwait(false);

        IPublicClientApplication application = PublicClientApplicationBuilder
            .Create(setup.ClientID)
            .WithAuthority(new Uri(setup.Authority))
            .Build();

        (String username, String password) = ResolveCredentials(setup,mode);

        AuthenticationResult result = await application
            .AcquireTokenByUsernamePassword(new[] { setup.Scope },username,password)
            .ExecuteAsync()
            .ConfigureAwait(false);

        if(String.IsNullOrWhiteSpace(result.AccessToken)) { throw new InvalidOperationException($"Failed to acquire an access token for mode '{mode}'."); }

        return result.AccessToken;
    }

    private static (String UserName, String Password) ResolveCredentials(EntraSetupRecord setup , String mode)
    {
        return mode.Trim().ToUpperInvariant() switch
        {
            "ADMIN" => (setup.AdminUserName,setup.AdminUserPass),
            "READ" => (setup.ReadUserName,setup.ReadUserPass),
            _ => (setup.WriteUserName,setup.WriteUserPass),
        };
    }
}
