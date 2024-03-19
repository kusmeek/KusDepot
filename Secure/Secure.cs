namespace KusDepot.Data;

[StatePersistence(StatePersistence.None)]
internal sealed partial class Secure : Actor , ISecure
{
    private String AdminClientID {get;set;} = String.Empty; private String AdminTenantID {get;set;} = String.Empty;

    public Secure(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    public Task<Boolean> IsAdmin(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("IsAdmin",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(new String[]{token,this.AdminTenantID,this.AdminClientID}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            SetOk(_); return this.ValidateTokenVerifyRole(token,AdminRole,this.AdminTenantID,this.AdminClientID,traceid,spanid);
        }
        catch ( Exception _ ) { Log.Error(_,IsAdminFail); return Task.FromResult(false); }
    }

    public Task<Boolean> SetAdmin(String token , String tenantid , String clientid , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("SetAdmin",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(new String[]{token,tenantid,clientid}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(!this.IsAdmin(token,traceid,spanid).Result) { Log.Error(SetAdminAuthFail); SetErr(_); return Task.FromResult(false); }

            this.AdminTenantID = tenantid; this.AdminClientID = clientid; Log.Information(SetAdminSuccess); SetOk(_); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,SetAdminFail); return Task.FromResult(false); }
    }

    public Task<Boolean> ValidateTokenVerifyRole(String token , String role , String tenantid , String clientid , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("ValidateTokenVerifyRole",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(new String[]{token,role,tenantid,clientid}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            OpenIdConnectConfiguration cfg =
            new ConfigurationManager<OpenIdConnectConfiguration>(OpenIDConfigURL,
            new OpenIdConnectConfigurationRetriever()).GetConfigurationAsync().Result;

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

            TokenValidationResult tvr = new JsonWebTokenHandler().ValidateTokenAsync(token,tvp).Result;

            if(!tvr.IsValid) { Log.Error(tvr.Exception,ValidateVerifyFail); SetErr(_); return Task.FromResult(false); } SetOk(_);

            return Task.FromResult(((JsonWebToken)tvr.SecurityToken).Claims.Any(_=>String.Equals(_.Type,"roles",StringComparison.Ordinal) && String.Equals(_.Value,role,StringComparison.Ordinal)));
        }
        catch ( Exception _ ) { Log.Error(_,ValidateVerifyFail); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync()
    {
        try
        {
            StorageSilo? _ = StorageSilo.FromFile(AdminFilePath);

            if(_ is null) { Log.Error(AdminLoadFail); return Task.FromResult(false); }

            this.AdminClientID = _.AppClientID; this.AdminTenantID = _.TenantID;

            Log.Information(Activated); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); return Task.FromResult(false); }
    }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }
}