namespace DataPodServices.DataConfigs.Security;

internal sealed class SecureComponent
{
    private String AdminClientID  = String.Empty;

    private String AdminTenantID = String.Empty;

    public void LoadAdmin(StorageSilo? adminsilo)
    {
        if(adminsilo is null) { return; }

        AdminClientID  = adminsilo.AppClientID ?? String.Empty;

        AdminTenantID = adminsilo.TenantID ?? String.Empty;
    }

    public async Task<Boolean> IsAdmin(String token , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(token)) { return false; }

            if(String.IsNullOrEmpty(this.AdminTenantID) || String.IsNullOrEmpty(this.AdminClientID)) { return false; }

            return await ValidateTokenVerifyRole(token,SecureStrings.AdminRole,this.AdminTenantID,this.AdminClientID,cancel).ConfigureAwait(false);
        }
        catch ( Exception ) { return false; }
    }

    public static async Task<Boolean> ValidateTokenVerifyRole(String token , String role , String tenantid , String clientid , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(token) || String.IsNullOrEmpty(role) || String.IsNullOrEmpty(tenantid) || String.IsNullOrEmpty(clientid)) { return false; }

            OpenIdConnectConfiguration cfg =
                await new ConfigurationManager<OpenIdConnectConfiguration>(SecureStrings.OpenIDConfigURL,
                new OpenIdConnectConfigurationRetriever()).GetConfigurationAsync(cancel).ConfigureAwait(false);

            TokenValidationParameters tvp = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(10),
                IssuerSigningKeys = cfg.SigningKeys,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidAudience = clientid,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = String.Concat("https://login.microsoftonline.com/",tenantid,"/v2.0")
            };

            cancel.ThrowIfCancellationRequested();

            TokenValidationResult tvr = await new JsonWebTokenHandler().ValidateTokenAsync(token,tvp).WaitAsync(cancel).ConfigureAwait(false);

            if(tvr.IsValid is false) { return false; }

            JsonWebToken jwt = (JsonWebToken)tvr.SecurityToken!;

            foreach(var claim in jwt.Claims)
            {
                if(String.Equals(claim.Type,"roles",StringComparison.Ordinal) && String.Equals(claim.Value,role,StringComparison.Ordinal)) { return true; }
            }

            return false;
        }
        catch ( Exception ) { return false; }
    }
}
