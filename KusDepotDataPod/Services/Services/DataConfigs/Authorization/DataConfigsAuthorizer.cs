namespace DataPodServices.DataConfigs;

internal sealed class DataConfigsAuthorizer
{
    private const String AdminRole = "Admin.All";
    private const String CatalogReadRole = "Catalog.Read";
    private const String CatalogWriteRole = "Catalog.Write";

    private readonly OidcMetadataManagerCache metadataManagers;

    public DataConfigsAuthorizer(OidcMetadataManagerCache metadataManagers)
    {
        this.metadataManagers = metadataManagers;
    }

    public Task<Boolean> IsAdminAsync(String token , DataConfigsSnapshot snapshot , CancellationToken cancel = default)
    {
        return this.ValidateRoleAsync(token,snapshot.AdminIdentity,[AdminRole],snapshot,cancel);
    }

    public Task<Boolean> IsCatalogAuthorizedAsync(String token , SnapshotCatalog catalog , CatalogAccess access , DataConfigsSnapshot snapshot , CancellationToken cancel = default)
    {
        String role = access switch
        {
            CatalogAccess.Read => CatalogReadRole,

            CatalogAccess.Write => CatalogWriteRole,

            _ => throw new ArgumentOutOfRangeException(nameof(access),access,"Unknown catalog access level.")
        };

        return this.ValidateRoleAsync(token,catalog.Identity,[role],snapshot,cancel);
    }

    public static Boolean TryGetTokenIdentity(String token , out SnapshotCatalogIdentity identity)
    {
        identity = new SnapshotCatalogIdentity(Guid.Empty,Guid.Empty);

        try
        {
            JsonWebTokenHandler handler = new();

            if(handler.CanReadToken(token) is false || handler.ReadToken(token) is not JsonWebToken jwt)
            {
                return false;
            }

            String? tenant = jwt.Claims.FirstOrDefault(static claim => String.Equals(claim.Type,"tid",StringComparison.Ordinal))?.Value;

            String? audience = jwt.Claims.FirstOrDefault(static claim => String.Equals(claim.Type,"aud",StringComparison.Ordinal))?.Value;

            if(Guid.TryParse(tenant, out Guid tenantId) is false || Guid.TryParse(audience, out Guid appClientId) is false)
            {
                return false;
            }

            identity = new SnapshotCatalogIdentity(tenantId,appClientId);

            return true;
        }
        catch ( Exception exception ) when(exception is ArgumentException or SecurityTokenException)
        {
            return false;
        }
    }

    private async Task<Boolean> ValidateRoleAsync(String token , SnapshotCatalogIdentity identity , IReadOnlyList<String> roles , DataConfigsSnapshot snapshot , CancellationToken cancel)
    {
        if(String.IsNullOrWhiteSpace(token) || identity.TenantId == Guid.Empty || identity.AppClientId == Guid.Empty || roles.Count == 0)
        {
            return false;
        }

        ConfigurationManager<OpenIdConnectConfiguration>? manager = this.metadataManagers.Get(identity.TenantId,snapshot);

        if(manager is null) { return false; }

        TokenValidationParameters parameters = new()
        {
            ClockSkew = TimeSpan.FromMinutes(10),
            ConfigurationManager = manager,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidAudience = identity.AppClientId.ToString(),
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = String.Concat("https://login.microsoftonline.com/",identity.TenantId,"/v2.0")
        };

        cancel.ThrowIfCancellationRequested();

        TokenValidationResult result = await new JsonWebTokenHandler().ValidateTokenAsync(token,parameters).WaitAsync(cancel);

        if(result.IsValid is false)
        {
            if(result.Exception is SecurityTokenException or ArgumentException)
            {
                return false;
            }

            throw new OperationFailedException("Token validation could not be completed.",result.Exception);
        }

        if(result.SecurityToken is not JsonWebToken jwt)
        {
            return false;
        }

        String? tenantClaim = jwt.Claims.FirstOrDefault(static claim => String.Equals(claim.Type,"tid",StringComparison.Ordinal))?.Value;

        if(Guid.TryParse(tenantClaim, out Guid tenantId) is false || tenantId != identity.TenantId)
        {
            return false;
        }

        return jwt.Claims.Any(claim => String.Equals(claim.Type,"roles",StringComparison.Ordinal) && roles.Contains(claim.Value,StringComparer.Ordinal));
    }
}
