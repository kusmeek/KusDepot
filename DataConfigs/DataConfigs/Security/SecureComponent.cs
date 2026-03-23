namespace KusDepot.Data.Configuration.Security;

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

    public async Task<Boolean> IsAdmin(String token)
    {
        try
        {
            if(String.IsNullOrEmpty(token)) { return false; }

            if(String.IsNullOrEmpty(this.AdminTenantID) || String.IsNullOrEmpty(this.AdminClientID)) { return false; }

            return await this.ValidateTokenVerifyRole(token,SecureStrings.AdminRole,this.AdminTenantID,this.AdminClientID).ConfigureAwait(false);
        }
        catch ( Exception ) { return false; }
    }

    public async Task<Boolean> ValidateTokenVerifyRole(String token , String role , String tenantid , String clientid)
    {
        try
        {
            if(new String[]{token,role,tenantid,clientid}.Any(_=>String.IsNullOrEmpty(_))) { return false; }

            OpenIdConnectConfiguration cfg =
                await new ConfigurationManager<OpenIdConnectConfiguration>(SecureStrings.OpenIDConfigURL,
                new OpenIdConnectConfigurationRetriever()).GetConfigurationAsync().ConfigureAwait(false);

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

            TokenValidationResult tvr = await new JsonWebTokenHandler().ValidateTokenAsync(token,tvp).ConfigureAwait(false);

            if(tvr.IsValid is false) { return false; }

            JsonWebToken jwt = (JsonWebToken)tvr.SecurityToken!;

            return jwt.Claims.Any(_=>String.Equals(_.Type,"roles",StringComparison.Ordinal) && String.Equals(_.Value,role,StringComparison.Ordinal));
        }
        catch ( Exception ) { return false; }
    }
}