namespace LabX08;

internal static class LabTokenProvider
{
    public static async Task<String> GetToken(String mode)
    {
        EntraSetupRecord setup = LoadSetup();

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

    private static EntraSetupRecord LoadSetup()
    {
        XmlDocument document = new();
        document.Load(Path.Combine(AppContext.BaseDirectory,"EntraSetup.xml"));

        XmlNode setup = document.SelectSingleNode("EntraSetup") ?? throw new InvalidOperationException("EntraSetup root element is missing.");

        return new()
        {
            Authority = ReadRequired(setup,"Authority"),
            ClientID = ReadRequired(setup,"ClientID"),
            Scope = ReadRequired(setup,"Scope"),
            AdminUserName = ReadRequired(setup,"AdminUserName"),
            AdminUserPass = ReadRequired(setup,"AdminUserPass"),
            ReadUserName = ReadRequired(setup,"ReadUserName"),
            ReadUserPass = ReadRequired(setup,"ReadUserPass"),
            WriteUserName = ReadRequired(setup,"WriteUserName"),
            WriteUserPass = ReadRequired(setup,"WriteUserPass"),
        };
    }

    private static String ReadRequired(XmlNode setup , String elementName)
    {
        String value = setup.SelectSingleNode(elementName)?.InnerText ?? String.Empty;

        if(String.IsNullOrWhiteSpace(value)) { throw new InvalidOperationException($"EntraSetup.xml is missing required element '{elementName}'."); }

        return value;
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
